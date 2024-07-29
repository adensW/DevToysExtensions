using Adens.DevToys.SimpleSequenceExecutor.Entities;
using Adens.DevToys.SimpleSequenceExecutor.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adens.DevToys.SimpleSequenceExecutor.UI.GUI;

namespace Adens.DevToys.SimpleSequenceExecutor;
public static class ExecutorGenerator
{
    public static IUIExecutorWrapper? Generate(ExecutorStep step)
    {
        var executor = UIExecutorWrapper( step);
      
        return executor;
    }
    public static IUIExecutor? Generate(string type)
    {
        IUIExecutor executor = null;
        switch (type)
        {
            case Constants.TextDisplayExecutor:
                executor = TextDisplayExecutor(Guid.NewGuid().ToString());
                break;
            case Constants.CliExecutor:
                executor =CliExecutor(Guid.NewGuid().ToString());
                break;
            case Constants.EmptyExecutor:
            default:
                executor = EmptyExecutor(Guid.NewGuid().ToString());
                break;
                ;
        }
        return executor;
    }
}
