using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.SimpleSequenceExecutor.Entities;
public class ExecutorBundle
{
    public ExecutorBundle(string name, string? description=null)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Unique name of the executor bundle.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? Description { get; set; }
    public List<ExecutorStep> Steps { get; set; } = new List<ExecutorStep>();
}
public class ExecutorStep
{
    public string Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
}
