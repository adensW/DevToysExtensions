using SQLite;

namespace Adens.DevToys.SimpleSequenceExecutor.Entities;

public  class BundleStepParameter
{
    [PrimaryKey,AutoIncrement]
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
    public Guid StepId { get; set; }
    [Ignore]
    public virtual BundleStep Step { get; set; }
}