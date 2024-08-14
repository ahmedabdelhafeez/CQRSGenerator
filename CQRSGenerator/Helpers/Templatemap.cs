using CQRSGenerator.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CQRSGenerator;

internal static class TemplateMap
{
	private static readonly string _folder;
	private static readonly List<string> _templateFiles = new List<string>();
	private const string _defaultExt = ".txt";
	private const string _templateDir = ".templates";

	static TemplateMap()
	{
		var folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		var userProfile = Path.Combine(folder, ".vs", _templateDir);

		if (Directory.Exists(userProfile))
		{
			_templateFiles.AddRange(Directory.GetFiles(userProfile, "*" + _defaultExt, SearchOption.AllDirectories));
		}

		var assembly = Assembly.GetExecutingAssembly().Location;
		_folder = Path.Combine(Path.GetDirectoryName(assembly), "Templates");
		_templateFiles.AddRange(Directory.GetFiles(_folder, "*" + _defaultExt, SearchOption.AllDirectories));
	}


    public static async Task GetTemplateFilePathAsync(TargetPosition target,FileToCreate file)
    {

        bool directFileMatchingPredicate(string path) => Path.GetFileName(path).Equals(file.type.ToString() + _defaultExt, StringComparison.OrdinalIgnoreCase);
        var tmpFile = _templateFiles.FirstOrDefault(directFileMatchingPredicate);

        var ns = target.FolderNS;
        if(!string.IsNullOrEmpty(file.path))
        {
            ns += '.' + file.path.Replace('/', '.');
        }

        string content = "";
        
        using (var reader = new StreamReader(tmpFile))
        {
             var con = await reader.ReadToEndAsync();

            content = con.Replace("{namespace}", ns)
                          .Replace("{name}", file.name);
        }

        string directoryPath = Path.Combine(target.FolderPath, file.path);

        Directory.CreateDirectory(directoryPath);

        var fileName = file.type == FileTypes.QueryHandler || file.type == FileTypes.CommandHandler
            ? file.name + "Handler"
            : file.name;

        var filePath = Path.Combine(
            directoryPath, 
            fileName+ ".cs"
            );

        await WriteToDiskAsync(filePath, content);

        target.Project.ProjectItems.AddFromFile(filePath);

    }

    private static async Task WriteToDiskAsync(string file, string content)
    {
        using (StreamWriter writer = new StreamWriter(file, false))
        {
            await writer.WriteAsync(content);
        }
    }

}
