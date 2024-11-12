using DevToys.Api;
using Adens.DevToys.Testbase.Mocks;
using Adens.DevToys;
namespace CompanyName.ProjectName.Tests;

public class ProjectNameGuiToolTests : TestBase
{
    private readonly UIToolView _toolView;
    private readonly ProjectNameGui _tool;

    public ProjectNameGuiToolTests()
    {
        _tool = new ProjectNameGui(new MockISettingsProvider());
        _toolView = _tool.View;
    }

    [Fact]
    public void Test1()
    {

    }
}