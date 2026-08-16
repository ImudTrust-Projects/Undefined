using System.Collections.Generic;
using Undefined.Menu;
using Undefined.Utilities;

namespace Undefined.Mods.Categories;

public static class EnabledMods
{
    public static void UpdateCategory()
    {
        List<ModButtonInfo> newCategory = new List<ModButtonInfo>
        {
            new ModButtonInfo
            {
                buttonText = "Return to Main",
                method = () => Main.activeCategory = Category.Main,
                isTogglable = false
            }
        };

        foreach (ModButtonInfo mod in ModButtons.GetActiveMods())
            newCategory.Add(mod);

        ModButtons.Buttons[Category.EnabledMods] = newCategory.ToArray();
    }
}