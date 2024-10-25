using Adens.DevToys.RsaGenerator.Helpers;
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
  
    private UIToolView? _view;
    private readonly ISettingsProvider _settingsProvider;

    [ImportingConstructor]
    public RsaGeneratorGui(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
        Generate(2048);
    }

    private readonly IUIMultiLineTextInput _pubKeyText = MultiLineTextInput("_pubKeyText").Title(RsaGenerator.Publickey).ReadOnly();
    private readonly IUIMultiLineTextInput _privKeyText = MultiLineTextInput("_privKeyText").Title(RsaGenerator.Privatekey).ReadOnly();
    private readonly IUIInfoBar _infoBar = InfoBar("infobar").Title(RsaGenerator.Error).Description(RsaGenerator.ErrorMessage).HideIcon().Error().Closable();
    private readonly IUINumberInput _bitInput = NumberInput("_bitInput")
                        .Title(RsaGenerator.Keylength)
                        .HideCommandBar()
                        .Minimum(384)
                        .Maximum(16384)
                        .Step(8)
                        .CannotCopyWhenEditable()
                        .Text("2048")
                        ;
    public UIToolView View
    {
        get
        {
            _view ??=
            new(
                 SplitGrid()
                .Horizontal()
                .TopPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                .BottomPaneLength(new UIGridLength(8, UIGridUnitType.Fraction))
                .WithTopPaneChild(
                              Stack()
                        .Vertical()
                        .WithChildren(
                            _bitInput.OnValueChanged(onSizeChanged),
                            _infoBar,

                            Button().Text(RsaGenerator.Generate)
                            .OnClick(onGenerateClick)
                            ))
                .WithBottomPaneChild(

                    SplitGrid()
                        .Vertical()
                        .LeftPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .RightPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .WithLeftPaneChild(_privKeyText)
                        .WithRightPaneChild(_pubKeyText))
            );
            return _view;
        }
    }
    private readonly IUILabel dialogLable = Label()
                               .Style(UILabelStyle.Body)
                               .Text("");
    private async Task<UIDialog> OpenCustomDialogAsync(bool dismissible)
    {
        dialogLable.Text(string.Format(RsaGenerator.Confirm, _bitInput.Value));
        // Open a dialog
        UIDialog dialog
            = await _view.OpenDialogAsync(
                dialogContent:
                    Stack()
                        .Vertical()
                        .WithChildren(
                           dialogLable),
                footerContent:
                Stack()
                        .Horizontal()
                        .WithChildren(
                            Button()
                        .AlignHorizontally(UIHorizontalAlignment.Right)
                        .Text("OK")
                        .OnClick(OnConfirmDialogButtonClick),
                             Button()
                        .AlignHorizontally(UIHorizontalAlignment.Right)
                        .Text("Cancel")
                        .OnClick(OnCloseDialogButtonClick)),
                   
                isDismissible: dismissible);

        async void OnConfirmDialogButtonClick()
        {
            // On click on OK button, close the dialog.
            await Generate((int)_bitInput.Value);
            _view.CurrentOpenedDialog?.Close();
        }
         void OnCloseDialogButtonClick()
        {
            // On click on OK button, close the dialog.
            _bitInput.Text("2048");
            _view.CurrentOpenedDialog?.Close();
        }

        return dialog;
    }
    private async ValueTask onGenerateClick()
    {
        await Generate((int)_bitInput.Value);
    }

    private async ValueTask onSizeChanged(double arg)
    {

        int bit = (int)arg;

        if (bit > 16384 || bit < 384 || bit % 8 != 0)
        {
            _infoBar.Open();
            _infoBar.Description(RsaGenerator.ErrorMessage);
            _privKeyText.Text("");
            _pubKeyText.Text("");
        }
        else
        {
            if (bit >= 2048 * 3)
            {
                await OpenCustomDialogAsync(dismissible: true);
            }
            else
            {
                await Generate(bit);
            }
            
        }

    }
    private async Task Generate(int bit)
    {
        dialogLable.Text(RsaGenerator.Generating);
        _infoBar.Close();
        if (bit > 16384 || bit < 384 || bit % 8 != 0)
        {
            _infoBar.Open();
            _infoBar.Description(RsaGenerator.ErrorMessage);
            return;
        }
        var (pri, pub) =await RsaGeneratorHelper.GenerateRsaKeyPairAsync(bit);
        _privKeyText.Text(pri);
        _pubKeyText.Text(pub);
    }
    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

}