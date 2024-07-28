using CliWrap;
using DevToys.Api;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;


namespace Adens.DevToys.SimpleSequenceExecutor.UI;
[Export(typeof(IUIExecutor))]
internal class UICliExecutor : UIElement, IUIExecutor
{
    public event EventHandler? ExecuteblePathChanged;
    public event EventHandler? ArgumentsChanged;
    public event EventHandler? WorkingDirectoryChanged;
    private string executeblePath = string.Empty;
    private string arguments = string.Empty;
    private string workingDirectory = string.Empty;

    public string ExecuteblePath { get => executeblePath;
        internal set => SetPropertyValue(ref executeblePath, value, ExecuteblePathChanged);
    }
    public string Arguments { get => arguments;
        internal set => SetPropertyValue(ref arguments, value, ArgumentsChanged);
    }
    public string WorkingDirectory { get => workingDirectory;
        internal set => SetPropertyValue(ref workingDirectory, value, WorkingDirectoryChanged);
    }
    public UICliExecutor(string? id) : base(id)
    {
        UIElement = Stack().Vertical().WithChildren(
                       Label().Text("Cli Executor"),
                       SingleLineTextInput().Title("ExecuteblePath").Text(executeblePath).OnTextChanged(OnExecuteblePathChanged),
                       SingleLineTextInput().Title("Arguments").Text(arguments).OnTextChanged(OnArgumentsChanged),
                       SingleLineTextInput().Title("WorkingDirectory").Text(workingDirectory).OnTextChanged(OnWorkingDirectoryChanged)
                                  );
    }

    private ValueTask OnWorkingDirectoryChanged(string arg)
    {
        WorkingDirectory = arg;
        return ValueTask.CompletedTask;
    }

    private ValueTask OnArgumentsChanged(string arg)
    {
        Arguments = arg;
        return ValueTask.CompletedTask;

    }

    private ValueTask OnExecuteblePathChanged(string arg)
    {

        ExecuteblePath = arg;
        return ValueTask.CompletedTask;
    }

    public bool CanExecute => true;

    public IUIElement UIElement { get; }

    public async ValueTask ExecuteAsync()
    {
        var command = Cli.Wrap(ExecuteblePath);
        if (!string.IsNullOrWhiteSpace(Arguments))
        {
            command = command.WithArguments(Arguments);

        }
        if (!string.IsNullOrWhiteSpace(WorkingDirectory))
        {
            command = command.WithWorkingDirectory(WorkingDirectory);
        }

        var result = await command
         .ExecuteAsync();

    }
}
public static partial class GUI
{

    public static IUIExecutor CliExecutor()
    {
        return CliExecutor(null);
    }

    public static IUIExecutor CliExecutor(string? id)
    {
        return new UICliExecutor(id);
    }
   

}