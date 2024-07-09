using DevToys.Api;
using System;
using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.Text;
using static DevToys.Api.GUI;

namespace Adens.DevToys.RsaGenerator;

[Export(typeof(IGuiTool))]
[Name("RsaGenerator")]                                                         // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                       // This font is available by default in DevToys
    IconGlyph = '\uE741',                                                     // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Generators,                    // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(RsaGeneratorResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "Adens.DevToys.RsaGenerator.RsaGenerator",                      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(RsaGenerator.ShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(RsaGenerator.LongDisplayTitle),
    DescriptionResourceName = nameof(RsaGenerator.Description),
    AccessibleNameResourceName = nameof(RsaGenerator.AccessibleName))]
internal sealed class RsaGeneratorGui : IGuiTool
{
    private enum GridRows
    {
        Settings,
        Results

    }

    private enum GridColumns
    {
        Private,
        Public
    }
    private UIToolView? _view;
    private readonly ISettingsProvider _settingsProvider;

    [ImportingConstructor]
    public RsaGeneratorGui(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
        Generate(2048);
    }

    private readonly IUIMultiLineTextInput _pubKeyText = MultiLineTextInput("_pubKeyText").ReadOnly();
    private readonly IUIMultiLineTextInput _privKeyText = MultiLineTextInput("_privKeyText").ReadOnly();
    private readonly IUIInfoBar _infoBar = InfoBar("infobar").Title("Error").Description("Bits should be 384  <= bits <= 16384 and be a multiple of 8").HideIcon().Error().Closable();
    private readonly IUINumberInput _bitInput = NumberInput("_bitInput")
                        .Title("Key Length")
                        .HideCommandBar()
                        .CannotCopyWhenEditable()
                        .Text("2048")
                        ;
    public UIToolView View
    {
        get
        {
            _view ??=
            new(
                Grid()
                .Rows(
                    (GridRows.Settings, Auto),
                    (GridRows.Results, new UIGridLength(1, UIGridUnitType.Fraction))
                )
                .Columns(
                    (GridColumns.Private, new UIGridLength(1, UIGridUnitType.Fraction)),
                    (GridColumns.Public, new UIGridLength(1, UIGridUnitType.Fraction))
                )
                .Cells(
                    Cell(
                        GridRows.Settings,
                        GridRows.Settings,
                        GridColumns.Private,
                        GridColumns.Public,
                        Stack()
                        .Vertical()
                        .WithChildren(
                            _bitInput.OnValueChanged(onSizeChanged),
                            _infoBar,

                            Button().Text("Generate")
                            .OnClick(onGenerateClick)
                            )
                        ),
                  
                    Cell(
                        GridRows.Results,
                        GridColumns.Private,
                        _privKeyText
                        ),
                    Cell(
                        GridRows.Results,
                        GridColumns.Public,
                        _pubKeyText
                        )
                 )
            );
            return _view;
        }
    }

    private async ValueTask onGenerateClick()
    {
        Generate((int)_bitInput.Value);
    }

    private async ValueTask onSizeChanged(double arg)
    {

        int bit = (int)arg;

        if (bit > 16384 || bit < 384 || bit % 8 != 0)
        {
            _infoBar.Open();
            _infoBar.Description("Bits should be 384  <= bits <= 16384 and be a multiple of 8");
            _privKeyText.Text("");
            _pubKeyText.Text("");
        }
        else
        {
            Generate(bit);
        }

    }
    private void Generate(int bit)
    {
        _infoBar.Close();
        if (bit > 16384 || bit < 384 || bit % 8 != 0)
        {
            _infoBar.Open();
            _infoBar.Description("Bits should be 384  <= bits <= 16384 and be a multiple of 8");
            return;
        }
        using var rsa = new RSACryptoServiceProvider(bit);
        string pri = rsa.ExportRSAPrivateKeyPem();
        _privKeyText.Text(pri);
        string pub = rsa.ExportRSAPublicKeyPem();
        _pubKeyText.Text(pub);

    }
    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

}