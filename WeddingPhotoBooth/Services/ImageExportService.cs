using System.IO;
using System.Windows.Media.Imaging;

namespace WeddingPhotoBooth.Services;

public class ImageExportService
{
    public string SaveJpeg(
        BitmapSource bitmap,
        string filename)
    {
        BitmapFrame frame = BitmapFrame.Create(bitmap);

        JpegBitmapEncoder encoder = new();
        encoder.QualityLevel = 95;
        encoder.Frames.Add(frame);

        
        using FileStream stream =
            File.Create(filename);

        encoder.Save(stream);

        return filename;
    }
}