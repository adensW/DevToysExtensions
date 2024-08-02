using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.SimpleSequenceExecutor.Entities;
public class Bundle
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public Bundle(string name, string? description=null)
    {
        Name = name;
        Description = description;
    }
    [Indexed(Unique =true)]
    /// <summary>
    /// Unique name of the executor bundle.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? Description { get; set; }
    [Ignore]
    public virtual List<BundleStep> Steps { get; set; } = new List<BundleStep>();
}
