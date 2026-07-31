using System.Collections.Generic;
using Undefined.Menu;
using Undefined.Utilities;

namespace Undefined.Mods.Categories;

public static class EnabledMods
{
    public static void UpdateCategory()
    {
        List<ButtonInfo> newCategory = new List<ButtonInfo>
        {
            new ButtonInfo
            {
                buttonText = "Return to Main",
                method = () => Main.activeCategory = Category.Main,
                isTogglable = false
            }
        };

        foreach (ButtonInfo mod in ModButtons.GetActiveMods())
            newCategory.Add(mod);

        ModButtons.buttons[(int)Category.EnabledMods] = newCategory.ToArray(); // not hardcoded anymore?
    }
}