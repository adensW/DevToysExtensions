using CliWrap;
using DevToys.Api;
using System.ComponentModel.Composition;
using System.Text;
using static DevToys.Api.GUI;

namespace Adens.DevToys.InitializationTool;

[Export(typeof(IGuiTool))]
[Name("InitializationTool")]                                                         // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                       // This font is available by default in DevToys
    IconGlyph = '\uF33A',                                                     // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Converters,                    // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(InitializationToolResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "Adens.DevToys.InitializationTool.InitializationTool",                      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(InitializationTool.ShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(InitializationTool.LongDisplayTitle),
    DescriptionResourceName = nameof(InitializationTool.Description),
    AccessibleNameResourceName = nameof(InitializationTool.AccessibleName))]
internal sealed partial class InitializationToolGui : IGuiTool
{

    private UIToolView? _view;
    private readonly ISettingsProvider _settingsProvider;
    private IUISingleLineTextInput _companyInput = SingleLineTextInput().Text("Adens.DevToys").Title("Company Name");
    private IUISingleLineTextInput _projectInput = SingleLineTextInput().Text("").Title("Product Name");
    private IUISingleLineTextInput _outputFilePathInput = SingleLineTextInput().Text("").Title("Solution Path");
    private readonly IFileStorage _fileStorage;
    [ImportingConstructor]
    public InitializationToolGui(ISettingsProvider settingsProvider, IFileStorage fileStorage)
    {
        _settingsProvider = settingsProvider;
        _fileStorage = fileStorage;
    }

    public UIToolView View
    {
        get
        {
            _view ??=
            new(Stack()
                .Vertical()
                .WithChildren(
                   _companyInput,
                   _projectInput,
                    _outputFilePathInput
                        .Title("Solution Path")
                         .CommandBarExtraContent(

                            Button()
                            .Text("Pick solution fold")
                            .OnClick(OnChooseOutputPathButtonClickAsync)),
                   Button().Text("Execute").OnClick(OnExecuteClicked)
            ));
            return _view;
        }
    }
    private async ValueTask OnChooseOutputPathButtonClickAsync()
    {
        // Ask the user to pick a TXT or JSON file.
        var folder = await _fileStorage.PickFolderAsync();
        if (folder == null)
        {
            return;
        }
        _outputFilePathInput.Text(folder);
    }
    private async ValueTask OnExecuteClicked()
    {
        var command = Cli.Wrap("dotnet");
        command = command.WithArguments(args =>
        {
            args.Add(["new", "atoys"]);
            if (!string.IsNullOrWhiteSpace(_companyInput.Text))
            {
                args.Add(["--CompanyName", _companyInput.Text]);
            }
            if (!string.IsNullOrWhiteSpace(_projectInput.Text))
            {
                args.Add(["-n", _projectInput.Text]);
            }
        });
        if (!string.IsNullOrWhiteSpace(_outputFilePathInput.Text))
        {
            command = command.WithWorkingDirectory(_outputFilePathInput.Text);
        }
        var result = await command
         .ExecuteAsync();
    }

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

}