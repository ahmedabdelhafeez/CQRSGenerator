using CQRSGenerator.Helpers;
using Microsoft.VisualStudio.PlatformUI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace CQRSGenerator;

public partial class FileNameDialog : DialogWindow
{
    private string DEFAULT_TEXT = "Enter an entity name";
    private static readonly Regex _reservedFileNamePattern = new Regex($@"(?i)^(PRN|AUX|NUL|CON|COM\d|LPT\d)(\.|$)");
    private static readonly HashSet<char> _invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
    private static readonly HashSet<string> _reservedKeywords = new HashSet<string> { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while" };

    public FileNameDialog(string folder)
    {
        InitializeComponent();

        lblFolder.Content = string.Format("{0}/", folder);

        Loaded += (s, e) =>
        {
            Title = Vsix.Name;

            txtName.Focus();
            txtName.CaretIndex = 0;
            txtName.Text = DEFAULT_TEXT;
            txtName.Select(0, txtName.Text.Length);

            txtName.PreviewKeyDown += (a, b) =>
            {
                errorMessage.Content = null;
                if (b.Key == Key.Escape)
                {
                    if (string.IsNullOrWhiteSpace(txtName.Text) || txtName.Text == DEFAULT_TEXT)
                    {
                        Close();
                    }
                    else
                    {
                        txtName.Text = string.Empty;
                    }
                }
                else if (txtName.Text.Equals(DEFAULT_TEXT))
                {
                    txtName.Text = string.Empty;
                    btnCreate.IsEnabled = true;
                }
            };
        };
    }

    public string Input => txtName.Text.Trim();

    public CreateTypes Type { get; set; } = CreateTypes.CQRS;

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var validateRes = ValidateName();
        if (!string.IsNullOrEmpty(validateRes))
        {
            errorMessage.Content = validateRes;
            return;
        }

        DialogResult = true;
        Close();
    }

    private void CreateTypeClickEvent(object sender, RoutedEventArgs e)
    {
        bool isDefault = Input.Equals(DEFAULT_TEXT);
        if (IsCommand.IsChecked == true)
        {
            Type = CreateTypes.Command;
            DEFAULT_TEXT = "Enter a Command name";
        }
        else if (IsQuery.IsChecked == true)
        {
            Type = CreateTypes.Query;
            DEFAULT_TEXT = "Enter a Query name";
        }
        else
        {
            Type = CreateTypes.CQRS;
            DEFAULT_TEXT = "Enter an Entity name";
        }
        if(string.IsNullOrEmpty(Input)||isDefault)
        {
            txtName.Text = DEFAULT_TEXT;
        }

    }

    private string ValidateName()
    {
        if(string.IsNullOrEmpty(Input))
        {
            return $"Name Is required";
        }
        if (_reservedFileNamePattern.IsMatch(Input))
        {
            return $"The name '{Input}' is a system reserved name.";
        }
        if (Input.Any(c => _invalidFileNameChars.Contains(c)))
        {
            return $"The name '{Input}' contains invalid characters.";
        }
        if (Input.Contains(" "))
        {
            return $"The name '{Input}' cannot contain spaces.";
        }
        if (_reservedKeywords.Contains(Input))
        {
            return $"The name '{Input}' is a reserved keyword in C#.";
        }
        return null;
    }
}