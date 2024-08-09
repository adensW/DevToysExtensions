using Adens.DevToys.SimpleSequenceExecutor.Entities;
using DevToys.Api;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;
using static Adens.DevToys.SimpleSequenceExecutor.UI.GUI;
using static DevToys.Api.GUI;
namespace Adens.DevToys.SimpleSequenceExecutor.UI;
public interface IUIExecutorPanel : IUICard
{
    void SetBundle(Bundle bundle);
    void Render();
}
internal class UIExecutorBundleExecutorsPanel : UIElement, IUIExecutorPanel
{

    private ObservableCollection<BundleStep> _steps = new ObservableCollection<BundleStep>();
    public ObservableCollection<BundleStep> Steps => _steps;
    public void Dispose()
    {
        _steps.CollectionChanged -= Bundles_CollectionChanged;
    }
    private void Bundles_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Render();
    }
    private Bundle? _bundle;

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
    internal UIExecutorBundleExecutorsPanel(string? id) : base(id)
    {
        _button.OnClick(AddButtonClick);
        ExecuteTrigger += Execute;
     
    }
    private void Execute(object? sender, EventArgs e)
    {
       
        foreach (var item in _executors)
        {
             var result = item.ExecuteAsync(RuntimeVariables);
        }
    }

    private IUIButton _button= Button().Text("Add");
    public void Render()
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

    private void OnAddBeforeClicked(object? sender, BundleStepArgs e)
    {
        int index = Steps.IndexOf(e.Step);
        if (index < 0) {
            index = 0;
        }
        Steps.Insert(index, new BundleStep() { 
            Id = Guid.NewGuid(),
            Type = Constants.EmptyExecutor,
            BundleId = _bundle.Id
        });
    }

    private void OnMoveDownClicked(object? sender, BundleStepArgs e)
    {
        int index = Steps.IndexOf(e.Step);
        if (index >= 0 && index < Steps.Count - 1)
        {
            // 将该项与前一项交换位置
            Steps.Move(index, index + 1);
        }
    }

    private void OnDeleteClicked(object? sender, BundleStepArgs e)
    {
        Steps.Remove(e.Step);
    }

    private void OnMoveUpClicked(object? sender, BundleStepArgs e)
    {
        int index = Steps.IndexOf(e.Step);
        if (index > 0 && index < Steps.Count)
        {
            // 将该项与前一项交换位置
            Steps.Move(index, index - 1);
        }
    }

    private void OnAddAfterClicked(object? sender, BundleStepArgs e)
    {
        int index = Steps.IndexOf( e.Step);
        if (index < 0)
        {
            index = 0;
        }
        Steps.Insert(index+1, new BundleStep() { 
            Id = Guid.NewGuid(),
            Type = Constants.EmptyExecutor,
            BundleId=_bundle.Id
        });

    }

    private async ValueTask OnExecuteClick()
    {
        Signal++;
    }

    private async ValueTask AddButtonClick()
    {
        Steps.Add(new BundleStep() { 
            Id = Guid.NewGuid(),
            Type = Constants.EmptyExecutor,
            BundleId = _bundle.Id
        });
    }

    private void Executor_StepChanged(object? sender, EventArgs e)
    {
        var executorWrapper = (IUIExecutorWrapper)sender!;
        foreach (var item in Steps)
        {
            if (item.Id == executorWrapper.Step.Id)
            {
                item.Type = executorWrapper.Step.Type;
            }
        }

    }
    public void SetBundle(Bundle bundle)
    {
        _bundle = bundle;
    }
    private event EventHandler? ExecuteTrigger;
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
   
    public static IUIExecutorPanel UIExecutorBundleExecutorsPanel( )
    {
        return UIExecutorBundleExecutorsPanel(null);
    }
    
    public static IUIExecutorPanel UIExecutorBundleExecutorsPanel(string? id)
    {
        return new UIExecutorBundleExecutorsPanel(id);
    }
    
    public static IUIExecutorPanel Fill(this IUIExecutorPanel element, Bundle bundle)
    {
        element.SetBundle(bundle);
        element.Render();
        return element;
    }
}