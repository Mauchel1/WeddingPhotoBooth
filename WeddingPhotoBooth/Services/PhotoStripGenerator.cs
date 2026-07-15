using System.Windows.Media.Imaging;
using WeddingPhotoBooth.Models;
using WeddingPhotoBooth.Templates;

namespace WeddingPhotoBooth.Services;

public class PhotoStripGenerator
{
    public BitmapSource GeneratePreview(
        List<string> photos,
        Settings settings)
    {
        var template = new DoubleStripTemplate();

        return template.Create(
            photos,
            settings);
    }

    public BitmapSource GeneratePrint(
    List<string> photos,
    Settings settings)
    {
        var template = new DoubleStripTemplate();

        return template.CreatePrint(
            photos,
            settings);
    }

    
}