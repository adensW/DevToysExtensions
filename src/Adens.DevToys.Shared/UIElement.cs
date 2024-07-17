using DevToys.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys;
[DebuggerDisplay($"Id = {{{nameof(Id)}}}, IsVisible = {{{nameof(IsVisible)}}}, IsEnabled = {{{nameof(IsEnabled)}}}")]
public abstract class UIElement : ViewModelBase, IUIElement
{
    private bool _isVisible = true;
    private bool _isEnabled = true;
    private UIHorizontalAlignment _horizontalAlignment = UIHorizontalAlignment.Stretch;
    private UIVerticalAlignment _verticalAlignment = UIVerticalAlignment.Stretch;

    protected UIElement(string? id)
    {
        Id = id;
    }

    public string? Id { get; }

    public bool IsVisible
    {
        get => _isVisible;
        internal set => SetPropertyValue(ref _isVisible, value, IsVisibleChanged);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        internal set => SetPropertyValue(ref _isEnabled, value, IsEnabledChanged);
    }

    public UIHorizontalAlignment HorizontalAlignment
    {
        get => _horizontalAlignment;
        internal set => SetPropertyValue(ref _horizontalAlignment, value, HorizontalAlignmentChanged);
    }

    public UIVerticalAlignment VerticalAlignment
    {
        get => _verticalAlignment;
        internal set => SetPropertyValue(ref _verticalAlignment, value, VerticalAlignmentChanged);
    }

    public event EventHandler? IsVisibleChanged;

    public event EventHandler? IsEnabledChanged;

    public event EventHandler? HorizontalAlignmentChanged;

    public event EventHandler? VerticalAlignmentChanged;
}
