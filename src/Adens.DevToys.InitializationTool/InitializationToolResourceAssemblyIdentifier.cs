using DevToys.Api;
using System.ComponentModel.Composition;
namespace Adens.DevToys.InitializationTool;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(InitializationToolResourceAssemblyIdentifier))]
internal sealed class InitializationToolResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}