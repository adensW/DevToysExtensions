using Adens.DevToys.SimpleSequenceExecutor.Args;
using DebounceThrottle;
using DevToys.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;

namespace Adens.DevToys.SimpleSequenceExecutor.UI;
[Export(typeof(IUIExecutor))]
internal class UITextDisplayExecutor : UIElement, IUIExecutor
{
    public Dictionary<string, object> Parameters { get; set; }

    public event EventHandler? ParametersChanged;
    private string text;
   
    public string Text
    {
        get => text;
        internal set => SetPropertyValue(ref text, value, ParametersChanged);
    }
    public bool textGainFromParameters = true;
    public bool TextGainFromParameters
    {
        get => textGainFromParameters;
        internal set => SetPropertyValue(ref textGainFromParameters, value, ParametersChanged);
    }

    private DebounceDispatcher _debouncer1;
    private IUILabel _displayLabel = Label().Text("");
    public UITextDisplayExecutor(string? id) : base(id)
    {
        _debouncer1 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
          maxDelay: TimeSpan.FromSeconds(3));
        if (Parameters.TryGetValue(nameof(Text), out object val1))
        {
            text = val1?.ToString() ?? string.Empty;
        }
        if (Parameters.TryGetValue(nameof(TextGainFromParameters), out object val2))
        {
            textGainFromParameters = true;
            if((bool.TryParse(val2?.ToString(), out bool isChoosed))){
                textGainFromParameters = isChoosed;
            }
        }
        var switcher = Switch()
                        .OnText("Gain from runtime")
                        .OffText("Not gain from runtime")
                        .OnToggle(ToggleTextGainFromParameters);
        if (textGainFromParameters)
        {
            switcher.On();
        }
        UIElement = 
            Stack().Vertical().WithChildren(
                  Stack().Horizontal().WithChildren(
                     switcher,
                      SingleLineTextInput().Text(text).HideCommandBar().OnTextChanged(OnDisplayTextChange)),
                  _displayLabel
                  );
    }

    private  ValueTask OnDisplayTextChange(string arg)
    {
        Parameters.Remove(nameof(Text));
        Parameters.Add(nameof(Text), arg);
        _debouncer1.Debounce(() => Text = arg);
        return ValueTask.CompletedTask;
    }

    private  ValueTask ToggleTextGainFromParameters(bool arg)
    {
        Parameters.Remove(nameof(TextGainFromParameters));
        Parameters.Add(nameof(TextGainFromParameters), arg);
        TextGainFromParameters = arg;
        return ValueTask.CompletedTask;
    }

    public bool CanExecute => false;

    public IUIElement UIElement { get; }

    public ValueTask<ExecutedResult> ExecuteAsync(Dictionary<string, object> runtimeVariables)
    {
        if (TextGainFromParameters)
        {
            _displayLabel.Text(runtimeVariables.GetValueOrDefault(Text, "") as string);

        }
        else
        {
            _displayLabel.Text(Text);
        }
        return ValueTask.FromResult(ExecutedResult.Create(runtimeVariables));
    }
   
}
public static partial class GUI
{

    public static IUIExecutor TextDisplayExecutor()
    {
        return TextDisplayExecutor(null);
    }
    public static IUIExecutor TextDisplayExecutor(string? id)
    {
        return new UITextDisplayExecutor(id);
    }

}