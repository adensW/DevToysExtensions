using Adens.DevToys.SimpleSequenceExecutor.Args;
using Adens.DevToys.SimpleSequenceExecutor.Entities;
using Adens.DevToys.SimpleSequenceExecutor.Helpers;
using DebounceThrottle;
using DevToys.Api;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;

namespace Adens.DevToys.SimpleSequenceExecutor.UI;
[Export(typeof(IUIExecutor))]
internal class UITextDisplayExecutor : UIElement, IUIExecutor
{
    public void Dispose()
    {
        Parameters.CollectionChanged -= SaveParameters;
    }
    public ObservableCollection<BundleStepParameter> Parameters { get; set; } = new ObservableCollection<BundleStepParameter>();
    private Dictionary<string, object> ParametersDict { get=> Parameters.ToDictionary< BundleStepParameter,string,object>(z=>z.Key,z=>z.Value); }

    public event EventHandler? ParametersChanged;
    private string text;
    private BundleStep _step;
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
    public UITextDisplayExecutor(string? id, BundleStep step): base(id)
    {
        _step = step;
        ReloadParameters();
        Parameters.CollectionChanged += SaveParameters;

        _debouncer1 = new DebounceDispatcher(interval: TimeSpan.FromMilliseconds(500),
          maxDelay: TimeSpan.FromSeconds(3));
        if (ParametersDict.TryGetValue(nameof(Text), out object val1))
        {
            text = val1?.ToString() ?? string.Empty;
        }
        if (ParametersDict.TryGetValue(nameof(TextGainFromParameters), out object val2))
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
        Parameters.AddRange(db.Table<BundleStepParameter>().Where(x => x.StepId==_step.Id).ToList());
    }

    private  ValueTask OnDisplayTextChange(string arg)
    {
        _debouncer1.Debounce(() => {
            var temp = Parameters.FirstOrDefault(z => z.Key == nameof(Text));
            if (temp == null)
            {
                temp = new BundleStepParameter()
                {
                    Id=Guid.NewGuid(),
                    Key = nameof(Text),
                    StepId = _step.Id,
                    Type = typeof(string).ToString(),
                    Value = arg
                };
                Parameters.Add(temp);
            }
            else
            {
                temp.Value = arg.ToString();
                Parameters.Remove(temp);
                Parameters.Add(temp);
            }
            Text = arg;
        });
        return ValueTask.CompletedTask;
    }

    private  ValueTask ToggleTextGainFromParameters(bool arg)
    {
        var temp = Parameters.FirstOrDefault(z => z.Key == nameof(TextGainFromParameters));
        if (temp == null)
        {
            temp = new BundleStepParameter()
            {
                Id = Guid.NewGuid(),
                Key = nameof(TextGainFromParameters),
                StepId = _step.Id,
                Type = typeof(bool).ToString(),
                Value = arg.ToString()
            };
            Parameters.Add(temp);
        }
        else
        {
            temp.Value = arg.ToString();
            Parameters.Remove(temp);
            Parameters.Add(temp);
        }
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

    public static IUIExecutor TextDisplayExecutor(BundleStep step)
    {
        return TextDisplayExecutor(null,step);
    }
    public static IUIExecutor TextDisplayExecutor(string? id, BundleStep step)
    {
        return new UITextDisplayExecutor(id,step);
    }

}