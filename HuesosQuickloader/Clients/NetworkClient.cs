using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using HuesosQuickloader.Models;

namespace HuesosQuickloader.Clients;

public sealed class NetworkClient : IDisposable
{
    private HttpClient _httpClient;
    private CancellationTokenSource _cancellationTokenSource;

    private long _progressReportInterval = 80;
    private Action<DownloadProgress>? _progressReporter;

    private long _totalBytesRead = 0;
    private long? _totalBytes = null;

    public NetworkClient()
    {
        _httpClient = new();
        _cancellationTokenSource = new();
    }

    public void SetProgressReportFrequency(long eachIteration)
    {
        _progressReportInterval = eachIteration;
    }

    public void SetTimeout(TimeSpan timeout)
    {
        _httpClient.Timeout = timeout;
    }

    public void SetProgressReporter(Action<DownloadProgress> reporter)
    {
        _progressReporter = reporter;
    }

    public async Task DownloadFileWithProgressAsync(string url, Stream destinationStream)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, _cancellationTokenSource.Token);

        response.EnsureSuccessStatusCode();
        _totalBytes = response.Content.Headers.ContentLength;

        using Stream contentStream = await response.Content.ReadAsStreamAsync();

        await ProcessDownloadWithProgressAsync(contentStream, destinationStream);
    }

    private async Task ProcessDownloadWithProgressAsync(Stream contentStream, Stream destinationStream)
    {
        byte[] buffer = new byte[8192];
        int bytesRead;
        long index = 0;

        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, _cancellationTokenSource.Token)) != 0)
        {
            await destinationStream.WriteAsync(buffer, 0, bytesRead, _cancellationTokenSource.Token);
            Interlocked.Add(ref _totalBytesRead, bytesRead);
            Interlocked.Increment(ref index);

            if (index >= _progressReportInterval)
            {
                index = 0;
                ReportProgress();
            }
        }

        ReportProgress();
    }

    private void ReportProgress()
    {
        if (_progressReporter is null)
            return;

        var progress = new DownloadProgress
        {
            TotalBytes = _totalBytes,
            BytesRead = _totalBytesRead
        };

        _progressReporter(progress);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();  // Cancel pending requests
        _httpClient.Dispose();
        _cancellationTokenSource.Dispose();
    }
}
