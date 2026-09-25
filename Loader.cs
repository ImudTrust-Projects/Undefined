using Undefined;
using Undefined.Menu;
using Undefined.Utilities;
using UnityEngine;

namespace LoadMenu;

public class Loader
{
    private static GameObject gameobject;
    private static GameObject loadingObject;

    public static void Load()
    {
        Debug.Log("inject successfully");
        
        gameobject = new GameObject();
        gameobject.AddComponent<Plugin>();
            
        Object.DontDestroyOnLoad(gameobject);
    }

    public static void Unload()
    {
        if (gameobject != null)
            Object.Destroy(gameobject);
    }
}