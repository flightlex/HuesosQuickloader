using System.Windows;

namespace HuesosQuickloader.Services.Dialog;

public sealed class DialogService : IDialogService
{
    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowMessage(string message)
    {
        MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowWarning(string message)
    {
        MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public bool ShowYesNoWarning(string message)
    {
        return MessageBox.Show(message, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }
}
