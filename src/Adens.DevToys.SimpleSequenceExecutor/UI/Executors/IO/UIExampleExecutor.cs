using Adens.DevToys.SimpleSequenceExecutor.Args;
using Adens.DevToys.SimpleSequenceExecutor.Entities;
using Adens.DevToys.SimpleSequenceExecutor.Helpers;
using DebounceThrottle;
using DevToys.Api;
using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using static DevToys.Api.GUI;
namespace Adens.DevToys.SimpleSequenceExecutor.UI;

[Export(typeof(IUIExecutor))]
internal class UIExampleExecutor : UIElement, IUIExecutor
{
    public event EventHandler? ParametersChanged;
    private BundleStep _step;
    private string exampleValue = string.Empty;

    public ObservableCollection<BundleStepParameter> Parameters { get; set; } = new ObservableCollection<BundleStepParameter>();
    private Dictionary<string, object> ParametersDict { get => Parameters.ToDictionary<BundleStepParameter, string, object>(z => z.Key, z => z.Value); }
    public string ExampleValue
    {
        get => exampleValue;
        internal set => SetPropertyValue(ref exampleValue, value, ParametersChanged);
    }
    
  
    public bool exampleValueGainFromParameters = true;
    public bool ExampleValueGainFromParameters
    {
        get => exampleValueGainFromParameters;
        internal set => SetPropertyValue(ref exampleValueGainFromParameters, value, ParametersChanged);
    }
    private DebounceDispatcher _debouncer1;
    private IUISingleLineTextInput _valueInput = SingleLineTextInput().Title("Value");
    public UIExampleExecutor(string? id,BundleStep step) : base(id)
    {
        _step = step;
        ReloadParameters();
        _debouncer1 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
            maxDelay: TimeSpan.FromSeconds(3));
        if (ParametersDict.TryGetValue(nameof(ExampleValue), out object val1))
        {
            exampleValue = val1?.ToString() ?? string.Empty;
        }
        if (ParametersDict.TryGetValue(nameof(ExampleValueGainFromParameters), out object val2))
        {
            exampleValueGainFromParameters = true;
            if ((bool.TryParse(val2?.ToString(), out bool isChoosed)))
            {
                exampleValueGainFromParameters = isChoosed;
            }
        }

        var switcher = Switch()
                        .OnText("Gain from runtime")
                        .OffText("Not gain from runtime")
                        .OnToggle(ToggleExampleValueGainFromParameters);

        if (exampleValueGainFromParameters)
        {
            switcher.On();
            _valueInput.Text(exampleValue).OnTextChanged(OnValueChanged);
        }
        else
        {
            switcher.Off();
            _valueInput = MultiLineTextInput().Title("Value").Text(exampleValue).Extendable().Editable().OnTextChanged(OnValueChanged);
        }
        UIElement = Stack().Vertical().WithChildren(
                       Label().Text("Write Executor"),
                       Stack().Vertical().WithChildren(switcher, _valueInput)
                    );
      
    }
    private void ReloadParameters()
    {
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        Parameters.Clear();
        Parameters.AddRange(db.Table<BundleStepParameter>().Where(x => x.StepId == _step.Id));
    }
    private ValueTask ToggleExampleValueGainFromParameters(bool arg)
    {
        Parameters.Remove(Parameters.FirstOrDefault(z => z.Key == nameof(ExampleValueGainFromParameters)));
        Parameters.Add(new BundleStepParameter() { StepId = _step.Id, Key = nameof(ExampleValueGainFromParameters), Value = arg.ToString() });
        ExampleValueGainFromParameters = arg;
        return ValueTask.CompletedTask;
    }
    private ValueTask OnValueChanged(string arg)
    {
     
        _debouncer1.Debounce(() => {
            Parameters.Remove(Parameters.FirstOrDefault(z => z.Key == nameof(ExampleValue)));
            Parameters.Add(new BundleStepParameter() { StepId = _step.Id, Key = nameof(ExampleValue), Value = arg });
            ExampleValue = arg; });
        return ValueTask.CompletedTask;

    }

   

    public bool CanExecute => true;

    public IUIElement UIElement { get; }

    public async ValueTask<ExecutedResult> ExecuteAsync(Dictionary<string, object> runtimeVariables)
    {
        return ExecutedResult.Create(runtimeVariables);
    }
}
public static partial class GUI
{

    public static IUIExecutor ExampleExecutor(BundleStep step)
    {
        return ExampleExecutor(null, step);
    }

    public static IUIExecutor ExampleExecutor(string? id,BundleStep step)
    {
        return new UIExampleExecutor(id,step);
    }
}