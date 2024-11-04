using Adens.DevToys.ScribanRenderer.Helpers;
using DevToys.Api;
using Scriban;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using static DevToys.Api.GUI;

namespace Adens.DevToys.ScribanRenderer;

[Export(typeof(IGuiTool))]
[Name("ScribanRenderer")]                                                         // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                       // This font is available by default in DevToys
    IconGlyph = '\uf53e',                                                     // An icon that represents a pizza
    GroupName = "Executors",                    // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(ScribanRendererResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "Adens.DevToys.ScribanRenderer.ScribanRenderer",                      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(ScribanRenderer.ShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(ScribanRenderer.LongDisplayTitle),
    DescriptionResourceName = nameof(ScribanRenderer.Description),
    AccessibleNameResourceName = nameof(ScribanRenderer.AccessibleName))]
internal sealed partial class ScribanRendererGui : IGuiTool
{

    private UIToolView? _view;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IUIMultiLineTextInput _jsonEditor = MultiLineTextInput().Title("JSON Editor")
                          .Language("json")
                                .Extendable();
    private readonly IUIMultiLineTextInput _templateEditor = MultiLineTextInput().Title("Scriban Template")
                                .Extendable();
    private readonly IUIMultiLineTextInput _templateOutput = MultiLineTextInput().Title("Output")
                                .Extendable().ReadOnly();
    [ImportingConstructor]
    public ScribanRendererGui(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }
    private enum GridRows
    {
        Top,
        Bottom
    }

    private enum GridColumns
    {
        Full,
    }
    public UIToolView View
    {
        get
        {
            _view ??=
            new(
               
                    SplitGrid()
                    .Horizontal()
                    .TopPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                    .BottomPaneLength(new UIGridLength(2, UIGridUnitType.Fraction))
                    .WithTopPaneChild(Grid()
                .ColumnSmallSpacing()
                .RowLargeSpacing()

                .Rows(
                    (GridRows.Top, Auto),                                              // Height automatically fits the content.
                    (GridRows.Bottom, new UIGridLength(50, UIGridUnitType.Pixel)))    // Takes up 100 pixels.

                .Columns(
                    (GridColumns.Full, new UIGridLength(1, UIGridUnitType.Fraction))  // Takes up 1/3 of the space.
                 ) 

                .Cells(
                    Cell(GridRows.Top, GridColumns.Full, _jsonEditor)
                      ,

                    Cell(GridRows.Bottom, GridColumns.Full,
                        Button().AccentAppearance().Text("Generate").OnClick(OnGenerateClick))
                    )
               
                    )
                    .WithBottomPaneChild(
                        SplitGrid()
                            .Vertical()
                            .LeftPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                            .RightPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                            .WithLeftPaneChild(
                              _templateEditor
                            )
                            .WithRightPaneChild(
                               _templateOutput
                            )
                        )
                );

            return _view;
        }
    }

    private async ValueTask OnGenerateClick()
    {
        try
        {
           var result = await ScribanTemplateGenerator.GenerateTemplate(_templateEditor.Text, _jsonEditor.Text);
            _templateOutput.Text(result);

        }catch (Exception ex)
        {
            _templateOutput.Text(ex.Message);
        }

    }

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

}