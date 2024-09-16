using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace HuesosQuickloader.Utils;

public static class SystemUtils
{
    public static void OpenFileLocation(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The file was not found.", filePath);
        }

        var argument = $"/select,\"{filePath}\"";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = argument,
            UseShellExecute = true
        };

        Process.Start(processStartInfo);
    }

    public static void OpenInBrowser(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be null or empty.", nameof(input));
        }

        var url = input;

        if (!IsValidUrl(input))
        {
            url = "http://" + input;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            throw new InvalidOperationException("Couldn't open this link");
        }
    }

    private static bool IsValidUrl(string url)
    {
        return Regex.IsMatch(url, @"^(http|https)://", RegexOptions.IgnoreCase);
    }

}
