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
internal class UIExecutorPanel : UIElement, IUIExecutorPanel
{
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

    private ExecutorBundle? _bundle;
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
    internal UIExecutorPanel(string? id):base(id)
    {
        _button.OnClick(AddButtonClick);
        BundleChanged += RenderEventAction;
        ExecuteTrigger += Execute;
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
        a.Steps.Add(new ExecutorStep());
        Bundle = a;
    }

   

    private void Executor_StepChanged(object? sender, StepChangedArgs e)
    {
        _bundle.Steps.ForEach(z =>
        {
            if (z.Id == e.Id) {
                z.Type = e.NewType;
            }
        }
        );
        var a = JsonSerializer.Deserialize<ExecutorBundle>(JsonSerializer.Serialize(_bundle));
        Bundle = a;

    }

    public ExecutorBundle? Bundle { 
        get=> _bundle;
        internal set => SetPropertyValue(ref _bundle, value, BundleChanged);
        }
   
    public event EventHandler? BundleChanged;
    public event EventHandler? OrientationChanged;
    public event EventHandler? SpacingChanged;
    private event EventHandler? ExecuteTrigger;


}
public static partial class GUI
{
   
    public static IUIExecutorPanel UIExecutorPanel()
    {
        return UIExecutorPanel(null);
    }

    
    public static IUIExecutorPanel UIExecutorPanel(string? id)
    {
        return new UIExecutorPanel(id);
    }
    
    public static IUIExecutorPanel Fill(this IUIExecutorPanel element, ExecutorBundle bundle)
    {
        ((UIExecutorPanel)element).Bundle = bundle;
        ((UIExecutorPanel)element).Render();
        return element;
    }
}