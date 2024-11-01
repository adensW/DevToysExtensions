# Adens.DevToys.Testbase
A Testbase for Adens DevToys Extensions.

## License
This extension is licensed under the GPL License - see the LICENSE file for details.


## Mock

Provides  mock objects for testing.

1.MockIFileStorage

2.MockILogger

3.MockISettingsProvider

4.MockSandboxedFileReader

## Use

```csharp


public class TestGuiToolTests:TestBase
{
    private readonly UIToolView _toolView;
    private readonly TestGui _tool;

    public TestGuiToolTests()
    {
        _tool = new TestGui(new MockISettingsProvider(), new MockIFileStorage());
        _toolView = _tool.View;
    }

    [Fact]
    public void Test1()
    {

    }
}
```
