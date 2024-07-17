using DevToys.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys;
[Export(typeof(GuiToolGroup))]
[Name(PredefinedAdensToolGroupNames.Executors)]
internal class ExecutorsGroup : GuiToolGroup
{
    [ImportingConstructor]
    internal ExecutorsGroup()
    {
        IconFontName = "FluentSystemIcons";
        IconGlyph = '\uE445';
        DisplayTitle = PredefinedAdensToolGroupNames.Executors;
        AccessibleName = "Adens.DevToys";
    }
}
