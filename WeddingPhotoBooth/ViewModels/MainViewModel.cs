using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using WeddingPhotoBooth.Models;
using WeddingPhotoBooth.Services;
using static System.Windows.Forms.AxHost;

namespace WeddingPhotoBooth.ViewModels;

public class MainViewModel : INotifyPropertyChanged, IDisposable
{

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    private IntPtr? _previousWindow;
    public event PropertyChangedEventHandler? PropertyChanged;
    private int _currentPhoto = 0;
    private readonly Settings _settings;
    private readonly FileWatcherService _watcher;
    private readonly SessionService _sessionService;
    private readonly CameraService _cameraService = new();
    private PhotoStripGenerator generator;
    private readonly PrinterService _printerService = new PrinterService();
    //private readonly DigiUsbService _usb = new();
    private NanoController? _nano;


    public ImageSource? PreviewImage
    {
        get => _previewImage;
        set
        {
            _previewImage = value;
            OnPropertyChanged();
        }
    }

    private ImageSource? _previewImage;

    public MainViewModel()
    {
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            return;

        _settings = SettingsService.Load();

        _sessionService = new SessionService(_settings);

        _watcher = new FileWatcherService(_settings.SaveDirectory);
        _watcher.ImageDetected += OnImageDetected;
        _watcher.Start();

        generator = new PhotoStripGenerator();

        _nano = NanoController.Connect();
        if (_nano == null)
        {
            System.Windows.MessageBox.Show("Nano nicht gefunden.");
        } 
        
        /*if (!_usb.Connect())
        {
            System.Windows.MessageBox.Show("DigiUSB nicht gefunden.");
        }*/

    }

    private void OnImageDetected(string file)
    {
        System.Diagnostics.Debug.WriteLine($"Bild erkannt: {file}");
    }

    public int CurrentPhoto
    {
        get => _currentPhoto;
        set
        {
            _currentPhoto = value;
            OnPropertyChanged();
        }
    }
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

    public async Task StartPhotoSessionAsync()
    {
        _sessionService.CreateNewSession();


        if (_nano != null)
        {
            if (!_nano.RelayOn())
            {
                System.Diagnostics.Debug.WriteLine("Nano antwortet nicht.");
            }
        }

        //_usb.Send((byte)DigiCommand.RelayOn);

        //System.Windows.Application.Current.MainWindow.Opacity = 0.2;
        for (int i = 1; i <= _settings.PicturesPerSession; i++)
        {
            CurrentPhoto = i;

            if (i > 1)
            {
                await RunCountdownAsync(_settings.CountdownNext);
            }
            else
            {
                await RunCountdownAsync(_settings.CountdownFirst);
            }

            if (!await CaptureAsync())
            {
                // TODO: Im UI anzeigen
                System.Windows.MessageBox.Show(
                    "Es ist ein Fehler beim Aufnehmen des Fotos aufgetreten. Bitte überprüfe die Kamera und versuche es erneut.");
                State = PhotoBoothState.Idle;
                _nano?.RelayOff();
                //_usb.Send((byte)DigiCommand.RelayOff);
                //System.Windows.Application.Current.MainWindow.Opacity = 1.0;
                return;
            }
        }

        _nano?.RelayOff();
        //_usb.Send((byte)DigiCommand.RelayOff);

        //System.Windows.Application.Current.MainWindow.Opacity = 1.0;
        State = PhotoBoothState.Processing;

        /*var preview =
            generator.GeneratePreview(
                _sessionService.CurrentSession.Photos,
                _settings);
        */

        var printImage =
            generator.GeneratePrint(
        _sessionService.CurrentSession.Photos,
        _settings);


        var exportService =
            new ImageExportService();

        string stripFile =
            Path.Combine(
                _sessionService.CurrentSession.SessionFolder,
                "Strip.jpg");

        System.Diagnostics.Debug.WriteLine(
            $"Print: {printImage.PixelWidth} x {printImage.PixelHeight}");

        exportService.SaveJpeg(
            printImage,
            stripFile);

        _sessionService.CurrentSession.StripFile =
            stripFile;

        PreviewImage = new BitmapImage(new Uri(stripFile)); ;

        State = PhotoBoothState.Preview;
    }

    private async Task RunCountdownAsync(int seconds)
    {
        State = PhotoBoothState.Countdown;

        for (int i = seconds; i >= 1; i--)
        {
            CountdownValue = i;
            await Task.Delay(1000);
        }

        CountdownValue = 0;
    }

    public void Restart()
    {
        State = PhotoBoothState.Idle;
        CurrentPhoto = 0;
        
    }

    private async Task<bool> CaptureAsync()
    {
        State = PhotoBoothState.Capturing;

        const int maxRetries = 3;
        _previousWindow = GetForegroundWindow();
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            bool triggered = await _cameraService.TriggerCaptureAsync(
                _settings.OmCaptureProcessName);

            if (!triggered)
            {
                // TODO: Im UI anzeigen
                System.Windows.MessageBox.Show(
                    "OM Capture wurde nicht gefunden oder besitzt kein aktives Fenster.");
                State = PhotoBoothState.Idle;
                return false;
            }





        string? image = await _watcher.WaitForNewImageAsync(
                TimeSpan.FromSeconds(8));

            
            if (image != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var window =
                        System.Windows.Application.Current.MainWindow;

                    window.Activate();
                    window.Topmost = true;
                    window.Topmost = false;
                    window.Focus();
                });
                // Hier zur Session hinzufügen
                _sessionService.AddPhoto(image);
                System.Diagnostics.Debug.WriteLine($"Neues Bild: {image}");

                return true;
            }
            else
            {
                Console.WriteLine(
                "kein Bild gefunden");
            }

                // Kurze Pause vor erneutem Versuch
                await Task.Delay(500);
        }

        return false;
    }

    public void PrintCurrentSession()
    {
        string? stripFile =
            _sessionService.CurrentSession.StripFile;

        if (string.IsNullOrWhiteSpace(stripFile))
        {
            System.Windows.MessageBox.Show(
                "Kein Fotostreifen vorhanden.");
            return;
        }

        bool success =
            _printerService.PrintFile(
                stripFile,
                _settings.PrinterName);

        if (!success)
        {
            System.Windows.MessageBox.Show(
                $"Druck auf '{_settings.PrinterName}' fehlgeschlagen.");
        }
    }

    public void Dispose()
    {
        try
        {
            _nano?.RelayOff();
        }
        catch
        {
            // Nano eventuell schon weg
        }

        _nano?.Dispose();
    }

}