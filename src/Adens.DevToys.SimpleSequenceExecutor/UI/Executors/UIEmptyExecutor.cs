using Adens.DevToys.SimpleSequenceExecutor.Args;
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
internal class UIEmptyExecutor : UIElement, IUIExecutor
{
    public Dictionary<string, object> Parameters { get; set; }

    public event EventHandler? ParametersChanged;
    public string Text { get;  set; } = string.Empty;
    public UIEmptyExecutor(string? id) : base(id)
    {
        UIElement = Label().Text("Empty");
    }

    public bool CanExecute => false;

    public IUIElement UIElement { get; }
 

    public ValueTask<ExecutedResult> ExecuteAsync(Dictionary<string, object> runtimeVariables)
    {

        return ValueTask.FromResult(ExecutedResult.Create(runtimeVariables));
    }
}
public static partial class GUI
{

    public static IUIExecutor EmptyExecutor()
    {
        return EmptyExecutor(null);
    }


    public static IUIExecutor EmptyExecutor(string? id)
    {
        return new UIEmptyExecutor(id);
    }
    public static IUIExecutor Text(this IUIExecutor element, string text)
    {
        ((UITextDisplayExecutor)element).Text = text;
        return element;
    }

}