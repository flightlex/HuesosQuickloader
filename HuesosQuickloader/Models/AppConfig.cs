using HuesosQuickloader.Mvvm.Models;
using System.Collections.Generic;

namespace HuesosQuickloader.Models;

public sealed class AppConfig
{
    public bool OpenFileWhenDownloaded { get; set; } = true;
    public bool ShowMessageWhenFinished { get; set; } = true;

    public IEnumerable<RequestHistoryModel> RequestHistory { get; set; } = [];
}
