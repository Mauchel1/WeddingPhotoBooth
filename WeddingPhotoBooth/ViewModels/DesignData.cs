using System;
using System.Windows.Media.Imaging;
using WeddingPhotoBooth.Models;
using WeddingPhotoBooth.ViewModels;
using static System.Windows.Forms.AxHost;

namespace WeddingPhotoBooth.DesignData;

public class DesignMainViewModel : MainViewModel
{
    public DesignMainViewModel()
    {
        // Gewünschte Ansicht in der Vorschau in Visual Studio festlegen
        State = PhotoBoothState.Idle;

        // Beispieldaten
        CurrentPhoto = 2;
        CountdownValue = 3;
        
        // Optional ein Testbild
         PreviewImage = new BitmapImage(new Uri("pack://siteoforigin:,,,/template.jpg"));
    }
}