using Adens.DevToys.SimpleSequenceExecutor.Entities;
using CommunityToolkit.Diagnostics;
using DevToys.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;
namespace Adens.DevToys.SimpleSequenceExecutor.UI;
public interface IUIExecutorPanelHeader : IUICard
{
    
}
internal class UIExecutorPanelHeader : UIElement, IUIExecutorPanelHeader
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

    public IUIElement UIElement { 
        get;
    }
    internal UIExecutorPanelHeader(string? id, IUIElement uiElement) :base(id)
    {
        Guard.IsNotNull(uiElement);
        UIElement = uiElement;
    }
   
    public event EventHandler? OrientationChanged;
    public event EventHandler? SpacingChanged;

   
}
public static partial class GUI
{
   
    public static IUIExecutorPanelHeader UIExecutorPanelHeader(IUIElement uiElement)
    {
        return UIExecutorPanelHeader(null, uiElement);
    }

    
    public static IUIExecutorPanelHeader UIExecutorPanelHeader(string? id, IUIElement uiElement)
    {
        return new UIExecutorPanelHeader(id, uiElement);
    }
    
   
}