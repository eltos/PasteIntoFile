using System;
using System.Configuration;
using System.IO;
using Bluegrams.Application;

namespace PasteIntoFile.Core;

/// <summary>
/// Utility to configure and apply PortableSettingsProvider with the correct
/// settings file path. In installer mode, settings are stored in
/// %APPDATA%\PasteIntoFile\user.config. In portable mode (#if PORTABLE),
/// settings are stored next to the executable.
/// </summary>
public static class SettingsUtil {
    private const string SettingsFileName = "user.config";
    private const string AppDataFolderName = "PasteIntoFile";

    /// <summary>
    /// Applies the PortableSettingsProvider to the given settings instance,
    /// configuring it to use the shared settings file path.
    /// Call this once at application startup before accessing any settings.
    /// </summary>
    public static void Apply(ApplicationSettingsBase settings) {
        PortableSettingsProvider.SettingsFileName = SettingsFileName;
        PortableSettingsProvider.SettingsDirectory = GetSettingsDirectory();
        PortableSettingsProvider.ApplyProvider(settings);
    }

    /// <summary>
    /// Returns the full path to the shared settings file.
    /// </summary>
    public static string SettingsFilePath => Path.Combine(GetSettingsDirectory(), SettingsFileName);

    private static string GetSettingsDirectory() {
#if PORTABLE
        var assemblyPath = typeof(SettingsUtil).Assembly.Location;
        return Path.GetDirectoryName(assemblyPath) ?? AppContext.BaseDirectory;
#else
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var settingsDir = Path.Combine(appData, AppDataFolderName);
        if (!Directory.Exists(settingsDir)) {
            Directory.CreateDirectory(settingsDir);
        }
        return settingsDir;
#endif
    }

    /// <summary>
    /// Reloads the settings from disk only if the settings file has been modified
    /// since the last successful reload. This avoids expensive file I/O on slow
    /// storage (USB flash drives, network shares) when the file hasn't changed.
    /// Returns true if a reload was performed, false if the cached value was still valid.
    /// </summary>
    public static bool ReloadIfChanged(ApplicationSettingsBase settings) {
        var filePath = SettingsFilePath;
        try {
            if (!File.Exists(filePath)) {
                // File doesn't exist — nothing to reload
                return false;
            }

            var lastWriteTime = File.GetLastWriteTimeUtc(filePath);

            if (lastWriteTime <= sLastReloadTime) {
                // File hasn't changed since last reload — skip the expensive Reload()
                return false;
            }

            settings.Reload();
            sLastReloadTime = lastWriteTime;
            return true;
        } catch {
            // If we can't check the file, fall back to always reloading
            // to ensure we get the latest value
            settings.Reload();
            return true;
        }
    }

    private static DateTime sLastReloadTime;
}
