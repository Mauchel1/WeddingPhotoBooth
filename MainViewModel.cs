using System.ComponentModel;
using System.Runtime.CompilerServices;
using WeddingPhotoBooth.Models;

namespace WeddingPhotoBooth.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    // STATE
    private PhotoBoothState _state = PhotoBoothState.Idle;
    public PhotoBoothState State
    {
        get => _state;
        set { _state = value; OnPropertyChanged(); }
    }

    // COUNTDOWN
    private int _countdownValue;
    public int CountdownValue
    {
        get => _countdownValue;
        set { _countdownValue = value; OnPropertyChanged(); }
    }

    // START BUTTON ENTRY POINT
    public async Task StartCountdownAsync(int seconds = 10)
    {
        State = PhotoBoothState.Countdown;

        for (int i = seconds; i >= 1; i--)
        {
            CountdownValue = i;
            await Task.Delay(1000);
        }

        CountdownValue = 0;

        await StartCaptureAsync();
    }

    private async Task StartCaptureAsync()
    {
        State = PhotoBoothState.Capturing;

        // später: OM Capture / Kamera Trigger
        await Task.Delay(500);

        await ProcessAsync();
    }

    private async Task ProcessAsync()
    {
        State = PhotoBoothState.Processing;

        await Task.Delay(1500);

        State = PhotoBoothState.Preview;
    }
}