using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys;
public static class DevToysConstants
{
    public static readonly string AppCacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppHelper.IsPreviewVersion ? "DevToys-preview" : "DevToys");

    public static string PluginInstallationFolder => Path.Combine(AppCacheDirectory, "Plugins");

    public static string AppTempFolder => Path.Combine(AppCacheDirectory, "Temp");
    public static string AppDataFolder => Path.Combine(AppCacheDirectory, "Data");
}
