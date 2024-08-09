using Adens.DevToys.SimpleSequenceExecutor.Args;
using Adens.DevToys.SimpleSequenceExecutor.Entities;
using CommunityToolkit.Diagnostics;
using DevToys.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;
namespace Adens.DevToys.SimpleSequenceExecutor.UI;
public class StepChangedArgs : EventArgs
{
    public string Id { get; set; }
    public string NewType { get; set; }
    

    public StepChangedArgs(string id, string newType) :base()
    {
        NewType = newType;
    }
}
public class BundleStepArgs : EventArgs
{
    public BundleStep Step { get; set; }


    public BundleStepArgs(BundleStep step) : base()
    {
        Step = step;
    }
}
public interface IUIExecutorWrapper : IUICard
{
    BundleStep Step { get; }
    IUIExecutor UIExecutor { get; }
    event EventHandler? UIExecutorChanged;
    event EventHandler? StepChanged;
    event EventHandler<BundleStepArgs>? OnAddAfterClicked;
    event EventHandler<BundleStepArgs>? OnMoveUpClicked;
    event EventHandler<BundleStepArgs>? OnDeleteClicked;
    event EventHandler<BundleStepArgs>? OnMoveDownClicked;
    event EventHandler<BundleStepArgs>? OnAddBeforeClicked;

    ValueTask<ExecutedResult> ExecuteAsync(Dictionary<string,object> runtimeVariables);
}
internal class UIBundleStepWrapper : UIElement, IUIExecutorWrapper
{
    private BundleStep _step;
    public BundleStep Step {
        get => _step;
        internal set=>SetPropertyValue(ref _step,value,StepChanged);
    }
    private IUIElement _ui;
    public IUIElement UIElement
    {
        get =>_ui;
    }
    private IUIExecutor _executor;
    public IUIExecutor UIExecutor { 
        get=> _executor;
        internal set=> SetPropertyValue(ref _executor,value,UIExecutorChanged); }
    //private IUIStack _select;
    //private IUISelectDropDownList _selectDropdownList = SelectDropDownList(Guid.NewGuid().ToString()).AlignHorizontally(UIHorizontalAlignment.Left);
    internal UIBundleStepWrapper(string? id, BundleStep step) : base(id)
    {
        UIExecutor = ExecutorGenerator.Generate(step.Type);
        StepChanged += Rerender;
        Step = step;

    }
    private void Rerender(object? sender, EventArgs e)
    {
        Render();
    }

    private void Render()
    {
        List<IUIDropDownListItem> menus = new List<IUIDropDownListItem>();

        foreach (var item in Constants.Executors)
        {
            menus.Add(Item(text: item, value: item));
        }
        int index = Array.IndexOf(Constants.Executors, Step.Type);
        IUISelectDropDownList _selectDropdownList = SelectDropDownList(Guid.NewGuid().ToString()).AlignHorizontally(UIHorizontalAlignment.Left).Title("Select a executor:")
                 .WithItems(
                 menus.ToArray())
                 .Select(index)
                 .OnItemSelected(OnItemClickAsync);

        IUIStack _select =
         Stack(Guid.NewGuid().ToString()).Horizontal().NoSpacing().WithChildren(
          _selectDropdownList);
        _ui =
            SplitGrid(Guid.NewGuid().ToString()).Horizontal().TopPaneLength(new UIGridLength(80, UIGridUnitType.Pixel)).BottomPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
            .WithTopPaneChild(_select).WithBottomPaneChild(
                    SplitGrid(Guid.NewGuid().ToString())
                        .Vertical()
                        .LeftPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .RightPaneLength(new UIGridLength(50, UIGridUnitType.Pixel))
                        .WithLeftPaneChild(UIExecutor)
                        .WithRightPaneChild(Stack(Guid.NewGuid().ToString()).SmallSpacing().Vertical().WithChildren(
                            Button().Icon("FluentSystemIcons", '\uE571').OnClick(OnAddBeforeClick),
                            Button().Icon("FluentSystemIcons", '\uF1A5').OnClick(OnMoveUpClick),
                            Button().Icon("FluentSystemIcons", '\uF34C').OnClick(OnDeleteClick),
                            Button().Icon("FluentSystemIcons", '\uF14F').OnClick(OnMoveDownClick),
                            Button().Icon("FluentSystemIcons", '\uE56F').OnClick(OnAddAfterClick)
            ))
                        );
    }

    private async ValueTask OnAddAfterClick()
    {
        OnAddAfterClicked?.Invoke(this, new BundleStepArgs(Step));
    }

    private async ValueTask OnMoveDownClick()
    {
        OnMoveDownClicked?.Invoke(this, new BundleStepArgs(Step));
    }
    private async ValueTask OnDeleteClick()
    {
        OnDeleteClicked?.Invoke(this, new BundleStepArgs(Step));
    }

    private async ValueTask OnMoveUpClick()
    {
        OnMoveUpClicked?.Invoke(this, new BundleStepArgs(Step));
    }

    private async ValueTask OnAddBeforeClick()
    {
        OnAddBeforeClicked?.Invoke(this, new BundleStepArgs(Step));
    }

    private async ValueTask OnItemClickAsync(IUIDropDownListItem? item)
    {
        if (item == null)
        {
            return;
        }
        UIExecutor = ExecutorGenerator.Generate(item.Value as string);
     
        Step = new BundleStep { Id= Step.Id, Type= item.Value as string };
    }

    public async ValueTask<ExecutedResult> ExecuteAsync(Dictionary<string, object> runtimeVariables)
    {

        if (_executor != null)
        {
            return await _executor.ExecuteAsync(runtimeVariables);
        }
        return ExecutedResult.Create(runtimeVariables);
    }
    public event EventHandler? StepChanged;
    private event EventHandler? RerenderTrigger;
    public event EventHandler<BundleStepArgs>? OnAddAfterClicked;
    public event EventHandler<BundleStepArgs>? OnMoveUpClicked;
    public event EventHandler<BundleStepArgs>? OnDeleteClicked;
    public event EventHandler<BundleStepArgs>? OnMoveDownClicked;
    public event EventHandler<BundleStepArgs>? OnAddBeforeClicked;
    #region 
    public event EventHandler? OrientationChanged;
    public event EventHandler? SpacingChanged;
    public event EventHandler? UIExecutorChanged;
  

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

    public static IUIExecutorWrapper UIExecutorWrapper(BundleStep step)
    {
        return UIExecutorWrapper(null, step);
    }
    public static IUIExecutorWrapper UIExecutorWrapper(string? id, BundleStep step)
    {
        return new UIBundleStepWrapper(id, step);
    }
}