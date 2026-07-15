using System.Windows.Media.Imaging;
using WeddingPhotoBooth.Models;

namespace WeddingPhotoBooth.Templates;

public interface IPhotoTemplate
{
    BitmapSource Create(
        List<string> photos,
        Settings settings
     );
}