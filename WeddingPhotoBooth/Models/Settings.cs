using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeddingPhotoBooth.Models
{
    public class Settings
    {
        public int CountdownFirst { get; set; } = 10;
        public int CountdownNext { get; set; } = 4;

        public int PicturesPerSession { get; set; } = 3;
        public int PreviewTimeoutSeconds { get; set; } = 20;
        public bool AutoReturnToIdle { get; set; } = true;
        public string OmCaptureProcessName { get; set; } = "OM Capture";
        public string SaveDirectory { get; set; } = @"C:\WeddingPhotos";

        public int PrintCopies { get; set; } = 1;

        public string PrinterName { get; set; } = "Canon SELPHY CP1500";

        public string WeddingText { get; set; } = "Daniel ♥ Annabelle";
        public string DateText { get; set; } = "10.10.2026";

        public string Template { get; set; } = "Default";

        public string TemplateFile { get; set; } = "template.jpg";

        public List<PhotoSlot> PhotoSlots { get; set; } = new();

    }

    public class PhotoSlot
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
