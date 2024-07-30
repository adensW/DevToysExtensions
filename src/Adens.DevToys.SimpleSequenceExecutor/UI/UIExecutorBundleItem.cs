using Adens.DevToys.SimpleSequenceExecutor.Entities;
using CommunityToolkit.Diagnostics;
using DevToys.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;

namespace Adens.DevToys.SimpleSequenceExecutor.UI;
internal sealed class UIExecutorBundleItem :ViewModelBase,IUIListItem
{
    private readonly Lazy<IUIElement> _ui;
    public IUIElement UIElement => _ui.Value;
    public object? Value { get =>_current; }
    private readonly SimpleSequenceExecutorGui _simpleSequenceExecutorGui;
    private readonly ISettingsProvider _settingsProvider;
    private ExecutorBundle _current;
 
    internal UIExecutorBundleItem(SimpleSequenceExecutorGui simpleSequenceExecutorGui, ExecutorBundle bundle, ISettingsProvider settingsProvider)
    {
        Guard.IsNotNull(simpleSequenceExecutorGui);
        _simpleSequenceExecutorGui = simpleSequenceExecutorGui;
        _current = bundle;
        _ui = new(
              Stack().Horizontal()
              .WithChildren(
                  Label().Text(_current.Name),
                  Button().Icon("FluentSystemIcons", '\uF34C').OnClick(OnDeleteButtonClickAsync))

            );
        _settingsProvider = settingsProvider;
    }
    private async ValueTask OnDeleteButtonClickAsync()
    {
        var bundles = _settingsProvider.GetSetting(SimpleSequenceExecutorGui.bundles);
        var thisBundle = Value as ExecutorBundle;
        var curBundle = bundles.FirstOrDefault(z=>z.Name==thisBundle.Name);
        if (curBundle != null)
        {
            bundles.Remove(curBundle);
        }
        _settingsProvider.SetSetting(SimpleSequenceExecutorGui.bundles, bundles);
        //_simpleSequenceExecutorGui.BundleList.Items.Remove(this);

    }
    public event EventHandler? CurrentChanged;
}
