using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FontAwesome.Sharp;
using HuesosQuickloader.Clients;
using HuesosQuickloader.Extensions;
using HuesosQuickloader.Models;
using HuesosQuickloader.Mvvm.Models;
using HuesosQuickloader.Services.Config;
using HuesosQuickloader.Services.Dialog;
using HuesosQuickloader.Utils;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace HuesosQuickloader.Mvvm.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly IConfigService _configService;

    private NetworkClient? _networkClient;
    private FileStream? _fileStream;
    private FileInfo? _fileInfo;

    public MainViewModel(IDialogService dialogService, IConfigService configService, CaptionViewModel captionViewModel)
    {
        _dialogService = dialogService;
        _configService = configService;

        CaptionViewModel = captionViewModel;
        LoadRequestHistory();
    }

    public CaptionViewModel CaptionViewModel { get; }

    public IconChar DownloadButtonChar => IsDownloading ? IconChar.StopCircle : IconChar.Download;

    public DataSize TotalDataSize { get; private set; } = new();
    public DataSize ReadDataSize { get; private set; } = new();
    public double TotalPercentage { get; private set; } = 0;

    [ObservableProperty]
    private ObservableCollection<RequestHistoryModel> _requestHistory = [];

    [ObservableProperty]
    private string _downloadLink = string.Empty;

    private bool _isDownloading = false;
    public bool IsDownloading
    {
        get => _isDownloading;
        set
        {
            if (_isDownloading == value)
                return;

            _isDownloading = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DownloadButtonChar));
        }
    }

    [RelayCommand]
    private void OnDragMove(Window window) => window.DragMove();

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task OnDownload()
    {
        IsDownloading = !IsDownloading;

        if (!IsDownloading)
        {
            DisposeAll(deleteFile: true);
            return;
        }

        _fileInfo = FileDialogUtils.SaveFile("Any File (*.*) | *.*", Path.GetFileName(DownloadLink));
        if (_fileInfo is null)
        {
            IsDownloading = false;
            return;
        }

        // adding history
        AddNewRequestHistory();

        _networkClient = new();

        _networkClient.SetTimeout(TimeSpan.FromSeconds(5));
        _networkClient.SetProgressReporter(ProgressReported);

        _fileStream = File.Open(_fileInfo.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _networkClient.DownloadFileWithProgressAsync(DownloadLink, _fileStream);
            stopwatch.Stop();
            ExecutePostDownloadActions(stopwatch);
            DisposeAll();
        }
        catch (InvalidOperationException)
        {
            DisposeAll(deleteFile: true);
            _dialogService.ShowError("Couldn't reach the servers, recheck your link and try again.");
        }
        catch (HttpRequestException)
        {
            DisposeAll(deleteFile: true);
            _dialogService.ShowError("Couldn't reach the servers, recheck your link and try again.");
        }
        catch
        {
            DisposeAll(deleteFile: true);
            _dialogService.ShowWarning("The download was interrupted.");
        }

        IsDownloading = false;
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task OnDownloadAgain(RequestHistoryModel requestHistory)
    {
        DisposeAll(deleteFile: true);
        DownloadLink = requestHistory.Url;
        await OnDownload();
    }

    [RelayCommand]
    private void OnOpenManually(RequestHistoryModel requestHistory)
    {
        try
        {
            SystemUtils.OpenInBrowser(requestHistory.Url);
        } catch
        {
            _dialogService.ShowError("Couldn't open this url in your browser.");
        }
    }

    [RelayCommand]
    private void OnDelete(RequestHistoryModel requestHistory)
    {
        var isYes = _dialogService.ShowYesNoWarning("Do you want to delete this request from the history?");

        if (!isYes)
            return;

        RequestHistory.Remove(requestHistory);
        UpdateConfig();
    }

    [RelayCommand]
    private void OnDeleteAll()
    {
        var isYes = _dialogService.ShowYesNoWarning("Do you want to delete the ENTIRE history?");

        if (!isYes)
            return;

        RequestHistory.Clear();
        UpdateConfig();
    }

    private void LoadRequestHistory()
    {
        RequestHistory.Clear();
        var config = _configService.Read();

        foreach (var request in config.RequestHistory)
        {
            RequestHistory.Add(request);

            request.DownloadAgainCommand = DownloadAgainCommand;
            request.OpenManuallyCommand = OpenManuallyCommand;
            request.DeleteCommand = DeleteCommand;
        }
    }

    private void AddNewRequestHistory()
    {
        var request = new RequestHistoryModel() { Url = DownloadLink, Date = DateTime.Now };
        request.DownloadAgainCommand = DownloadAgainCommand;
        request.OpenManuallyCommand = OpenManuallyCommand;
        request.DeleteCommand = DeleteCommand;

        RequestHistory.Add(request);
        UpdateConfig();
    }

    private void UpdateConfig()
    {
        var cfg = _configService.Read();
        cfg.RequestHistory = RequestHistory;
        _configService.Write(cfg);
    }

    private void DisposeAll(bool deleteFile = false)
    {
        _networkClient?.Dispose();
        _networkClient = null;

        _fileStream?.Dispose();
        _fileStream = null;

        _fileInfo?.Refresh();
        if (deleteFile && _fileInfo?.Exists == true)
            _fileInfo?.Delete();

        _fileInfo = null;
    }

    private void ExecutePostDownloadActions(Stopwatch stopwatch)
    {
        var config = _configService.Read();

        if (config.OpenFileWhenDownloaded)
        {
            SystemUtils.OpenFileLocation(_fileInfo!.FullName);
        }

        if (config.ShowMessageWhenFinished)
        {
            _dialogService.ShowMessage($"The download successfully finished in {stopwatch.Elapsed.ConvertToClosest()}!");
        }
    }

    private void ProgressReported(DownloadProgress progress)
    {
        TotalDataSize.RoundFromBytes(progress.TotalBytes);
        ReadDataSize.RoundFromBytes(progress.BytesRead);
        TotalPercentage = Math.Round(((double)progress.BytesRead / progress.TotalBytes ?? 0) * 100, 2).ZeroIfNaN();

        OnPropertyChanged(nameof(TotalDataSize));
        OnPropertyChanged(nameof(ReadDataSize));
        OnPropertyChanged(nameof(TotalPercentage));
    }
}
