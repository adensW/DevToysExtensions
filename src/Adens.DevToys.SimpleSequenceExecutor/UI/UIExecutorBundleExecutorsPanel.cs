using Adens.DevToys.SimpleSequenceExecutor.Entities;
using Adens.DevToys.SimpleSequenceExecutor.Helpers;
using DevToys.Api;
using SQLite;
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
        _steps.CollectionChanged -= Steps_CollectionChanged;
    }
    private void Steps_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        StoreSteps();
        Render();
    }
    private void StoreSteps()
    {
        if (Bundle == null)
        {
            return;
        }
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        //db.Table<BundleStep>().Delete(x => x.BundleId == Bundle.Id);
        foreach (var item in _steps)
        {
            db.InsertOrReplace(item);
        }
    }
    private Bundle? _bundle;
    public Bundle? Bundle { get => _bundle;
        internal set => SetPropertyValue(ref _bundle, value, OnBundleChanged);
    }

    private void OnBundleChanged(object? sender, EventArgs e)
    {
        Render();
    }

    private IUIElement? _ui;
    public IUIElement? UIElement
    {
        get => _ui;
        internal set => _ui = value;
    }
    private List<IUIExecutorWrapper> _executors = new List<IUIExecutorWrapper>();
    private int _signal = 0;
    public int Signal
    {
        get => _signal;
        internal set => SetPropertyValue(ref _signal, value, ExecuteTrigger);
    }
    private bool _rerenderTrigger = false;
    public bool RerenderTrigger
    {
        get => _rerenderTrigger;
        internal set => SetPropertyValue(ref _rerenderTrigger, value, Rerender);
    }

   

    private Dictionary<string, object> RuntimeVariables { get; set; } = new Dictionary<string, object>();
    internal UIExecutorBundleExecutorsPanel(string? id, Bundle bundle) : base(id)
    {
        _bundle = bundle;
        ReloadSteps();
        _steps.CollectionChanged += Steps_CollectionChanged;
        _button.OnClick(AddButtonClick);
        ExecuteTrigger += Execute;
        Render();
    }
  
    private void Execute(object? sender, EventArgs e)
    {

        foreach (var item in _executors)
        {
            var result = item.ExecuteAsync(RuntimeVariables);
        }
    }

    private IUIButton _button = Button().Text("Add");
    private void Rerender(object? sender, EventArgs e)
    {
        Render();
    }
    public void Render()
    {
        if (Bundle == null)
        {
            return;
        }
        UIElement = null;
        _executors.Clear();
        foreach (var step in Steps)
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
            Label().Text(Bundle.Name),
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
        if (index < 0)
        {
            index = 0;
        }
        Steps.Insert(index, new BundleStep()
        {
            Id = Guid.NewGuid(),
            Type = Constants.EmptyExecutor,
            BundleId = Bundle.Id
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
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        db.Table<BundleStep>().Delete(x => x.Id == e.Step.Id);
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
        int index = Steps.IndexOf(e.Step);
        if (index < 0)
        {
            index = 0;
        }
        Steps.Insert(index + 1, new BundleStep()
        {
            Id = Guid.NewGuid(),
            Type = Constants.EmptyExecutor,
            BundleId = Bundle.Id
        });

    }

    private async ValueTask OnExecuteClick()
    {
        Signal++;
    }

    private async ValueTask AddButtonClick()
    {
        Steps.Add(new BundleStep()
        {
            Id = Guid.NewGuid(),
            Type = Constants.EmptyExecutor,
            BundleId = Bundle.Id
        });
    }

    private void Executor_StepChanged(object? sender, EventArgs e)
    {
        var executorWrapper = (IUIExecutorWrapper)sender!;
        foreach (var item in Steps)
        {
            if (item.Id == executorWrapper.Step.Id)
            {
                Steps.Remove(item);
                Steps.Add(executorWrapper.Step);
            }
        }
    }
    public void SetBundle(Bundle bundle)
    {
        _bundle = bundle;
        ReloadSteps();
    }
    private void ReloadSteps()
    {
        if (Bundle == null)
        {
            return;
        }
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        Steps.Clear();
        Steps.AddRange(db.Table<BundleStep>().Where(x => x.BundleId == Bundle.Id));
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

    public static IUIExecutorPanel UIExecutorBundleExecutorsPanel(Bundle bundle)
    {
        return UIExecutorBundleExecutorsPanel(null,bundle);
    }

    public static IUIExecutorPanel UIExecutorBundleExecutorsPanel(string? id, Bundle bundle)
    {
        return new UIExecutorBundleExecutorsPanel(id,bundle);
    }

    public static IUIExecutorPanel Fill(this IUIExecutorPanel element, Bundle bundle)
    {
        element.SetBundle(bundle);
        return element;
    }
}