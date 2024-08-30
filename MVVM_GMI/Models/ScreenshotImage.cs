using System.Drawing;
using System.Windows.Media.Imaging;

namespace MVVM_GMI.Models
{
    public class ScreenshotImage
    {

        public required BitmapSource Image { get; set; }
        public required string Filename { get; set; }
        public required string Date { get; set; }
        public required string SizeInMB { get; set; }
        public required string FilePath { get; set; }

    }
}
