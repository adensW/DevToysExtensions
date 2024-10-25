using DevToys.Api;
using System.ComponentModel.Composition;
namespace Adens.DevToys.OpenApiToCode;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(OpenApiToCodeResourceAssemblyIdentifier))]
internal sealed class OpenApiToCodeResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}