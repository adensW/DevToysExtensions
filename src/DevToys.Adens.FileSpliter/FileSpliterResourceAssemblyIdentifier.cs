using DevToys.Api;
using System.ComponentModel.Composition;
namespace DevToys.Adens.FileSpliter;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(FileSpliterResourceAssemblyIdentifier))]
internal sealed class FileSpliterResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}