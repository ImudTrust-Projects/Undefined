using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;
using System;
using Undefined.Mods.Categories;

namespace Undefined.Patches;

public class Torso
{
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.PostTick))]
    public class TorsoPatch
    {
        private static Quaternion frozenRotation;
        private static bool hasFrozenRotation = false;
        public static event Action VRRigLateUpdate;
        public static bool enabled;
        public static int mode = 0;
        private static float storedTorsoYaw;
        private static bool hasStoredYaw = false;
        private static Quaternion? storedJoystickRotation = null;
        private static Quaternion storedCase5Rotation;
        private static bool hasStoredCase5 = false;

        public static void Postfix(VRRig __instance)
        {
            if (!__instance.enabled)
            {
                return;
            }

            if (__instance.isLocal)
            {
                if (enabled)
                {
                    Quaternion rotation = Quaternion.identity;
                    switch (mode)
                    {
                        case 0:
                            rotation = Quaternion.Euler(0f, Time.time * 180f % 360, 0f);
                            break;
                        case 1:
                            rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
                            break;
                        case 2:
                            rotation = Quaternion.Euler(0f, GorillaTagger.Instance.headCollider.transform.rotation.eulerAngles.y + 180f, 0f);
                            break;
                        case 3:
                            rotation = Quaternion.Euler(0f, Rig.recBodyRotary.transform.rotation.eulerAngles.y, 0f);
                            break;
                        case 4:
                            if (!hasFrozenRotation)
                            {
                                frozenRotation = __instance.transform.rotation;
                                hasFrozenRotation = true;
                            }

                            rotation = frozenRotation;
                            break;
                        case 5:
                            {
                                Transform a = GTPlayer.Instance.LeftHand.controllerTransform;
                                Transform b = GTPlayer.Instance.RightHand.controllerTransform;

                                Vector3 pos = __instance.transform.position;

                                Vector3 dirA = a.position - pos;
                                Vector3 dirB = b.position - pos;

                                dirA.y = 0f;
                                dirB.y = 0f;

                                dirA.Normalize();
                                dirB.Normalize();

                                Vector3 currentForward = __instance.transform.forward;
                                currentForward.y = 0f;
                                currentForward.Normalize();

                                float dot = Vector3.Dot(dirA, dirB);

                                Vector3 blendedDir;

                                if (dot < -0.5f)
                                {
                                    float dotA = Vector3.Dot(currentForward, dirA);
                                    float dotB = Vector3.Dot(currentForward, dirB);

                                    blendedDir = dotA > dotB ? dirA : dirB;
                                }
                                else
                                {
                                    blendedDir = dirA + dirB;
                                }

                                Quaternion targetRotation = __instance.transform.rotation;
                                if (blendedDir.sqrMagnitude > 0.0001f)
                                {
                                    blendedDir.Normalize();

                                    float angle = Vector3.SignedAngle(currentForward, blendedDir, Vector3.up);

                                    float maxAngle = 100f;
                                    float clampedAngle = Mathf.Clamp(angle, -maxAngle, maxAngle);

                                    Quaternion clampedRot = Quaternion.AngleAxis(clampedAngle, Vector3.up);
                                    Vector3 finalForward = clampedRot * currentForward;

                                    targetRotation = Quaternion.LookRotation(finalForward, Vector3.up);
                                }

                                if (!hasStoredCase5)
                                {
                                    storedCase5Rotation = targetRotation;
                                    hasStoredCase5 = true;
                                }

                                storedCase5Rotation = Quaternion.Slerp(storedCase5Rotation, targetRotation, 15f * Time.deltaTime);
                                rotation = storedCase5Rotation;

                                break;
                            }
                        case 6:
                            {
                                float deadzone = 20f;
                                float baseSpeed = 150f;
                                float maxSpeed = int.MaxValue;

                                float headYaw = GorillaTagger.Instance.headCollider.transform.rotation.eulerAngles.y;

                                Transform lefthand = GTPlayer.Instance.LeftHand.controllerTransform;
                                Transform righthand = GTPlayer.Instance.RightHand.controllerTransform;
                                Vector3 pos = __instance.transform.position;

                                Vector3 dirA = lefthand.position - pos;
                                Vector3 dirB = righthand.position - pos;

                                dirA.y = 0f;
                                dirB.y = 0f;

                                float handYaw = headYaw;

                                if (dirA.sqrMagnitude > 0.001f && dirB.sqrMagnitude > 0.001f)
                                {
                                    dirA.Normalize();
                                    dirB.Normalize();

                                    Vector3 blended = dirA + dirB;

                                    if (blended.sqrMagnitude > 0.001f)
                                    {
                                        blended.Normalize();
                                        handYaw = Quaternion.LookRotation(blended, Vector3.up).eulerAngles.y;
                                    }
                                }

                                float handWeight = 0.2f;
                                float targetYaw = Mathf.LerpAngle(headYaw, handYaw, handWeight);

                                if (!hasStoredYaw)
                                {
                                    storedTorsoYaw = targetYaw;
                                    hasStoredYaw = true;
                                }

                                float delta = Mathf.DeltaAngle(storedTorsoYaw, targetYaw);
                                float absDelta = Mathf.Abs(delta);

                                if (absDelta > deadzone)
                                {
                                    float excess = absDelta - deadzone;
                                    float speed = Mathf.Min(maxSpeed, baseSpeed * (excess / 30f));
                                    storedTorsoYaw += Mathf.Sign(delta) * speed * Time.deltaTime;
                                }

                                float headPitch = GorillaTagger.Instance.headCollider.transform.rotation.eulerAngles.x;
                                if (headPitch > 180f) headPitch -= 360f;

                                Vector3 dirA_vertical = lefthand.position - pos;
                                Vector3 dirB_vertical = righthand.position - pos;

                                float handPitchA = Mathf.Atan2(dirA_vertical.y, Mathf.Sqrt(dirA_vertical.x * dirA_vertical.x + dirA_vertical.z * dirA_vertical.z)) * Mathf.Rad2Deg;
                                float handPitchB = Mathf.Atan2(dirB_vertical.y, Mathf.Sqrt(dirB_vertical.x * dirB_vertical.x + dirB_vertical.z * dirB_vertical.z)) * Mathf.Rad2Deg;

                                float handPitch = -(handPitchA + handPitchB) * 0.5f;

                                float handPitchWeight = 0.2f;
                                float targetPitch = Mathf.LerpAngle(headPitch, handPitch, handPitchWeight);

                                float finalPitch = Mathf.Clamp(targetPitch, -50f, 50f);

                                finalPitch -= 25f;

                                float headRoll = GorillaTagger.Instance.headCollider.transform.rotation.eulerAngles.z;
                                if (headRoll > 180f) headRoll -= 360f;

                                Vector3 handDifference = righthand.position - lefthand.position;
                                float handRoll = Mathf.Atan2(handDifference.y, Mathf.Sqrt(handDifference.x * handDifference.x + handDifference.z * handDifference.z)) * Mathf.Rad2Deg;

                                float handRollWeight = 0.50f;
                                float targetRoll = Mathf.LerpAngle(headRoll, handRoll, handRollWeight);

                                float finalRoll = Mathf.Clamp(targetRoll, -30f, 30f);

                                rotation = Quaternion.Euler(finalPitch, storedTorsoYaw, finalRoll);
                                break;
                            }
                        case 7:
                            Quaternion headRotation = GorillaTagger.Instance.headCollider.transform.rotation;
                            rotation = headRotation * Quaternion.Euler(90f, 0f, 0f);
                            break;
                        case 8:
                            Quaternion headRotation2 = GorillaTagger.Instance.headCollider.transform.rotation;
                            rotation = headRotation2 * Quaternion.Euler(-90f, 0f, 0f);
                            break;
                        case 9:
                            {
                                Vector2 joyl = ControllerInputPoller.instance.leftControllerPrimary2DAxis;
                                Vector2 joyr = ControllerInputPoller.instance.rightControllerPrimary2DAxis;

                                float yawInput = 0f;
                                float pitchInput = 0f;
                                bool hasInput = false;

                                if (Mathf.Abs(joyl.x) > 0.5f)
                                {
                                    yawInput = joyl.x;
                                    hasInput = true;
                                }

                                if (Mathf.Abs(joyr.y) > 0.5f)
                                {
                                    pitchInput = joyr.y;
                                    hasInput = true;
                                }

                                if (hasInput)
                                {
                                    if (!storedJoystickRotation.HasValue)
                                    {
                                        storedJoystickRotation = GorillaTagger.Instance.headCollider.transform.rotation;
                                    }

                                    float yaw = yawInput * 200f * Time.deltaTime;
                                    float pitch = -pitchInput * 200f * Time.deltaTime;

                                    Quaternion currentRot = storedJoystickRotation.Value;

                                    currentRot = Quaternion.AngleAxis(yaw, Vector3.up) * currentRot;
                                    Vector3 localRight = currentRot * Vector3.right;
                                    currentRot = Quaternion.AngleAxis(pitch, localRight) * currentRot;

                                    currentRot.Normalize();
                                    storedJoystickRotation = currentRot;
                                }
                                else if (!storedJoystickRotation.HasValue)
                                {
                                    storedJoystickRotation = GorillaTagger.Instance.headCollider.transform.rotation;
                                }

                                rotation = storedJoystickRotation.Value;
                                break;
                            }
                    }


                    if (mode != 4)
                    {
                        hasFrozenRotation = false;
                    }
                    if (mode != 5)
                    {
                        hasStoredCase5 = false;
                    }
                    if (mode != 6)
                    {
                        hasStoredYaw = false;
                    }
                    if (mode != 9)
                    {
                        storedJoystickRotation = null;
                    }

                    __instance.transform.rotation = rotation;

                    if (GTPlayer.Instance != null && GTPlayer.Instance.bodyCollider != null)
                    {
                        if (mode >= 6 && mode <= 9)
                        {
                            GTPlayer.Instance.bodyCollider.transform.rotation = rotation;
                        }
                    }

                    __instance.head.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);
                    __instance.leftHand.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);
                    __instance.rightHand.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);
                }

                VRRigLateUpdate?.Invoke();
            }
        }
    }
}