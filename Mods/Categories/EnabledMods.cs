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
            ModButtonInfo.Back(Category.Main)
        };

        HashSet<string> addedMods = new HashSet<string>();

        foreach (ModButtonInfo mod in ModButtons.GetActiveMods())
        {
            if (!addedMods.Contains(mod.buttonText))
            {
                addedMods.Add(mod.buttonText);
                newCategory.Add(mod);
            }
        }

        ModButtons.Buttons[Category.EnabledMods] = newCategory.ToArray();
    }
}