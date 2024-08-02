using SQLite;

namespace Adens.DevToys.SimpleSequenceExecutor.Entities;

public class BundleStep
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public string Type { get; set; }
    public Guid BundleId { get; set; }
    [Ignore]
    public virtual Bundle Bundle { get; set; }
    [Ignore]
    public virtual List<BundleStepParameter> Parameters { get; set; } = new List<BundleStepParameter>();
}