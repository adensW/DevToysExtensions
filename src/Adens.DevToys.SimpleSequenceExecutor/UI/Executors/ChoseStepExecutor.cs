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
    private string[] _executors= ["TextDisplay"];
    public ChoseStepExecutor(string? id
    ) : base(id)
    {
        List<IUIDropDownListItem> menus = new List<IUIDropDownListItem>();
        foreach (var item in _executors)
        {
            menus.Add(Item(text: item, value: item));
        }
        UIElement = SelectDropDownList()
                .AlignHorizontally(UIHorizontalAlignment.Left)
                .WithItems(
                menus.ToArray())
                .OnItemSelected(OnItemClickAsync);
    }

    private async ValueTask OnItemClickAsync(IUIDropDownListItem? item)
    {
        var executorName = item.Value;

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