using System;
using System.Text;

namespace HuesosQuickloader.Extensions;

public static class TimeSpanExtensions
{
    public static string ConvertToClosest(this TimeSpan value)
    {
        StringBuilder sb = new();

        if (value.Days > 0)
        {
            sb.Append(value.Days).Append("d ");
        }

        if (value.Hours > 0)
        {
            sb.Append(value.Hours).Append("h ");
        }

        if (value.Minutes > 0)
        {
            sb.Append(value.Minutes).Append("m ");
        }

        if (value.Seconds > 0)
        {
            sb.Append(value.Seconds).Append("s ");
        }

        if (value.Milliseconds > 0)
        {
            sb.Append(value.Milliseconds).Append("ms ");
        }

        return sb.ToString().TrimEnd();
    }
}
