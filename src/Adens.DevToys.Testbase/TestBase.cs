using System.Globalization;
using DevToys.Api;
using Microsoft.Extensions.Logging;

namespace Adens.DevToys;

public abstract class TestBase
{
    protected TestBase()
    {
        LoggingExtensions.LoggerFactory = LoggerFactory.Create(builder => { });

        // Set language to english for unit tests.
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    }
}
