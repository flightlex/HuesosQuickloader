namespace HuesosQuickloader.Models;

public sealed class DownloadProgress
{
    public long? TotalBytes { get; set; }
    public long BytesRead { get; set; }
    public double Percentage => TotalBytes.HasValue ? (double)BytesRead / TotalBytes.Value * 100 : 0;
}