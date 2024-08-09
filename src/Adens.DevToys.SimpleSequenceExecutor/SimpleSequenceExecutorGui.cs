using Adens.DevToys.SimpleSequenceExecutor.Entities;
using Adens.DevToys.SimpleSequenceExecutor.Helpers;
using Adens.DevToys.SimpleSequenceExecutor.UI;
using DevToys.Api;
using SQLite;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Text.Json;
using static Adens.DevToys.SimpleSequenceExecutor.UI.GUI;
using static DevToys.Api.GUI;

namespace Adens.DevToys.SimpleSequenceExecutor;

[Export(typeof(IGuiTool))]
[Name("SimpleSequenceExecutor")]                                                         // A unique, internal name of the tool.
#if DEBUG
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                       // This font is available by default in DevToys
    IconGlyph = '\uE607',                                                     // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Generators,                    // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(SimpleSequenceExecutorResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "Adens.DevToys.SimpleSequenceExecutor.SimpleSequenceExecutor",                      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(SimpleSequenceExecutor.WIPShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(SimpleSequenceExecutor.WIPLongDisplayTitle),
    DescriptionResourceName = nameof(SimpleSequenceExecutor.Description),
    AccessibleNameResourceName = nameof(SimpleSequenceExecutor.AccessibleName))]
#else
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
#endif
internal sealed class SimpleSequenceExecutorGui :ViewModelBase,IGuiTool
{
  
    private UIToolView? _view;
    #region save logic
    private ObservableCollection<Bundle> _bundles = new ObservableCollection<Bundle>();
    public ObservableCollection<Bundle> Bundles => _bundles;
    public void Dispose()
    {
        _bundles.CollectionChanged -= Bundles_CollectionChanged;
    }
    private void Bundles_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshBundles();
    }

    //private CurrentBundle _currentBundle;
    //public CurrentBundle CurrentBundle { get=>_currentBundle; internal set =>SetPropertyValue(ref _currentBundle, value, (sender,args) => {
    //}); }
  
    #endregion
    #region sqlite
    private void CheckSqliteInit()
    {
        SqliteLoadHepler.EnsureSqliteLoaded();
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        db.CreateTable<Bundle>();
        db.CreateTable<CurrentBundle>();
        db.CreateTable<BundleStep>();
        db.CreateTable<BundleStepParameter>();
    }
    #endregion
 
    private readonly IUIStack BundleList = Stack().Vertical();
    private readonly IUIExecutorPanel _executorPanel;
    [ImportingConstructor]
    public SimpleSequenceExecutorGui(ISettingsProvider settingsProvider)
    {
        _bundles.CollectionChanged += Bundles_CollectionChanged;
        CheckSqliteInit();
        RestoreBundles();
         _executorPanel = UIExecutorBundleExecutorsPanel(nameof(_executorPanel));
        RefreshBundles(); 
        RefreshCurrentBundles();
    }
    private void RestoreBundles()
    {
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        _bundles.Clear();
        _bundles.AddRange( db.Table<Bundle>().ToList());
    }

    private void RefreshCurrentBundles()
    {
        //_executorPanel.Fill(CurrentBundle);
    }
    private void RefreshBundles()
    {
        //BundleList.Rows.Clear();
        ////_bundleList.Items.Clear();
        List<IUIElement> bundleItems = new List<IUIElement>();
        foreach (var bundle in Bundles)
        {
            bundleItems.Add(SplitGrid()
                        .Vertical()
                        .LeftPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .RightPaneLength(new UIGridLength(50, UIGridUnitType.Pixel))
                        .WithLeftPaneChild(
                            Button().Text(bundle.Name).OnClick(() => { SelectBundle(bundle); })
                        )
                        .WithRightPaneChild(
                            Button().Icon("FluentSystemIcons", '\uF34C').OnClick(() => { DeleteBundle(bundle); })
                        )
                

            );
        }
        BundleList.WithChildren(bundleItems.ToArray());
        //_bundleList.Select(Bundles.IndexOf(CurrentBundle));
    }
    private void SelectBundle(Bundle bundle)
    {
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        var currentEntity= db.Table<CurrentBundle>().FirstOrDefault();
        if (currentEntity == null)
        {
            currentEntity = new CurrentBundle() { BundleId = bundle.Id };
        }
        else
        {
            currentEntity.BundleId = bundle.Id;
        }
        db.InsertOrReplace(currentEntity);
        
    }
    private void DeleteBundle(Bundle bundle)
    {
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        db.Delete(bundle);
        Bundles.Remove(bundle);
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
                     Stack().Horizontal().WithChildren(
                        Button().Text("new").OnClick(OpenAddBundleDialogClick)
                        //Button().Text("export").OnClick(ExportButtonClick),
                        //Button().Text("import").OnClick(OpenImportDialogClick),
                        //Button().Icon("FluentSystemIcons", '\uF34C').OnClick(OpenSettingDialogClick)
                     )
                 )
                .WithBottomPaneChild(

                    SplitGrid()
                        .Vertical()
                        .LeftPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .RightPaneLength(new UIGridLength(2, UIGridUnitType.Fraction))
                        .WithLeftPaneChild(BundleList)
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
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        var bundle = new Bundle() { Id = Guid.NewGuid(), Name = _dialogNameInput.Text, Description = _dialogDescriptionInput.Text };
        db.Insert(bundle);
        Bundles.Add(bundle);
        ResetDialog();
        _view.CurrentOpenedDialog?.Close();
    }
#endregion
   

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

}