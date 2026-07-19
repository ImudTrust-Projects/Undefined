using System.Collections.Generic;
using System.Linq;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Visuals
{
    private class ESPData
    {
        public GameObject[] objs;
        public Renderer[] rends;
    }

    private static Dictionary<VRRig, ESPData> esp = new();
    private static Material mat;

    public static void BoxESP2DEnable()
    {
        mat = new Material(Shader.Find("GUI/Text Shader"));
    }

    public static void BoxESP2D()
    {
        if (mat == null) return;

        foreach (VRRig rig in VRRigCache.ActiveRigs)
        {
            if (rig == null || rig == GorillaTagger.Instance.offlineVRRig)
                continue;

            if (!esp.TryGetValue(rig, out var data))
            {
                data = new ESPData
                {
                    objs = new GameObject[4],
                    rends = new Renderer[4]
                };

                for (int i = 0; i < 4; i++)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.Destroy(cube.GetComponent<BoxCollider>());

                    var r = cube.GetComponent<Renderer>();
                    r.material = mat;

                    data.objs[i] = cube;
                    data.rends[i] = r;
                }

                esp[rig] = data;
            }

            Transform t = rig.transform;

            Quaternion rot = Quaternion.LookRotation(
                t.position - GorillaTagger.Instance.headCollider.transform.position
            );

            Vector3 up = rot * Vector3.up;
            Vector3 right = rot * Vector3.right;
            Vector3 pos = t.position;

            Vector3[] p =
            {
                pos + up * 0.35f,
                pos + up * -0.55f,
                pos + right * -0.33f + up * -0.10f,
                pos + right * 0.33f + up * -0.10f
            };

            Vector3[] s =
            {
                new Vector3(0.66f, 0.02f, 0.01f),
                new Vector3(0.66f, 0.02f, 0.01f),
                new Vector3(0.02f, 0.9f, 0.01f),
                new Vector3(0.02f, 0.9f, 0.01f)
            };

            Color c = rig.playerColor;

            var d = esp[rig];

            for (int i = 0; i < 4; i++)
            {
                var obj = d.objs[i];
                if (!obj) continue;

                obj.transform.position = p[i];
                obj.transform.rotation = rot;
                obj.transform.localScale = s[i];

                d.rends[i].material.color = c;
            }
        }
        List<VRRig> remove = new();

        foreach (var kvp in esp)
        {
            if (!VRRigCache.ActiveRigs.Contains(kvp.Key))
            {
                foreach (var o in kvp.Value.objs)
                    if (o) Object.Destroy(o);

                remove.Add(kvp.Key);
            }
        }

        foreach (var r in remove)
            esp.Remove(r);
    }

    public static void BoxESP2DDisable()
    {
        foreach (var kvp in esp)
        {
            foreach (var o in kvp.Value.objs)
                if (o) Object.Destroy(o);
        }

        esp.Clear();

        if (mat)
            Object.Destroy(mat);

        mat = null;
    }

    // for ghost monke and invis monke.
    public static void Hologram(Material material)
    {
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }

    public static void UnHologram(Material material)
    {
        material.SetFloat("_Mode", 0);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 2000;
    }

    private static GameObject handL;
    private static GameObject handR;

    public static void CreateCubes()
    {
        Transform left = VRRig.LocalRig.leftHandTransform;
        Transform right = VRRig.LocalRig.rightHandTransform;

        handR = GameObject.CreatePrimitive(PrimitiveType.Cube);
        UnityEngine.Object.Destroy(handR.GetComponent<Collider>());
        UnityEngine.Object.Destroy(handR.GetComponent<Rigidbody>());
        handR.transform.SetPositionAndRotation(right.position, right.rotation);
        handR.transform.localScale = Vector3.one * 0.063f;

        handL = GameObject.CreatePrimitive(PrimitiveType.Cube);
        UnityEngine.Object.Destroy(handL.GetComponent<Collider>());
        UnityEngine.Object.Destroy(handL.GetComponent<Rigidbody>());
        handL.transform.SetPositionAndRotation(left.position, left.rotation);
        handL.transform.localScale = Vector3.one * 0.063f;

        Color bg = MENUSETTINGS.Settings.backgroundColor.colors[0].color;
        bg.a = 0.5f;

        Renderer rightRenderer = handR.GetComponent<Renderer>();
        Renderer leftRenderer = handL.GetComponent<Renderer>();

        Hologram(rightRenderer.material);
        Hologram(leftRenderer.material);

        rightRenderer.material.color = bg;
        leftRenderer.material.color = bg;

        UnityEngine.Object.Destroy(handR, Time.deltaTime);
        UnityEngine.Object.Destroy(handL, Time.deltaTime);
    }
}