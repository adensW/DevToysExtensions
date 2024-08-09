using Adens.DevToys.SimpleSequenceExecutor.Args;
using CliWrap;
using DebounceThrottle;
using DevToys.Api;
using System;
using System.Collections.Generic;
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
    private string path = string.Empty;
    private string _value = string.Empty;

    public Dictionary<string, object> Parameters { get; set; }
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
    public UIWriteFileExecutor(string? id) : base(id)
    {
        _debouncer1 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
            maxDelay: TimeSpan.FromSeconds(3));
        _debouncer2 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
         maxDelay: TimeSpan.FromSeconds(3));
        
        if (Parameters.TryGetValue(nameof(Path), out object val1))
        {
            path = val1?.ToString() ?? string.Empty;
        }
        if (Parameters.TryGetValue(nameof(Value), out object val2))
        {
            _value = val2?.ToString() ?? string.Empty;
        }
        if (Parameters.TryGetValue(nameof(ValueGainFromParameters), out object val3))
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
    private ValueTask ToggleValueGainFromParameters(bool arg)
    {
        Parameters.Remove(nameof(ValueGainFromParameters));
        Parameters.Add(nameof(ValueGainFromParameters), arg);
        ValueGainFromParameters = arg;
        return ValueTask.CompletedTask;
    }
    private ValueTask OnValueChanged(string arg)
    {
        Parameters.Remove(nameof(Value));
        Parameters.Add(nameof(Value), arg);
        _debouncer1.Debounce(() => Value = arg);
        return ValueTask.CompletedTask;

    }

    private ValueTask OnPathChanged(string arg)
    {
        Parameters.Remove(nameof(Path));
        Parameters.Add(nameof(Path), arg);
        _debouncer2.Debounce(() => Path = arg);

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

    public static IUIExecutor WriteFileExecutor()
    {
        return WriteFileExecutor(null);
    }

    public static IUIExecutor WriteFileExecutor(string? id)
    {
        return new UIWriteFileExecutor(id);
    }


}