using DevToys.Api;
using System.ComponentModel.Composition;
using System.Text;
using static DevToys.Api.GUI;

namespace CompanyName.ProjectName;

[Export(typeof(IGuiTool))]
[Name("ProjectName")]                                                         // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                       // This font is available by default in DevToys
    IconGlyph = '\uF33A',                                                     // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Converters,                    // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(ProjectNameResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "CompanyName.ProjectName.ProjectName",                      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(ProjectName.ShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(ProjectName.LongDisplayTitle),
    DescriptionResourceName = nameof(ProjectName.Description),
    AccessibleNameResourceName = nameof(ProjectName.AccessibleName))]
internal sealed partial class ProjectNameGui : IGuiTool
{
   
    private UIToolView? _view;
    private readonly ISettingsProvider _settingsProvider;
    
    [ImportingConstructor]
    public ProjectNameGui(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }
 
    public UIToolView View
    {
        get
        {
            _view ??=
            new(Stack()
                .Vertical()
                .WithChildren(
                    Label().Text("Hello World!")
            ));
            return _view;
        }
    }
  
    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

}