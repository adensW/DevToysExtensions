using Adens.DevToys.SimpleSequenceExecutor.Entities;
using CliWrap;
using DebounceThrottle;
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
    public event EventHandler? ParametersChanged;
    private string executeblePath = string.Empty;
    private string arguments = string.Empty;
    private string workingDirectory = string.Empty;

    public Dictionary<string, object> Parameters { get; set; }
     
    public string ExecuteblePath { get => executeblePath;
        internal set => SetPropertyValue(ref executeblePath, value, ParametersChanged);
    }
    public string Arguments { get => arguments;
        internal set => SetPropertyValue(ref arguments, value, ParametersChanged);
    }
    public string WorkingDirectory { get => workingDirectory;
        internal set => SetPropertyValue(ref workingDirectory, value, ParametersChanged);
    }
    private DebounceDispatcher _debouncer1;
    private DebounceDispatcher _debouncer2;
    private DebounceDispatcher _debouncer3;
    public UICliExecutor(string? id, Dictionary<string, object> parameters) : base(id)
    {
        _debouncer1 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
            maxDelay: TimeSpan.FromSeconds(3));
        _debouncer2 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
         maxDelay: TimeSpan.FromSeconds(3));
        _debouncer3 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
         maxDelay: TimeSpan.FromSeconds(3));
        Parameters = parameters;
        if (Parameters.TryGetValue(nameof(ExecuteblePath), out object val1))
        {
            executeblePath = val1?.ToString() ?? string.Empty;
        }
        if (Parameters.TryGetValue(nameof(Arguments), out object val2))
        {
            arguments = val2?.ToString() ?? string.Empty;
        }
        if (Parameters.TryGetValue(nameof(WorkingDirectory), out object val3))
        {
            workingDirectory = val3?.ToString() ?? string.Empty;
        }
        UIElement = Stack().Vertical().WithChildren(
                       Label().Text("Cli Executor"),
                       SingleLineTextInput().Title("ExecuteblePath").Text(executeblePath).OnTextChanged(OnExecuteblePathChanged),
                       SingleLineTextInput().Title("Arguments").Text(arguments).OnTextChanged(OnArgumentsChanged),
                       SingleLineTextInput().Title("WorkingDirectory").Text(workingDirectory).OnTextChanged(OnWorkingDirectoryChanged)
                                  );
    }

    private ValueTask OnWorkingDirectoryChanged(string arg)
    {
        Parameters.Remove(nameof(WorkingDirectory));
        Parameters.Add(nameof(WorkingDirectory), arg);
        _debouncer1.Debounce(()=> WorkingDirectory = arg);
        return ValueTask.CompletedTask;
    }

    private ValueTask OnArgumentsChanged(string arg)
    {
        Parameters.Remove(nameof(Arguments));
        Parameters.Add(nameof(Arguments), arg);
        _debouncer2.Debounce(() => Arguments = arg);
        return ValueTask.CompletedTask;

    }

    private ValueTask OnExecuteblePathChanged(string arg)
    {
        Parameters.Remove(nameof(ExecuteblePath));
        Parameters.Add(nameof(ExecuteblePath), arg);
        _debouncer3.Debounce(() => ExecuteblePath = arg);

        return ValueTask.CompletedTask;
    }

    public bool CanExecute => true;

    public IUIElement UIElement { get; }

    public async ValueTask<ExecutedResult> ExecuteAsync(Dictionary<string, object> runtimeVariables)
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
        return ExecutedResult.Create(runtimeVariables);
    }
}
public static partial class GUI
{

    public static IUIExecutor CliExecutor(Dictionary<string, object> parameters)
    {
        return CliExecutor(null,parameters);
    }

    public static IUIExecutor CliExecutor(string? id, Dictionary<string, object> parameters)
    {
        return new UICliExecutor(id,parameters);
    }
   

}