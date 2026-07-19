using System.Collections.Generic;
using Undefined.Utilities;

namespace Undefined.Mods.Categories;

public static class EnabledMods
{
    public static void UpdateCategory()
    {
        List<ButtonInfo> newCategory = new List<ButtonInfo>
        {
            new ButtonInfo { buttonText = "Return to Main", method = () => Menu.Main.activeCategory = 0, isTogglable = false }
        };

        foreach (var mod in ModButtons.GetActiveMods())
        {
            newCategory.Add(mod);
        }

        ModButtons.buttons[3] = newCategory.ToArray(); // hard coded??
    }
}