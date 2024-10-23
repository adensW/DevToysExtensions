using DevToys.Api;
using Adens.DevToys.Testbase.Mocks;
namespace Adens.DevToys.OpenApiToCode.Tests;

public class OpenApiToCodeGuiToolTests : TestBase
{
    private readonly UIToolView _toolView;
    private readonly OpenApiToCodeGui _tool;
    private readonly IUISingleLineTextInput _openApiUrlInput;
    private readonly IUIButton _openApiGenerateButton;
    public OpenApiToCodeGuiToolTests()
    {
        _tool = new OpenApiToCodeGui(new MockISettingsProvider());
        _toolView = _tool.View;
        _openApiUrlInput = (IUISingleLineTextInput)_toolView.GetChildElementById(nameof(_openApiUrlInput));
        _openApiGenerateButton = (IUIButton)_toolView.GetChildElementById("openApiGenerate");
    }

    //[Fact]
    //public void Test1()
    //{
    //    _openApiUrlInput.Text("https://localhost:5001/swagger/v1/swagger.json");
    //    _openApiGenerateButton.c
    //}
}