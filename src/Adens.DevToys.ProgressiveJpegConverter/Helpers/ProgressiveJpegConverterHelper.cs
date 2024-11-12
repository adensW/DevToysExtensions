using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Formats.Jpeg;
using ImageMagick;

namespace Adens.DevToys.ProgressiveJpegConverter.Helpers;
public static class ProgressiveJpegConverterHelper
{
    public static async Task ConvertBySixLaborsAsync(Stream destinationStream, Image<Rgba32> image, CancellationToken cancellationToken)
    {
        await image.SaveAsJpegAsync(destinationStream,new JpegEncoder() {Quality=100,Interleaved=true,ColorType= JpegEncodingColor.Rgb });
    }
    public static async Task ConvertByMagickAsync(Stream destinationStream, Stream imageStream, CancellationToken cancellationToken)
    {
        using var imageFromStream = new MagickImage(imageStream);
        imageFromStream.Format = MagickFormat.Pjpeg;
        imageFromStream.Settings.Interlace = Interlace.Jpeg;
        imageFromStream.Quality = 100;
        await imageFromStream.WriteAsync(destinationStream);

    }
}
