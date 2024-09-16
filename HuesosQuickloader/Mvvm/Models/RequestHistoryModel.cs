using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;

namespace HuesosQuickloader.Mvvm.Models;

public sealed partial class RequestHistoryModel
{
    [JsonIgnore]
    public IRelayCommand<RequestHistoryModel> DownloadAgainCommand { get; set; } = null!;

    [JsonIgnore]
    public IRelayCommand<RequestHistoryModel> OpenManuallyCommand { get; set; } = null!;
    
    [JsonIgnore]
    public IRelayCommand<RequestHistoryModel> DeleteCommand { get; set; } = null!;

    public string Url { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
