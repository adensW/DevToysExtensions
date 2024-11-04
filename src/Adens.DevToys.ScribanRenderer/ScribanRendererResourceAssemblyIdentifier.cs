using DevToys.Api;
using System.ComponentModel.Composition;
namespace Adens.DevToys.ScribanRenderer;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(ScribanRendererResourceAssemblyIdentifier))]
internal sealed class ScribanRendererResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}