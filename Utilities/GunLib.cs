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

    public static float GunLineWidth = 0.03f;
    public static float SphereSize = 0.2f;


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

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f))
            return;

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


        DrawLine(
            GorillaTagger.Instance.rightHandTransform.position,
            pos
        );


        if (Mouse.current.leftButton.isPressed)
        {
            if (!lockOn || LockedPlayer != null)
                action.Invoke();
        }
        else
        {
            LockedPlayer = null;
        }
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
            1000f))
            return;


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


        DrawLine(
            GorillaTagger.Instance.rightHandTransform.position,
            pos
        );


        if (InputHandler.Instance.RightTrigger.IsPressed)
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
            new Vector3(SphereSize, SphereSize, SphereSize);

        spherepointer.GetComponent<Renderer>().material.shader =
            Shader.Find("GUI/Text Shader");
    }


    private static void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject obj = new GameObject("GunLine");

        LineRenderer line = obj.AddComponent<LineRenderer>();

        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.startWidth = GunLineWidth;
        line.endWidth = GunLineWidth;

        line.material = new Material(
            Shader.Find("Sprites/Default")
        );

        line.startColor = GunColor;
        line.endColor = GunColor;

        UnityEngine.Object.Destroy(obj, Time.deltaTime);
    }


    public static Vector3 GetPointerPos()
    {
        return spherepointer != null
            ? spherepointer.transform.position
            : Vector3.zero;
    }


    public static void ChangeGunLineSize(bool increase)
    {
        float step = 0.005f;

        GunLineWidth += increase ? step : -step;

        GunLineWidth = Mathf.Clamp(
            GunLineWidth,
            0.001f,
            0.1f
        );
    }


    public static void ChangeGunSphereScale(bool increase)
    {
        float step = 0.05f;

        SphereSize += increase ? step : -step;

        SphereSize = Mathf.Clamp(
            SphereSize,
            0.05f,
            0.5f
        );


        if (spherepointer != null)
            spherepointer.transform.localScale =
                new Vector3(SphereSize, SphereSize, SphereSize);
    }


    public static void ResetGunDefaults()
    {
        GunLineWidth = 0.03f;
        SphereSize = 0.2f;

        if (spherepointer != null)
            spherepointer.transform.localScale =
                new Vector3(SphereSize, SphereSize, SphereSize);
    }


    public static void CleanupPointer()
    {
        if (spherepointer != null)
            UnityEngine.Object.Destroy(spherepointer);

        spherepointer = null;
        LockedPlayer = null;
    }


    public static bool IsXRDeviceActive()
    {
        List<XRDisplaySubsystem> list = new List<XRDisplaySubsystem>();

        SubsystemManager.GetInstances(list);

        foreach (XRDisplaySubsystem xr in list)
        {
            if (xr.running)
                return true;
        }

        return false;
    }
}