using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.SimpleSequenceExecutor.Entities;
public class BundleDbContext : DbContext
{
    public DbSet<Bundle> Bundles { get; set; }
    public DbSet<BundleStep> BundleSteps { get; set; }
    public DbSet<BundleStepParameter> BundleStepParameters { get; set; }
    public string DbPath { get; }
    public BundleDbContext()
    {
        string appFolder = AppContext.BaseDirectory;
        var pluginFolder = Path.Combine(appFolder!, "Plugins", "Adens.DevToys.SimpleSequenceExecutor");
        if (!Directory.Exists(pluginFolder))
        {
            Directory.CreateDirectory(pluginFolder);
        }
        var databasePath = Path.Combine(pluginFolder, "SimpleSequenceExecutor.db");
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "SimpleSequenceExecutor.db");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
       => options.UseSqlite($"Data Source={DbPath}");
}
