using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.ScribanRenderer.Entities;
public  class CurrentTemplateItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public Guid ItemId { get; set; }
}
