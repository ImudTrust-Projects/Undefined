using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GorillaNetworking;
using Undefined.Admin.Menu;
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
        Variables.TeleportPlayer(new Vector3(-84f, 31f, -78f));
    }
    #endregion

    #region LavaForest
    public static void LavaForest()
    {
        ZoneManagement.SetActiveZone(GTZone.VIMExperience1);
        Variables.TeleportPlayer(new Vector3(213.67f, 78.19f, 238.52f));
    }
    #endregion
    
    #region Mall

    public static void Mall()
    {
        ZoneManagement.SetActiveZone(GTZone.mall);
        Variables.TeleportPlayer(new Vector3(-65.0762f, 5.6907f, -101.3384f));
    }
    #endregion    
    
    #region Ranked

    public static void Ranked()
    {
        ZoneManagement.SetActiveZone(GTZone.ranked);
        Variables.TeleportPlayer(new Vector3(-105.5082f, 17.4496f, -273.5027f));
    }
    #endregion
    
    #region Critters

    public static void Critters()
    {
        ZoneManagement.SetActiveZone(GTZone.critters);
        Variables.TeleportPlayer(new Vector3(95.0941f, -93.3268f, 44.3547f));
    }
    #endregion
    
    #region Studio
    
    public static void Studio()
    {
        ZoneManagement.SetActiveZone(GTZone.SilverbackStudios);
        Variables.TeleportPlayer(new Vector3(371.8013f, 164.424f, -673.3091f));
    }
    #endregion
    
    #region MonkeBlocks
    
    public static void MonkeBlocks()
    {
        ZoneManagement.SetActiveZone(GTZone.monkeBlocks);
        Variables.TeleportPlayer(new Vector3(-122.9342f, 16.841f, -222.5052f));
    }
    #endregion
    
    #region SkatePark
    
    public static void SkatePark()
    {
        ZoneManagement.SetActiveZone(GTZone.hoverboard);
        Variables.TeleportPlayer(new Vector3(-90.2826f, -16.7527f, 29.9876f));
    }
    #endregion
    
    #region Tutorial
    
    public static void Tutorial()
    {
        ZoneManagement.SetActiveZone(GTZone.tutorial);
        Variables.TeleportPlayer(new Vector3(-55.2344f, 2.0516f, -56.9323f));
    }
    #endregion
    
    #region Beach
    
    public static void Beach()
    {
        ZoneManagement.SetActiveZone(GTZone.beach);
        Variables.TeleportPlayer(new Vector3(-14.3614f, 28.5491f, -19.602f));
    }
    #endregion
    
    #region Cave
    
    public static void Cave()
    {
        ZoneManagement.SetActiveZone(GTZone.cave);
        Variables.TeleportPlayer(new Vector3(-60.7809f, -5.7066f, -72.5816f));
    }
    #endregion
    
    #region Basement
    
    public static void Basement()
    {
        ZoneManagement.SetActiveZone(GTZone.basement);
        Variables.TeleportPlayer(new Vector3(-31.4515f, 14.1817f, -91.3555f));
    }
    #endregion
    
    #region Metropolis
    
    public static void Metropolis()
    {
        ZoneManagement.SetActiveZone(GTZone.Metropolis);
        Variables.TeleportPlayer(new Vector3(66.3536f, 3.9001f, -240.9903f));
    }
    #endregion
    
    #region Mountain
    
    public static void Mountain()
    {
        ZoneManagement.SetActiveZone(GTZone.mountain);
        Variables.TeleportPlayer(new Vector3(-19.7542f, 17.9882f, -107.039f));
    }
    #endregion
    
    #region SkyJungle
    
    public static void SkyJungle()
    {
        ZoneManagement.SetActiveZone(GTZone.skyJungle);
        Variables.TeleportPlayer(new Vector3(-76.0857f, 162.5829f, -97.3752f));
    }
    #endregion
}