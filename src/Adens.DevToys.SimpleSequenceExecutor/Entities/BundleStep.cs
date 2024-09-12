using SQLite;
using System.ComponentModel;

namespace Adens.DevToys.SimpleSequenceExecutor.Entities;

public class BundleStep
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public string Type { get ; set ; }
    public Guid BundleId { get; set; }
  
}