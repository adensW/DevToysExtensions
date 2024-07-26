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
    public string Text { get;  set; } = string.Empty;
    public UITextDisplayExecutor(string? id) : base(id)
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

    public static IUIExecutor TextDisplayExecutor()
    {
        return TextDisplayExecutor(null);
    }
    public static IUIExecutor TextDisplayExecutor(string? id)
    {
        return new UITextDisplayExecutor(id);
    }

}