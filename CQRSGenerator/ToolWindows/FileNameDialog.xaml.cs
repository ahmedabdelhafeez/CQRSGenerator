using Microsoft.VisualStudio.PlatformUI;
using System.Windows;
using System.Windows.Input;
using CQRSGenerator.Helpers;

namespace CQRSGenerator;

public partial class FileNameDialog : DialogWindow
{
    private const string DEFAULT_TEXT = "Enter an entity name";

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
                else if (txtName.Text == DEFAULT_TEXT)
                {
                    txtName.Text = string.Empty;
                    btnCreate.IsEnabled = true;
                }
            };

        };
    }

    public string Input => txtName.Text.Trim();

    public CreateTypes Type { get; set; }= CreateTypes.CQRS;
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CreateTypeClickEvent(object sender, RoutedEventArgs e)
    {
        if (IsCommand.IsChecked == true)
        {
            Type = CreateTypes.Command;
        }
        else if(IsQuery.IsChecked == true)
        {
            Type=CreateTypes.Query;
        }
        else
        {
            Type = CreateTypes.CQRS;
        }
    }
}
