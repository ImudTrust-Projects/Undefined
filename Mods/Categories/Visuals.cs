using POpusCodec.Enums;
using System.Collections.Generic;
using System.Linq;
using Undefined.Menu;
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

    static Dictionary<VRRig, List<GameObject>> humanoidesp = new Dictionary<VRRig, List<GameObject>>();
    static List<TrailRenderer> TrailObj = new List<TrailRenderer>();
    private static readonly Dictionary<VRRig, float> delays = new();
    static List<LineRenderer> bonespob = new List<LineRenderer>();
    static List<LineRenderer> traceronj = new List<LineRenderer>();
    public static void HumanoidESP()
    {
        if (humanoidesp.Count > 1)
        {
            foreach (var pairs in humanoidesp)
            {
                foreach (var pair in pairs.Value)
                {
                    if (!VRRigCache.ActiveRigs.Contains(pairs.Key))
                    {
                        Object.Destroy(pair);
                        humanoidesp.Remove(pairs.Key);
                    }
                }
            }
        }
        foreach (var rig in VRRigCache.ActiveRigs)
        {
            if (!rig.isOfflineVRRig && rig != null)
            {
                if (!humanoidesp.ContainsKey(rig))
                {
                    var humanoids = new List<GameObject>();

                    var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Object.Destroy(head.GetComponent<Collider>());
                    head.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    Visuals.Hologram(head.GetComponent<Renderer>().material);
                    head.transform.position = rig.head.rigTarget.position;
                    head.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    if (!humanoids.Contains(head))
                        humanoids.Add(head);

                    var lhand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Object.Destroy(lhand.GetComponent<Collider>());
                    lhand.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    Visuals.Hologram(lhand.GetComponent<Renderer>().material);
                    lhand.transform.position = rig.leftHand.rigTarget.position;
                    lhand.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    if (!humanoids.Contains(lhand))
                        humanoids.Add(lhand);

                    var rhand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Object.Destroy(rhand.GetComponent<Collider>());
                    rhand.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    Visuals.Hologram(rhand.GetComponent<Renderer>().material);
                    rhand.transform.position = rig.rightHand.rigTarget.position;
                    rhand.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    if (!humanoids.Contains(rhand))
                        humanoids.Add(rhand);

                    humanoidesp.Add(rig, humanoids);
                }
                else
                {
                    for (var i = 0; i < 1; i++)
                    {
                        var f0 = i / 1f;
                        var f1 = (i + 1) / 1f;

                        foreach (var go in humanoidesp[rig])
                        {
                            var color = Color.Lerp(Color.purple, Color.darkBlue, f0);
                            color.a = 0.3f;
                            go.GetComponent<Renderer>().material.color = color;
                        }
                    }

                    humanoidesp[rig][0].transform.position = rig.head.rigTarget.position + new Vector3(0f, 0.1f, 0f);
                    humanoidesp[rig][1].transform.position = rig.leftHand.rigTarget.position;
                    humanoidesp[rig][2].transform.position = rig.rightHand.rigTarget.position;
                }
            }
        }
    }
    public static void HumanoidESPOff()
    {
        foreach (var pairs in humanoidesp.Values)
        {
            foreach (var pair in pairs)
            {
                if (pair != null)
                    Object.Destroy(pair);
            }
        }
        humanoidesp.Clear();
    }

    public static void Trails()
    {
        float t = Time.time * 0.2f;
        foreach (VRRig rig in VRRigCache.ActiveRigs)
        {
            if (rig == GorillaTagger.Instance.offlineVRRig || rig == null) continue;

            Material material = new Material(Shader.Find("GUI/Text Shader"));

            if (!rig.gameObject.GetComponent<TrailRenderer>())
            {
                rig.gameObject.AddComponent<TrailRenderer>();
                TrailObj.Add(rig.gameObject.GetComponent<TrailRenderer>());
            }

            rig.gameObject.GetComponent<TrailRenderer>();
            rig.GetComponent<TrailRenderer>().startWidth = 0.1f;
            rig.GetComponent<TrailRenderer>().endWidth = 0f;
            rig.GetComponent<TrailRenderer>().material = material;

            rig.GetComponent<TrailRenderer>().startColor = Color.purple;
            rig.GetComponent<TrailRenderer>().endColor = Color.darkBlue;

            rig.GetComponent<TrailRenderer>().time = 1;
        }
    }
    public static void DisableTrail()
    {
        foreach (var rig in TrailObj)
        {
            if (rig != null)
            {
                Object.Destroy(rig);
            }
        }
    }

    public static void FixRigMaterialESPColors(VRRig rig)
    {
        if (rig == null || rig.mainSkin == null)
            return;

        if (delays.TryGetValue(rig, out var time) && Time.time < time)
            return;

        delays[rig] = Time.time + 5f;

        var mesh = rig.mainSkin.sharedMesh;

        var colors32 = new Color32[mesh.colors32.Length];
        for (int i = 0; i < colors32.Length; i++)
            colors32[i] = Color.white;

        mesh.colors32 = colors32;

        var colors = new Color[mesh.colors.Length];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.white;

        mesh.colors = colors;
    }

    public static void ChamESPOn()
    {
        foreach (var rig in VRRigCache.ActiveRigs)
        {
            if (!rig.isOfflineVRRig)
            {
                FixRigMaterialESPColors(rig);

                rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                Visuals.Hologram(rig.mainSkin.material);
                var t = Time.time * 0.2f;
                for (var i = 0; i < 1; i++)
                {
                    var f0 = i / 1f;
                    var f1 = (i + 1) / 1f;
                    var color = MENUSETTINGS.Settings.backgroundColor.colors[0].color;
                    color.a = 0.3f;
                    rig.mainSkin.material.color = color;
                }
            }
        }
    }
    public static void ChamESPOff()
    {
        foreach (var rig in VRRigCache.ActiveRigs)
        {
            if (!rig.isOfflineVRRig)
            {
                Visuals.UnHologram(rig.mainSkin.material);
                rig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");

                if (rig.mainSkin.material.name.Contains("gorilla_body"))
                    rig.mainSkin.material.color = rig.playerColor;
            }
        }
    }

    public static void BoneESP()
    {
        foreach (var rig in VRRigCache.ActiveRigs)
        {
            if (rig.isOfflineVRRig) continue;

            var headGO = rig.head.rigTarget.gameObject;
            var headLR = headGO.GetComponent<LineRenderer>()
                      ?? headGO.AddComponent<LineRenderer>();

            if (!bonespob.Contains(headLR))
                bonespob.Add(headLR);

            headLR.startWidth = headLR.endWidth = 0.015f;
            headLR.material.shader = Shader.Find("GUI/Text Shader");
            headLR.startColor = Color.purple;
            headLR.endColor = Color.darkBlue;

            headLR.positionCount = 2;
            headLR.SetPosition(0, headGO.transform.position + Vector3.up * 0.16f);
            headLR.SetPosition(1, headGO.transform.position - Vector3.up * 0.4f);

            int total = Variables.bones.Count();

            for (int i = 0; i < total; i += 2)
            {
                var b0 = rig.mainSkin.bones[Variables.bones[i]];
                var b1 = rig.mainSkin.bones[Variables.bones[i + 1]];

                var boneLR = b0.gameObject.GetComponent<LineRenderer>()
                           ?? b0.gameObject.AddComponent<LineRenderer>();

                if (!bonespob.Contains(boneLR))
                    bonespob.Add(boneLR);

                boneLR.startWidth = boneLR.endWidth = 0.015f;
                boneLR.material.shader = Shader.Find("GUI/Text Shader");

                boneLR.startColor = Color.purple;
                boneLR.endColor = Color.darkBlue;

                boneLR.positionCount = 2;
                boneLR.SetPosition(0, b0.position);
                boneLR.SetPosition(1, b1.position);
            }
        }
    }

    public static void BoneESPOff()
    {
        foreach (var line in bonespob)
        {
            if (line != null)
                Object.Destroy(line);
        }

        bonespob.Clear();
    }

    public static void TracerESP()
    {
        float t = Time.time * 0.2f;
        foreach (var rig in VRRigCache.ActiveRigs)
        {
            if (!rig.isOfflineVRRig)
            {
                if (rig.head.rigTarget.gameObject.GetComponent<LineRenderer>() == null)
                {
                    rig.head.rigTarget.gameObject.AddComponent<LineRenderer>();
                    traceronj.Add(rig.head.rigTarget.gameObject.GetComponent<LineRenderer>());
                }
                var lr = rig.head.rigTarget.gameObject.GetComponent<LineRenderer>();

                lr.startWidth = 0.0075f;
                lr.endWidth = 0.0075f;
                lr.startColor = MENUSETTINGS.Settings.backgroundColor.colors[0].color;
                lr.material.shader = Shader.Find("GUI/Text Shader");
                lr.SetPosition(0, Variables.TrueRightHand().position);
                lr.SetPosition(1, rig.head.rigTarget.transform.position);
            }
        }
    }
    public static void TracerESPOff()
    {
        foreach (var rig in traceronj)
        {
            if (rig != null)
            {
                Object.Destroy(rig);
            }
        }
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