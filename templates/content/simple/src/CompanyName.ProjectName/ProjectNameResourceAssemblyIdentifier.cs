using DevToys.Api;
using System.ComponentModel.Composition;
namespace CompanyName.ProjectName;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(ProjectNameResourceAssemblyIdentifier))]
internal sealed class ProjectNameResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}