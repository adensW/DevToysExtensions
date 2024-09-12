using Adens.DevToys.SimpleSequenceExecutor.Args;
using Adens.DevToys.SimpleSequenceExecutor.Entities;
using Adens.DevToys.SimpleSequenceExecutor.Helpers;
using CliWrap;
using DebounceThrottle;
using DevToys.Api;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;
namespace Adens.DevToys.SimpleSequenceExecutor.UI;

[Export(typeof(IUIExecutor))]
internal class UIWriteFileExecutor : UIElement, IUIExecutor
{
    public event EventHandler? ParametersChanged;
    private BundleStep _step;
    private string path = string.Empty;
    private string _value = string.Empty;

    public ObservableCollection<BundleStepParameter> Parameters { get; set; } = new ObservableCollection<BundleStepParameter>();
    private Dictionary<string, object> ParametersDict { get => Parameters.ToDictionary<BundleStepParameter, string, object>(z => z.Key, z => z.Value); }

    public string Path
    {
        get => path;
        internal set => SetPropertyValue(ref path, value, ParametersChanged);
    }
    
    public string Value
    {
        get => _value;
        internal set => SetPropertyValue(ref _value, value, ParametersChanged);
    }
    public bool valueGainFromParameters = true;
    public bool ValueGainFromParameters
    {
        get => valueGainFromParameters;
        internal set => SetPropertyValue(ref valueGainFromParameters, value, ParametersChanged);
    }
    private DebounceDispatcher _debouncer1;
    private DebounceDispatcher _debouncer2;
    private IUISingleLineTextInput _valueInput = SingleLineTextInput().Title("Value");
    public UIWriteFileExecutor(string? id, BundleStep step) : base(id)
    {
        _step = step;
        ReloadParameters();
        ParametersChanged += SaveParameters;
        _debouncer1 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
            maxDelay: TimeSpan.FromSeconds(3));
        _debouncer2 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
         maxDelay: TimeSpan.FromSeconds(3));
        
        if (ParametersDict.TryGetValue(nameof(Path), out object val1))
        {
            path = val1?.ToString() ?? string.Empty;
        }
        if (ParametersDict.TryGetValue(nameof(Value), out object val2))
        {
            _value = val2?.ToString() ?? string.Empty;
        }
        if (ParametersDict.TryGetValue(nameof(ValueGainFromParameters), out object val3))
        {
            valueGainFromParameters = true;
            if ((bool.TryParse(val3?.ToString(), out bool isChoosed)))
            {
                valueGainFromParameters = isChoosed;
            }
        }

        var switcher = Switch()
                        .OnText("Gain from runtime")
                        .OffText("Not gain from runtime")
                        .OnToggle(ToggleValueGainFromParameters);

        if (valueGainFromParameters)
        {
            switcher.On();
            _valueInput.Text(_value).OnTextChanged(OnValueChanged);
        }
        else
        {
            switcher.Off();
            _valueInput = MultiLineTextInput().Title("Value").Text(_value).Extendable().Editable().OnTextChanged(OnValueChanged);
        }
        UIElement = Stack().Vertical().WithChildren(
                       Label().Text("Write Executor"),
                       SingleLineTextInput().Title("Path").Text(path).OnTextChanged(OnPathChanged),
                       Stack().Vertical().WithChildren(switcher, _valueInput)
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
    private ValueTask ToggleValueGainFromParameters(bool arg)
    {
      

        Parameters.Remove(Parameters.FirstOrDefault(z => z.Key == nameof(ValueGainFromParameters)));
        Parameters.Add(new BundleStepParameter() { StepId = _step.Id, Key = nameof(ValueGainFromParameters), Value = arg.ToString() });
        ValueGainFromParameters = arg;
        return ValueTask.CompletedTask;
    }
    private ValueTask OnValueChanged(string arg)
    {
       
        _debouncer1.Debounce(() => {

            Parameters.Remove(Parameters.FirstOrDefault(z => z.Key == nameof(Value)));
            Parameters.Add(new BundleStepParameter() { StepId = _step.Id, Key = nameof(Value), Value = arg.ToString() });
            Value = arg; });
        return ValueTask.CompletedTask;

    }

    private ValueTask OnPathChanged(string arg)
    {
        _debouncer2.Debounce(() => {
            Parameters.Remove(Parameters.FirstOrDefault(z => z.Key == nameof(Path)));
            Parameters.Add(new BundleStepParameter() { StepId = _step.Id, Key = nameof(Path), Value = arg.ToString() });
            Path = arg; });

        return ValueTask.CompletedTask;
    }

    public bool CanExecute => true;

    public IUIElement UIElement { get; }

    public async ValueTask<ExecutedResult> ExecuteAsync(Dictionary<string, object> runtimeVariables)
    {

        using (FileStream fs = new FileStream(Path, FileMode.Create))
        {
            string val = Value;
            if(ValueGainFromParameters)
            {
                val = runtimeVariables.GetValueOrDefault(Value, "") as string;
            }
            await fs.WriteAsync(Encoding.UTF8.GetBytes(val));
            await fs.FlushAsync();
        }
        return ExecutedResult.Create(runtimeVariables);
    }
}
public static partial class GUI
{

    public static IUIExecutor WriteFileExecutor(BundleStep step)
    {
        return WriteFileExecutor(null, step);
    }

    public static IUIExecutor WriteFileExecutor(string? id, BundleStep step)
    {
        return new UIWriteFileExecutor(id, step);
    }


}