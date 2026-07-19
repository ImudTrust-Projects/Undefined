using System;
using System.Collections.Generic;
using System.Text;
using Undefined.Utilities;
using UnityEngine;
using static UnityEngine.GridBrushBase;

namespace Undefined.Mods.Categories;

public class MapLoader
{
    #region City
    public static void City()
    {
        ZoneManagement.SetActiveZone(GTZone.city);
        Variables.TeleportPlayer(new Vector3(-63.04f, 15.85f, -100.04f));
    }
    #endregion

    #region Forest
    public static void Forest()
    {
        ZoneManagement.SetActiveZone(GTZone.forest);
        Variables.TeleportPlayer(new Vector3(-66.90f, 12.24f, -78.63f));
    }
    #endregion

    #region Canyon
    public static void Canyon()
    {
        ZoneManagement.SetActiveZone(GTZone.canyon);
        Variables.TeleportPlayer(new Vector3(-76.97f, 13.16f, -95.17f));
    }
    #endregion

    #region LavaForest
    public static void LavaForest()
    {
        ZoneManagement.SetActiveZone(GTZone.VIMExperience1);
        Variables.TeleportPlayer(new Vector3(213.67f, 78.19f, 238.52f));
    }
    #endregion
}