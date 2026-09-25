using Undefined.Admin.Menu;
using GorillaLocomotion;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Undefined.Menu;
using Undefined.Utilities;
using UnityEngine;
using static Bindings;
using static UnityEngine.GridBrushBase;
using CXS = Undefined.Admin.Menu.CXS;

namespace Undefined.Mods.Categories;

public class ConsoleAssets
{
    #region Pistol
    private static int allocatedPistolId = -1;
    private static bool lastTriggerPistol;
    private static float shootCooldown;

    public static void spawnPistol()
    {
        if (allocatedPistolId < 0)
        {
            allocatedPistolId = CXS.GetFreeAssetID();

            CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Pistol", allocatedPistolId);

            CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, allocatedPistolId, 2);

            Variables.RPCProtection();
        }
    }

    public static void UpdatePistol()
    {
        if (allocatedPistolId < 0) return;

        if (!CXS.CXSAssets.TryGetValue(allocatedPistolId, out CXS.CXSAsset asset) || asset.assetObject == null)
            return;

        Transform RayPoint = asset.assetObject.transform.Find("Model/RayPoint");
        if (RayPoint == null) return;

        Physics.Raycast(RayPoint.position, RayPoint.forward, out RaycastHit CrosshairRay, 512f, CXS.NoInvisLayerMask());
        GameObject Crosshair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Crosshair.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
        Crosshair.transform.position = CrosshairRay.point == Vector3.zero ? (RayPoint.position + (RayPoint.forward * 20f)) : CrosshairRay.point;
        Crosshair.GetComponent<Renderer>().material.color = MENUSETTINGS.Settings.backgroundColor.colors[0].color;
        UnityEngine.Object.Destroy(Crosshair.GetComponent<Collider>());
        UnityEngine.Object.Destroy(Crosshair, Time.deltaTime);

        bool rightTrigger = InputHandler.Instance.RightTrigger.WasPressed;

        if (rightTrigger && !lastTriggerPistol && Time.time > shootCooldown)
        {
            shootCooldown = Time.time + 0.3f;

            CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedPistolId, "Model", "Shoot");
            CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedPistolId, "Model", "PistolShoot");

            try
            {
                VRRig Target = CrosshairRay.collider?.GetComponentInParent<VRRig>();
                if (Target != null && !Target.isOfflineVRRig)
                {
                    CXS.ExecuteCommand("kick", Target.Creator.ActorNumber, Target.Creator.UserId);
                }
            }
            catch { }
        }
        else if (!rightTrigger)
        {
            CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedPistolId, "Model", "Default");
        }

        lastTriggerPistol = rightTrigger;
    }

    public static void destroyPistol()
    {
        if (allocatedPistolId >= 0)
        {
            CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, allocatedPistolId);
            allocatedPistolId = -1;
            lastTriggerPistol = false;
            shootCooldown = 0f;
        }
    }
    #endregion

    #region Battle Arena
    private static int assetId;
    private static Vector3 cachedStartPosition;
    private static Coroutine platfRoutine;

    public static void spawnBattleArena()
    {
        if (assetId != 0) return;

        cachedStartPosition = GorillaTagger.Instance.bodyCollider.transform.position;

        platfRoutine = CoroutineManager.instance.StartCoroutine(PlatfRoutine());

        CXS.ExecuteCommand("tpsmooth", ReceiverGroup.All, new Vector3(504.92f, 51f, 500.87f), 2f);

        assetId = CXS.GetFreeAssetID();
        CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "VideoPlayer", assetId);
        CXS.ExecuteCommand("asset-setposition", ReceiverGroup.All, assetId, new Vector3(486f, 53f, 500f));
        CXS.ExecuteCommand("asset-setrotation", ReceiverGroup.All, assetId, Quaternion.Euler(0f, 90f, 0f));
        CXS.ExecuteCommand("asset-setscale", ReceiverGroup.All, assetId, new Vector3(0.6f, 0.6f, 0.6f));
        CXS.ExecuteCommand("asset-setvideo", ReceiverGroup.All, assetId, "Video", "https://github.com/ImudTrust/Mod-Resources/raw/refs/heads/main/lil%20pump%20boss%20x%20hunnid%20dolla%20(slowed%20+%20reverb).mp4");
        CXS.ExecuteCommand("notify", ReceiverGroup.All, "♪ Arena opened — lil pump boss x hunnid dolla (slowed + reverb) ♪");

        Variables.RPCProtection();
    }

    public static void destroyBattleArena()
    {
        if (assetId < 0) return;

        if (platfRoutine != null)
        {
            CoroutineManager.instance.StopCoroutine(platfRoutine);
            platfRoutine = null;
        }

        CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, assetId);
        CXS.ExecuteCommand("tpsmooth", ReceiverGroup.All, cachedStartPosition, 2f);

        assetId = -1;
    }

    private static IEnumerator PlatfRoutine()
    {
        while (true)
        {
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 49.5f, 500f), new Vector3(30f, 0.5f, 30f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 49.78f, 500f), new Vector3(20f, 0.06f, 20f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 53f, 515f), new Vector3(30f, 6f, 1.2f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 53f, 485f), new Vector3(30f, 6f, 1.2f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(515f, 53f, 500f), new Vector3(1.2f, 6f, 30f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(485f, 53f, 500f), new Vector3(1.2f, 6f, 30f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(514f, 54.5f, 514f), new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(486f, 54.5f, 514f), new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(514f, 54.5f, 486f), new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(486f, 54.5f, 486f), new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 56.3f, 515f), new Vector3(32f, 0.9f, 1.8f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 56.3f, 485f), new Vector3(32f, 0.9f, 1.8f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(515f, 56.3f, 500f), new Vector3(1.8f, 0.9f, 32f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(485f, 56.3f, 500f), new Vector3(1.8f, 0.9f, 32f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(511f, 53f, 511f), new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(511f, 55f, 511f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(489f, 53f, 511f), new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(489f, 55f, 511f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(511f, 53f, 489f), new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(511f, 55f, 489f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(489f, 53f, 489f), new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(489f, 55f, 489f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 51.5f, 511f), new Vector3(20f, 1f, 3f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 53f, 512f), new Vector3(20f, 1f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 51.5f, 489f), new Vector3(20f, 1f, 3f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 53f, 488f), new Vector3(20f, 1f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);

            yield return new WaitForSeconds(10);
        }
    }
    #endregion

    #region GorillaTV
    private static int GorillaTVAssetID;

    public static void GorillaTv()
    {
        if (GorillaTVAssetID != 0) return;

        GorillaTVAssetID = CXS.GetFreeAssetID();

        CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "gorillatv", "TV", GorillaTVAssetID);

        CXS.ExecuteCommand("asset-setposition", ReceiverGroup.All, GorillaTVAssetID,
            new Vector3(-57.1f, 5.6f, -37f));

        CXS.ExecuteCommand("asset-setrotation", ReceiverGroup.All, GorillaTVAssetID,
            Quaternion.Euler(270f, 0f, 0f));

        CXS.ExecuteCommand("asset-setvideo", ReceiverGroup.All, GorillaTVAssetID, nameof(VideoPlayer),
            GUIUtility.systemCopyBuffer);

        Variables.RPCProtection();
    }

    public static void DestroyGorillaTv()
    {
        if (GorillaTVAssetID == 0) return;

        CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, GorillaTVAssetID);

        GorillaTVAssetID = 0;
    }
    #endregion

    #region Rainbow Sword Asset
    public static int allocatedRSwordId = -1;
    private static bool lastVelTooHighRS;
    private static float pauseSfx;
    private static float slashDelay;

    public static void spawnRainbowSword()
    {
        if (allocatedRSwordId < 0)
        {
            allocatedRSwordId = CXS.GetFreeAssetID();

            CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "rbsword", "Sword", allocatedRSwordId);
            CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, allocatedRSwordId, 2);

            // I finally fixed it, chat.

            if (!ModButtons.IsEnabled("Disable Asset Music").enabled)
                CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedRSwordId, "Sword", "Music");
            else
                CXS.ExecuteCommand("asset-stopsound", ReceiverGroup.All, allocatedRSwordId, "Sword");

            Variables.RPCProtection();
        }
    }

    public static void UpdateRainbowSword()
    {
        if (allocatedRSwordId < 0) return;

        if (!CXS.CXSAssets.TryGetValue(allocatedRSwordId, out CXS.CXSAsset asset) || asset.assetObject == null)
            return;

        Transform rayPoint = asset.assetObject.transform.Find("Sword/HitBox");
        if (rayPoint == null) return;

        Physics.SphereCast(rayPoint.position, 0.1f, rayPoint.forward, out RaycastHit Ray, 0.7f, CXS.NoInvisLayerMask());

        if (Time.time > slashDelay && Ray.collider != null)
        {
            try
            {
                VRRig Target = Ray.collider.GetComponentInParent<VRRig>();
                if (Target != null && !Target.isOfflineVRRig)
                {
                    slashDelay = Time.time + 0.5f;
                    pauseSfx = Time.time + 1f;

                    CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedRSwordId, "Sword/SFX", $"Slash{UnityEngine.Random.Range(1, 3)}");
                    CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedRSwordId, "Sword", "Particles");

                    NetPlayer player = Target.Creator;
                    CXS.ExecuteCommand("silkick", player.ActorNumber, player.UserId);
                }
            }
            catch { }
        }

        bool velTooHigh = (GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0) - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 10f;

        if (velTooHigh && !lastVelTooHighRS && Time.time > pauseSfx)
        {
            pauseSfx = Time.time + 0.3f;

            CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedRSwordId, "Sword/SFX", $"Swing{UnityEngine.Random.Range(1, 3)}");
        }

        lastVelTooHighRS = velTooHigh;
    }

    public static void destroyRainbowSword()
    {
        if (allocatedRSwordId >= 0)
        {
            CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, allocatedRSwordId);
            allocatedRSwordId = -1;
            lastVelTooHighRS = false;
            pauseSfx = 0f;
            slashDelay = 0f;
        }
    }
    #endregion

    #region Roblox Sword
    private static int RobloxSwordid = -1;
    private static bool lastVelTooHigh;
    private static float swingDelay;

    public static void spawnRobloxSword()
    {
        if (RobloxSwordid < 0)
        {
            RobloxSwordid = CXS.GetFreeAssetID();

            CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Sword", RobloxSwordid);

            CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, RobloxSwordid, 2);
            CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, RobloxSwordid, "Model", "Unsheath");

            Variables.RPCProtection();
        }
    }

    public static void UpdateRobloxSword()
    {
        if (RobloxSwordid < 0) return;

        if (!CXS.CXSAssets.TryGetValue(RobloxSwordid, out CXS.CXSAsset asset) || asset.assetObject == null)
            return;

        bool velTooHigh = (GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0) - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 10f;

        if (velTooHigh && !lastVelTooHigh && Time.time > swingDelay)
        {
            swingDelay = Time.time + 0.3f;
            CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, RobloxSwordid, "Model", "Slash");
        }

        lastVelTooHigh = velTooHigh;
    }

    public static void destroyRobloxSword()
    {
        if (RobloxSwordid >= 0)
        {
            CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, RobloxSwordid);
            RobloxSwordid = -1;
            lastVelTooHigh = false;
            swingDelay = 0f;
        }
    }
    #endregion

    #region super-crown
    private static int supercrownid = -1;

    public static void supercrown()
    {
        if (supercrownid < 0)
        {
            supercrownid = CXS.GetFreeAssetID();

            CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "super-crown", "super-crown", supercrownid);

            CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, supercrownid, 3);
            CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, supercrownid, "super-crown", "crown");

            Variables.RPCProtection();
        }
    }

    public static void destroysupercrown()
    {
        if (supercrownid >= 0)
        {
            CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, supercrownid);
            supercrownid = -1;
        }
    }
    #endregion

    #region Ban Hammer
    private static int allocatedBanHammerId = -1;
    private static bool lastVelTooHighRS2;
    private static float pauseSfx2;
    private static float slashDelay2;

    public static void spawnBanHammer()
    {
        if (allocatedBanHammerId >= 0)
            return;

        allocatedBanHammerId = CXS.GetFreeAssetID();
        CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "banhammer", "BanHammer", allocatedBanHammerId);
        CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, allocatedBanHammerId, 2);

        Variables.RPCProtection();
    }

    public static void UpdateBanHammer()
    {
        if (allocatedBanHammerId < 0) return;

        if (!CXS.CXSAssets.TryGetValue(allocatedBanHammerId, out CXS.CXSAsset asset) || asset.assetObject == null)
            return;

        Transform RayPoint = asset.assetObject.transform.Find("Model/HitBox");
        if (RayPoint == null) return;

        if (!RayPoint.TryGetComponent(out MeshCollider _))
            RayPoint.gameObject.AddComponent<MeshCollider>();

        Physics.SphereCast(RayPoint.position, 0.2f, RayPoint.forward, out RaycastHit Ray, 0.4f, CXS.NoInvisLayerMask());
        Physics.SphereCast(RayPoint.position, 0.2f, RayPoint.forward, out RaycastHit ColliderRay, 0.4f, GTPlayer.Instance.locomotionEnabledLayers);

        bool velTooHigh = (GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0) - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 10f;

        if (Time.time > slashDelay2)
        {
            if (Ray.collider != null)
            {
                VRRig Target = Ray.collider.GetComponentInParent<VRRig>();
                if (Target != null && !Target.isOfflineVRRig)
                {
                    slashDelay2 = Time.time + 1f;
                    pauseSfx2 = Time.time + 1f;

                    CoroutineManager.instance.StartCoroutine(KillFX());

                    NetPlayer player = Target.Creator;
                    //CXS.ExecuteCommand("block", player.ActorNumber, 100L);
                    CXS.ExecuteCommand("silkick", player.ActorNumber, player.UserId);
                }
            }

            if (ColliderRay.collider != null)
            {
                slashDelay2 = Time.time + 0.3f;
                pauseSfx2 = Time.time + 0.5f;

                Vector3 surfaceNormal = ColliderRay.normal;
                Vector3 handVelocity = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
                Vector3 bodyVelocity = GorillaTagger.Instance.rigidbody.linearVelocity;
                float totalVelocity = handVelocity.magnitude + bodyVelocity.magnitude;
                float pushStrength = Mathf.Clamp(totalVelocity, 1f, 14f);
                GorillaTagger.Instance.rigidbody.linearVelocity += surfaceNormal * pushStrength;

                CoroutineManager.instance.StartCoroutine(HitFX());
            }
        }

        if (velTooHigh && !lastVelTooHighRS2 && Time.time > pauseSfx2)
        {
            pauseSfx2 = Time.time + 0.3f;
            CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedBanHammerId, "Model/SwingSFX", "Swing");
        }

        lastVelTooHighRS2 = velTooHigh;
    }

    private static IEnumerator HitFX()
    {
        CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedBanHammerId, "Model", "Default");

        yield return null;
        yield return null;

        CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedBanHammerId, "Model/SwingSFX", "HammerHit");
        CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedBanHammerId, "Model", "HitGround");

        foreach (VRRig rig in VRRigCache.ActiveRigs.Where(rig =>
            Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.transform.position) < 2f))
        {
            CXS.ExecuteCommand("vel", rig.Creator.ActorNumber,
                (rig.transform.position - GorillaTagger.Instance.rightHandTransform.position).normalized * 5f);
        }
    }

    private static IEnumerator KillFX()
    {
        CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedBanHammerId, "Model", "Default");

        yield return null;
        yield return null;

        CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedBanHammerId, "Model/KillSFX", "HammerKill");
        CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedBanHammerId, "Model", "HitPlayer");
    }

    public static void destroyBanHammer()
    {
        if (allocatedBanHammerId >= 0)
        {
            CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, allocatedBanHammerId);
            allocatedBanHammerId = -1;
            lastVelTooHighRS2 = false;
            pauseSfx2 = 0f;
            slashDelay2 = 0f;
        }
    }
    #endregion

    #region Concerts
    private static int minitravisScottId = -1;

    public static void spawnMiniTravis()
    {
        if (minitravisScottId < 0)
        {
            minitravisScottId = CXS.GetFreeAssetID();

            CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "minitravis", "travisscott", minitravisScottId);
            CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, minitravisScottId, 1);
            CXS.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, minitravisScottId, new Vector3(-0.6f, 0.2f, 0f));
            CXS.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, minitravisScottId, Quaternion.Euler(80f, 160f, 180f));

            Variables.RPCProtection();
        }
    }

    public static void destroyminiTravis()
    {
        if (minitravisScottId >= 0)
        {
            CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, minitravisScottId);
            minitravisScottId = -1;
        }
    }

    public static int travisScottId = -1;

    public static void TravisScottConcert(bool forest = true)
    {
        travisScottId = CXS.GetFreeAssetID();
        Vector3 position = forest ? new Vector3(-66.91f, 2.71f, -57.58f) : new Vector3(15, 9, 27);
        CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "travis", "TravisScott", travisScottId);
        CXS.ExecuteCommand("asset-setposition", ReceiverGroup.All, travisScottId, position);
        if (!forest) CXS.ExecuteCommand("asset-setrotation", ReceiverGroup.All, travisScottId, Quaternion.Euler(0, 45, 0));
        CXS.ExecuteCommand("asset-setscale", ReceiverGroup.All, travisScottId, new Vector3(0.35f, 0.35f, 0.35f));
        CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, travisScottId, "Sound", "travis");
    }

    public static void destroyTravisScottConcert()
    {
        CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, travisScottId);
        travisScottId = -1;
    }
    #endregion

    #region Bait Menu
    private static int BaitMenuId = -1;

    public static void spawnBaitMenu()
    {
        if (BaitMenuId < 0)
        {
            BaitMenuId = CXS.GetFreeAssetID();

            CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "clickbaitmenu‎", "Mod Menu", BaitMenuId);
            CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, BaitMenuId, 1);
            CXS.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, BaitMenuId, new Vector3(-0.09f, 0.125f, 0f));
            CXS.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, BaitMenuId, Quaternion.Euler(0f, 110f, 80f));

            Variables.RPCProtection();
        }
    }

    public static void destroyBaitMenu()
    {
        if (BaitMenuId >= 0)
        {
            CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, BaitMenuId);
            BaitMenuId = -1;
        }
    }
    #endregion

    #region cheezburger
    public static int cheezburgerId = -1;
    private static float cheezburgerdelay;

    public static void spawnCheezburger()
    {
        if (cheezburgerId < 0)
        {
            cheezburgerId = CXS.GetFreeAssetID();

            CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "effects", "rblxcheezburger", cheezburgerId);

            CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, cheezburgerId, 2);
            CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, cheezburgerId, "Sound", "canihaveachezburger");

            Variables.RPCProtection();
        }
    }

    public static void UpdateCheezburger()
    {
        if (cheezburgerId < 0) return;

        if (!CXS.CXSAssets.TryGetValue(cheezburgerId, out CXS.CXSAsset asset) || asset.assetObject == null)
            return;

        if (Time.time < cheezburgerdelay) return;

        foreach (VRRig rig in VRRigCache.ActiveRigs.Where(r =>
            Vector3.Distance(r.headMesh.transform.position, GorillaTagger.Instance.rightHandTransform.position) <= 0.4f))
        {
            CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, cheezburgerId, "Sound", "mmmchezburger");
            cheezburgerdelay = Time.time + 2f;
            break;
        }
    }

    public static void destroyCheezburger()
    {
        if (cheezburgerId >= 0)
        {
            CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, cheezburgerId);
            cheezburgerId = -1;
            cheezburgerdelay = 0f;
        }
    }
    #endregion

    #region Video Player
    public static int videoplayerId;

    public static void VideoPlayer()
    {
        assetId = CXS.GetFreeAssetID();
        CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "VideoPlayer", assetId);

        CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, assetId, 1);
        CXS.ExecuteCommand("asset-setscale", ReceiverGroup.All, assetId,
                new Vector3(0.05f, 0.05f, 0.05f));

        CXS.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, assetId,
                new Vector3(0f, 0.04f, 0.12f));

        CXS.ExecuteCommand("asset-destroycolliders", ReceiverGroup.All, assetId);

        CXS.ExecuteCommand("asset-setvideo", ReceiverGroup.All, assetId, "Video",
                GUIUtility.systemCopyBuffer);
    }

    public static void destroyVideoPlayer() =>
        CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, videoplayerId);
    #endregion

    #region TikTok Videos
    public static List<string> tiktokVideos = new List<string>
    {
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/#australia #highschool #school #students #funny_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/#bulun_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/#fyp #tiktok #skit #comedy #funny_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/10 October 2025 (1)_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/10 October 2025_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/ACTUAL VIDEO VS BEHIND THE SCENES! - #shorts_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/AI Marketing Tools With No Restrictions_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/African parents be like 😡😡_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/COMMENT FOR 7 YEARS OF GOOD LUCK! 🍀😅 - #dance #funny #couple #shorts IB@Zarathebanana_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Can you do this (1)_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Can you do this_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/DON’T CHECK SOUND BRO! (1)_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/DON’T CHECK SOUND BRO!_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/DON’T CLICK THE SOUND 💀_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Don't Check The Sound.. ⚠️😞_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/HOW FAST CAN I INSTALL MODS FOR GORILLA TAG ⁉️_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/He found something very cute #shorts_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/His Positive Attitude Brightens Everyone’s Day…❤️👏_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Hopefully we’re not TOO strict😭💀 @Prymrr #kanebailey #prymrr #kaneandprymrr_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/How to Fly in Gorilla Tag.. sorta_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/I Bought the CHEAPEST $1 SLIMES! 🤑😱  Unboxing & Haul_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/I Cooked A Pizza With Power Tools_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/I found a secret in Yatagarasu..._rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/I hope she had THE BEST DAY #explore #teacherlife #fyp #teacher_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/It was on beat too 😭💀 #basketball_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Just Use game mechanics  brutal 😭_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Kids can now design their own 3D Games!_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/October 6 2025_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Outsmarted 😂_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Ranking Best Whirlpool Filter Moments_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Ranking the Funniest Useless Car Features 🚗😂_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/She fixes roads now... #shorts #shortsfeed #youtubeshorts #cringe #thecleangirl #comedy #funny_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Spiderman Destroyed Him 😂   The Amazing Spiderman   #shorts_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Stages of 99 Nights in The Forest Players fr #shorts #viral_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Stop saying ✨6 7✨ (1)_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Stop saying ✨6 7✨_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/The Best Drive Thru_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/The MOST CREATIVE Marketing Ever!🤯📈   Milka’s Last Square_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/The PERFECT Burger BUN ‼️😂 #TheManniiShow.com series_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/The opposites 🤍 #shorts_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/This GRANDPA is an AMAZING gymnast! #interestingfacts (1)_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/This GRANDPA is an AMAZING gymnast! #interestingfacts_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/This Is The LUCKIEST Cat 🍀🐈‍⬛ #shorts (1)_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Tired Girl Packs Soap Fast_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/WE CAN’T BELIEVE WE JUST HIT 23M FAMILY MEMBERS! 🥹😭🥰 (1)_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/WE CAN’T BELIEVE WE JUST HIT 23M FAMILY MEMBERS! 🥹😭🥰_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Watch what happens.. It was a trap 🪤 😅 #viral youtuber #viral #funny_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Worlds Fastest PITSTOP! (@nocontroleracing)_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/You always Know 😂_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/pov you hand animated a lion in 1 day #blender3d #vfx_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/좋은 것만 주고 싶어🥰_rotated.mp4",
        "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/📶 HOW TO LAG IN MONKE BLOCKS⁉️ #gorillatag #vr #gtag #gtagmods #monke_rotated.mp4"
    };
    #endregion

    #region TikTok iPhone Variables
    private static Dictionary<int, int> allocatediPhoneTikTok = new Dictionary<int, int>();
    private static Dictionary<int, int> currentVideoDict = new Dictionary<int, int>();
    private static Dictionary<int, bool> phonePausedDict = new Dictionary<int, bool>();
    private static Dictionary<int, bool> lastTriggerDict = new Dictionary<int, bool>();
    private static Dictionary<int, bool> lastGripDict = new Dictionary<int, bool>();
    private static Dictionary<int, bool> lastPrimaryDict = new Dictionary<int, bool>();
    private static bool tiktokInit = false;
    #endregion

    #region TikTok iPhone Methods
    public static void iPhoneTikTok(VRRig rig)
    {
        int actorNum = rig.OwningNetPlayer.ActorNumber;

        if (!tiktokInit)
        {
            int n = tiktokVideos.Count;
            System.Random rng = new System.Random();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (tiktokVideos[k], tiktokVideos[n]) = (tiktokVideos[n], tiktokVideos[k]);
            }
            tiktokInit = true;
        }

        if (!allocatediPhoneTikTok.ContainsKey(actorNum)) allocatediPhoneTikTok[actorNum] = -1;
        if (!currentVideoDict.ContainsKey(actorNum)) currentVideoDict[actorNum] = 0;
        if (!phonePausedDict.ContainsKey(actorNum)) phonePausedDict[actorNum] = false;
        if (!lastTriggerDict.ContainsKey(actorNum)) lastTriggerDict[actorNum] = false;
        if (!lastGripDict.ContainsKey(actorNum)) lastGripDict[actorNum] = false;
        if (!lastPrimaryDict.ContainsKey(actorNum)) lastPrimaryDict[actorNum] = false;

        int iPhoneId = allocatediPhoneTikTok[actorNum];
        int currentVideo = currentVideoDict[actorNum];
        bool phonePaused = phonePausedDict[actorNum];
        bool lastTrigger = lastTriggerDict[actorNum];
        bool lastGrip = lastGripDict[actorNum];
        bool lastPrimary = lastPrimaryDict[actorNum];

        if (iPhoneId < 0)
        {
            iPhoneId = CXS.GetFreeAssetID();
            allocatediPhoneTikTok[actorNum] = iPhoneId;

            CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "iphone", "iPhone", iPhoneId);
            CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, iPhoneId, 1, actorNum);

            string initialVideo = phonePaused
                ? "https://github.com/josephabyt/Videos/raw/refs/heads/main/blank.mp4"
                : tiktokVideos[currentVideo];

            CXS.ExecuteCommand("asset-setvideo", ReceiverGroup.All, iPhoneId, "Model/Video", initialVideo);
            Variables.RPCProtection();
        }

        float lTrigger = rig.leftIndex.calcT;
        bool lGrab = rig.leftMiddle.calcT > 0.25f;
        bool lPrimary = rig.leftThumb.calcT > 0.25f;

        if (phonePaused)
        {
            lastTrigger = lTrigger > 0.5f;
            lastGrip = lGrab;
        }

        if (lTrigger > 0.5f && !lastTrigger)
        {
            currentVideo--;
            if (currentVideo < 0) currentVideo = tiktokVideos.Count - 1;
            CXS.ExecuteCommand("asset-setvideo", ReceiverGroup.All, iPhoneId, "Model/Video", tiktokVideos[currentVideo]);
            Variables.RPCProtection();
        }

        if (lGrab && !lastGrip)
        {
            currentVideo++;
            currentVideo %= tiktokVideos.Count;
            CXS.ExecuteCommand("asset-setvideo", ReceiverGroup.All, iPhoneId, "Model/Video", tiktokVideos[currentVideo]);
            Variables.RPCProtection();
        }

        if (lPrimary && !lastPrimary)
        {
            phonePaused = !phonePaused;
            string videoUrl = phonePaused
                ? "https://github.com/josephabyt/Videos/raw/refs/heads/main/blank.mp4"
                : tiktokVideos[currentVideo];
            CXS.ExecuteCommand("asset-setvideo", ReceiverGroup.All, iPhoneId, "Model/Video", videoUrl);
            Variables.RPCProtection();
        }

        currentVideoDict[actorNum] = currentVideo;
        phonePausedDict[actorNum] = phonePaused;
        lastTriggerDict[actorNum] = lTrigger > 0.5f;
        lastGripDict[actorNum] = lGrab;
        lastPrimaryDict[actorNum] = lPrimary;
    }

    public static void destroyiPhoneTikTok(VRRig rig)
    {
        int actorNum = rig.OwningNetPlayer.ActorNumber;
        if (!allocatediPhoneTikTok.ContainsKey(actorNum)) return;

        int iPhoneId = allocatediPhoneTikTok[actorNum];
        CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, iPhoneId);
        allocatediPhoneTikTok[actorNum] = -1;
    }
    #endregion

    #region Cherry Bomb
    public static int allocatedId = -1;
    private static float timeSinceSpawn;
    private static bool thing;

    public static void CherryBomb()
    {
        if (allocatedId < 0)
        {
            allocatedId = CXS.GetFreeAssetID();

            CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "cherrybomb", "beam", allocatedId);
            CXS.ExecuteCommand("asset-setposition", ReceiverGroup.All, allocatedId, GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 9.5f, 0f) + (GorillaTagger.Instance.bodyCollider.transform.forward * -0.25f));
            CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedId, "beam", "cherrybomb");

            Variables.RPCProtection();

            timeSinceSpawn = Time.time + 3.66f;
        }
    }

    public static void UpdateCherryBomb()
    {
        if (allocatedId < 0) return;

        if (Time.time > timeSinceSpawn)
        {
            if (!thing)
            {
                thing = true;
                CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedId, "beam", "show");
            }

            if (CXS.CXSAssets.TryGetValue(allocatedId, out CXS.CXSAsset asset) && asset.assetObject != null)
            {
                Variables.TeleportPlayer(Vector3.Lerp(GorillaTagger.Instance.bodyCollider.transform.position, asset.assetObject.transform.position + new Vector3(0f, -2f + Mathf.Sin(Time.time * 5f) * 1.25f, 0f), 0.01f));
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            }
        }
    }

    public static void destroyCherryBomb()
    {
        if (allocatedId >= 0)
        {
            CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, allocatedId);
            allocatedId = -1;
            timeSinceSpawn = -1;
            thing = false;
        }
    }
    #endregion

    #region boombox
    private static Dictionary<int, int> allocatedBoombox = new Dictionary<int, int>();
    private static Dictionary<int, float> networkDelayByBoombox = new Dictionary<int, float>();
    private static Dictionary<int, Vector3> scaleNetworkedByBoombox = new Dictionary<int, Vector3>();

    public static void Boombox(VRRig rig)
    {
        int actorNum = rig.OwningNetPlayer.ActorNumber;
        int boomboxId = allocatedBoombox.ContainsKey(actorNum) ? allocatedBoombox[actorNum] : -1;

        if (boomboxId < 0)
        {
            boomboxId = CXS.GetFreeAssetID();
            allocatedBoombox[actorNum] = boomboxId;

            CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Boombox", boomboxId);
            CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, boomboxId, 1, actorNum);
            CXS.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, boomboxId, new Vector3(0f, 0f, 0.15f));
            CXS.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, boomboxId, Quaternion.Euler(0f, 90f, 90f));
            CXS.ExecuteCommand("asset-setsound", ReceiverGroup.All, boomboxId, "Model", GUIUtility.systemCopyBuffer);
            CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, boomboxId, "Model");

            Variables.RPCProtection();

            networkDelayByBoombox[boomboxId] = 0f;
            scaleNetworkedByBoombox[boomboxId] = Vector3.one;
        }

        if (!CXS.CXSAssets.ContainsKey(boomboxId)) return;

        GameObject target = CXS.CXSAssets[boomboxId].assetObject;
        if (target == null) return;

        AudioSource audioSource = target.transform.Find("Model")?.GetComponent<AudioSource>();
        if (audioSource != null && audioSource.isPlaying)
        {
            float[] samples = new float[1024];
            audioSource.GetOutputData(samples, 0);

            float currentEnergy = 0f;
            for (int i = 0; i < samples.Length; i++)
                currentEnergy += samples[i] * samples[i];

            currentEnergy = Mathf.Sqrt(currentEnergy / samples.Length);

            if (Time.time > networkDelayByBoombox[boomboxId])
            {
                float scale = 1f + (currentEnergy / 0.1f) * 0.25f;
                Vector3 newScale = Vector3.one * scale;

                if (scaleNetworkedByBoombox[boomboxId] != newScale)
                {
                    scaleNetworkedByBoombox[boomboxId] = newScale;
                    networkDelayByBoombox[boomboxId] = Time.time + 0.05f;
                    CXS.ExecuteCommand("asset-setscale", ReceiverGroup.All, boomboxId, newScale);
                }
            }
        }
    }

    public static void destroyBoombox(VRRig rig)
    {
        int actorNum = rig.OwningNetPlayer.ActorNumber;
        if (!allocatedBoombox.ContainsKey(actorNum)) return;

        int boomboxId = allocatedBoombox[actorNum];
        if (boomboxId != -1)
            CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, boomboxId);

        allocatedBoombox[actorNum] = -1;
        networkDelayByBoombox[boomboxId] = 0f;
        scaleNetworkedByBoombox[boomboxId] = Vector3.one;
    }
    #endregion
}