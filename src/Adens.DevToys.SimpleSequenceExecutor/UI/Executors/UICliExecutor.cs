using Adens.DevToys.SimpleSequenceExecutor.Args;
using Adens.DevToys.SimpleSequenceExecutor.Entities;
using Adens.DevToys.SimpleSequenceExecutor.Helpers;
using CliWrap;
using DebounceThrottle;
using DevToys.Api;
using OneOf.Types;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;
using static System.Net.Mime.MediaTypeNames;


namespace Adens.DevToys.SimpleSequenceExecutor.UI;
[Export(typeof(IUIExecutor))]
internal class UICliExecutor : UIElement, IUIExecutor
{

    public event EventHandler? ParametersChanged;
    private string executeblePath = string.Empty;
    private string arguments = string.Empty;
    private string workingDirectory = string.Empty;
    private BundleStep _step;
    public ObservableCollection<BundleStepParameter> Parameters { get; set; } = new ObservableCollection<BundleStepParameter>();
    private Dictionary<string, object> ParametersDict { get => Parameters.ToDictionary<BundleStepParameter, string, object>(z => z.Key, z => z.Value); }


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
    public UICliExecutor(string? id, BundleStep step) : base(id)
    {
        _step = step;
        ReloadParameters();
        ParametersChanged += SaveParameters;
        _debouncer1 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
            maxDelay: TimeSpan.FromSeconds(3));
        _debouncer2 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
         maxDelay: TimeSpan.FromSeconds(3));
        _debouncer3 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
         maxDelay: TimeSpan.FromSeconds(3));
        if (ParametersDict.TryGetValue(nameof(ExecuteblePath), out object val1))
        {
            executeblePath = val1?.ToString() ?? string.Empty;
        }
        if (ParametersDict.TryGetValue(nameof(Arguments), out object val2))
        {
            arguments = val2?.ToString() ?? string.Empty;
        }
        if (ParametersDict.TryGetValue(nameof(WorkingDirectory), out object val3))
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
    private void SaveParameters(object? sender, EventArgs e)
    {
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        foreach (var item in Parameters)
        {
            db.InsertOrReplace(item);
        }
    }
    private void ReloadParameters()
    {
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        Parameters.Clear();
        Parameters.AddRange(db.Table<BundleStepParameter>().Where(x => x.StepId == _step.Id));
    }
    private ValueTask OnWorkingDirectoryChanged(string arg)
    {
      
        _debouncer1.Debounce(()=> {
            Parameters.Remove(Parameters.FirstOrDefault(z => z.Key == nameof(WorkingDirectory)));
            Parameters.Add(new BundleStepParameter() { StepId = _step.Id, Key = nameof(WorkingDirectory), Value = arg });
            WorkingDirectory = arg; 

        });
        return ValueTask.CompletedTask;
    }

    private ValueTask OnArgumentsChanged(string arg)
    {
      
        _debouncer2.Debounce(() => {
            Parameters.Remove(Parameters.FirstOrDefault(z => z.Key == nameof(Arguments)));
            Parameters.Add(new BundleStepParameter() { StepId = _step.Id, Key = nameof(Arguments), Value = arg });
            Arguments = arg; });
        return ValueTask.CompletedTask;

    }

    private ValueTask OnExecuteblePathChanged(string arg)
    {
      
        _debouncer3.Debounce(() => {
            Parameters.Remove(Parameters.FirstOrDefault(z => z.Key == nameof(ExecuteblePath)));
            Parameters.Add(new BundleStepParameter() { StepId = _step.Id, Key = nameof(ExecuteblePath), Value = arg });
            ExecuteblePath = arg; });

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

    public static IUIExecutor CliExecutor(BundleStep step)
    {
        return CliExecutor(null, step);
    }

    public static IUIExecutor CliExecutor(string? id, BundleStep step)
    {
        return new UICliExecutor(id,step);
    }
   

}