using Adens.DevToys.ScribanRenderer.Entities;
using Adens.DevToys.ScribanRenderer.Helpers;
using DevToys.Api;
using Scriban;
using SQLite;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using static DevToys.Api.GUI;

namespace Adens.DevToys.ScribanRenderer;

[Export(typeof(IGuiTool))]
[Name("ScribanRenderer")]                                                         // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                       // This font is available by default in DevToys
    IconGlyph = '\uf53e',                                                     // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Generators,                    // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(ScribanRendererResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "Adens.DevToys.ScribanRenderer.ScribanRenderer",                      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(ScribanRenderer.ShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(ScribanRenderer.LongDisplayTitle),
    DescriptionResourceName = nameof(ScribanRenderer.Description),
    AccessibleNameResourceName = nameof(ScribanRenderer.AccessibleName))]
internal sealed partial class ScribanRendererGui : ViewModelBase, IGuiTool
{
    private  void CheckSqliteInit()
    {
        if (SqliteLoadHepler.HasDatabase())
        {
            IsFirst=false;
            return;
        }
        SqliteLoadHepler.EnsureSqliteLoaded();
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        db.CreateTable<TemplateItem>();
        db.CreateTable<CurrentTemplateItem>();
   
    }
    private ObservableCollection<TemplateItem> _bundles = new ObservableCollection<TemplateItem>();
    public ObservableCollection<TemplateItem> Bundles => _bundles;
    public void Dispose()
    {
        _bundles.CollectionChanged -= Bundles_CollectionChanged;
    }
    private void Bundles_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshBundles();
    }
    private TemplateItem? _current;
    public TemplateItem? Current
    {
        get => _current; internal set => SetPropertyValue(ref _current, value, (sender, args) =>
        {
            RefreshCurrentBundles();
        });
    }
    private UIToolView? _view;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IUIMultiLineTextInput _jsonEditor = MultiLineTextInput().Title("JSON Editor")
                          .Language("json")
                                .Extendable();
    private readonly IUIMultiLineTextInput _templateEditor = MultiLineTextInput().Title("Scriban Template")
                                .Extendable();
    private readonly IUIMultiLineTextInput _templateOutput = MultiLineTextInput().Title("Output")
                                .Extendable().ReadOnly();
    private readonly IUIStack BundleList = Stack().Vertical();
    private bool _isFirst=true;
    public bool IsFirst
    {
        get => _isFirst; internal set => SetPropertyValue(ref _isFirst, value, (sender, args) =>
        {
            if (value == false) { 
                RestoreBundles();
            }
        });
    }
    [ImportingConstructor]
    public ScribanRendererGui(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
        _bundles.CollectionChanged += Bundles_CollectionChanged;
        CheckSqliteInit();
       
    }
    private void RestoreBundles()
    {
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        _bundles.Clear();
        _bundles.AddRange(db.Table<TemplateItem>().ToList());
        var cur = db.Table<CurrentTemplateItem>().FirstOrDefault();
        if (cur != null)
        {
            Current = _bundles.FirstOrDefault(z => z.Id == cur.ItemId);
        }
    }
    private enum GridRows
    {
        Top,
        Bottom
    }

    private enum GridColumns
    {
        Full,
    }
    public UIToolView View
    {
        get
        {
            _view ??=
            new(
                SplitGrid()
                            .Vertical()
                            .LeftPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                            .RightPaneLength(new UIGridLength(3, UIGridUnitType.Fraction))
                            .WithLeftPaneChild(
                    Stack().Vertical().WithChildren(
                        Button().Text("new").OnClick(OpenAddBundleDialogClick),
                        BundleList)
                            )
                            .WithRightPaneChild(

                                SplitGrid()
                                .Horizontal()
                                .TopPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                                .BottomPaneLength(new UIGridLength(2, UIGridUnitType.Fraction))
                                .WithTopPaneChild(Grid()
                            .ColumnSmallSpacing()
                            .RowLargeSpacing()

                            .Rows(
                                (GridRows.Top, Auto),                                              // Height automatically fits the content.
                                (GridRows.Bottom, new UIGridLength(50, UIGridUnitType.Pixel)))    // Takes up 100 pixels.

                            .Columns(
                                (GridColumns.Full, new UIGridLength(1, UIGridUnitType.Fraction))  // Takes up 1/3 of the space.
                             )

                            .Cells(
                                Cell(GridRows.Top, GridColumns.Full, _jsonEditor)
                                  ,

                                Cell(GridRows.Bottom, GridColumns.Full,
                                    Button().AccentAppearance().Text("Generate").OnClick(OnGenerateClick))
                                )

                                )
                                .WithBottomPaneChild(
                                    SplitGrid()
                                        .Vertical()
                                        .LeftPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                                        .RightPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                                        .WithLeftPaneChild(
                                          _templateEditor
                                        )
                                        .WithRightPaneChild(
                                           _templateOutput
                                        )
                                    )
                    )
                );

            return _view;
        }
    }
    private readonly IUISingleLineTextInput _dialogNameInput = SingleLineTextInput().Title("add");
    private readonly IUISingleLineTextInput _dialogNameEditInput = SingleLineTextInput().Title("edit");

    private TemplateItem? _editTemplate;

    private async ValueTask OpenEditBundleDialogClick(TemplateItem item)
    {
        _editTemplate = item;
        _dialogNameEditInput.Text(item.Name);
        UIDialog dialog = await OpenEditDialogAsync(dismissible: true);

        // (optional) Wait for the dialog to close before continuing.
        await dialog.DialogCloseAwaiter;

        // Do something after the dialog is closed.
    }
    private async Task<UIDialog> OpenEditDialogAsync(bool dismissible)
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
                                .Text("Edit"),
                           _dialogNameEditInput
                            ),
                footerContent:
                    Button()
                        .AlignHorizontally(UIHorizontalAlignment.Right)
                        .Text("Save")
                        .OnClick(EditRendererBundleClick),
                isDismissible: dismissible);

        return dialog;
    }
    private async ValueTask EditRendererBundleClick()
    {
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        _editTemplate.Name= _dialogNameEditInput.Text;
        db.Update(_editTemplate);
        RestoreBundles();
        ResetDialog();
        _view.CurrentOpenedDialog?.Close();
    }
    private async ValueTask OpenAddBundleDialogClick()
    {
        UIDialog dialog = await OpenCustomDialogAsync(dismissible: true);

        // (optional) Wait for the dialog to close before continuing.
        await dialog.DialogCloseAwaiter;

        // Do something after the dialog is closed.
    }
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
                                .Text("Add"),
                           _dialogNameInput
                            ),
                footerContent:
                    Button()
                        .AlignHorizontally(UIHorizontalAlignment.Right)
                        .Text("Save")
                        .OnClick(AddRendererBundleClick),
                isDismissible: dismissible);

        return dialog;
    }
    private void ResetDialog()
    {
        _dialogNameEditInput.Text(string.Empty);
        _dialogNameInput.Text(string.Empty);
    }
    private async ValueTask AddRendererBundleClick()
    {
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        var bundle = new TemplateItem() { Id = Guid.NewGuid(), Name = _dialogNameInput.Text };
        db.Insert(bundle);
        Bundles.Add(bundle);
        ResetDialog();
        _view.CurrentOpenedDialog?.Close();
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
                        .RightPaneLength(new UIGridLength(100, UIGridUnitType.Pixel))
                        .WithLeftPaneChild(
                            Button().Text(bundle.Name).OnClick(() => { SelectBundle(bundle); })
                        )
                        .WithRightPaneChild(
                Stack().Horizontal().WithChildren(
                            Button().Icon("FluentSystemIcons", '\uf3de').OnClick(async () => { await OpenEditBundleDialogClick(bundle); }),

                            Button().Icon("FluentSystemIcons", '\uF34C').OnClick(() => { DeleteBundle(bundle); })
                        )
                )


            );
        }
        BundleList.WithChildren(bundleItems.ToArray());
        //_bundleList.Select(Bundles.IndexOf(CurrentBundle));
    }
    private void RefreshCurrentBundles()
    {
        _jsonEditor.Text(_current.JsonData);
        _templateEditor.Text(_current.Template);
        _templateOutput.Text("");
    }
    private void SelectBundle(TemplateItem bundle)
    {
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        var currentEntity = db.Table<CurrentTemplateItem>().FirstOrDefault();
        if (currentEntity == null)
        {
            currentEntity = new CurrentTemplateItem() { ItemId = bundle.Id };
        }
        else
        {
            currentEntity.ItemId = bundle.Id;
        }
        db.InsertOrReplace(currentEntity);
        Current = _bundles.FirstOrDefault(z => z.Id == currentEntity.ItemId);
    }
    private void DeleteBundle(TemplateItem bundle)
    {
        using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
        db.Delete(bundle);
        Bundles.Remove(bundle);
    }
    private async ValueTask ChooseItem()
    {
        _templateOutput.Text(Guid.NewGuid().ToString());
    }

    private async ValueTask OnGenerateClick()
    {
        try
        {
           var result = await ScribanTemplateGenerator.GenerateTemplate(_templateEditor.Text, _jsonEditor.Text);
            _templateOutput.Text(result);
            using var db = new SQLiteConnection(SqliteLoadHepler.GetDatabasePath());
            Current.JsonData = _jsonEditor.Text;
            Current.Template = _templateEditor.Text;
            db.Update(Current);

        }
        catch (Exception ex)
        {
            _templateOutput.Text(ex.Message);
        }

    }

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

}