using System.Collections.Generic;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.MENUSETTINGS;

public class Settings
{
    public static ExtGradient backgroundColor = new ExtGradient
    {
        colors = ExtGradient.GetSimpleGradient(
            new Color32(25, 25, 25, 255),
            new Color32(45, 45, 45, 255)
        )
    };

    public static ExtGradient[] buttonColors =
    {
        new ExtGradient
        {
            colors = ExtGradient.GetSolidGradient(
                new Color32(55, 55, 55, 255)
            )
        },
        new ExtGradient
        {
            colors = ExtGradient.GetSolidGradient(
                new Color32(100, 100, 100, 255)
            )
        }
    };

    public static Color[] textColors =
    {
        Color.white,
        Color.white
    };

    public static Font[] Fonts;

    public static int fontIndex = 2;

    public static List<string> fontOptions = new List<string>
    {
        "Arial",
        "Comic Sans",
        "Minecraft"
    };

    public static void SetFont(string fontName)
    {
        fontIndex = fontName switch
        {
            "Arial" => 0,
            "Comic Sans" => 1,
            "Minecraft" => 2,
            _ => 0
        };
    }

    public static Font currentFont
    {
        get
        {
            if (Fonts == null)
            {
                FontManager.LoadFonts();

                Fonts = new Font[]
                {
                    FontManager.GetFont("Arial"), // 0
                    FontManager.GetFont("Comic Sans"), // 1
                    FontManager.GetFont("Minecraft") // 2
                };
            }

            return Fonts[fontIndex];
        }
    }
}