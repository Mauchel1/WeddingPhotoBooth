using System.Globalization;
using System.IO;
using System.Runtime;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WeddingPhotoBooth.Models;

namespace WeddingPhotoBooth.Templates;

public class DoubleStripTemplate : IPhotoTemplate
{

    private BitmapImage LoadTemplate(string file)
    {
        var image = new BitmapImage();

        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = new Uri(file);
        image.EndInit();

        image.Freeze();

        return image;
    }

    public BitmapSource Create(
        List<string> photos,
        Settings settings
         )
    {
        return CreateLayout(
            photos,
            settings,
            600,
            900,
            96
            ); // Vorschaugröße
    }

    public BitmapSource CreatePrint(
        List<string> photos,
        Settings settings)
    {
        return CreateLayout(
            photos,
            settings,
            1181,
            1772,
            300
            ); // 10x15 @300dpi
    }

    private BitmapSource CreateLayout(
        List<string> photos,
        Settings settings,
        int width,
        int height,
        int dpi
        )
    {
        var visual = new DrawingVisual();

        using (var dc = visual.RenderOpen())
        {
            string templatePath =
                Path.Combine(
                    AppContext.BaseDirectory,
                    settings.TemplateFile);

            if (File.Exists(templatePath))
            {
                // Hintergrund zeichnen
                var template =
                    LoadTemplate(templatePath);
                dc.DrawImage(
                    template,
                    new Rect(
                        0,
                        0,
                        width,
                        height));

                //linker Strip
                for (int i = 0; i < photos.Count && i < 3; i++)
                {
                    DrawPhotoWithStretchingAllowed(
                        dc,
                        new Rect(
                            settings.PhotoSlots[i].X,
                            settings.PhotoSlots[i].Y,
                            settings.PhotoSlots[i].Width,
                            settings.PhotoSlots[i].Height),
                        photos[i]);
                }
                //rechter Strip
                for (int i = 0; i < photos.Count && i < 3; i++)
                {
                    DrawPhotoWithStretchingAllowed(
                        dc,
                        new Rect(
                            settings.PhotoSlots[i + 3].X,
                            settings.PhotoSlots[i + 3].Y,
                            settings.PhotoSlots[i + 3].Width,
                            settings.PhotoSlots[i + 3].Height),
                        photos[i]);
                }
            }
            else
            {
                dc.DrawRectangle(
                    System.Windows.Media.Brushes.White,
                    null,
                    new Rect(
                        0,
                        0,
                        width,
                        height));
            
                double margin = width * 0.04;
                double gap = width * 0.03;

                double headerHeight = height * 0.12;

                double stripWidth =
                    (width - margin * 2 - gap) / 2;

                DrawHeader(
                    dc,
                    settings.WeddingText,
                    settings.DateText,
                    margin,
                    stripWidth,
                    headerHeight);

                DrawHeader(
                    dc,
                    settings.WeddingText,
                    settings.DateText,
                    margin + stripWidth + gap,
                    stripWidth,
                    headerHeight);

                DrawStrip(
                    dc,
                    photos,
                    margin,
                    headerHeight,
                    stripWidth,
                    height - headerHeight - margin);

                DrawStrip(
                    dc,
                    photos,
                    margin + stripWidth + gap,
                    headerHeight,
                    stripWidth,
                    height - headerHeight - margin);

                DrawCutLine(dc, width, height);
            }

        }
        var bmp = new RenderTargetBitmap(
            width,
            height,
            96,
            96,
            PixelFormats.Pbgra32);

        bmp.Render(visual);
        return bmp;
        
        
    }

    private void DrawHeader(
    DrawingContext dc,
    string weddingText,
    string dateText,
    double stripX,
    double stripWidth,
    double headerHeight)
    {
        double titleSize = headerHeight * 0.25;
        double dateSize = headerHeight * 0.15;

        var title = CreateText(weddingText, titleSize);

        double titleX =
            stripX + (stripWidth - title.Width) / 2;

        dc.DrawText(
            title,
            new System.Windows.Point(titleX, 10));

        var date = CreateText(dateText, dateSize);

        double dateX =
            stripX + (stripWidth - date.Width) / 2;

        dc.DrawText(
            date,
            new System.Windows.Point(
                dateX,
                10 + titleSize + 5));
    }

    private void DrawStrip(
        DrawingContext dc,
        List<string> photos,
        double x,
        double y,
        double width,
        double height)
    {
        double gap = 10;

        double photoHeight =
            (height - gap * 4) / 3;

        System.Windows.Media.Pen border =
            new System.Windows.Media.Pen(
                System.Windows.Media.Brushes.LightGray,
                2);

        for (int i = 0; i < 3; i++)
        {
            double top =
                y + gap + i * (photoHeight + gap);

            Rect rect =
                new Rect(
                    x,
                    top,
                    width,
                    photoHeight);

            if (i < photos.Count)
            {
                DrawPhoto(
                    dc,
                    rect,
                    photos[i]);
            }
            else
            {
                dc.DrawRectangle(
                    System.Windows.Media.Brushes.LightGray,
                    border,
                    rect);

                var text =
                    CreateText(
                        $"Foto {i + 1}",
                        24);

                dc.DrawText(
                    text,
                    new System.Windows.Point(
                        rect.Left + 20,
                        rect.Top + 20));
            }

          

         
        }
    }

    private void DrawCutLine(
        DrawingContext dc,
        int width,
        int height)
    {
        System.Windows.Media.Pen pen =
            new System.Windows.Media.Pen(
                System.Windows.Media.Brushes.Black,
                1);

        pen.DashStyle =
            DashStyles.Dash;

        dc.DrawLine(
            pen,
            new System.Windows.Point(width / 2, 0),
            new System.Windows.Point(width / 2, height));
    }

    private FormattedText CreateText(
        string text,
        double size)
    {
        return new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            System.Windows.FlowDirection.LeftToRight,
            new Typeface("Segoe UI"),
            size,
            System.Windows.Media.Brushes.Black,
            1.0);
    }

    private BitmapImage LoadImage(string path)
    {
        var image = new BitmapImage();

        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = new Uri(path);
        image.EndInit();

        image.Freeze();

        return image;
    }


    private void DrawPhotoWithStretchingAllowed(
    DrawingContext dc,
    Rect targetRect,
    string imagePath)
    {
        if (!File.Exists(imagePath))
            return;

        var image = LoadImage(imagePath);

        dc.DrawImage(
            image,
            targetRect);
    }


    private void DrawPhotoNEW(
    DrawingContext dc,
    Rect targetRect,
    string imagePath)
    {
        if (!File.Exists(imagePath))
            return;

        var image = LoadImage(imagePath);

        double scale =
            Math.Min(
                targetRect.Width / image.PixelWidth,
                targetRect.Height / image.PixelHeight);

        double width =
            image.PixelWidth * scale;

        double height =
            image.PixelHeight * scale;

        double x =
            targetRect.X +
            (targetRect.Width - width) / 2;

        double y =
            targetRect.Y +
            (targetRect.Height - height) / 2;

        dc.DrawImage(
            image,
            new Rect(
                x,
                y,
                width,
                height));
    }

    private void DrawPhoto(
    DrawingContext dc,
    Rect targetRect,
    string imagePath)
    {
        System.Diagnostics.Debug.WriteLine(
        $"Target: {targetRect}");
        if (!File.Exists(imagePath))
            return;

        var image = LoadImage(imagePath);
        System.Diagnostics.Debug.WriteLine(
        $"Image: {image.PixelWidth} x {image.PixelHeight}");

        double imageRatio =
            (double)image.PixelWidth /
            image.PixelHeight;

        double targetRatio =
            targetRect.Width /
            targetRect.Height;

        Rect sourceRect;

        if (imageRatio > targetRatio)
        {
            // Bild zu breit
            double cropWidth =
                image.PixelHeight * targetRatio;

            double x =
                (image.PixelWidth - cropWidth) / 2;

            sourceRect = new Rect(
                x,
                0,
                cropWidth,
                image.PixelHeight);
        }
        else
        {
            // Bild zu hoch
            double cropHeight =
                image.PixelWidth / targetRatio;

            double y =
                (image.PixelHeight - cropHeight) / 2;

            sourceRect = new Rect(
                0,
                y,
                image.PixelWidth,
                cropHeight);
        }

        dc.DrawImage(
            new CroppedBitmap(
                image,
                new Int32Rect(
                    (int)sourceRect.X,
                    (int)sourceRect.Y,
                    (int)sourceRect.Width,
                    (int)sourceRect.Height)),
            targetRect);
    }
}