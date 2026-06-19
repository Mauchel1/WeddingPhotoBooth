using System.Windows.Media.Imaging;

namespace WeddingPhotoBooth.Templates;

public interface IPhotoTemplate
{
    BitmapSource Create(
        List<string> photos,
        string weddingText,
        string dateText);
}