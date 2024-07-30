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
    private List<IUIExecutorWrapper> _executors ;
    private int _signal = 0;
    public int Signal
    {
        get=>_signal;
        internal set => SetPropertyValue(ref _signal, value, ExecuteTrigger);
    }
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
              item.ExecuteAsync();
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
        _executors = new List<IUIExecutorWrapper>();
        foreach (var step in _bundle.Steps)
        {
            var wrapper = ExecutorGenerator.Generate(step);
            if (wrapper == null)
            {
                continue;
            }
            wrapper.StepChanged += Executor_StepChanged;
            _executors.Add(wrapper);
        }
        UIElement = Stack().Vertical().WithChildren(
            Stack().Horizontal().WithChildren(
            Label().Text(_bundle.Name),
            Button().Text("Execute").OnClick(OnExecuteClick)

            ),
            Stack().Vertical().WithChildren(
            _executors.ToArray()
                ),
            _button
            );
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