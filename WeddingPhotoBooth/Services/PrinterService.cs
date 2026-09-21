using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PrintDialog = System.Windows.Controls.PrintDialog;
using Image = System.Drawing.Image;

namespace WeddingPhotoBooth.Services;

public class PrinterService
{
    // ============================================================
    // AUSWAHL DES DRUCKWEGS
    // ============================================================
    //
    // true  = GDI+ / System.Drawing.Printing
    // false = bisheriger WPF PrintVisual-Weg
    //
    private const bool UseGdiPrinting = true;


    // ============================================================
    // ÖFFENTLICHER DRUCKAUFRUF
    // ============================================================

    public bool PrintFile(
        string fileName,
        string printerName)
    {
        try
        {
            if (!File.Exists(fileName))
                return false;

            if (UseGdiPrinting)
            {
                return PrintImageGdi(
                    fileName,
                    printerName);
            }

            BitmapImage image = new();

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(fileName);
            image.EndInit();
            image.Freeze();

            return PrintImageWpf(
                image,
                printerName);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            return false;
        }
    }


    // ============================================================
    // GDI+ DRUCKWEG
    // ============================================================

    private bool PrintImageGdi(
        string fileName,
        string printerName)
    {
        try
        {
            using Image image = Image.FromFile(fileName);

            PrintDocument document = new();

            document.PrinterSettings.PrinterName =
                printerName;

            if (!document.PrinterSettings.IsValid)
                return false;

            // ---------------------------------------------------------
            // SELPHY Borderless-Kompensation
            // ---------------------------------------------------------
            //
            // Gemessener Verlust bei 1181 x 1748:
            // links/rechts  ~35 px
            // oben          ~50 px
            // unten         ~60 px
            //
            // Wir geben dem Drucker einen künstlichen "Opferrand".
            // Das Originalbild bleibt dabei unverändert.
            // ---------------------------------------------------------

            const int fakeBorderX = 35;
            const int fakeBorderYup = 50;
            const int fakeBorderYdown = 60;

            // ----------------------------------------------------
            // Papier: Jap. Postkarte = 100 x 148 mm
            // ----------------------------------------------------

            PaperSize paperSize = new(
                "Jap. Postkarte",
                HundredthsOfAnInch(100),
                HundredthsOfAnInch(148));

            document.DefaultPageSettings.PaperSize =
                paperSize;


            // ----------------------------------------------------
            // Randlos versuchen
            // ----------------------------------------------------
            //
            // Die Windows-Druckertreiber können hier
            // unterschiedliche Unterstützung bieten.
            // Wir setzen zunächst keinen künstlichen Rand.
            // ----------------------------------------------------

            document.DefaultPageSettings.Margins =
                new Margins(0, 0, 0, 0);


            document.PrintPage += (_, e) =>
            {
                Graphics g = e.Graphics;

                // Hochwertige Skalierung
                g.InterpolationMode =
                    System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                g.SmoothingMode =
                    System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                g.PixelOffsetMode =
                    System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                g.CompositingQuality =
                    System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                Rectangle page = e.PageBounds;

                // Unsere virtuelle Druckfläche
                const double referenceWidth = 1181.0;
                const double referenceHeight = 1748.0;

                // Das eigentliche Bild
                const double imageWidth = 1108.0;
                const double imageHeight = 1640.0;

                // Skalierungsfaktoren auf die tatsächliche Druckfläche
                double scaleX =
                    page.Width / referenceWidth;

                double scaleY =
                    page.Height / referenceHeight;

                int drawWidth =
                    (int)Math.Round(imageWidth * scaleX);

                int drawHeight =
                    (int)Math.Round(imageHeight * scaleY);

                int x =
                    page.X +
                    (page.Width - drawWidth) / 2;

                int y =
                    page.Y +
                    (page.Height - drawHeight) / 2;

                // Weißer Hintergrund
                g.Clear(Color.White);

                // Bild exakt mittig
                g.DrawImage(
                    image,
                    new Rectangle(
                        x,
                        y,
                        drawWidth,
                        drawHeight));

                e.HasMorePages = false;
            };

            /*
            document.PrintPage += (_, e) =>
            {
                // ------------------------------------------------
                // Gesamte verfügbare Seite
                // ------------------------------------------------

                Rectangle pageBounds =
                    e.PageBounds;


                // ------------------------------------------------
                // Bild proportional auf die Seite skalieren.
                //
                // Wir verwenden "Fill":
                // Die komplette Seite wird ausgefüllt.
                // ------------------------------------------------

                double scaleX =
                    (double)pageBounds.Width /
                    image.Width;

                double scaleY =
                    (double)pageBounds.Height /
                    image.Height;

                double scale =
                    Math.Max(
                        scaleX,
                        scaleY);


                int width =
                    (int)Math.Round(
                        image.Width * scale);

                int height =
                    (int)Math.Round(
                        image.Height * scale);


                int x =
                    pageBounds.X +
                    (pageBounds.Width - width) / 2;

                int y =
                    pageBounds.Y +
                    (pageBounds.Height - height) / 2;


                e.Graphics.DrawImage(
                    image,
                    new Rectangle(
                        x,
                        y,
                        width,
                        height));


                e.HasMorePages = false;
            };*/


            document.Print();

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            return false;
        }
    }


    // ============================================================
    // BISHERIGER WPF-DRUCKWEG
    // ============================================================

    private bool PrintImageWpf(
        BitmapSource image,
        string printerName)
    {
        try
        {
            PrintDialog dialog = new();

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

                // Gesamte Druckfläche ausfüllen
                Stretch =
                    System.Windows.Media.Stretch.Fill
            };


            // ----------------------------------------------------
            // Jap. Postkarte: 100 x 148 mm bei 96 DPI
            // ----------------------------------------------------

            System.Windows.Size paperSize =
                new(
                    377.95,
                    559.37);


            visual.Measure(
                paperSize);

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
            System.Diagnostics.Debug.WriteLine(ex);
            return false;
        }
    }


    // ============================================================
    // HILFSMETHODE
    // ============================================================

    private static int HundredthsOfAnInch(
        double millimeters)
    {
        return (int)Math.Round(
            millimeters /
            25.4 *
            100);
    }


    // ============================================================
    // DRUCKER AUFLISTEN
    // ============================================================

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