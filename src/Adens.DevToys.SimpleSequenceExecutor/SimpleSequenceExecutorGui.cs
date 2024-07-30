﻿using Adens.DevToys.SimpleSequenceExecutor.Entities;
using Adens.DevToys.SimpleSequenceExecutor.UI;
using DevToys.Api;
using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static DevToys.Api.GUI;
using static Adens.DevToys.SimpleSequenceExecutor.UI.GUI;

namespace Adens.DevToys.SimpleSequenceExecutor;

[Export(typeof(IGuiTool))]
[Name("SimpleSequenceExecutor")]                                                         // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                       // This font is available by default in DevToys
    IconGlyph = '\uE607',                                                     // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Generators,                    // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(SimpleSequenceExecutorResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "Adens.DevToys.SimpleSequenceExecutor.SimpleSequenceExecutor",                      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(SimpleSequenceExecutor.ShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(SimpleSequenceExecutor.LongDisplayTitle),
    DescriptionResourceName = nameof(SimpleSequenceExecutor.Description),
    AccessibleNameResourceName = nameof(SimpleSequenceExecutor.AccessibleName))]
internal sealed class SimpleSequenceExecutorGui :ViewModelBase,IGuiTool
{
  
    private UIToolView? _view;
    #region stored settings
    private readonly ISettingsProvider _settingsProvider;
    public static readonly SettingDefinition<List<ExecutorBundle>> bundles // Define a setting.
    = new(
        name: $"{nameof(SimpleSequenceExecutorGui)}.{nameof(bundles)}", // Unique name for the setting. Use the tool name to avoid conflicts.
        defaultValue: new List<ExecutorBundle>());                              // Default value for the setting.
    public static readonly SettingDefinition<ExecutorBundle?> currentBundle // Define a setting.
  = new(
      name: $"{nameof(SimpleSequenceExecutorGui)}.{nameof(currentBundle)}", // Unique name for the setting. Use the tool name to avoid conflicts.
      defaultValue: null,
      serialize: (obj) => { 
          return JsonSerializer.Serialize(obj);
      },
      deserialize: (str) => {
          return JsonSerializer.Deserialize<ExecutorBundle>(str);
      });                              // Default value for the setting.
    #endregion
    #region save logic
    public List<ExecutorBundle> _bundles;
    public List<ExecutorBundle> Bundles { 
        get=>_bundles;
        internal set => SetPropertyValue(ref _bundles, value, (sender,args) => { 
            _settingsProvider.SetSetting(bundles, value);
            RefreshBundles();

        });
    }
    private ExecutorBundle _currentBundle;
    public ExecutorBundle CurrentBundle { get=>_currentBundle; internal set =>SetPropertyValue(ref _currentBundle, value, (sender,args) => {
        _settingsProvider.SetSetting(currentBundle, value);
    }); }
    private void ExecutorPanel_BundleChanged(object? sender, EventArgs e)
    {
        var current = _executorPanel.Bundle;
        var c = JsonSerializer.Deserialize<ExecutorBundle>(JsonSerializer.Serialize(current));
        CurrentBundle = c;
        foreach (var item in Bundles)
        {
            if (item.Name == current.Name)
            {
                item.Steps = current.Steps;
            }
        }
        var bs = JsonSerializer.Deserialize<List<ExecutorBundle>>(JsonSerializer.Serialize(Bundles));
        Bundles = bs;
    }
    #endregion
    private readonly IUIList _bundleList = List(nameof(_bundleList));
    internal IUIList BundleList => _bundleList;
    private readonly IUIExecutorPanel _executorPanel;
    [ImportingConstructor]
    public SimpleSequenceExecutorGui(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;

        _currentBundle = settingsProvider.GetSetting(currentBundle);
        _bundles = settingsProvider.GetSetting(bundles);

        //_settingsProvider.SettingChanged += OnSettingChanged;
        _executorPanel = UIExecutorPanel(nameof(_executorPanel), _settingsProvider);
        _executorPanel.BundleChanged += ExecutorPanel_BundleChanged;
        BundleList.SelectedItemChanged+=OnBundleSelected;
        BundleList.Select(Bundles.IndexOf(CurrentBundle));
        RefreshBundles(); 
        RefreshCurrentBundles();
    }

    private void OnBundleSelected(object? sender, EventArgs e)
    {
        if (sender == null||((IUIList)sender).SelectedItem==null|| ((IUIList)sender).SelectedItem?.Value == null)
        {
            return;
        }
        CurrentBundle = ((IUIList)sender).SelectedItem.Value as ExecutorBundle;
    }

    //private async ValueTask OnBundleSelected(IUIListItem? item)
    //{
    //    if(item== null)
    //    {
    //        return;
    //    }
    //    var selecteditem = BundleList.SelectedItem;
    //    CurrentBundle = item.Value as ExecutorBundle;
    //}

    private void RefreshCurrentBundles()
    {
        _executorPanel.Fill(CurrentBundle);
    }
    private void RefreshBundles()
    {
        _bundleList.Items.Clear();
        foreach (var bundle in Bundles)
        {
            var bundleItem = new UIExecutorBundleItem(this, bundle, _settingsProvider);
            _bundleList.Items.Add(bundleItem);
        }
        _bundleList.Select(Bundles.IndexOf(CurrentBundle));
    }
    public UIToolView View
    {
        get
        {
            _view ??=
            new(
                 SplitGrid()
                .Horizontal()
                .TopPaneLength(new UIGridLength(100, UIGridUnitType.Pixel))
                .BottomPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                .WithTopPaneChild(
                     Button().Text("new")
                     .OnClick(OpenAddBundleDialogClick)
                     )
                .WithBottomPaneChild(

                    SplitGrid()
                        .Vertical()
                        .LeftPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .RightPaneLength(new UIGridLength(2, UIGridUnitType.Fraction))
                        .WithLeftPaneChild(_bundleList)
                        .WithRightPaneChild(
                        _executorPanel
                        ))
            );
            return _view;
        }
    }
#region AddBundle
    private async ValueTask OpenAddBundleDialogClick()
    {
        UIDialog dialog = await OpenCustomDialogAsync(dismissible: true);

        // (optional) Wait for the dialog to close before continuing.
        await dialog.DialogCloseAwaiter;

        // Do something after the dialog is closed.
    }
    private readonly IUISingleLineTextInput _dialogNameInput = SingleLineTextInput().Title(SimpleSequenceExecutor.AddNewBundleDialog_Name);
    private readonly IUISingleLineTextInput _dialogDescriptionInput = SingleLineTextInput().Title(SimpleSequenceExecutor.AddNewBundleDialog_Description);
    private async Task<UIDialog> OpenCustomDialogAsync(bool dismissible)
    {
        // Open a dialog
        UIDialog dialog
            = await _view.OpenDialogAsync(
                dialogContent:
                    Stack()
                        .Vertical()
                        .WithChildren(
                            Label()
                                .Style(UILabelStyle.Subtitle)
                                .Text(SimpleSequenceExecutor.AddNewBundleDialog_Title),
                           _dialogNameInput,
                            _dialogDescriptionInput
                            ),
                footerContent:
                    Button()
                        .AlignHorizontally(UIHorizontalAlignment.Right)
                        .Text(SimpleSequenceExecutor.AddNewBundleDialog_Button_Save)
                        .OnClick(AddExecutorBundleClick),
                isDismissible: dismissible);

        return dialog;
    }
    private void ResetDialog()
    {
        _dialogNameInput.Text(string.Empty);
        _dialogDescriptionInput.Text(string.Empty);
    }
    private async ValueTask AddExecutorBundleClick()
    {
        Bundles.Add(new ExecutorBundle(_dialogNameInput.Text, _dialogDescriptionInput.Text));
        Bundles = JsonSerializer.Deserialize<List<ExecutorBundle>>(JsonSerializer.Serialize(Bundles));
        ResetDialog();
        _view.CurrentOpenedDialog?.Close();
    }
#endregion
   

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

}