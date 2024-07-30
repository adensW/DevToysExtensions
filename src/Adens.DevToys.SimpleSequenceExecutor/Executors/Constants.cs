using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.SimpleSequenceExecutor;
public static class Constants
{
    public const string EmptyExecutor = "Empty";
#if DEBUG
    public const string TextDisplayExecutor = "TextDisplay";
#endif
    public const string CliExecutor = "Cli";
    public static string[] Executors = [
       EmptyExecutor,
#if DEBUG

       TextDisplayExecutor,
#endif
        CliExecutor,

        ];
}
