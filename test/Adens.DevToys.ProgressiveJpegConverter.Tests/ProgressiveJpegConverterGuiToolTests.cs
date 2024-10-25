using DevToys.Api;
using Adens.DevToys.Testbase.Mocks;
namespace Adens.DevToys.ProgressiveJpegConverter.Tests;

public class ProgressiveJpegConverterGuiToolTests : TestBase
{
    private readonly UIToolView _toolView;
    private readonly ProgressiveJpegConverterGui _tool;
    private readonly IUISingleLineTextInput _openApiUrlInput;
    private readonly IUIButton _openApiGenerateButton;
    public ProgressiveJpegConverterGuiToolTests()
    {
        _tool = new ProgressiveJpegConverterGui(new MockISettingsProvider(),new MockIFileStorage());
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