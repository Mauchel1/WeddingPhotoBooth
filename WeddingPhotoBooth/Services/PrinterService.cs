using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WeddingPhotoBooth.Services;

public class PrinterService
{
    public bool PrintFile(
        string fileName,
        string printerName)
    {
        try
        {
            if (!File.Exists(fileName))
                return false;

            BitmapImage image = new();

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(fileName);
            image.EndInit();
            image.Freeze();

            return PrintImage(image, printerName);
        }
        catch
        {
            return false;
        }
    }

    public bool PrintImage(
        BitmapSource image,
        string printerName)
    {
        try
        {
            System.Windows.Controls.PrintDialog dialog = new();

            LocalPrintServer server = new();

            PrintQueue? queue =
                server.GetPrintQueues()
                      .FirstOrDefault(
                          p => p.Name == printerName);

            if (queue == null)
                return false;

            dialog.PrintQueue = queue;

            System.Windows.Controls.Image visual = new()
            {
                Source = image,
                Stretch = Stretch.Uniform
            };

            // 10x15 cm bei 96 DPI
            System.Windows.Size paperSize = new(
                378,
                567);

            visual.Measure(paperSize);
            visual.Arrange(
                new Rect(
                    new System.Windows.Point(),
                    paperSize));

            dialog.PrintVisual(
                visual,
                "Wedding Photo Booth");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public List<string> GetInstalledPrinters()
    {
        LocalPrintServer server = new();

        return server
            .GetPrintQueues()
            .Select(p => p.Name)
            .OrderBy(p => p)
            .ToList();
    }
}