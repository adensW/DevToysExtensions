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
internal class TextDisplayExecutor : UIElement, IUIExecutor
{
    public string Text { get;  set; } = string.Empty;
    public TextDisplayExecutor(string? id) : base(id)
    {
        UIElement = Label();
    }

    public bool CanExecute => false;

    public IUIElement UIElement { get; }

}
public static partial class GUI
{

    public static IUIExecutor TextDisplayExecutor()
    {
        return TextDisplayExecutor(null);
    }


    public static IUIExecutor TextDisplayExecutor(string? id)
    {
        return new TextDisplayExecutor(id);
    }
    public static IUIExecutor Text(this IUIExecutor element, string text)
    {
        ((TextDisplayExecutor)element).Text = text;
        return element;
    }

}