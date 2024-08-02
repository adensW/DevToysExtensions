using Adens.DevToys.SimpleSequenceExecutor.Entities;
using DebounceThrottle;
using DevToys.Api;
using System.ComponentModel.Composition;
using static DevToys.Api.GUI;
namespace Adens.DevToys.SimpleSequenceExecutor.UI;

[Export(typeof(IUIExecutor))]
internal class UIExampleExecutor : UIElement, IUIExecutor
{
    public event EventHandler? ParametersChanged;
    private string exampleValue = string.Empty;

    public Dictionary<string, object> Parameters { get; set; }
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
    public UIExampleExecutor(string? id, Dictionary<string, object> parameters) : base(id)
    {
        _debouncer1 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
            maxDelay: TimeSpan.FromSeconds(3));
     
        Parameters = parameters;
      
        if (Parameters.TryGetValue(nameof(ExampleValue), out object val1))
        {
            exampleValue = val1?.ToString() ?? string.Empty;
        }
        if (Parameters.TryGetValue(nameof(ExampleValueGainFromParameters), out object val2))
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
    private ValueTask ToggleExampleValueGainFromParameters(bool arg)
    {
        Parameters.Remove(nameof(ExampleValueGainFromParameters));
        Parameters.Add(nameof(ExampleValueGainFromParameters), arg);
        ExampleValueGainFromParameters = arg;
        return ValueTask.CompletedTask;
    }
    private ValueTask OnValueChanged(string arg)
    {
        Parameters.Remove(nameof(ExampleValue));
        Parameters.Add(nameof(ExampleValue), arg);
        _debouncer1.Debounce(() => ExampleValue = arg);
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

    public static IUIExecutor ExampleExecutor(Dictionary<string, object> parameters)
    {
        return ExampleExecutor(null, parameters);
    }

    public static IUIExecutor ExampleExecutor(string? id, Dictionary<string, object> parameters)
    {
        return new UIExampleExecutor(id, parameters);
    }
}