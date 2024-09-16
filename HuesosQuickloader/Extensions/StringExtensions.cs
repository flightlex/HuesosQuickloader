using System.IO;

namespace HuesosQuickloader.Extensions;

public static class StringExtensions
{
    public static string CombineWith(this string path, params string[] parts)
    {
        return Path.Combine([path, .. parts]);
    }
}
