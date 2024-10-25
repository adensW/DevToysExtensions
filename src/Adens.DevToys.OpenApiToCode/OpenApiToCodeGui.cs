using DevToys.Api;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi;
using System;
using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.Text;
using static DevToys.Api.GUI;
using Microsoft.OpenApi.Extensions;
using Adens.DevToys.OpenApiToCode.Helper;

namespace Adens.DevToys.OpenApiToCode;

[Export(typeof(IGuiTool))]
[Name("OpenApiToCode")]                                                         // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                       // This font is available by default in DevToys
    IconGlyph = '\uE741',                                                     // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Generators,                    // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(OpenApiToCodeResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "Adens.DevToys.OpenApiToCode.OpenApiToCode",                      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(OpenApiToCode.ShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(OpenApiToCode.LongDisplayTitle),
    DescriptionResourceName = nameof(OpenApiToCode.Description),
    AccessibleNameResourceName = nameof(OpenApiToCode.AccessibleName))]
internal sealed class OpenApiToCodeGui : IGuiTool
{
  
    private UIToolView? _view;
    private readonly ISettingsProvider _settingsProvider;

    [ImportingConstructor]
    public OpenApiToCodeGui(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
       
    }

    private readonly IUISingleLineTextInput _openApiUrlInput = SingleLineTextInput(nameof(_openApiUrlInput)).Title(OpenApiToCode.OpenApiUrl);
  
    public UIToolView View
    {
        get
        {
            _view ??=
            new(
                 Stack()
                .Vertical().WithChildren(_openApiUrlInput, Button("openApiGenerate").Text(OpenApiToCode.Generate).OnClick(onGenerateClick)));
            return _view;
        }
    }

    private async ValueTask onGenerateClick()
    {
        await Generate(_openApiUrlInput.Text);
    }

  
    private async ValueTask Generate(string url)
    {

       //OpenApiCodeGeneratorHelper.GenerateCode(url);

    }
    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

}