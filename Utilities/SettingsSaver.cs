using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Undefined.Menu;
using Undefined.Mods;

namespace Undefined.Utilities;

public static class SettingsSaver
{
    public static string SavePath => Path.Combine(BepInEx.Paths.GameRootPath, Constants.PluginName, "settings.json");

    public class SaveData
    {
        public List<string> ActiveMods = new List<string>();
        public Dictionary<string, int> IncrementalSettings = new Dictionary<string, int>();
    }

    public static void Save()
    {
        var data = new SaveData();

        foreach (var btn in ModButtons.GetActiveMods())
        {
            data.ActiveMods.Add(btn.buttonText);
        }

        foreach (var category in ModButtons.buttons)
        {
            foreach (var btn in category)
            {
                if (btn != null && btn.isIncremental)
                {
                    data.IncrementalSettings[btn.buttonText] = btn.currentIncrementalIndex;
                }
            }
        }

        Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
        File.WriteAllText(SavePath, JsonConvert.SerializeObject(data, Formatting.Indented));
    }

    public static void Load()
    {
        if (!File.Exists(SavePath)) return;

        var json = File.ReadAllText(SavePath);
        var data = JsonConvert.DeserializeObject<SaveData>(json);

        if (data == null) return;

        foreach (var category in ModButtons.buttons)
        {
            foreach (var btn in category)
            {
                if (btn != null && btn.isIncremental && data.IncrementalSettings.TryGetValue(btn.buttonText, out int index))
                {
                    btn.currentIncrementalIndex = index;
                    btn.incrementalMethod?.Invoke(btn.GetCurrentIncrementalValue());
                }
            }
        }

        foreach (var category in ModButtons.buttons)
        {
            foreach (var btn in category)
            {
                if (btn != null && btn.isTogglable && !string.IsNullOrEmpty(btn.buttonText) && !btn.buttonText.StartsWith("Return"))
                {
                    bool shouldBeActive = data.ActiveMods.Contains(btn.buttonText);

                    if (shouldBeActive && !btn.enabled)
                    {
                        btn.enabled = true;
                        btn.enableMethod?.Invoke();
                    }
                    else if (!shouldBeActive && btn.enabled)
                    {
                        btn.enabled = false;
                        btn.disableMethod?.Invoke();
                    }
                }
            }
        }
    }
}
