using DevToys.Api;
using System.ComponentModel.Composition;
namespace Adens.DevToys.FileSplitter;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(FileSplitterResourceAssemblyIdentifier))]
internal sealed class FileSplitterResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}