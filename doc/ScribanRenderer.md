# Adens.DevToys.ScribanRenderer
A scriban template render utility for DevToys.

## License
This extension is licensed under the GPL License - see the LICENSE file for details.

## Installation
1. Download the [Adens.DevToys.ScribanRenderer](https://www.nuget.org/packages/Adens.DevToys.ScribanRenderer/) NuGet package from NuGet.org.
2. For DevToys, open Manager Extensions, click on Install and select the downloaded NuGet package.

## Limitations

Not support for DevToys CLI (for now).

## Changelog

- 0.0.4

To ensure that data persists during plugin upgrades or uninstalls, move the SQLite storage path to "%LocalAppData%/DevToys-preview/Data/ScibanRenderer.db"

The data will not be deleted when the plugin is uninstalled. If you need to delete the saved data, it must be done manually.

- 0.0.3

add sqlite storage.stored