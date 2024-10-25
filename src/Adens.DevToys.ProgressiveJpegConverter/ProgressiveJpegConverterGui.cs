using DevToys.Api;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Text;
using static DevToys.Api.GUI;

namespace Adens.DevToys.ProgressiveJpegConverter;

[Export(typeof(IGuiTool))]
[Name("ProgressiveJpegConverter")]                                                         // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                       // This font is available by default in DevToys
    IconGlyph = '\uf492',                                                     // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Graphic,                    // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(ProgressiveJpegConverterResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "Adens.DevToys.ProgressiveJpegConverter.ProgressiveJpegConverter",                      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(ProgressiveJpegConverter.ShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(ProgressiveJpegConverter.LongDisplayTitle),
    DescriptionResourceName = nameof(ProgressiveJpegConverter.Description),
    AccessibleNameResourceName = nameof(ProgressiveJpegConverter.AccessibleName))]
internal sealed partial class ProgressiveJpegConverterGui : IGuiTool
{
    private readonly ILogger _logger;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IFileStorage _fileStorage;
    private readonly IUIList _itemList = List("progressive-jpeg-converter-task-list");
    private readonly IUIButton _saveAllButton = Button("progressive-jpeg-converter-save-all-button");
    private readonly IUIButton _deleteAllButton = Button("progressive-jpeg-converter-delete-all-button");
    [ImportingConstructor]
    public ProgressiveJpegConverterGui(ISettingsProvider settingsProvider, IFileStorage fileStorage)
    {
        _logger = this.Log();
        _settingsProvider = settingsProvider;
        _fileStorage = fileStorage;

        _itemList.Items.CollectionChanged += OnItemListItemsChanged;
    }

   
    internal IUIList ItemList => _itemList;

    public UIToolView View
        => new(
            isScrollable: true,
            Stack()
                .Vertical()
                .LargeSpacing()

                .WithChildren(

                    FileSelector()
                        .CanSelectManyFiles()
                        .LimitFileTypesTo(new[]
    {
        ".bmp",
        ".jpeg",
        ".jpg",
        ".pbm",
        ".png",
        ".tiff",
        ".tga",
        ".webp"
    })
                        .OnFilesSelected(OnFilesSelected),

                    Stack()
                        .Horizontal()
                        .AlignHorizontally(UIHorizontalAlignment.Right)
                        .MediumSpacing()

                        .WithChildren(
                            _saveAllButton
                                .Icon("FluentSystemIcons", '\uF67F')
                                .Text(ProgressiveJpegConverter.SaveAll)
                                .AccentAppearance()
                                .Disable()
                                .OnClick(OnSaveAllAsync),

                            _deleteAllButton
                                .Icon("FluentSystemIcons", '\uF34C')
                                .Text(ProgressiveJpegConverter.DeleteAll)
                                .Disable()
                                .OnClick(OnDeleteAllAsync)),

                    _itemList
                        .ForbidSelectItem()));

    public async void OnDataReceived(string dataTypeName, object? parsedData)
    {
        if (dataTypeName == PredefinedCommonDataTypeNames.Image && parsedData is Image<Rgba32> image)
        {
            FileInfo temporaryFile = _fileStorage.CreateSelfDestroyingTempFile("png");

            using (image)
            {
                using Stream fileStream = _fileStorage.OpenWriteFile(temporaryFile.FullName, replaceIfExist: true);
                await image.SaveAsPngAsync(fileStream);
            }

            _itemList.Items.Insert(0, new ProgressiveJpegConverterTaskItemGui(this, SandboxedFileReader.FromFileInfo(temporaryFile), _fileStorage));
        }
        else if (dataTypeName == "StaticImageFile" && parsedData is FileInfo file)
        {
            _itemList.Items.Insert(0, new ProgressiveJpegConverterTaskItemGui(this, SandboxedFileReader.FromFileInfo(file), _fileStorage));
        }
        else if (dataTypeName == "StaticImageFiles" && parsedData is FileInfo[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                _itemList.Items.Insert(0, new ProgressiveJpegConverterTaskItemGui(this, SandboxedFileReader.FromFileInfo(files[i]), _fileStorage));
            }
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < _itemList.Items.Count; i++)
        {
            if (_itemList.Items[i] is IDisposable disposableItem)
            {
                disposableItem.Dispose();
            }
        }

        _itemList.Items.CollectionChanged -= OnItemListItemsChanged;
    }

    private void OnFilesSelected(SandboxedFileReader[] files)
    {
        for (int i = 0; i < files.Length; i++)
        {
            _itemList.Items.Insert(
                0,
                new ProgressiveJpegConverterTaskItemGui(this, files[i], _fileStorage));
        }
    }

    private async ValueTask OnSaveAllAsync()
    {
        string? folderPath = await _fileStorage.PickFolderAsync();

        if (!string.IsNullOrEmpty(folderPath))
        {
            for (int i = 0; i < _itemList.Items.Count; i++)
            {
                if (_itemList.Items[i] is ProgressiveJpegConverterTaskItemGui item)
                {
                    string newFileName = Path.GetFileNameWithoutExtension(item.InputFile.FileName) + ".jpeg" ;
                    string newFilePath = Path.Combine(folderPath, newFileName);
                    await item.SaveAsync(newFilePath);
                }
            }
        }
    }

    private ValueTask OnDeleteAllAsync()
    {
        for (int i = 0; i < _itemList.Items.Count; i++)
        {
            if (_itemList.Items[i] is IDisposable disposableItem)
            {
                disposableItem.Dispose();
            }
        }

        _itemList.Items.Clear();

        return ValueTask.CompletedTask;
    }

    private void OnItemListItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateButtonsState();
    }

    private void UpdateButtonsState()
    {
        bool hasItems = _itemList.Items.Count > 0;
        if (hasItems)
        {
            _saveAllButton.Enable();
            _deleteAllButton.Enable();
        }
        else
        {
            _saveAllButton.Disable();
            _deleteAllButton.Disable();
        }
    }
}