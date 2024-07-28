using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.SimpleSequenceExecutor;
public static class Constants
{
    public const string EmptyExecutor = "Empty";
    public const string TextDisplayExecutor = "TextDisplay";
    public const string CliExecutor = "Cli";
    public static string[] Executors = [
       EmptyExecutor,
       TextDisplayExecutor,
        CliExecutor,

        ];
}
