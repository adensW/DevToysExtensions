using SQLite;

namespace Adens.DevToys.SimpleSequenceExecutor.Entities;

public  class BundleStepParameter
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
    public Guid StepId { get; set; }
   
}