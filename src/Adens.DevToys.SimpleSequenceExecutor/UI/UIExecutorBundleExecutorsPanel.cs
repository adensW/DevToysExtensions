using Adens.DevToys.SimpleSequenceExecutor.Entities;
using DevToys.Api;
using System.Text.Json;
using static Adens.DevToys.SimpleSequenceExecutor.UI.GUI;
using static DevToys.Api.GUI;
namespace Adens.DevToys.SimpleSequenceExecutor.UI;
public interface IUIExecutorPanel : IUICard
{
    /// <summary>
    /// Gets the list of child elements.
    /// </summary>
    ExecutorBundle? Bundle { get; }
    /// <summary>
    /// Raised when <see cref="Children"/> is changed.
    /// </summary>
    event EventHandler? BundleChanged;
}
internal class UIExecutorBundleExecutorsPanel : UIElement, IUIExecutorPanel
{
    private ISettingsProvider _settingProvider;
  

    private ExecutorBundle? _bundle;
    public ExecutorBundle? Bundle {
        get => _bundle;
        internal set => SetPropertyValue(ref _bundle, value, BundleChanged);
    }
    private IUIElement? _ui;
    public IUIElement? UIElement { 
        get=>_ui;
        internal set => _ui=value;
    }
    private List<IUIExecutorWrapper> _executors =new List<IUIExecutorWrapper>();
    private int _signal = 0;
    public int Signal
    {
        get=>_signal;
        internal set => SetPropertyValue(ref _signal, value, ExecuteTrigger);
    }
    private Dictionary<string, object> RuntimeVariables { get; set; } = new Dictionary<string, object>();
    internal UIExecutorBundleExecutorsPanel(string? id, ISettingsProvider settingProvider) : base(id)
    {
        _button.OnClick(AddButtonClick);
        BundleChanged += RenderEventAction;
        ExecuteTrigger += Execute;
        _settingProvider = settingProvider;
        _settingProvider.SettingChanged += _settingProvider_SettingChanged;
    }

    private void _settingProvider_SettingChanged(object? sender, SettingChangedEventArgs e)
    {
        var currentBundle = _settingProvider.GetSetting(SimpleSequenceExecutorGui.currentBundle);
        if (currentBundle == null)
        {
            return;
        }
        if(currentBundle.Name!=_bundle?.Name)
        {
            Bundle = currentBundle;
        }
    }

    private void Execute(object? sender, EventArgs e)
    {
       
        foreach (var item in _executors)
        {
             var result = item.ExecuteAsync(RuntimeVariables);
        }
    }

    private void RenderEventAction(object? sender, EventArgs e)
    {
        if(_bundle==null)
        {
            return;
        }
     
        Render();
    }

    private IUIButton _button= Button().Text("Add");
    internal void Render()
    {
        if (_bundle == null)
        {
            return;
        }
        UIElement = null;
        _executors.Clear();
        foreach (var step in _bundle.Steps)
        {
            var wrapper = ExecutorGenerator.Generate(step);
            if (wrapper == null)
            {
                continue;
            }
            wrapper.StepChanged += Executor_StepChanged;
            wrapper.OnAddAfterClicked += OnAddAfterClicked;
            wrapper.OnMoveUpClicked += OnMoveUpClicked;
            wrapper.OnDeleteClicked += OnDeleteClicked;
            wrapper.OnMoveDownClicked += OnMoveDownClicked;
            wrapper.OnAddBeforeClicked += OnAddBeforeClicked;
            _executors.Add(wrapper);
        }
        UIElement = Stack(Guid.NewGuid().ToString()).Vertical().WithChildren(
            Stack(Guid.NewGuid().ToString()).Horizontal().WithChildren(
            Label().Text(_bundle.Name),
            Button().Text("Execute").OnClick(OnExecuteClick)

            ),
            Stack(Guid.NewGuid().ToString()).Vertical().WithChildren(
            _executors.ToArray()
                ),
            _button
            );
    }

    private void OnAddBeforeClicked(object? sender, ExecutorStepArgs e)
    {
        var a = JsonSerializer.Deserialize<ExecutorBundle>(JsonSerializer.Serialize(_bundle));
        int index = a.Steps.FindIndex(0, z => z.Id == e.Step.Id);
        if (index < 0) {
            index = 0;
        }
        a.Steps.Insert(index, new ExecutorStep() { 
            Id = Guid.NewGuid().ToString(),
            Type = Constants.EmptyExecutor
        });
        Bundle = a;
    }

    private void OnMoveDownClicked(object? sender, ExecutorStepArgs e)
    {
        var a = JsonSerializer.Deserialize<ExecutorBundle>(JsonSerializer.Serialize(_bundle));
        int index = a.Steps.FindIndex(0, z => z.Id == e.Step.Id);
        if (index >= 0 && index < a.Steps.Count - 1)
        {
            // 将该项与前一项交换位置
            ExecutorStep temp = a.Steps[index];
            a.Steps[index] = a.Steps[index + 1];
            a.Steps[index + 1] = temp;
            Bundle = a;
        }
    }

    private void OnDeleteClicked(object? sender, ExecutorStepArgs e)
    {
        _bundle.Steps.Remove(e.Step);
        Bundle = JsonSerializer.Deserialize<ExecutorBundle>(JsonSerializer.Serialize(_bundle));

    }

    private void OnMoveUpClicked(object? sender, ExecutorStepArgs e)
    {

        var a = JsonSerializer.Deserialize<ExecutorBundle>(JsonSerializer.Serialize(_bundle));
        int index = a.Steps.FindIndex(0, z => z.Id == e.Step.Id);
        if (index > 0 && index < a.Steps.Count)
        {
            // 将该项与前一项交换位置
            ExecutorStep temp = a.Steps[index];
            a.Steps[index] = a.Steps[index - 1];
            a.Steps[index - 1] = temp;
            Bundle = a;
        }
    }

    private void OnAddAfterClicked(object? sender, ExecutorStepArgs e)
    {
        var a = JsonSerializer.Deserialize<ExecutorBundle>(JsonSerializer.Serialize(_bundle));
        int index = a.Steps.FindIndex(0, z => z.Id == e.Step.Id);
        if (index < 0)
        {
            index = 0;
        }
        a.Steps.Insert(index+1, new ExecutorStep() { 
            Id = Guid.NewGuid().ToString(),
            Type = Constants.EmptyExecutor
        });
        Bundle = a;

    }

    private async ValueTask OnExecuteClick()
    {
        Signal++;
    }

    private async ValueTask AddButtonClick()
    {
        var a = JsonSerializer.Deserialize< ExecutorBundle >(JsonSerializer.Serialize( _bundle));
        a.Steps.Add(new ExecutorStep() { 
            Id = Guid.NewGuid().ToString(),
            Type = Constants.EmptyExecutor
        });
        Bundle = a;
    }

    private void Executor_StepChanged(object? sender, EventArgs e)
    {
        var executorWrapper = (IUIExecutorWrapper)sender!;
        foreach (var item in _bundle.Steps)
        {
            if (item.Id == executorWrapper.Step.Id)
            {
                item.Type = executorWrapper.Step.Type;
                item.Parameters = executorWrapper.Step.Parameters;
            }
        }
        //updated data 
        var bundles = _settingProvider.GetSetting(SimpleSequenceExecutorGui.bundles);
        foreach (var item in bundles)
        {
            if (item.Name == _bundle.Name)
            {
                item.Steps = _bundle.Steps;
            }
        }
        _settingProvider.SetSetting(SimpleSequenceExecutorGui.bundles, bundles);
        var a = JsonSerializer.Deserialize<ExecutorBundle>(JsonSerializer.Serialize(_bundle));
        Bundle = a;

    }
    private event EventHandler? ExecuteTrigger;
    public event EventHandler? BundleChanged;
    #region
    public event EventHandler? OrientationChanged;
    public event EventHandler? SpacingChanged;
    private UIOrientation _orientation = UIOrientation.Horizontal;
    private UISpacing _spacing = UISpacing.Small;
    public UIOrientation Orientation
    {
        get => _orientation;
        internal set => SetPropertyValue(ref _orientation, value, OrientationChanged);
    }

    public UISpacing Spacing
    {
        get => _spacing;
        internal set => SetPropertyValue(ref _spacing, value, SpacingChanged);
    }
    #endregion

}
public static partial class GUI
{
   
    public static IUIExecutorPanel UIExecutorPanel( ISettingsProvider settingsProvider)
    {
        return UIExecutorPanel(null, settingsProvider);
    }

    
    public static IUIExecutorPanel UIExecutorPanel(string? id,ISettingsProvider settingsProvider)
    {
        return new UIExecutorBundleExecutorsPanel(id, settingsProvider);
    }
    
    public static IUIExecutorPanel Fill(this IUIExecutorPanel element, ExecutorBundle bundle)
    {
        ((UIExecutorBundleExecutorsPanel)element).Bundle = bundle;
        ((UIExecutorBundleExecutorsPanel)element).Render();
        return element;
    }
}