using System.Collections.Generic;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Undefined.Mods.Categories;

public static class FavouriteMods
{
    public static readonly List<string> Favourites = new List<string>();

    private static Font starFont;

    public static bool IsFavourite(ModButtonInfo button)
    {
        return Favourites.Contains(button.buttonText);
    }

    public static void AddStar(RectTransform buttonText)
    {
        if (starFont == null)
            starFont = Font.CreateDynamicFontFromOSFont("Segoe UI Symbol", 32);

        Text star = new GameObject { transform = { parent = buttonText.parent } }.AddComponent<Text>();
        star.font = starFont;
        star.text = "\u2605";
        star.color = Color.yellow;
        star.fontSize = 1;
        star.alignment = TextAnchor.MiddleLeft;
        star.resizeTextForBestFit = true;
        star.resizeTextMinSize = 0;

        RectTransform starTrans = star.GetComponent<RectTransform>();
        starTrans.localPosition = buttonText.localPosition;
        starTrans.sizeDelta = new Vector2(0.2f, 0.03f);
        starTrans.rotation = buttonText.rotation;
    }

    public static bool CanFavourite(ModButtonInfo button)
    {
        return !button.isCategory && (button.isTogglable || button.isIncremental || button.method != null);
    }

    public static bool HoldingFavouriteGrip()
    {
        if (InputHandler.Instance == null)
            return false;

        return Variables.rightHanded
            ? InputHandler.Instance.LeftGrip.IsPressed
            : InputHandler.Instance.RightGrip.IsPressed;
    }

    public static void Toggle(ModButtonInfo button)
    {
        if (!Favourites.Remove(button.buttonText))
            Favourites.Add(button.buttonText);
    }

    public static void UpdateCategory()
    {
        List<ModButtonInfo> newCategory = new List<ModButtonInfo>
        {
            ModButtonInfo.Back(Category.Main)
        };

        foreach (string name in Favourites)
        {
            ModButtonInfo button = ModButtons.IsEnabled(name);

            if (button != null && !newCategory.Contains(button))
                newCategory.Add(button);
        }

        ModButtons.Buttons[Category.FavouriteMods] = newCategory.ToArray();
    }
}
