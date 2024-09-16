using Microsoft.Win32;
using System.IO;

namespace HuesosQuickloader.Utils;

public static class FileDialogUtils
{
    public static FileInfo? SaveFile(string? filter = null, string? fileName = null)
    {
        var sfd = new SaveFileDialog()
        {
            Filter = filter,
            FileName = fileName
        };

        if (sfd.ShowDialog() != true)
            return null;

        return new FileInfo(sfd.FileName);
    }
}
