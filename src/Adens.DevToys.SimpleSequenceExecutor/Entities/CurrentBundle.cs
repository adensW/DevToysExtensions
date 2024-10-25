using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.SimpleSequenceExecutor.Entities;
public class CurrentBundle
{
    [PrimaryKey,AutoIncrement]
    public int Id { get; set; }
    public Guid BundleId { get; set; }

}
