using DevToys.Api;
using Adens.DevToys.Testbase.Mocks;
namespace Adens.DevToys.FileSplitter.Tests;

public class FileSplitterGuiToolTests:TestBase
{
    private readonly UIToolView _toolView;
    private readonly FileSplitterGui _tool;

    public FileSplitterGuiToolTests()
    {
        _tool = new FileSplitterGui(new MockISettingsProvider(), new MockIFileStorage());
        _toolView = _tool.View;
    }

    [Fact]
    public void Test1()
    {

    }
}