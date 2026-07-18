using BepInEx;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.XR;
using Valve.Newtonsoft.Json.Linq;
using Valve.VR;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Undefined.Mods.Categories;

public class Movement
{
    public static float startX = -1f;
    public static float startY = -1f;

    public static float subThingy;
    public static float subThingyZ;

    public static float FlySpeed = 10f; // this is very bad, but it works for now. I will fix this later

    public static int platMode = 0;
    public static int platInput = 0;

    public static List<string> PlatformMode = new()
    {
            "Normal",
            "Invisible",
            "Noclip"
    };

    private static GameObject leftPlat;
    private static GameObject rightPlat;
    private static bool lastLeftHeld;
    private static bool lastRightHeld;
    private static bool noclipActive;

    private static bool LeftInput =>
        platInput == 0
            ? ControllerInputPoller.instance.leftGrab
            : ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;

    private static bool RightInput =>
        platInput == 0
            ? ControllerInputPoller.instance.rightGrab
            : ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f;

    public static void Platforms()
    {
        HandlePlatform(ref leftPlat, ref lastLeftHeld, LeftInput, GorillaTagger.Instance.leftHandTransform);
        HandlePlatform(ref rightPlat, ref lastRightHeld, RightInput, GorillaTagger.Instance.rightHandTransform);

        if (platMode == 2)
        {
            bool holding = LeftInput || RightInput;

            if (holding != noclipActive)
            {
                noclipActive = holding;
                foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
                {
                    collider.enabled = !holding;
                }
            }
        }
        else if (noclipActive)
        {
            noclipActive = false;
            foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
            {
                collider.enabled = true;
            }
        }
    }

    private static void HandlePlatform(ref GameObject platform, ref bool lastHeld, bool held, Transform hand)
    {
        if (held && !lastHeld)
        {
            platform = CreatePlatform(hand);
        }

        if (held && platform != null)
        {
            ApplyMode(platform.GetComponent<Renderer>());
        }

        if (!held && lastHeld)
        {
            if (platform != null)
            {
                Object.Destroy(platform);
                platform = null;
            }
        }

        lastHeld = held;
    }

    private static GameObject CreatePlatform(Transform hand)
    {
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);

        platform.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
        platform.transform.SetPositionAndRotation(hand.position, hand.rotation);

        Renderer renderer = platform.GetComponent<Renderer>();
        renderer.material.shader = Shader.Find("Sprites/Default");

        ApplyMode(renderer);

        return platform;
    }

    private static void ApplyMode(Renderer renderer)
    {
        Color platformColor = MENUSETTINGS.Settings.backgroundColor.colors[0].color;

        switch (platMode)
        {
            case 0:
                renderer.enabled = true;
                renderer.material.color = platformColor;
                break;

            case 1:
                renderer.enabled = false;
                break;

            case 2:
                renderer.enabled = true;
                renderer.material.color = new Color(
                    platformColor.r,
                    platformColor.g,
                    platformColor.b,
                    0.35f
                );
                break;
        }
    }

    public static void SetPlatformMode(string mode)
    {
        platMode = PlatformMode.IndexOf(mode);

        if (platMode < 0)
            platMode = 0;

        if (leftPlat != null)
            ApplyMode(leftPlat.GetComponent<Renderer>());

        if (rightPlat != null)
            ApplyMode(rightPlat.GetComponent<Renderer>());

        NotificationLib.SendNotification(
            NotificationLib.NotificationType.Info,
            $"Mode: {PlatformMode[platMode]}"
        );
    }

    public static void PlatformDisable()
    {
        if (leftPlat != null)
        {
            Object.Destroy(leftPlat);
            leftPlat = null;
        }

        if (rightPlat != null)
        {
            Object.Destroy(rightPlat);
            rightPlat = null;
        }

        if (noclipActive)
        {
            noclipActive = false;
            foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
            {
                collider.enabled = true;
            }
        }

        lastLeftHeld = false;
        lastRightHeld = false;
    }

    public static void Fly()
    {
        if (InputHandler.Instance.RightPrimary.IsPressed)
        {
            GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * (Time.deltaTime * FlySpeed);
            GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
        }
    }

    public static void TPSTUMP()
    {
        Noclipistuff(true);
        GTPlayer.Instance.TeleportTo(new Vector3(-68.647f, 12.406f, -83.699f), GTPlayer.Instance.transform.rotation, false, true);
        GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
        Noclipistuff(false);
    }
    
    private static bool Ghost_Toggled = false;
    private static bool ghost_wasPressed = false;

    public static void GhostMonke()
    {
        bool isPressed = ControllerInputPoller.instance.rightControllerSecondaryButton;

        if (isPressed && !ghost_wasPressed)
        {
            Ghost_Toggled = !Ghost_Toggled;
            VRRig.LocalRig.enabled = !Ghost_Toggled;
        }

        ghost_wasPressed = isPressed;
    }

    private static bool Invis_Toggled = false;
    private static bool invis_wasPressed = false;

    public static void InvisMonke()
    {
        bool isPressed = ControllerInputPoller.instance.rightControllerPrimaryButton;

        if (isPressed && !invis_wasPressed)
            Invis_Toggled = !Invis_Toggled;

        invis_wasPressed = isPressed;

        if (Invis_Toggled)
        {
            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = new Vector3(0f, -100f, 0f);
        }
        else
        {
            VRRig.LocalRig.enabled = true;
        }
    }

    public static void NoClip()
    {
        bool DisableColliders = InputHandler.Instance.RightTrigger.IsPressed;
        MeshCollider[] colliders = Resources.FindObjectsOfTypeAll<MeshCollider>();

        foreach (MeshCollider collider in colliders)
        {
            collider.enabled = !DisableColliders;
        }
    }

    public static void Bouncy()
    {
        GorillaTagger.Instance.bodyCollider.material.bounciness = 1f;
        GorillaTagger.Instance.bodyCollider.material.bounceCombine = (PhysicsMaterialCombine)3;
        GorillaTagger.Instance.bodyCollider.material.dynamicFriction = 0f;
    }

    public static void ResetBouncy()
    {
        GorillaTagger.Instance.bodyCollider.material.bounciness = 0f;
        GorillaTagger.Instance.bodyCollider.material.bounceCombine = 0;
        GorillaTagger.Instance.bodyCollider.material.dynamicFriction = 0f;
    }

    public static void PbbvWalk()
    {
        GTPlayer.Instance.disableMovement = true;
    }
    public static void PbbvWalkDisable()
    {
        GTPlayer.Instance.disableMovement = false;
    }

    public static void AutoFunnyRun()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            float sinValue = MathF.Sin(-Time.time * 40f) * -0.3f;
            float cosValue = MathF.Cos(-Time.time * 40f) * -0.3f;
            GTPlayer.Instance.RightHand.controllerTransform.position =
                GTPlayer.Instance.bodyCollider.transform.position +
                GTPlayer.Instance.bodyCollider.transform.forward * cosValue +
                GTPlayer.Instance.bodyCollider.transform.up * sinValue;
            GTPlayer.Instance.LeftHand.controllerTransform.position =
                GTPlayer.Instance.bodyCollider.transform.position +
                GTPlayer.Instance.bodyCollider.transform.forward * -cosValue +
                GTPlayer.Instance.bodyCollider.transform.up * -sinValue;
        }
    }

    public static void AutoElevatorClimb()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            float sinValue = MathF.Sin(Time.frameCount / 2.5f) * 0.6f;
            float cosValue = MathF.Cos(Time.frameCount / 2.5f) * 0.3f;
            GTPlayer.Instance.RightHand.controllerTransform.position =
                GTPlayer.Instance.bodyCollider.transform.position +
                GTPlayer.Instance.bodyCollider.transform.right * (0.31f + cosValue) +
                GTPlayer.Instance.bodyCollider.transform.up * sinValue +
                GTPlayer.Instance.bodyCollider.transform.forward * 0.65f;
        }
    }

    public static GameObject checkpoint;

    public static void CheckPoint()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            if (checkpoint == null)
            {
                checkpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                UnityEngine.Object.Destroy(checkpoint.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(checkpoint.GetComponent<SphereCollider>());

                checkpoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }

            checkpoint.transform.position = GorillaTagger.Instance.rightHandTransform.position;
        }

        if (checkpoint != null)
        {
            if (InputHandler.Instance.RightPrimary.WasPressed)
            {
                Noclipistuff(true);
                checkpoint.GetComponent<Renderer>().material.color = Color.gray;

                Variables.TeleportPlayer(checkpoint.transform.position);
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
            else
            {
                checkpoint.GetComponent<Renderer>().material.color = Color.navyBlue;
                Noclipistuff(false);
            }
        }
    }


    private static bool wasPressed;

    public static void Reverse_velocity()
    {
        bool isPressed = ControllerInputPoller.instance.rightControllerPrimaryButton;

        if (isPressed && !wasPressed)
        {
            GorillaTagger.Instance.rigidbody.linearVelocity = -GorillaTagger.Instance.rigidbody.linearVelocity;
        }

        wasPressed = isPressed;
    }

    public static void GravityManager(Gravitytypes type)
    {
        switch (type)
        {
            case Gravitytypes.Low:
                GorillaTagger.Instance.rigidbody.AddForce(Vector3.up * 6.57f, ForceMode.Acceleration); 
                break;
            case Gravitytypes.High:
                GorillaTagger.Instance.rigidbody.AddForce(Vector3.down * 7.67f, ForceMode.Acceleration); // omg 67
                break;
            case Gravitytypes.Zero:
                GorillaTagger.Instance.rigidbody.AddForce( -Physics.gravity * (Time.deltaTime / Time.fixedDeltaTime), ForceMode.Acceleration);
                break;
            case Gravitytypes.Reverse:
                GorillaTagger.Instance.rigidbody.AddForce(-Physics.gravity * 3f, ForceMode.Acceleration);
                GTPlayer.Instance.GetControllerTransform(false).parent.rotation = Quaternion.Euler(180f, 0f, 0f); // I like the turning feature on the S menu so I added it
                break;
        }
    }

    public static void Reset_upsidedown() => GTPlayer.Instance.GetControllerTransform(false).parent.rotation = Quaternion.identity;
    
    public enum Gravitytypes
    {
        Low,
        High,
        Zero,
        Reverse
    }

    public static void CheckPointDisable()
    {
        if (checkpoint != null)
        {
            UnityEngine.Object.Destroy(checkpoint);
            checkpoint = null;
        }
    }

    public static void BarkFly()
    {
        Vector2 leftJoystick = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
        Vector2 rightJoystick = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
        Vector3 inputDirection = new Vector3(leftJoystick.x, rightJoystick.y, leftJoystick.y);

        Vector3 playerForward = GTPlayer.Instance.bodyCollider.transform.forward;
        playerForward.y = 0;
        Vector3 playerRight = GTPlayer.Instance.bodyCollider.transform.right;
        playerRight.y = 0;

        GTPlayer.Instance.GetComponent<Rigidbody>().AddForce(-Physics.gravity, ForceMode.Acceleration);

        Vector3 velocity = inputDirection.x * playerRight + inputDirection.y * Vector3.up + inputDirection.z * playerForward;
        velocity *= FlySpeed;
        GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.Lerp(GorillaTagger.Instance.rigidbody.linearVelocity, velocity, 0.12875f);
    }

    private static bool wasLeftTouching;
    private static bool wasRightTouching;

    public static float CurrentPower = 0.15f;

    public static void PullMod()
    {
        bool leftTouching = GTPlayer.Instance.IsHandTouching(true);
        bool rightTouching = GTPlayer.Instance.IsHandTouching(false);

        bool releasedWall =
            (!leftTouching && wasLeftTouching) ||
            (!rightTouching && wasRightTouching);

        if (releasedWall && InputHandler.Instance.RightPrimary.IsPressed)
        {
            Vector3 velocity = GorillaTagger.Instance.rigidbody.linearVelocity;
            Vector3 movement = new Vector3(velocity.x, 0f, velocity.z) * CurrentPower;

            GTPlayer.Instance.transform.position += movement;
        }

        wasLeftTouching = leftTouching;
        wasRightTouching = rightTouching;
    }

    public static void NoTagFreeze() =>
            GTPlayer.Instance.disableMovement = false;

    public static void WASDFly()
    {
        var kb = Keyboard.current;

        bool W = kb.wKey.isPressed;
        bool A = kb.aKey.isPressed;
        bool S = kb.sKey.isPressed;
        bool D = kb.dKey.isPressed;
        bool Space = kb.spaceKey.isPressed;
        bool Ctrl = kb.leftCtrlKey.isPressed;
        bool Shift = kb.leftShiftKey.isPressed;
        bool Alt = kb.leftAltKey.isPressed;

        bool LeftArrow = kb.leftArrowKey.isPressed;
        bool RightArrow = kb.rightArrowKey.isPressed;
        bool UpArrow = kb.upArrowKey.isPressed;
        bool DownArrow = kb.downArrowKey.isPressed;

        Transform parentTransform = GTPlayer.Instance.GetControllerTransform(false).parent;
        
        
        float turnSpeed = 250f;

        if (LeftArrow)
            parentTransform.eulerAngles += new Vector3(0, -turnSpeed, 0) * Time.deltaTime;
        if (RightArrow)
            parentTransform.eulerAngles += new Vector3(0, turnSpeed, 0) * Time.deltaTime;
        if (UpArrow)
            parentTransform.eulerAngles += new Vector3(-turnSpeed, 0, 0) * Time.deltaTime;
        if (DownArrow)
            parentTransform.eulerAngles += new Vector3(turnSpeed, 0, 0) * Time.deltaTime;

        if (Mouse.current.rightButton.isPressed)
        {
            Quaternion currentRotation = parentTransform.rotation;
            Vector3 euler = currentRotation.eulerAngles;

            if (startX < 0)
            {
                startX = euler.y;
                subThingy = Mouse.current.position.value.x / Screen.width;
            }
            if (startY < 0)
            {
                startY = euler.x;
                subThingyZ = Mouse.current.position.value.y / Screen.height;
            }

            float newX = startY - (Mouse.current.position.value.y / Screen.height - subThingyZ) * 360 * 1.33f;
            float newY = startX + (Mouse.current.position.value.x / Screen.width - subThingy) * 360 * 1.33f;

            newX = newX > 180f ? newX - 360f : newX;
            newX = Mathf.Clamp(newX, -90f, 90f);

            parentTransform.rotation = Quaternion.Euler(newX, newY, euler.z);
        }
        else
        {
            startX = -1;
            startY = -1;
        }

        float speed = FlySpeed;
        if (Shift) speed *= 2f;
        else if (Alt) speed /= 2f;

        Transform cam = parentTransform;

        if (W)
            GorillaTagger.Instance.rigidbody.transform.position += cam.forward * (Time.deltaTime * speed);

        if (S)
            GorillaTagger.Instance.rigidbody.transform.position += cam.forward * (-Time.deltaTime * speed);

        if (A)
            GorillaTagger.Instance.rigidbody.transform.position += cam.right * (-Time.deltaTime * speed);

        if (D)
            GorillaTagger.Instance.rigidbody.transform.position += cam.right * (Time.deltaTime * speed);

        if (Space)
            GorillaTagger.Instance.rigidbody.transform.position += Vector3.up * (Time.deltaTime * speed);
        

        if (Ctrl)
            GorillaTagger.Instance.rigidbody.transform.position += Vector3.down * (Time.deltaTime * speed);

        VRRig.LocalRig.head.rigTarget.transform.rotation =
            GorillaTagger.Instance.headCollider.transform.rotation;
    }

    public static void TeleportGun()
    {
        GunLib.start2guns(delegate ()
        {
            Vector3 targetPos = GunLib.GetPointerPos();

            Noclipistuff(true);

            GorillaLocomotion.GTPlayer.Instance.transform.position = targetPos;
            GorillaTagger.Instance.transform.position = targetPos;

            Noclipistuff(false);
        }, false);

        Noclipistuff(false);
    }

    public static void Noclipistuff(bool b)
    {
        foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
        {
            if (b)
            {
                collider.enabled = false;
            }
            else
            {
                collider.enabled = true;
            }
        }
    }
}