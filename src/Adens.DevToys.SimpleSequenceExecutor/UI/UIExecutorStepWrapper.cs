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
public interface IUIExecutorWrapper : IUICard
{
    ExecutorStep Step { get; }
    IUIExecutor UIExecutor { get; }
    event EventHandler? UIExecutorChanged;
    event EventHandler? StepChanged;
    ValueTask ExecuteAsync();
}
internal class UIExecutorStepWrapper : UIElement, IUIExecutorWrapper
{
    private ExecutorStep _step;
    public ExecutorStep Step {
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
    private IUIStack _select;

    internal UIExecutorStepWrapper(string? id, ExecutorStep step) : base(id)
    {
        UIExecutor = ExecutorGenerator.Generate(step.Type);
        StepChanged += Rerender;

        List<IUIDropDownListItem> menus = new List<IUIDropDownListItem>();

        foreach (var item in Constants.Executors)
        {
            menus.Add(Item(text: item, value: item));
        }
        _select =
         Stack().Horizontal().NoSpacing().WithChildren(
             Card(Label().Style(UILabelStyle.Body).Text("Select a executor:")), Card(SelectDropDownList(Guid.NewGuid().ToString())
                 .AlignHorizontally(UIHorizontalAlignment.Left)
                 .WithItems(
                 menus.ToArray())
                 .OnItemSelected(OnItemClickAsync)));
        Step = step;
    }

    private void Rerender(object? sender, EventArgs e)
    {
        Render();
    }

    private void Render()
    {
        _ui =
            SplitGrid().Horizontal().TopPaneLength(new UIGridLength(80, UIGridUnitType.Pixel)).BottomPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
            .WithTopPaneChild(_select).WithBottomPaneChild(
                    SplitGrid()
                        .Vertical()
                        .LeftPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .RightPaneLength(new UIGridLength(50, UIGridUnitType.Pixel))
                        .WithLeftPaneChild(UIExecutor)
                        .WithRightPaneChild(Stack().SmallSpacing().Vertical().WithChildren(
                            Button().Icon("FluentSystemIcons", '\uE571'),
                            Button().Icon("FluentSystemIcons", '\uF1A5'),
                            Button().Icon("FluentSystemIcons", '\uF34C'),
                            Button().Icon("FluentSystemIcons", '\uF14F'),
                            Button().Icon("FluentSystemIcons", '\uE56F')
            ))
                        );
    }
    private async ValueTask OnItemClickAsync(IUIDropDownListItem? item)
    {
        if (item == null)
        {
            return;
        }
       
        UIExecutor = ExecutorGenerator.Generate(item.Value as string);
        Step = new ExecutorStep { Id= Step.Id, Type= item.Value as string };
    }

    public async ValueTask ExecuteAsync()
    {

        if (_executor != null)
        {
            await _executor.ExecuteAsync();
        }
    }
    public event EventHandler? StepChanged;
    private event EventHandler? RerenderTrigger;

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

    public static IUIExecutorWrapper UIExecutorWrapper(ExecutorStep step)
    {
        return UIExecutorWrapper(null, step);
    }
    public static IUIExecutorWrapper UIExecutorWrapper(string? id, ExecutorStep step)
    {
        return new UIExecutorStepWrapper(id, step);
    }
}