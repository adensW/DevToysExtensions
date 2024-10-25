using Adens.DevToys.ProgressiveJpegConverter.Helpers;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace Adens.DevToys.ProgressiveJpegConverter.Tests;
public class ProgressiveJpegConverterHelperTests:TestBase
{
    [Fact]
    public async Task ProgressiveJpegConverterHelperShouldWork_BySixLabors() { 
        using FileStream fs = new FileStream("test.png", FileMode.Open);
        var image = await Image.LoadAsync<Rgba32>(fs);
        using FileStream savedfs = new FileStream("test-progressive.jpeg", FileMode.Create);

        await ProgressiveJpegConverterHelper.ConvertBySixLaborsAsync(savedfs, image, default);
    }
    [Fact]
    public async Task ProgressiveJpegConverterHelperShouldWork_ByMagick()
    {
        using FileStream fs = new FileStream("test.png", FileMode.Open);
        using FileStream savedfs = new FileStream("test-progressive.jpeg", FileMode.Create);

        await ProgressiveJpegConverterHelper.ConvertByMagickAsync(savedfs, fs, default);
    }

}
