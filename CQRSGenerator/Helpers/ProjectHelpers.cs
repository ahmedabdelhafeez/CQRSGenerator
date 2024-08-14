using EnvDTE;
using EnvDTE80;
using System.Linq;
using System.Threading.Tasks;


namespace CQRSGenerator.Helpers;

public static class ProjectHelpers
{
    private static readonly DTE2 _dte = CQRSGeneratorPackage._dte;
    public static async Task<TargetPosition> GetSelectedPositionAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        Array items = (Array)_dte.ToolWindows.SolutionExplorer.SelectedItems;

        if (items.Length == 1)
        {
            UIHierarchyItem selection = items.Cast<UIHierarchyItem>().First();

            if (selection.Object is Project project)
            {
                string projectPath = project.Properties.Item("FullPath").Value.ToString();

                return new TargetPosition
                {
                    Project = project,
                    TargetPath = projectPath,
                    RootNS = project.GetNamespace(),
                };
            }
            else if (selection.Object is ProjectItem projectItem)
            {
                Project pro = projectItem.ContainingProject;
                string folderPath = projectItem.FileNames[1];
                return new TargetPosition
                {
                    Project = pro,
                    TargetPath = folderPath,
                    RootNS = pro.GetNamespace(folderPath),
                };

            }
        }

        return  null;
    }

    public static string GetNamespace(this Project project, string folderPath="")
    {
        ThreadHelper.ThrowIfNotOnUIThread();


        var rootNamespace = project.Properties.Item("DefaultNamespace")?.Value.ToString() ?? project.Properties.Item("RootNamespace")?.Value.ToString();
        if (rootNamespace == null)
            throw new InvalidOperationException("Unable to determine the root namespace of the project.");

        string projectPath = project.Properties.Item("FullPath").Value.ToString();

        string relativePath = folderPath.Replace(projectPath, "").Replace("\\", ".").TrimEnd('.');
        string itemNamespace = relativePath.StartsWith(".") ? relativePath.Substring(1) : relativePath;

        return string.IsNullOrEmpty(itemNamespace) ? rootNamespace : $"{rootNamespace}.{itemNamespace}";
    }

    public static string Pluralize(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        if (word.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
            !IsVowel(word[word.Length - 2]))
        {
            return word.Substring(0, word.Length - 1) + "ies";
        }

        if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("sh", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("z", StringComparison.OrdinalIgnoreCase))
        {
            return word + "es";
        }

        return word + "s";
    }

    private static bool IsVowel(char c)
    {
        return "aeiouAEIOU".IndexOf(c) >= 0;
    }

}

