using System.Runtime.InteropServices;

namespace Adens.DevToys.ProgressiveJpegConverter.Helpers;
public static class MagickLoadHepler
{
    internal static void EnsureMagickLoaded()
    {
        string sqiteDllPath = Path.GetDirectoryName(typeof(MagickLoadHepler).Assembly.Location)!;
        string sqliteDllName = "Magick.Native-Q16-HDRI-x64.dll";
        if (OperatingSystem.IsWindows())
        {
            bool is64Bit = Environment.Is64BitOperatingSystem;
            if (is64Bit)
            {
                bool isArm64 = RuntimeInformation.ProcessArchitecture == Architecture.Arm64;
                if (isArm64)
                {
                    sqliteDllName = "Magick.Native-Q16-HDRI-arm64.dll";
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
                    sqliteDllName = "Magick.Native-Q16-HDRI-arm.dll";
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "win-arm", "native", sqliteDllName);
                }
                else
                {
                    sqliteDllName = "Magick.Native-Q16-HDRI-x86.dll";
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "win-x86", "native", sqliteDllName);
                }
            }
        }
        else if (OperatingSystem.IsMacOS())
        {
          
            bool isArm64 = RuntimeInformation.ProcessArchitecture == Architecture.Arm64;
            if (isArm64)
            {
                sqliteDllName = "Magick.Native-Q16-HDRI-arm64.dll.dylib";
                sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "osx-arm64", "native", sqliteDllName);
            }
            else
            {
                sqliteDllName = "Magick.Native-Q16-HDRI-x64.dll.dylib";
                sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "osx-x64", "native", sqliteDllName);
            }
        }
      
        else if (OperatingSystem.IsLinux())
        {
            
            switch (RuntimeInformation.ProcessArchitecture)
            {
               
                case Architecture.X64:
                    sqliteDllName = "Magick.Native-Q16-HDRI-x64.dll.so";
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "linux-x64", "native", sqliteDllName);
                    break;
              
                case Architecture.Arm64:
                    sqliteDllName = "Magick.Native-Q16-HDRI-arm64.dll.so";
                    sqiteDllPath = Path.Combine(sqiteDllPath, "runtimes", "linux-arm64", "native", sqliteDllName);
                    break;
              
                default:
                    break;
            }
        }
        // file copy to root
        File.Copy(sqiteDllPath, Path.Combine(Path.GetDirectoryName(typeof(MagickLoadHepler).Assembly.Location)!, sqliteDllName), true);
    }
}
