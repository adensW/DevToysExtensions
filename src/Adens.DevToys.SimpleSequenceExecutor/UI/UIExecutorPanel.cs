using Adens.DevToys.SimpleSequenceExecutor.Entities;
using CommunityToolkit.Diagnostics;
using DevToys.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;
using static Adens.DevToys.SimpleSequenceExecutor.UI.GUI;
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
    internal UIExecutorPanel(string? id):base(id)
    {
        _button.OnClick(OnAddStepClickAsync);
    }
    private IUIButton _button = Button().Text("add");
    internal void Render()
    {
        if (_bundle == null)
        {
            return;
        }
        List<IUIElement> list = new List<IUIElement>();
        foreach (var step in _bundle.Steps)
        {
            list.Add(Label().Text(step.Type));
        }
        list.Add(UIExecutorWrapper(ChoseStepExecutor()));
        UIElement = Stack().Vertical().WithChildren(
            Label().Text(_bundle.Name),
            Stack().Vertical().WithChildren(
            list.ToArray()
                ),
            _button
            );
    }

    private async ValueTask OnAddStepClickAsync()
    {
        _bundle.Steps.Add(new ExecutorStep() { Type= "1"});
        Render();
        Bundle = _bundle;
    }

    public ExecutorBundle? Bundle { 
        get=> _bundle;
        internal set => SetPropertyValue(ref _bundle, value, BundleChanged);
        }
   
    public event EventHandler? BundleChanged;
    public event EventHandler? OrientationChanged;
    public event EventHandler? SpacingChanged;
    public event EventHandler? ChildrenChanged;

   
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