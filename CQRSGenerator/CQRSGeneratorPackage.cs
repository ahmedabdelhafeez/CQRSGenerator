global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Project = EnvDTE.Project;
global using Task = System.Threading.Tasks.Task;
using CQRSGenerator.Helpers;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;


namespace CQRSGenerator;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.CQRSGeneratorString)]
public sealed class CQRSGeneratorPackage : ToolkitPackage
{
    private const string _solutionItemsProjectName = "Solution Items";
    private static readonly Regex _reservedFileNamePattern = new Regex($@"(?i)^(PRN|AUX|NUL|CON|COM\d|LPT\d)(\.|$)");
    private static readonly HashSet<char> _invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());

    public static DTE2 _dte;

    public static List<FileToCreate> cqrsFiles;
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        _dte = await GetServiceAsync(typeof(DTE)) as DTE2;
        Assumes.Present(_dte);
        if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService mcs)
        {
            CommandID menuCommandID = new CommandID(PackageGuids.GenerateCQRS, PackageIds.cmdidMyCommand);
            OleMenuCommand menuItem = new OleMenuCommand(ExecuteAsync, menuCommandID);
            mcs.AddCommand(menuItem);
        }
        
    }

    private async void ExecuteAsync(object sender, EventArgs e)
    {

        TargetPosition target = await ProjectHelpers.GetSelectedPositionAsync();
        if (target == null)
        {
            await VS.MessageBox.ShowWarningAsync("Selected Path", "No valid path selected.");
            return;
        }

        DirectoryInfo dir = new DirectoryInfo(target.TargetPath);
        FileNameDialog dialog = new FileNameDialog(target.RootNS.Replace('.','/'))
        {
            Owner = Application.Current.MainWindow
        };

        bool? result = dialog.ShowDialog();
       var input=(result.HasValue && result.Value) 
            ? dialog.Input.Trim()
            : string.Empty;
        if (string.IsNullOrEmpty(input))
        {
            return;
        }

        target.SetEntity(input,dialog.Type);

        var validateRes = ValidatePath(target.Entity);
        if (validateRes!=null)
        {
            await VS.MessageBox.ShowWarningAsync(
            $"Error creating CQRS '{target.Entity}':{Environment.NewLine}{validateRes}",
            Vsix.Name);
        }
        switch (dialog.Type)
        {
            case CreateTypes.CQRS:
                GenerateCQRSFilesAsync(target).Forget();
                break;
            case CreateTypes.Command:
                GenerateCommandFilesAsync(target).Forget();
                break;
            case CreateTypes.Query:
                GenerateQueryFilesAsync(target).Forget();
                break;
            default:
                return;
        }
    }

    private async Task GenerateCQRSFilesAsync(TargetPosition target)
    {
        cqrsFiles = [
            new(FileTypes.Command, $"Create{target.Entity}Command",$"Commands/CreateCommand"),
            new(FileTypes.CommandHandler, $"Create{target.Entity}Command",$"Commands/CreateCommand"),
            new(FileTypes.Response, $"Create{target.Entity}CommandResponse",$"Commands/CreateCommand"),
            new(FileTypes.Command, $"Delete{target.Entity}Command",$"Commands/DeleteCommand"),
            new(FileTypes.CommandHandler, $"Delete{target.Entity}Command",$"Commands/DeleteCommand"),
            new(FileTypes.Response, $"Delete{target.Entity}CommandResponse",$"Commands/DeleteCommand"),
            new(FileTypes.Command, $"Update{target.Entity}Command",$"Commands/UpdateCommand"),
            new(FileTypes.CommandHandler, $"Update{target.Entity}Command",$"Commands/UpdateCommand"),
            new(FileTypes.Response, $"Update{target.Entity}CommandResponse",$"Commands/UpdateCommand"),
            new(FileTypes.Query, $"GetAll{target.PluralizedEntity}Query",$"Queries/GetAllQuery"),
            new(FileTypes.QueryHandler, $"GetAll{target.PluralizedEntity}Query",$"Queries/GetAllQuery"),
            new(FileTypes.Response, $"GetAll{target.PluralizedEntity}QueryResponse",$"Queries/GetAllQuery"),
            new(FileTypes.Query, $"Get{target.Entity}ByIdQuery",$"Queries/GetByIdQuery"),
            new(FileTypes.QueryHandler, $"Get{target.Entity}ByIdQuery",$"Queries/GetByIdQuery"),
            new(FileTypes.Response, $"Get{target.Entity}ByIdQueryResponse",$"Queries/GetByIdQuery"),
            new(FileTypes.Mapper, $"{target.Entity}Mapping",$"Mapper"),
        ];
        await StartGenerateFilesAsync(target);
    }

    private async Task GenerateCommandFilesAsync(TargetPosition target)
    {
        cqrsFiles = [
           new(FileTypes.Command, target.Entity),
           new(FileTypes.CommandHandler,target.Entity),
           new(FileTypes.Response, $"{target.Entity}Response"),

         ];

        await StartGenerateFilesAsync(target);
    }

    private async Task GenerateQueryFilesAsync(TargetPosition target)
    {
        cqrsFiles = [
            new(FileTypes.Query, target.Entity),
            new(FileTypes.QueryHandler,target.Entity),
            new(FileTypes.Response, $"{target.Entity}Response"),
        ];
        await StartGenerateFilesAsync(target);
    }        
    private async Task  StartGenerateFilesAsync(TargetPosition target)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync();

        DirectoryInfo dirctory = new(target.FolderPath);
        if (!dirctory.Exists)
        {
            Directory.CreateDirectory(dirctory.FullName);
            foreach (var item in cqrsFiles)
            {
                TemplateMap.GetTemplateFilePathAsync(target, item).Forget();
            }
        }
        else
        {
            await VS.MessageBox.ShowWarningAsync($"The Folder '{target.Entity}' already exists.", Vsix.Name);
        }

    }


    private string ValidatePath(string path)
    {
        do
        {
            string name = Path.GetFileName(path);

            if (_reservedFileNamePattern.IsMatch(name))
            {
                return $"The name '{name}' is a system reserved name.";
            }

            if (name.Any(c => _invalidFileNameChars.Contains(c)))
            {
                return $"The name '{name}' contains invalid characters.";
            }

            path = Path.GetDirectoryName(path);
        } while (!string.IsNullOrEmpty(path));
        return null;
    }


  

}