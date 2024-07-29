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
    public object? Value { get; }
    private readonly SimpleSequenceExecutorGui _simpleSequenceExecutorGui;
    private readonly ISettingsProvider _settingsProvider;
    private ExecutorBundle _current;
    public ExecutorBundle Current
    {
        get => _current;
        internal set => SetPropertyValue(ref _current, value, CurrentChanged);
    }
    internal UIExecutorBundleItem(SimpleSequenceExecutorGui simpleSequenceExecutorGui, ExecutorBundle bundle, ISettingsProvider settingsProvider)
    {
        Guard.IsNotNull(simpleSequenceExecutorGui);
        _simpleSequenceExecutorGui = simpleSequenceExecutorGui;
        Value = bundle;

        _ui = new(
              Stack().Horizontal()
              .WithChildren(
                  Button().Text(bundle.Name).OnClick(onCurrentBundleClickAsync),
                  Button().Icon("FluentSystemIcons", '\uF34C').OnClick(OnDeleteButtonClickAsync))

            );
        _settingsProvider = settingsProvider;
    }

    private  void onCurrentBundleClickAsync()
    {
        _settingsProvider.SetSetting(SimpleSequenceExecutorGui.currentBundle, Current);
        Current = (Value as ExecutorBundle);
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
