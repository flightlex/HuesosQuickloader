namespace HuesosQuickloader.Services.Dialog;

public interface IDialogService
{
    void ShowError(string message);
    void ShowMessage(string message);
    void ShowWarning(string message);
    bool ShowYesNoWarning(string message);
}
