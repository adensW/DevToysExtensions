using Adens.DevToys.SimpleSequenceExecutor.Entities;
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
internal sealed class SimpleSequenceExecutorGui : IGuiTool
{
  
    private UIToolView? _view;
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
    private readonly IUIList _bundleList = List(nameof(_bundleList));
    internal IUIList BundleList => _bundleList;
    private readonly IUIExecutorPanel _executorPanel = UIExecutorPanel(nameof(_executorPanel));
    [ImportingConstructor]
    public SimpleSequenceExecutorGui(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
        _settingsProvider.SettingChanged += OnSettingChanged;
        RefreshBundles();
    }

    private void OnSettingChanged(object? sender, SettingChangedEventArgs e)
    {
        RefreshBundles();
        RefreshCurrentBundles();
    }

    private void RefreshCurrentBundles()
    {
        var current = _settingsProvider.GetSetting(currentBundle);
        _executorPanel.Fill(current);
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
        var curbundles = _settingsProvider.GetSetting(bundles);
        curbundles.Add(new ExecutorBundle(_dialogNameInput.Text,_dialogDescriptionInput.Text));
        _settingsProvider.SetSetting(bundles, curbundles);
        RefreshBundles();
        ResetDialog();
        _view.CurrentOpenedDialog?.Close();


    }
    private void RefreshBundles()
    {
        _bundleList.Items.Clear();
        var curbundles = _settingsProvider.GetSetting(bundles);
        foreach (var bundle in curbundles)
        {
            _bundleList.Items.Add(new UIExecutorBundleItem(this,bundle,_settingsProvider));
        }
    }

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

}