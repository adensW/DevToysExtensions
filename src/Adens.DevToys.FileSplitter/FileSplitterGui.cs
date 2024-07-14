using DevToys.Api;
using System.ComponentModel.Composition;
using System.Text;
using static DevToys.Api.GUI;

namespace Adens.DevToys.FileSplitter;

[Export(typeof(IGuiTool))]
[Name("FileSplitter")]                                                         // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                       // This font is available by default in DevToys
    IconGlyph = '\uE511',                                                     // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Converters,                    // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(FileSplitterResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "Adens.DevToys.FileSplitter.FileSplitter",                      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(FileSplitter.ShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(FileSplitter.LongDisplayTitle),
    DescriptionResourceName = nameof(FileSplitter.Description),
    AccessibleNameResourceName = nameof(FileSplitter.AccessibleName))]
internal sealed class FileSplitterGui : IGuiTool
{
    private enum SplitType
    {
        Size,
        Line
    }
    private UIToolView? _view;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IFileStorage _fileStorage ;
 
    private static readonly SettingDefinition<string?> outputFilePath
    = new(
        name: $"{nameof(FileSplitterGui)}.{nameof(outputFilePath)}",
        defaultValue: null);
    private static readonly SettingDefinition<SplitType> splitType
    = new(
        name: $"{nameof(FileSplitterGui)}.{nameof(splitType)}",
        defaultValue: SplitType.Size);
    private static readonly SettingDefinition<int> size
  = new(
      name: $"{nameof(FileSplitterGui)}.{nameof(size)}",
      defaultValue: 1024*1024*2);
    private static readonly SettingDefinition<int> line
  = new(
      name: $"{nameof(FileSplitterGui)}.{nameof(line)}",
      defaultValue: 1024);
    [ImportingConstructor]
    public FileSplitterGui(ISettingsProvider settingsProvider, IFileStorage fileStorage)
    {
        _settingsProvider = settingsProvider;
        _fileStorage = fileStorage;
        //_settingsProvider.SettingChanged += OnSettingChanged;
    }

    //private void OnSettingChanged(object? sender, SettingChangedEventArgs e)
    //{
    //    if (e.SettingName == filePath.Name)
    //    {
    //        string trueoutput = GetOutputFilePath((string)e.NewValue);
    //        _outputFilePathIinput.Text(trueoutput);

    //        //_settingsProvider.SetSetting(FileSplitterGui.outputFilePath, trueoutput);
    //    }
    //}
    private SandboxedFileReader inputFileReader=null;
    private readonly IUISingleLineTextInput _outputFilePathIinput = SingleLineTextInput();
    private readonly IUIProgressRing _progressRing = ProgressRing();
    private readonly IUILabel _resultLable = Label();
    private readonly IUIFileSelector _fileSelector = FileSelector()
                        .CanSelectOneFile();
    public UIToolView View
    {
        get
        {
            _view ??=
            new(Stack()
                .Vertical()
                .WithChildren(
                     _fileSelector.OnFilesSelected(oninputFileSelected),
                    Stack()
                        .Horizontal()
                        .WithChildren(
                    SelectDropDownList()
                        .AlignHorizontally(UIHorizontalAlignment.Left)
                        .WithItems(
                            Item("Size", SplitType.Size),
                            Item("Line", SplitType.Line)
                            )
                        .OnItemSelected(OnSplitTypeSelected)
                         .Select(0),
                    NumberInput()
                        .HideCommandBar()
                        .CannotCopyWhenEditable()
                        .Text("2097152")
                        .OnValueChanged(onSizeChanged)
                        ),
                  
                          _outputFilePathIinput
                        .Title("output file path")
                        .OnTextChanged(onOutputFilepathChanged)
                         .CommandBarExtraContent(
                            
                            Button()
                            .Text("choose")
                            .OnClick(OnChooseOutputPathButtonClickAsync)),
                        
                   
                    Button()
                        .Text("Split")
                        .OnClick(OnButtonClick),
                    _progressRing.Hide(),
                    _resultLable.Hide()

            ));
            return _view;
        }
    }



    private async ValueTask OnChooseOutputPathButtonClickAsync()
    {
        // Ask the user to pick a TXT or JSON file.
        var folder = await _fileStorage.PickFolderAsync();
        if (folder == null)
        {
            return;
        }
        _outputFilePathIinput.Text(folder);
    }
    private async ValueTask OnSplitTypeSelected(IUIDropDownListItem? item)
    {
        _settingsProvider.SetSetting(splitType, (SplitType)item.Value);

    }

    private async ValueTask onSizeChanged(double arg)
    {
        _settingsProvider.SetSetting(size, (int)arg);

    }
    private string GetOutputFilePath(string originPath,int surfix)
    {
        string path = originPath;
        if (path.Contains("."))
        {
            path = originPath.Substring(0, originPath.LastIndexOf('.'));
            path += surfix.ToString() + originPath.Substring(originPath.LastIndexOf('.'));
        }
        else
        {
            path += surfix.ToString();
        }
        return path;
    }
    private async ValueTask SplitBySize()
    {
        var filename = inputFileReader.FileName;
        
        // Split file by size
        using (var reader = await inputFileReader.GetNewAccessToFileContentAsync(default))
        {
            int chunkSize = _settingsProvider.GetSetting(size);
            int surfix = 1;
            int lastIndex = 0;
            while (lastIndex<reader.Length)
            {
                if(chunkSize > reader.Length-lastIndex)
                {
                    chunkSize = (int)reader.Length - lastIndex;
                }
                byte[] buffer = new byte[chunkSize];
                reader.Read(buffer, 0, chunkSize);
                var tureOutputPath = _settingsProvider.GetSetting(FileSplitterGui.outputFilePath).Replace("{0}", surfix.ToString());
                // 保存到文件
                using (FileStream fs = new FileStream(tureOutputPath, FileMode.Create))
                {
                    await fs.WriteAsync(buffer);
                    await fs.FlushAsync();
                }
                surfix++;
                lastIndex+= chunkSize;
            }
            inputFileReader.Dispose();
        }
    }
    private async ValueTask SplitByLine()
    {
        if (inputFileReader == null)
        {
            return;
        }
        // split file by line
        using (var reader = new StreamReader(await inputFileReader.GetNewAccessToFileContentAsync(default)))
        {
            int cur = 0;
            int max = _settingsProvider.GetSetting(size);
            int surfix = 1;
            string line;
            StringBuilder sb = new StringBuilder();
            while ((line = reader.ReadLine()) != null)
            {
                sb.AppendLine(line);
                cur++;
                if (cur >= max)
                {
                    var tureOutputPath = _settingsProvider.GetSetting(FileSplitterGui.outputFilePath).Replace("{0}", surfix.ToString());
                    // 保存到文件
                    cur = 0;
                    using (FileStream fs = new FileStream(tureOutputPath, FileMode.Create))
                    {
                        await fs.WriteAsync(Encoding.UTF8.GetBytes(sb.ToString()));
                        await fs.FlushAsync();
                    }
                    surfix++;
                    sb.Clear();
                }

            }
        }
    }
    private async ValueTask OnButtonClick()
    {
        try { 
        _resultLable.Show();
        _resultLable.Text("Splitting...");
        _progressRing.Show();
        var st= _settingsProvider.GetSetting(splitType);
        switch (st)
        {
            case SplitType.Size:
                await SplitBySize();
                break;
            case SplitType.Line:
                await SplitByLine();
                break;
            default:
                break;
        }
        _progressRing.Hide();
        _resultLable.Text("Splited");
        }catch(IOException ex)
        {
            _resultLable.Text(ex.Message);

        }
    }

    private async ValueTask oninputFileSelected(SandboxedFileReader[] files)
    {
        if (files.Length > 1||files.Length<1)
        {
            return;
        }
        inputFileReader = files[0];
    }
   
    private async ValueTask onOutputFilepathChanged(string arg)
    {
        _settingsProvider.SetSetting(FileSplitterGui.outputFilePath, arg);
    }
    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }
 
}