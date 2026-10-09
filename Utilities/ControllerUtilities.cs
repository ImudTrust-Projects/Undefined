using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

namespace Undefined.Utilities;

public static class ControllerUtilities
{
    public enum ControllerType
    {
        Unknown,
        Quest2,
        Quest3,
        ValveIndex,
        VIVE
    }

    public struct HandPose
    {
        public Vector3 position;
        public Quaternion rotation;

        public Vector3 up => rotation * Vector3.up;
        public Vector3 forward => rotation * Vector3.forward;
        public Vector3 right => rotation * Vector3.right;
    }

    private static readonly ControllerType[] cachedType = new ControllerType[2];
    private static readonly float[] checkedAt = { -10f, -10f };

    public static ControllerType GetControllerType(bool left)
    {
        int side = left ? 0 : 1;

        if (Time.time - checkedAt[side] < 1f)
            return cachedType[side];

        checkedAt[side] = Time.time;
        cachedType[side] = ReadControllerType(left);
        return cachedType[side];
    }

    public static ControllerType GetLeftControllerType() => GetControllerType(true);

    public static ControllerType GetRightControllerType() => GetControllerType(false);

    private static ControllerType ReadControllerType(bool left)
    {
        if (!XRSettings.isDeviceActive || ControllerInputPoller.instance == null)
            return ControllerType.Unknown;

        InputDevice device = left
            ? ControllerInputPoller.instance.leftControllerDevice
            : ControllerInputPoller.instance.rightControllerDevice;

        string name = (device.name ?? "").ToLowerInvariant();

        if (name.Contains("quest3"))
            return ControllerType.Quest3;

        if (name.Contains("quest2"))
            return ControllerType.Quest2;

        if (name.Contains("knuckles") || name.Contains("index"))
            return ControllerType.ValveIndex;

        if (name.Contains("vive"))
            return ControllerType.VIVE;

        return ControllerType.Unknown;
    }

    public static HandPose GetHand(bool left)
    {
        Transform controller = left ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;
        GTPlayer.HandState hand = left ? GTPlayer.Instance.LeftHand : GTPlayer.Instance.RightHand;

        return new HandPose
        {
            position = controller.position +controller.rotation * hand.handOffset * GTPlayer.Instance.scale,
            rotation = controller.rotation * hand.handRotOffset
        };
    }

    public static HandPose GetTrueLeftHand() => GetHand(true);

    public static HandPose GetTrueRightHand() => GetHand(false);
}
