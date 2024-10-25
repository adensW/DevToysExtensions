using System.Runtime.InteropServices;

namespace Adens.DevToys.SimpleSequenceExecutor.Helpers;
internal static class SqliteLoadHepler
{
    internal static Task EnsureSqliteLoaded()
    {
        string sqiteDllPath = Path.GetDirectoryName(typeof(SqliteLoadHepler).Assembly.Location)!;
        string sqliteDllName = "e_sqlite3.dll";
        if (OperatingSystem.IsWindows())
        {
            bool is64Bit = Environment.Is64BitOperatingSystem;
            if (is64Bit)
            {
                bool isArm64 = RuntimeInformation.ProcessArchitecture == Architecture.Arm64;
                if (isArm64)
                {
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "win-arm64", "native", sqliteDllName);
                }
                else
                {
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "win-x64", "native", sqliteDllName);

                }
            }
            else
            {
                bool isArm = RuntimeInformation.ProcessArchitecture == Architecture.Arm;
                if (isArm)
                {
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "win-arm", "native", sqliteDllName);
                }
                else
                {
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "win-x86", "native", sqliteDllName);
                }
            }
        }
        else if (OperatingSystem.IsMacOS())
        {
            sqliteDllName = "e_sqlite3.dylib";
            bool isArm64 = RuntimeInformation.ProcessArchitecture == Architecture.Arm64;
            if (isArm64)
            {
                sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "osx-arm64", "native", sqliteDllName);
            }
            else
            {
                sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "osx-x64", "native", sqliteDllName);
            }
        }
        else if (OperatingSystem.IsMacCatalyst())
        {
            sqliteDllName = "e_sqlite3.dylib";
            bool isArm64 = RuntimeInformation.ProcessArchitecture == Architecture.Arm64;
            if (isArm64)
            {
                sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "maccatalyst-arm64", "native", sqliteDllName);
            }
            else
            {
                sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "maccatalyst-x64", "native", sqliteDllName);
            }
        }
        else if (OperatingSystem.IsLinux())
        {
            sqliteDllName = "e_sqlite3.so";
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86:
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "linux-x86", "native", sqliteDllName);
                    break;
                case Architecture.X64:
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "linux-x64", "native", sqliteDllName);
                    break;
                case Architecture.Arm:
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "linux-arm", "native", sqliteDllName);
                    break;
                case Architecture.Arm64:
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "linux-arm64", "native", sqliteDllName);
                    break;
                case Architecture.S390x:
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "linux-s390x", "native", sqliteDllName);
                    break;
                default:
                    break;
            }
        }
        // file copy to root
        File.Copy(sqiteDllPath, Path.Combine(Path.GetDirectoryName(typeof(SqliteLoadHepler).Assembly.Location)!, sqliteDllName), true);
            return Task.CompletedTask;
    }
    internal static string GetDatabasePath()
    {
        string appFolder = Path.GetDirectoryName(typeof(SqliteLoadHepler).Assembly.Location)!;
        var pluginFolder = Path.Combine(appFolder, "data");
        if (!Directory.Exists(pluginFolder))
        {
            Directory.CreateDirectory(pluginFolder);
        }
        var databasePath = Path.Combine(pluginFolder, "SimpleSequenceExecutor.db");
        if (!File.Exists(databasePath))
        {
            File.Create(databasePath).Flush();
        }
        return databasePath;
    }
}
