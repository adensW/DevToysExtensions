using DevToys.Api;
using System.ComponentModel.Composition;
using System.Reflection;

namespace Adens.DevToys.SimpleSequenceExecutor;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(SimpleSequenceExecutorResourceAssemblyIdentifier))]
internal sealed class SimpleSequenceExecutorResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}