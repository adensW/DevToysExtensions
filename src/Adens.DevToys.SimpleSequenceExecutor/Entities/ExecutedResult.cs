using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.SimpleSequenceExecutor.Entities;
public  class ExecutedResult
{
    public Dictionary<string, object> RuntimeVariables { get; set; }
    = new Dictionary<string, object>();
    public Dictionary<string, object> OutputVariables { get; set; } = new Dictionary<string, object>();
    public static ExecutedResult Create(Dictionary<string,object>? runtimeVariables=null,Dictionary<string,object>? output=null)
    {
        return new ExecutedResult
        {
            RuntimeVariables = runtimeVariables??new Dictionary<string, object>(),
            OutputVariables = output??new Dictionary<string, object>()
        };
    }
}
