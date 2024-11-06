using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.ScribanRenderer.Entities;
public  class TemplateItem
{
    public TemplateItem()
    {
    }
  
    [PrimaryKey]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Template { get; set; }
    public string JsonData { get; set; }
}
