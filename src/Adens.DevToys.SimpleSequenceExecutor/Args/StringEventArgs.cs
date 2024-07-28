using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.SimpleSequenceExecutor.Args;
public class StringEventArgs : EventArgs
{
    public string Value { get; set; } = string.Empty;
    public StringEventArgs(string value):base()
    {
        Value = value;
    }
}
