using Adens.DevToys.SimpleSequenceExecutor.Entities;
using DevToys.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;

namespace Adens.DevToys.SimpleSequenceExecutor.UI;
[Export(typeof(IUIExecutor))]
internal class UITextDisplayExecutor : UIElement, IUIExecutor
{
    public Dictionary<string, object> Parameters { get; set; }

    public event EventHandler? ParametersChanged;
    public string Text { get;  set; } = string.Empty;
    public UITextDisplayExecutor(string? id, Dictionary<string, object> parameters) : base(id)
    {
        UIElement = Label().Text("111");
    }
    public bool CanExecute => false;

    public IUIElement UIElement { get; }

    public ValueTask ExecuteAsync()
    {
        (UIElement as IUILabel).Text("222");
        return ValueTask.CompletedTask;
    }
}
public static partial class GUI
{

    public static IUIExecutor TextDisplayExecutor(Dictionary<string, object> parameters)
    {
        return TextDisplayExecutor(null, parameters);
    }
    public static IUIExecutor TextDisplayExecutor(string? id, Dictionary<string, object> parameters)
    {
        return new UITextDisplayExecutor(id,parameters);
    }

}