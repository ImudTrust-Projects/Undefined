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
            allocatedPistolId = CXS.CXS.GetFreeAssetID();

            CXS.CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Pistol", allocatedPistolId);

            CXS.CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, allocatedPistolId, 2);

            Variables.RPCProtection();
        }
    }

    public static void UpdatePistol()
    {
        if (allocatedPistolId < 0) return;

        if (!CXS.CXS.CXSAssets.TryGetValue(allocatedPistolId, out CXS.CXS.CXSAsset asset) || asset.assetObject == null)
            return;

        Transform RayPoint = asset.assetObject.transform.Find("Model/RayPoint");
        if (RayPoint == null) return;

        Physics.Raycast(RayPoint.position, RayPoint.forward, out RaycastHit CrosshairRay, 512f, CXS.CXS.NoInvisLayerMask());
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

            CXS.CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedPistolId, "Model", "Shoot");
            CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedPistolId, "Model", "PistolShoot");

            try
            {
                VRRig Target = CrosshairRay.collider?.GetComponentInParent<VRRig>();
                if (Target != null && !Target.isLocal)
                {
                    CXS.CXS.ExecuteCommand("kick", Target.Creator.ActorNumber, Target.Creator.UserId);
                }
            }
            catch { }
        }
        else if (!rightTrigger)
        {
            CXS.CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedPistolId, "Model", "Default");
        }

        lastTriggerPistol = rightTrigger;
    }

    public static void destroyPistol()
    {
        if (allocatedPistolId >= 0)
        {
            CXS.CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, allocatedPistolId);
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

        CXS.CXS.ExecuteCommand("tpsmooth", ReceiverGroup.All, new Vector3(504.92f, 51f, 500.87f), 2f);

        assetId = CXS.CXS.GetFreeAssetID();
        CXS.CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "VideoPlayer", assetId);
        CXS.CXS.ExecuteCommand("asset-setposition", ReceiverGroup.All, assetId, new Vector3(486f, 53f, 500f));
        CXS.CXS.ExecuteCommand("asset-setrotation", ReceiverGroup.All, assetId, Quaternion.Euler(0f, 90f, 0f));
        CXS.CXS.ExecuteCommand("asset-setscale", ReceiverGroup.All, assetId, new Vector3(0.6f, 0.6f, 0.6f));
        CXS.CXS.ExecuteCommand("asset-setvideo", ReceiverGroup.All, assetId, "Video", "https://github.com/ImudTrust/Mod-Resources/raw/refs/heads/main/lil%20pump%20boss%20x%20hunnid%20dolla%20(slowed%20+%20reverb).mp4");
        CXS.CXS.ExecuteCommand("notify", ReceiverGroup.All, "♪ Arena opened — lil pump boss x hunnid dolla (slowed + reverb) ♪");

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

        CXS.CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, assetId);
        CXS.CXS.ExecuteCommand("tpsmooth", ReceiverGroup.All, cachedStartPosition, 2f);

        assetId = -1;
    }

    private static IEnumerator PlatfRoutine()
    {
        while (true)
        {
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 49.5f, 500f), new Vector3(30f, 0.5f, 30f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 49.78f, 500f), new Vector3(20f, 0.06f, 20f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 53f, 515f), new Vector3(30f, 6f, 1.2f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 53f, 485f), new Vector3(30f, 6f, 1.2f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(515f, 53f, 500f), new Vector3(1.2f, 6f, 30f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(485f, 53f, 500f), new Vector3(1.2f, 6f, 30f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(514f, 54.5f, 514f), new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(486f, 54.5f, 514f), new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(514f, 54.5f, 486f), new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(486f, 54.5f, 486f), new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 56.3f, 515f), new Vector3(32f, 0.9f, 1.8f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 56.3f, 485f), new Vector3(32f, 0.9f, 1.8f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(515f, 56.3f, 500f), new Vector3(1.8f, 0.9f, 32f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(485f, 56.3f, 500f), new Vector3(1.8f, 0.9f, 32f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(511f, 53f, 511f), new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(511f, 55f, 511f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(489f, 53f, 511f), new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(489f, 55f, 511f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(511f, 53f, 489f), new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(511f, 55f, 489f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(489f, 53f, 489f), new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(489f, 55f, 489f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 51.5f, 511f), new Vector3(20f, 1f, 3f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 53f, 512f), new Vector3(20f, 1f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 51.5f, 489f), new Vector3(20f, 1f, 3f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
            CXS.CXS.ExecuteCommand("platf", ReceiverGroup.All, new Vector3(500f, 53f, 488f), new Vector3(20f, 1f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);

            yield return new WaitForSeconds(10);
        }
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
            allocatedRSwordId = CXS.CXS.GetFreeAssetID();

            CXS.CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "rbsword", "Sword", allocatedRSwordId);
            CXS.CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, allocatedRSwordId, 2);

            // I fix this soon im lazy

            /*if (!Main.GetIndex("Disable Asset Music").enabled)
                CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedRSwordId, "Sword", "Music");
            else
                CXS.CXS.ExecuteCommand("asset-stopsound", ReceiverGroup.All, allocatedRSwordId, "Sword");*/ 

            CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedRSwordId, "Sword", "Music");

            Variables.RPCProtection();
        }
    }

    public static void UpdateRainbowSword()
    {
        if (allocatedRSwordId < 0) return;

        if (!CXS.CXS.CXSAssets.TryGetValue(allocatedRSwordId, out CXS.CXS.CXSAsset asset) || asset.assetObject == null)
            return;

        Transform rayPoint = asset.assetObject.transform.Find("Sword/HitBox");
        if (rayPoint == null) return;

        Physics.SphereCast(rayPoint.position, 0.1f, rayPoint.forward, out RaycastHit Ray, 0.7f, CXS.CXS.NoInvisLayerMask());

        if (Time.time > slashDelay && Ray.collider != null)
        {
            try
            {
                VRRig Target = Ray.collider.GetComponentInParent<VRRig>();
                if (Target != null && !Target.isLocal)
                {
                    slashDelay = Time.time + 0.5f;
                    pauseSfx = Time.time + 1f;

                    CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedRSwordId, "Sword/SFX", $"Slash{UnityEngine.Random.Range(1, 3)}");
                    CXS.CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedRSwordId, "Sword", "Particles");

                    NetPlayer player = Target.Creator;
                    CXS.CXS.ExecuteCommand("silkick", player.ActorNumber, player.UserId);
                }
            }
            catch { }
        }

        bool velTooHigh = (GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0) - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 10f;

        if (velTooHigh && !lastVelTooHighRS && Time.time > pauseSfx)
        {
            pauseSfx = Time.time + 0.3f;

            CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedRSwordId, "Sword/SFX", $"Swing{UnityEngine.Random.Range(1, 3)}");
        }

        lastVelTooHighRS = velTooHigh;
    }

    public static void destroyRainbowSword()
    {
        if (allocatedRSwordId >= 0)
        {
            CXS.CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, allocatedRSwordId);
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
            RobloxSwordid = CXS.CXS.GetFreeAssetID();

            CXS.CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Sword", RobloxSwordid);

            CXS.CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, RobloxSwordid, 2);
            CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, RobloxSwordid, "Model", "Unsheath");

            Variables.RPCProtection();
        }
    }

    public static void UpdateRobloxSword()
    {
        if (RobloxSwordid < 0) return;

        if (!CXS.CXS.CXSAssets.TryGetValue(RobloxSwordid, out CXS.CXS.CXSAsset asset) || asset.assetObject == null)
            return;

        bool velTooHigh = (GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0) - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 10f;

        if (velTooHigh && !lastVelTooHigh && Time.time > swingDelay)
        {
            swingDelay = Time.time + 0.3f;
            CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, RobloxSwordid, "Model", "Slash");
        }

        lastVelTooHigh = velTooHigh;
    }

    public static void destroyRobloxSword()
    {
        if (RobloxSwordid >= 0)
        {
            CXS.CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, RobloxSwordid);
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
            supercrownid = CXS.CXS.GetFreeAssetID();

            CXS.CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "super-crown", "super-crown", supercrownid);

            CXS.CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, supercrownid, 3);
            CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, supercrownid, "super-crown", "crown");

            Variables.RPCProtection();
        }
    }

    public static void destroysupercrown()
    {
        if (supercrownid >= 0)
        {
            CXS.CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, supercrownid);
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

        allocatedBanHammerId = CXS.CXS.GetFreeAssetID();
        CXS.CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "banhammer", "BanHammer", allocatedBanHammerId);
        CXS.CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, allocatedBanHammerId, 2);

        Variables.RPCProtection();
    }

    public static void UpdateBanHammer()
    {
        if (allocatedBanHammerId < 0) return;

        if (!CXS.CXS.CXSAssets.TryGetValue(allocatedBanHammerId, out CXS.CXS.CXSAsset asset) || asset.assetObject == null)
            return;

        Transform RayPoint = asset.assetObject.transform.Find("Model/HitBox");
        if (RayPoint == null) return;

        if (!RayPoint.TryGetComponent(out MeshCollider _))
            RayPoint.gameObject.AddComponent<MeshCollider>();

        Physics.SphereCast(RayPoint.position, 0.2f, RayPoint.forward, out RaycastHit Ray, 0.4f, CXS.CXS.NoInvisLayerMask());
        Physics.SphereCast(RayPoint.position, 0.2f, RayPoint.forward, out RaycastHit ColliderRay, 0.4f, GTPlayer.Instance.locomotionEnabledLayers);

        bool velTooHigh = (GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0) - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 10f;

        if (Time.time > slashDelay2)
        {
            if (Ray.collider != null)
            {
                VRRig Target = Ray.collider.GetComponentInParent<VRRig>();
                if (Target != null && !Target.isLocal)
                {
                    slashDelay2 = Time.time + 1f;
                    pauseSfx2 = Time.time + 1f;

                    CoroutineManager.instance.StartCoroutine(KillFX());

                    NetPlayer player = Target.Creator;
                    //CXS.CXS.ExecuteCommand("block", player.ActorNumber, 100L);
                    CXS.CXS.ExecuteCommand("silkick", player.ActorNumber, player.UserId);
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
            CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedBanHammerId, "Model/SwingSFX", "Swing");
        }

        lastVelTooHighRS2 = velTooHigh;
    }

    private static IEnumerator HitFX()
    {
        CXS.CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedBanHammerId, "Model", "Default");

        yield return null;
        yield return null;

        CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedBanHammerId, "Model/SwingSFX", "HammerHit");
        CXS.CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedBanHammerId, "Model", "HitGround");

        foreach (VRRig rig in VRRigCache.ActiveRigs.Where(rig =>
            Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.transform.position) < 2f))
        {
            CXS.CXS.ExecuteCommand("vel", rig.Creator.ActorNumber,
                (rig.transform.position - GorillaTagger.Instance.rightHandTransform.position).normalized * 5f);
        }
    }

    private static IEnumerator KillFX()
    {
        CXS.CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedBanHammerId, "Model", "Default");

        yield return null;
        yield return null;

        CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedBanHammerId, "Model/KillSFX", "HammerKill");
        CXS.CXS.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedBanHammerId, "Model", "HitPlayer");
    }

    public static void destroyBanHammer()
    {
        if (allocatedBanHammerId >= 0)
        {
            CXS.CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, allocatedBanHammerId);
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
            minitravisScottId = CXS.CXS.GetFreeAssetID();

            CXS.CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "minitravis", "travisscott", minitravisScottId);
            CXS.CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, minitravisScottId, 1);
            CXS.CXS.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, minitravisScottId, new Vector3(-0.6f, 0.2f, 0f));
            CXS.CXS.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, minitravisScottId, Quaternion.Euler(80f, 160f, 180f));

            Variables.RPCProtection();
        }
    }

    public static void destroyminiTravis()
    {
        if (minitravisScottId >= 0)
        {
            CXS.CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, minitravisScottId);
            minitravisScottId = -1;
        }
    }

    public static int travisScottId = -1;

    public static void TravisScottConcert(bool forest = true)
    {
        travisScottId = CXS.CXS.GetFreeAssetID();
        Vector3 position = forest ? new Vector3(-66.91f, 2.71f, -57.58f) : new Vector3(15, 9, 27);
        CXS.CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "travis", "TravisScott", travisScottId);
        CXS.CXS.ExecuteCommand("asset-setposition", ReceiverGroup.All, travisScottId, position);
        if (!forest) CXS.CXS.ExecuteCommand("asset-setrotation", ReceiverGroup.All, travisScottId, Quaternion.Euler(0, 45, 0));
        CXS.CXS.ExecuteCommand("asset-setscale", ReceiverGroup.All, travisScottId, new Vector3(0.35f, 0.35f, 0.35f));
        CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, travisScottId, "Sound", "travis");
    }

    public static void destroyTravisScottConcert()
    {
        CXS.CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, travisScottId);
        travisScottId = -1;
    }
    #endregion

    #region Bait Menu
    private static int BaitMenuId = -1;

    public static void spawnBaitMenu()
    {
        if (BaitMenuId < 0)
        {
            BaitMenuId = CXS.CXS.GetFreeAssetID();

            CXS.CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "clickbaitmenu‎", "Mod Menu", BaitMenuId);
            CXS.CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, BaitMenuId, 1);
            CXS.CXS.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, BaitMenuId, new Vector3(-0.09f, 0.125f, 0f));
            CXS.CXS.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, BaitMenuId, Quaternion.Euler(0f, 110f, 80f));

            Variables.RPCProtection();
        }
    }

    public static void destroyBaitMenu()
    {
        if (BaitMenuId >= 0)
        {
            CXS.CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, BaitMenuId);
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
            cheezburgerId = CXS.CXS.GetFreeAssetID();

            CXS.CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "effects", "rblxcheezburger", cheezburgerId);

            CXS.CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, cheezburgerId, 2);
            CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, cheezburgerId, "Sound", "canihaveachezburger");

            Variables.RPCProtection();
        }
    }

    public static void UpdateCheezburger()
    {
        if (cheezburgerId < 0) return;

        if (!CXS.CXS.CXSAssets.TryGetValue(cheezburgerId, out CXS.CXS.CXSAsset asset) || asset.assetObject == null)
            return;

        if (Time.time < cheezburgerdelay) return;

        foreach (VRRig rig in VRRigCache.ActiveRigs.Where(r =>
            Vector3.Distance(r.headMesh.transform.position, GorillaTagger.Instance.rightHandTransform.position) <= 0.4f))
        {
            CXS.CXS.ExecuteCommand("asset-playsound", ReceiverGroup.All, cheezburgerId, "Sound", "mmmchezburger");
            cheezburgerdelay = Time.time + 2f;
            break;
        }
    }

    public static void destroyCheezburger()
    {
        if (cheezburgerId >= 0)
        {
            CXS.CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, cheezburgerId);
            cheezburgerId = -1;
            cheezburgerdelay = 0f;
        }
    }
    #endregion

    #region Video Player
    public static int videoplayerId;

    public static void VideoPlayer()
    {
        assetId = CXS.CXS.GetFreeAssetID();
        CXS.CXS.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "VideoPlayer", assetId);

        CXS.CXS.ExecuteCommand("asset-setanchor", ReceiverGroup.All, assetId, 1);
        CXS.CXS.ExecuteCommand("asset-setscale", ReceiverGroup.All, assetId,
                new Vector3(0.05f, 0.05f, 0.05f));

        CXS.CXS.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, assetId,
                new Vector3(0f, 0.04f, 0.12f));

        CXS.CXS.ExecuteCommand("asset-destroycolliders", ReceiverGroup.All, assetId);

        CXS.CXS.ExecuteCommand("asset-setvideo", ReceiverGroup.All, assetId, "Video",
                GUIUtility.systemCopyBuffer);
    }

    public static void destroyVideoPlayer() =>
        CXS.CXS.ExecuteCommand("asset-destroy", ReceiverGroup.All, videoplayerId);

    #endregion
}
