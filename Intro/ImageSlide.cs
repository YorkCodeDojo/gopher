using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Intro;

public class ImageSlide(string filename, int? width = null, int? height = null, bool? sizeToFit = false): Control, ISlide
{
    public DisplayResult Display(bool reset)
    {
        if (reset)
        {
            return DisplayResult.MoreToDisplay;
        }
        return DisplayResult.Completed;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var image = new Bitmap(filename);

        var aspectRatio = sizeToFit.GetValueOrDefault() ? (Bounds.Width / image.Size.Width) : 1;
        var imageWidth = (width ?? image.Size.Width) * aspectRatio;
        var imageHeight = (height ?? image.Size.Height) * aspectRatio;
        
        context.DrawImage(image, new Rect(
            (Bounds.Width - imageWidth) / 2,
            (Bounds.Height - imageHeight) / 2,
            imageWidth,
            imageHeight) );
    }
}