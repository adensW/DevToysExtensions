using DevToys.Api;
using Adens.DevToys.Testbase.Mocks;
using Adens.DevToys;
namespace Adens.DevToys.ScribanRenderer.Tests;

public class ScribanRendererGuiToolTests : TestBase
{
    private readonly UIToolView _toolView;
    private readonly ScribanRendererGui _tool;

    public ScribanRendererGuiToolTests()
    {
        _tool = new ScribanRendererGui(new MockISettingsProvider());
        _toolView = _tool.View;
    }

    [Fact]
    public void Test1()
    {

    }
}