using System.IO;

namespace WeddingPhotoBooth.Services;

public class FileWatcherService
{
    private readonly FileSystemWatcher _watcher;

    public event Action<string>? ImageDetected;
    private TaskCompletionSource<string>? _tcs;

    public FileWatcherService(string folder)
    {
        Directory.CreateDirectory(folder);

        _watcher = new FileSystemWatcher(folder)
        {
            Filter = "*.jpg",
            EnableRaisingEvents = true
        };

        _watcher.Created += OnCreated;
        _watcher.Created += Watcher_Created;

    }

    private async void Watcher_Created(object? sender, FileSystemEventArgs e)
    {

        await WaitUntilFileIsReadyAsync(e.FullPath);
        _tcs?.TrySetResult(e.FullPath);
    }

    private void OnCreated(object? sender, FileSystemEventArgs e)
    {
        ImageDetected?.Invoke(e.FullPath);
        _tcs?.TrySetResult(e.FullPath);

    }

    private async Task WaitUntilFileIsReadyAsync(string path)
    {
        while (true)
        {
            try
            {
                using var stream = File.Open(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None);

                break; // Datei ist vollständig geschrieben
            }
            catch
            {
                await Task.Delay(100);
            }
        }
    }

    public async Task<string?> WaitForNewImageAsync(TimeSpan timeout)
    {
        _tcs = new TaskCompletionSource<string>();

        var completed = await Task.WhenAny(
            _tcs.Task,
            Task.Delay(timeout));

        if (completed == _tcs.Task)
            return await _tcs.Task;

        return null;
    }

    public void Start()
    {
        _watcher.EnableRaisingEvents = true;
    }

    public void Stop()
    {
        _watcher.EnableRaisingEvents = false;
    }
}


