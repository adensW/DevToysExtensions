using DevToys.Api;
using Adens.DevToys.Testbase.Mocks;
using Adens.DevToys;
namespace Adens.DevToys.InitializationTool.Tests;

public class InitializationToolGuiToolTests : TestBase
{
    private readonly UIToolView _toolView;
    private readonly InitializationToolGui _tool;

    public InitializationToolGuiToolTests()
    {
        _tool = new InitializationToolGui(new MockISettingsProvider(), new MockIFileStorage());
        _toolView = _tool.View;
    }

    [Fact]
    public void Test1()
    {

    }
}