using DevToys.Api;
using System.ComponentModel.Composition;
using System.Text;
using static DevToys.Api.GUI;

namespace Adens.DevToys.FileSplitter;

[Export(typeof(IGuiTool))]
[Name("FileSplitter")]                                                         // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                       // This font is available by default in DevToys
    IconGlyph = '\uF33A',                                                     // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Converters,                    // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(FileSplitterResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "Adens.DevToys.FileSplitter.FileSplitter",                      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(FileSplitter.ShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(FileSplitter.LongDisplayTitle),
    DescriptionResourceName = nameof(FileSplitter.Description),
    AccessibleNameResourceName = nameof(FileSplitter.AccessibleName))]
internal sealed partial class FileSplitterGui : IGuiTool
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
        defaultValue: "");
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
    }

    
    private SandboxedFileReader inputFileReader=null;
    private readonly IUISingleLineTextInput _outputFilePathIinput = SingleLineTextInput();
    private readonly IUIProgressBar _progressBar = ProgressBar(nameof(_progressBar));
    private readonly IUIInfoBar _infoBar = InfoBar(nameof(_infoBar)).Informational()
                        .Title(FileSplitter.SplittedTitle)
                        .Description(FileSplitter.SplittedDescrtion)
                       ;
    private readonly IUIInfoBar _errorBar = InfoBar(nameof(_errorBar))
                      .Title(FileSplitter.ErrorInfoTitle)
                      .Description(FileSplitter.ErrorInfoDescription)
                     ;
    private readonly IUIFileSelector _fileSelector = FileSelector()
                        .CanSelectOneFile();
    private readonly IUINumberInput _sizeInpt = NumberInput(nameof(_sizeInpt))
                        .HideCommandBar()
                        .CannotCopyWhenEditable();
    private readonly IUINumberInput _lineInpt = NumberInput(nameof(_lineInpt))
                        .HideCommandBar()
                        .CannotCopyWhenEditable();
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
                            Item(FileSplitter.SplitType_Size, SplitType.Size),
                            Item(FileSplitter.SplitType_Line, SplitType.Line)
                            )
                        .OnItemSelected(OnSplitTypeSelected)
                         .Select((int)_settingsProvider.GetSetting(splitType)),
                  _sizeInpt
                  .Hide()
                        .Text(_settingsProvider.GetSetting(size).ToString())
                        .OnValueChanged(onSizeChanged),
                   _lineInpt
                      .Hide()
                        .Text(_settingsProvider.GetSetting(line).ToString())
                        .OnValueChanged(onLineChanged)
                        ),
                          _outputFilePathIinput
                        .Title(FileSplitter.OutputFilePath)
                        .OnTextChanged(onOutputFilepathChanged)
                         .CommandBarExtraContent(
                            
                            Button()
                            .Text(FileSplitter.PickAFolder)
                            .OnClick(OnChooseOutputPathButtonClickAsync)),
                        
                   
                    Button()
                        .Text(FileSplitter.Split)
                        .OnClick(OnButtonClick),
                    _progressBar.StartIndeterminateProgress().Hide(),
                    _infoBar.Informational()
                        .Open().Hide()

            ));
            SwitchSplitType();
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
    private void SwitchSplitType()
    {
        var st = _settingsProvider.GetSetting(splitType);
        if (st == SplitType.Size)
        {
            _sizeInpt.Show();
            _lineInpt.Hide();
        }
        else
        {
            _sizeInpt.Hide();
            _lineInpt.Show();
        }
    }
    private async ValueTask OnSplitTypeSelected(IUIDropDownListItem? item)
    {
        _settingsProvider.SetSetting(splitType, (SplitType)item.Value);
        SwitchSplitType();
    }

    private async ValueTask onSizeChanged(double arg)
    {
        _settingsProvider.SetSetting(size, (int)arg);

    }
    private async ValueTask onLineChanged(double arg)
    {
        _settingsProvider.SetSetting(line, (int)arg);

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
    private string GetFileName(string fileName)
    {
        return fileName.Substring(0, fileName.LastIndexOf('.'));
    }
    private string GetFileNameExtension(string fileName)
    {
        return fileName.Substring(fileName.LastIndexOf('.'));
    }
    private async ValueTask SplitBySize()
    {
        if (inputFileReader == null)
        {
            return;
        }
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
                var tureOutputPath = Path.Combine( _settingsProvider.GetSetting(outputFilePath), GetFileName(filename) + "_"+surfix.ToString()+ GetFileNameExtension(filename));
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
        var filename = inputFileReader.FileName;

        // split file by line
        using (var reader = new StreamReader(await inputFileReader.GetNewAccessToFileContentAsync(default)))
        {
            int cur = 0;
            int max = _settingsProvider.GetSetting(line);
            int surfix = 1;
            string curline;
            StringBuilder sb = new StringBuilder();
            while ((curline = reader.ReadLine()) != null)
            {
                sb.AppendLine(curline);
                cur++;
                if (cur >= max)
                {
                    // 保存到文件
                    await SaveLineFile(sb, filename, surfix);
                    cur = 0;
                    surfix+=1;
                    sb.Clear();
                }
            }
            if (sb.Length > 0)
            {
                await SaveLineFile(sb, filename, surfix);
                cur = 0;
                surfix += 1;
                sb.Clear();
            }
        }
    }
    private async Task SaveLineFile(StringBuilder sb,string filename,int surfix)
    {
        var tureOutputPath = Path.Combine(_settingsProvider.GetSetting(outputFilePath), GetFileName(filename) + "_" + surfix.ToString() + GetFileNameExtension(filename));
        // 保存到文件
        using (FileStream fs = new FileStream(tureOutputPath, FileMode.Create))
        {
            await fs.WriteAsync(Encoding.UTF8.GetBytes(sb.ToString()));
            await fs.FlushAsync();
        }
    }
    private async ValueTask OnButtonClick()
    {
        try { 
        _progressBar.Show();
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
            _progressBar.Hide();
        _infoBar.Show();
        }catch(IOException ex)
        {
            _errorBar.Description(ex.Message);
            _errorBar.Show();
        }
        finally
        {
            _progressBar.Hide();

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