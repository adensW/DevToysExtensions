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
    private static readonly SettingDefinition<string?> filePath
     = new(
         name: $"{nameof(FileSplitterGui)}.{nameof(filePath)}",
         defaultValue: null);
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
      defaultValue: 1024);
    [ImportingConstructor]
    public FileSplitterGui(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
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

    private readonly IUISingleLineTextInput _outputFilePathIinput = SingleLineTextInput();
    private readonly IUIProgressRing _progressRing = ProgressRing();
    private readonly IUILabel _resultLable = Label();
    public UIToolView View
    {
        get
        {
            _view ??=
            new(Stack()
                .Vertical()
                .WithChildren(
                    SingleLineTextInput("filechangedinput").Title("File path")
                        .OnTextChanged(onFilepathChanged),
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
                        .Text("1024")
                        .OnValueChanged(onSizeChanged)
                        ),
                    _outputFilePathIinput
                        .Title("output file path")
                        .OnTextChanged(onOutputFilepathChanged),
                    Button()
                        .Text("Split")
                        .OnClick(OnButtonClick),
                    _progressRing.Hide(),
                    _resultLable.Hide()

            ));
            return _view;
        }
    }
    

   

    private async ValueTask OnSplitTypeSelected(IUIDropDownListItem? item)
    {
        _settingsProvider.SetSetting(splitType, (SplitType)item.Value);

    }

    private async ValueTask onSizeChanged(double arg)
    {
        _settingsProvider.SetSetting(size, (int)arg);

    }
    private async ValueTask SplitBySize()
    {
        var filepath = _settingsProvider.GetSetting(FileSplitterGui.filePath);
        // Split file by size
        using (var reader = new FileStream(filepath, FileMode.Open))
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
        }
    }
    private async ValueTask SplitByLine()
    {
        var filepath = _settingsProvider.GetSetting(FileSplitterGui.filePath);
        // split file by line
        using (var reader = new StreamReader(filepath))
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

    private async ValueTask onFilepathChanged(string arg)
    {
        _settingsProvider.SetSetting(FileSplitterGui.filePath, arg);
        string trueoutput = GetOutputFilePath(arg);
        _outputFilePathIinput.Text(trueoutput);
    }
    private string GetOutputFilePath(string originPath)
    {
        string path = originPath;
        if (path.Contains("."))
        {
            path = originPath.Substring(0, originPath.LastIndexOf('.'));
            path += "_{0}" + originPath.Substring(originPath.LastIndexOf('.'));
        }
        else
        {
            path += "_{0}";
        }
        return path;
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