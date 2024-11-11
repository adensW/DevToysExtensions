using Xunit;
using Adens.DevToys.InitializationTool.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.InitializationTool.Helper.Tests;

public class CliWrapHelperTests
{
    [Fact()]
    public async Task ExecuteCommandTest()
    {
        var result =  await CliWrapHelper.ExecuteCommand("dotnet", new string[] { "--version" }, "C:\\Users\\A\\Repos\\Test\\test4");
        Assert.True(result);
    }
}