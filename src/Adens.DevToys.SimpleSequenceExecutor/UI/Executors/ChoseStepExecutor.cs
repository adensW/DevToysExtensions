using DevToys.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevToys.Api.GUI;

namespace Adens.DevToys.SimpleSequenceExecutor.UI;
internal class ChoseStepExecutor : UIElement, IUIExecutor
{
    private IEnumerable<Lazy<IUIExecutor>> _executors;
    public ChoseStepExecutor(string? id
    ) : base(id)
    {
        UIElement = DropDownButton()
                .AlignHorizontally(UIHorizontalAlignment.Left)
                .Icon("FluentSystemIcons", '\uE670')
                .Text("Click to open")
                .WithMenuItems(
                    DropDownMenuItem("Item 1"),
                    DropDownMenuItem()
                        .Text("Item 2")
                        .Icon("FluentSystemIcons", '\uE670'),
                    DropDownMenuItem()
                        .Text("Item 3")
                      ,
                    DropDownMenuItem("Item 4"));
    }

    public bool CanExecute => false;

    public IUIElement UIElement { get; }

}
public static partial class GUI
{

    public static IUIExecutor ChoseStepExecutor()
    {
        return ChoseStepExecutor(null);
    }


    public static IUIExecutor ChoseStepExecutor(string? id)
    {
        return new ChoseStepExecutor(id);
    }
    

}