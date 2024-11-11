using CliWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.InitializationTool.Helper;
public static class CliWrapHelper
{
    public static async Task<bool> ExecuteCommand(string command, string[] arguments, string workingDirectory)
    {
        string output = string.Empty;
        string error = string.Empty;
        try
        {
            var stdOut = new StringBuilder();
            var stdErr = new StringBuilder();
            var result = await Cli.Wrap(command)
                .WithArguments(arguments)
                .WithWorkingDirectory(workingDirectory)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOut))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErr))
                .ExecuteAsync();
            output = stdOut.ToString();
            error = stdErr.ToString();
            return result.ExitCode == 0;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }
}
