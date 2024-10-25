using Adens.DevToys.RsaGenerator.Helpers;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace Adens.DevToys.RsaGenerator.Tests;
public class RsaGeneratorHelperTests:TestBase
{

    [Fact]
    public async Task RsaGeneratorHelperShouldWork_BySixLabors() {
      var  (p2,u2) = await RsaGeneratorHelper.GenerateRsaKeyPairAsync(2048);
      
    }
  

}
