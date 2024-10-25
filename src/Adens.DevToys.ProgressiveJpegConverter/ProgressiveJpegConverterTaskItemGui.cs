using DevToys.Api;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;
using System.Threading;
using Adens.DevToys.ProgressiveJpegConverter.Helpers;
using DevToys.Tools.Strings.GlobalStrings;
using CommunityToolkit.Diagnostics;
namespace Adens.DevToys.ProgressiveJpegConverter;
internal sealed class ProgressiveJpegConverterTaskItemGui : IUIListItem, IDisposable
{
    public static readonly string[] SizesStrings
      = {
            GlobalStrings.Bytes,
            GlobalStrings.Kilobytes,
            GlobalStrings.Megabytes,
            GlobalStrings.Gigabytes,
            GlobalStrings.Terabytes
        };

    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly IFileStorage _fileStorage;
    private readonly Lazy<IUIElement> _ui;
    private readonly IUISetting _setting = Setting();
    private readonly ProgressiveJpegConverterGui _imageConverterGuiTool;

    internal ProgressiveJpegConverterTaskItemGui(ProgressiveJpegConverterGui imageConverterGuiTool, SandboxedFileReader inputFile, IFileStorage fileStorage)
    {
        Guard.IsNotNull(imageConverterGuiTool);
        Guard.IsNotNull(inputFile);
        Guard.IsNotNull(fileStorage);

        _imageConverterGuiTool = imageConverterGuiTool;
        _fileStorage = fileStorage;
        Value = inputFile;
        InputFile = inputFile;

        ComputePropertiesAsync().ForgetSafely();

        _ui
            = new Lazy<IUIElement>(
                _setting
                    .Icon("FluentSystemIcons", '\uF488')
                    .Title(inputFile.FileName)

                    .InteractiveElement(
                        Stack()
                            .Horizontal()
                            .MediumSpacing()

                            .WithChildren(
                                Stack()
                                    .Horizontal()
                                    .SmallSpacing()

                                    .WithChildren(
                                        Button()
                                            .Icon("FluentSystemIcons", '\uF67F')
                                            .OnClick(OnSaveAsAsync),
                                        Button()
                                            .Icon("FluentSystemIcons", '\uF34C')
                                            .OnClick(OnDeleteAsync)))));
    }

    public IUIElement UIElement => _ui.Value;

    public object? Value { get; }

    internal SandboxedFileReader InputFile { get; }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        InputFile.Dispose();
    }

    internal async Task SaveAsync(string filePath)
    {
        using Stream inputFileStream = await InputFile.GetNewAccessToFileContentAsync(_cancellationTokenSource.Token);
        using FileStream outputFileStream = File.Create(filePath);
        await ProgressiveJpegConverterHelper.ConvertByMagickAsync(outputFileStream, inputFileStream, _cancellationTokenSource.Token);
    }

    private async ValueTask OnSaveAsAsync()
    {
        // Load the progressive-jpeg first, in case if the user will overwrite it (so we can read it before PickSaveFileAsync opens an access to the file).
        using Stream inputFileStream = await InputFile.GetNewAccessToFileContentAsync(_cancellationTokenSource.Token);

        // Ask the user to pick up a file.
        using Stream? outputFileStream = await _fileStorage.PickSaveFileAsync("jpeg");
        if (outputFileStream is not null)
        {
            await ProgressiveJpegConverterHelper.ConvertByMagickAsync(outputFileStream, inputFileStream, _cancellationTokenSource.Token);
        }
    }

    private ValueTask OnDeleteAsync()
    {
        _imageConverterGuiTool.ItemList.Items.Remove(this);
        return ValueTask.CompletedTask;
    }

    private async Task ComputePropertiesAsync()
    {
        await TaskSchedulerAwaiter.SwitchOffMainThreadAsync(_cancellationTokenSource.Token);

        using Stream fileStream = await InputFile.GetNewAccessToFileContentAsync(_cancellationTokenSource.Token);

        long storageFileSize = fileStream.Length;
        string? fileSize = HumanizeFileSize(storageFileSize, ProgressiveJpegConverter.FileSizeDisplay);

        _setting.Description(fileSize);
    }

    private static string HumanizeFileSize(double fileSize, string fileSizeDisplay)
    {
        int order = 0;
        while (fileSize >= 1024 && order < SizesStrings.Length - 1)
        {
            order++;
            fileSize /= 1024;
        }

        string fileSizeString = string.Format(fileSizeDisplay, fileSize, SizesStrings[order]);
        return fileSizeString;
    }
}
