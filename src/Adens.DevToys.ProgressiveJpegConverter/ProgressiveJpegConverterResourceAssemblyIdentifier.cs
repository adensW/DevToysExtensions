using DevToys.Api;
using System.ComponentModel.Composition;
namespace Adens.DevToys.ProgressiveJpegConverter;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(ProgressiveJpegConverterResourceAssemblyIdentifier))]
internal sealed class ProgressiveJpegConverterResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}