using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Undefined.MENUSETTINGS;

namespace Undefined.Utilities;

public class GunLib
{
    public static GameObject spherepointer;
    public static VRRig LockedPlayer;

    public static float GunLineWidth = 0.012f;
    public static float SphereSize = 0.15f;

    private static LineRenderer gunLine;

    public static readonly string[] bypassLayers =
    {
        "Gorilla Trigger",
        "Gorilla Boundary",
        "GorillaHand",
        "GorillaObject",
        "Zone",
        "Water",
        "GorillaCosmetics",
        "GorillaParticle",
    };

    public static readonly LayerMask BypassLayers = ~LayerMask.GetMask(bypassLayers);

    public static Color GunColor =>
        Settings.backgroundColor.colors[0].color;

    public static Color PointerColor =>
        Color.Lerp(GunColor, Color.white, 0.35f);

    public static void start2guns(Action action, bool lockOn)
    {
        if (IsXRDeviceActive())
            StartVrGun(action, lockOn);
        else
            StartPcGun(action, lockOn);
    }

    public static void StartPcGun(Action action, bool lockOn)
    {
        if (!Mouse.current.rightButton.isPressed)
        {
            CleanupPointer();
            return;
        }

        Camera cam = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, BypassLayers))
            return;

        UpdateGun(hit, action, lockOn, cam.transform.position);
    }

    public static void StartVrGun(Action action, bool lockOn)
    {
        if (!InputHandler.Instance.RightGrip.IsPressed)
        {
            CleanupPointer();
            return;
        }

        if (!Physics.Raycast(
            GorillaTagger.Instance.rightHandTransform.position,
            -GorillaTagger.Instance.rightHandTransform.up,
            out RaycastHit hit,
            1000f,
            BypassLayers))
            return;

        UpdateGun(hit, action, lockOn, GorillaTagger.Instance.rightHandTransform.position);
    }

    private static void UpdateGun(RaycastHit hit, Action action, bool lockOn, Vector3 start)
    {
        if (spherepointer == null)
            CreatePointer();

        if (lockOn && LockedPlayer == null)
        {
            VRRig rig = hit.collider.GetComponentInParent<VRRig>();

            if (rig != null && rig != GorillaTagger.Instance.offlineVRRig)
                LockedPlayer = rig;
        }

        Vector3 pos = LockedPlayer != null
            ? LockedPlayer.transform.position
            : hit.point;

        spherepointer.transform.position = pos;
        spherepointer.GetComponent<Renderer>().material.color = PointerColor;

        UpdateLine(start, pos);

        bool pressed = IsXRDeviceActive()
            ? InputHandler.Instance.RightTrigger.IsPressed
            : Mouse.current.leftButton.isPressed;

        if (pressed)
        {
            if (!lockOn || LockedPlayer != null)
                action.Invoke();
        }
        else
        {
            LockedPlayer = null;
        }
    }

    private static void CreatePointer()
    {
        spherepointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        UnityEngine.Object.Destroy(
            spherepointer.GetComponent<Collider>()
        );

        spherepointer.transform.localScale =
            Vector3.one * SphereSize;

        spherepointer.GetComponent<Renderer>().material.shader =
            Shader.Find("GUI/Text Shader");

        CreateLine();
    }

    private static void CreateLine()
    {
        GameObject obj = new GameObject("GunLine");

        gunLine = obj.AddComponent<LineRenderer>();

        gunLine.positionCount = 2;
        gunLine.startWidth = GunLineWidth;
        gunLine.endWidth = GunLineWidth;

        gunLine.material = new Material(
            Shader.Find("Sprites/Default")
        );

        gunLine.numCapVertices = 5;
        gunLine.numCornerVertices = 5;
    }

    private static void UpdateLine(Vector3 start, Vector3 end)
    {
        if (gunLine == null)
            return;

        gunLine.SetPosition(0, start);
        gunLine.SetPosition(1, end);

        gunLine.startWidth = GunLineWidth;
        gunLine.endWidth = GunLineWidth;

        gunLine.startColor = GunColor;
        gunLine.endColor = GunColor;
    }

    public static Vector3 GetPointerPos()
    {
        return spherepointer != null
            ? spherepointer.transform.position
            : Vector3.zero;
    }

    public static void ChangeGunLineSize(bool increase)
    {
        GunLineWidth += increase ? 0.002f : -0.002f;

        GunLineWidth = Mathf.Clamp(
            GunLineWidth,
            0.001f,
            0.05f
        );
    }

    public static void ChangeGunSphereScale(bool increase)
    {
        SphereSize += increase ? 0.02f : -0.02f;

        SphereSize = Mathf.Clamp(
            SphereSize,
            0.05f,
            0.5f
        );

        if (spherepointer != null)
            spherepointer.transform.localScale =
                Vector3.one * SphereSize;
    }

    public static void ResetGunDefaults()
    {
        GunLineWidth = 0.012f;
        SphereSize = 0.15f;

        if (spherepointer != null)
            spherepointer.transform.localScale =
                Vector3.one * SphereSize;
    }

    public static void CleanupPointer()
    {
        if (spherepointer != null)
            UnityEngine.Object.Destroy(spherepointer);

        if (gunLine != null)
            UnityEngine.Object.Destroy(gunLine.gameObject);

        spherepointer = null;
        gunLine = null;
        LockedPlayer = null;
    }

    public static bool IsXRDeviceActive()
    {
        List<XRDisplaySubsystem> list = new();

        SubsystemManager.GetInstances(list);

        foreach (XRDisplaySubsystem xr in list)
        {
            if (xr.running)
                return true;
        }

        return false;
    }
}