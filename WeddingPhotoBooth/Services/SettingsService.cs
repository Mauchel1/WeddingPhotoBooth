using System.IO;
using System.Text.Json;
using WeddingPhotoBooth.Models;

namespace WeddingPhotoBooth.Services;

public static class SettingsService
{
    public static Settings Load()
    {
        var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        if (!File.Exists(file))
            return new Settings();

        var json = File.ReadAllText(file);

        return JsonSerializer.Deserialize<Settings>(json)
               ?? new Settings();
    }
}