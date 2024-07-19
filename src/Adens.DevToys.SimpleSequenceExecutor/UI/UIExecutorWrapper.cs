using Adens.DevToys.SimpleSequenceExecutor.Entities;
using CommunityToolkit.Diagnostics;
using DevToys.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;
namespace Adens.DevToys.SimpleSequenceExecutor.UI;

public interface IUIExecutorWrapper : IUICard
{
    IUIExecutor UIExecutor { get; }
}
internal class UIExecutorWrapper : UIElement, IUIExecutorWrapper
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

    public IUIElement UIElement
    {
        get;
    }
    private IUIExecutor _executor;
    public IUIExecutor UIExecutor { get=> _executor

            ; internal set=> SetPropertyValue(ref _executor,value,UIExecutorChanged); }
    private IUISelectDropDownList _select;

    internal UIExecutorWrapper(string? id,IUIExecutor executor) : base(id)
    {
        List<IUIDropDownListItem> menus = new List<IUIDropDownListItem>();

        foreach (var item in Constants.Executors)
        {
            menus.Add(Item(text: item, value: item));
        }
        _select = SelectDropDownList()
                    .AlignHorizontally(UIHorizontalAlignment.Left)
                    .WithItems(
                    menus.ToArray())
                    .OnItemSelected(OnItemClickAsync);
        UIElement =
            SplitGrid().Horizontal().TopPaneLength(new UIGridLength(300, UIGridUnitType.Pixel)).BottomPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
            .WithTopPaneChild(Button().Text("Top")).WithBottomPaneChild(
                    SplitGrid()
                        .Vertical()
                        .LeftPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .RightPaneLength(new UIGridLength(50, UIGridUnitType.Pixel))
                        .WithLeftPaneChild(executor)
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
     
    }
    public event EventHandler? OrientationChanged;
    public event EventHandler? SpacingChanged;
    public event EventHandler? UIExecutorChanged;


}
public static partial class GUI
{

    public static IUIExecutorWrapper UIExecutorWrapper(IUIExecutor executor)
    {
        return UIExecutorWrapper(null, executor);
    }
    public static IUIExecutorWrapper UIExecutorWrapper(string? id, IUIExecutor executor)
    {
        return new UIExecutorWrapper(id,executor);
    }
}