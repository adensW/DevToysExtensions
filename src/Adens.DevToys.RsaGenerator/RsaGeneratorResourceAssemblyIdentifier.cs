using DevToys.Api;
using System.ComponentModel.Composition;
namespace Adens.DevToys.RsaGenerator;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(RsaGeneratorResourceAssemblyIdentifier))]
internal sealed class RsaGeneratorResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}