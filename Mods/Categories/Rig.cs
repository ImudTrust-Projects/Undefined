using Undefined.Patches;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Rig
{
    public static GameObject recBodyRotary;
    
    [Utilities.Tooltip("Spazes ur head when u hold right grip.")]
    public static void SpazHead()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x += Random.Range(1f, 360f);
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y += Random.Range(1f, 360f);
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z += Random.Range(1f, 360f);
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x = 0f;
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = 0f;
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 0f;
        }
    }
    
    [Utilities.Tooltip("Spins ur head X.")]
    public static void SpinHeadX()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x += Random.Range(1f, 360f);
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x = 0f;
        }
    }
    [Utilities.Tooltip("Spins ur head Y.")]
    public static void SpinHeadY()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y += Random.Range(1f, 360f);
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = 0f;
        }
    }
    [Utilities.Tooltip("Spins ur head Z.")]
    public static void SpinHeadZ()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z += Random.Range(1f, 360f);
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 0f;
        }
    }
    [Utilities.Tooltip("Makes u an helicopter.")]
    public static void HelicopterRig()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            GorillaTagger.Instance.offlineVRRig.enabled = false;

            GorillaTagger.Instance.offlineVRRig.transform.position += new Vector3(0f, 0.05f, 0f);


            GorillaTagger.Instance.offlineVRRig.transform.rotation = Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));


            GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;

            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * -1f;
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * 1f;

            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;

            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation *= Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.leftHand.trackingRotationOffset);
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation *= Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.rightHand.trackingRotationOffset);
        }
        else
        {
            GorillaTagger.Instance.offlineVRRig.enabled = true;
        }
    }
    
    public static void GrabRig()
    {
        if (InputHandler.Instance.RightGrip.IsPressed)
        {
            VRRig.LocalRig.enabled = false; 
            VRRig.LocalRig.transform.position = GorillaTagger.Instance.rightHandTransform.position;
        }
        else
        {
            VRRig.LocalRig.enabled = true;
        }
    }
    
    public static void MoveRigGun()
    {
        GorillaTagger.Instance.offlineVRRig.enabled = true;

        GunLib.StartGun(() =>
        {
            GorillaTagger.Instance.offlineVRRig.enabled = false;
            GorillaTagger.Instance.offlineVRRig.transform.position = GunLib.GetPointerPos() + Vector3.up * 1f;
        }, false);
    }
    
    public static void UpsideDownHead()
    {
        GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 180f;
    }

    public static void BackwardsHead()
    {
        GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = 180f;
    }

    public static void ResetHead()
    {
        GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset = Vector3.zero;
    }

    private static Vector3 rigOffset;

    private static bool HoldRig()
    {
        VRRig rig = GorillaTagger.Instance.offlineVRRig;

        if (!InputHandler.Instance.RightGrip.IsPressed)
        {
            rig.enabled = true;
            return false;
        }

        if (rig.enabled)
        {
            rigOffset = rig.transform.position - GorillaTagger.Instance.bodyCollider.transform.position;
            rig.enabled = false;
        }

        rig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + rigOffset;
        rig.transform.rotation = Quaternion.Euler(0f, GorillaTagger.Instance.headCollider.transform.eulerAngles.y, 0f);
        rig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
        return true;
    }

    private static void SetHands(Vector3 left, Vector3 right)
    {
        VRRig rig = GorillaTagger.Instance.offlineVRRig;

        rig.leftHand.rigTarget.transform.position = left;
        rig.rightHand.rigTarget.transform.position = right;

        rig.leftHand.rigTarget.transform.rotation = rig.transform.rotation * Quaternion.Euler(rig.leftHand.trackingRotationOffset);
        rig.rightHand.rigTarget.transform.rotation = rig.transform.rotation * Quaternion.Euler(rig.rightHand.trackingRotationOffset);
    }

    public static void FlapArms()
    {
        if (!HoldRig())
            return;

        Transform rig = GorillaTagger.Instance.offlineVRRig.transform;
        Vector3 flap = Vector3.up * (Mathf.Sin(Time.time * 14f) * 0.35f);

        SetHands(rig.position - rig.right * 0.55f + flap, rig.position + rig.right * 0.55f + flap);
    }

    public static void Clap()
    {
        if (!HoldRig())
            return;

        Transform rig = GorillaTagger.Instance.offlineVRRig.transform;
        Vector3 front = rig.position + rig.forward * 0.35f + Vector3.up * 0.1f;
        float gap = 0.05f + Mathf.Abs(Mathf.Sin(Time.time * 10f)) * 0.35f;

        SetHands(front - rig.right * gap, front + rig.right * gap);
    }

    public static void Wave()
    {
        if (!HoldRig())
            return;

        Transform rig = GorillaTagger.Instance.offlineVRRig.transform;
        Vector3 left = rig.position - rig.right * 0.3f - Vector3.up * 0.4f;
        Vector3 right = rig.position + rig.right * (0.35f + Mathf.Sin(Time.time * 10f) * 0.15f) + Vector3.up * 0.45f;

        SetHands(left, right);
    }


    public static void ResetRig()
    {
        GorillaTagger.Instance.offlineVRRig.enabled = true;
    }
    
    private static bool Ghost_Toggled = false;
    private static bool Invis_Toggled = false;

    public static void GhostMonke()
    {
        bool isPressed = Variables.rightHanded
            ? InputHandler.Instance.LeftSecondary.WasPressed
            : InputHandler.Instance.RightSecondary.WasPressed;

        if (isPressed)
        {
            Ghost_Toggled = !Ghost_Toggled;
            VRRig.LocalRig.enabled = !Ghost_Toggled;
        }
    }

    public static void InvisMonke()
    {
        if (InputHandler.Instance.RightPrimary.WasPressed)
            Invis_Toggled = !Invis_Toggled;

        if (Invis_Toggled)
        {
            VRRig.LocalRig.enabled = false;
            Variables.bypasstp(new Vector3(0f, -100f, 0f), true);
        }
        else
        {
            VRRig.LocalRig.enabled = true;
        }
    }
    
    public static void AscendMonke()
    {
        var rig = GorillaTagger.Instance.offlineVRRig;

        if (rig == null)
            return;

        bool active = InputHandler.Instance.RightPrimary.IsPressed;
        rig.enabled = !active;

        if (!active)
            return;

        var transform = rig.transform;

        transform.position += Vector3.up * 0.01f;
        rig.head.rigTarget.rotation = transform.rotation;
        rig.leftHand.rigTarget.position = transform.position - transform.right;
        rig.rightHand.rigTarget.position = transform.position + transform.right;
        rig.head.trackingRotationOffset = new Vector3(
            rig.head.trackingRotationOffset.x,
            rig.head.trackingRotationOffset.y,
            180f
        );
    }
    
    public static void SetBodyPatch(bool enabled, int mode = 0)
    {
        Torso.TorsoPatch.enabled = enabled;
        Torso.TorsoPatch.mode = mode;

        if (!enabled && recBodyRotary != null)
        {
            UnityEngine.Object.Destroy(recBodyRotary);
            recBodyRotary = null;
        }
    }

    private static void UpdateRecBodyRotary()
    {
        if (recBodyRotary == null)
            recBodyRotary = new GameObject("recBodyRotary");

        recBodyRotary.transform.rotation = Quaternion.Lerp(recBodyRotary.transform.rotation,
            Quaternion.Euler(0f, GorillaTagger.Instance.headCollider.transform.rotation.eulerAngles.y, 0f),
            Time.deltaTime * 6.5f);
    }
    
    public static void RecRoomTorso()
    {
        SetBodyPatch(true, 5);
        UpdateRecBodyRotary();
    }

    public static void RecRoomRig()
    {
        SetBodyPatch(true, 3);
        UpdateRecBodyRotary();
    }

    public static void FullBodyTracking()
    {
        SetBodyPatch(true, 6);
        UpdateRecBodyRotary();
    }

    public static void Spider()
    {
        SetBodyPatch(true, 7);
        UpdateRecBodyRotary();
    }

    public static void InverseSpider()
    {
        SetBodyPatch(true, 8);
        UpdateRecBodyRotary();
    }

    public static void JoystickRot()
    {
        SetBodyPatch(true, 9);
        UpdateRecBodyRotary();
    }
    
    public static void DisableRecRoomBody() => SetBodyPatch(false);
}