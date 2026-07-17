using BepInEx;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using GorillaTagScripts;
using Undefined.Utilities;
using UnityEngine;
using static Undefined.Utilities.GunLib;

namespace Undefined.Mods.Categories;

public class Overpowered
{
    public static HitTargetNetworkState[] tagetcache;
    public static void SpazTargets()
    {
        if (tagetcache == null)
        {
            tagetcache = Resources.FindObjectsOfTypeAll<HitTargetNetworkState>();
        }
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (HitTargetNetworkState item in tagetcache)
            {
                item.hitCooldownTime = 0;
                item.TargetHit(Vector3.zero, Vector3.zero);
            }
        }
    }
    
    public static void BreakTargets()
    {
        if (tagetcache == null)
        {
            tagetcache = Resources.FindObjectsOfTypeAll<HitTargetNetworkState>();
        }
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (HitTargetNetworkState item in tagetcache)
            {
                PhotonNetwork.Destroy(item.GetView);
            }
        }
    }
    
    public static void UntagSelf()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
            gorillaTagManager.currentInfected.Remove(PhotonNetwork.LocalPlayer);
        }
    }

    public static void UntagAll()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
                gorillaTagManager.currentInfected.Remove(player);
            }
        }
    }
    
    
    public static void ForceTagLag()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
            gorillaTagManager.tagCoolDown = 200000;
        }
    }

    public static void NoTagCooldown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
            gorillaTagManager.tagCoolDown = 0;
        }
    }

    public static void BreakElevator()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveInstantiatedGO(GRElevatorManager._instance.gameObject, false); 
        }
    }
    
    public static void DestroyAll()
    {
        PhotonNetwork.OpRemoveCompleteCache();
    }
    
    public static float hoverboarddelay = 0;
    public static void HoverboardMinigun()
    {
        if (ControllerInputPoller.instance.rightGrab)
        {
            if (hoverboarddelay < Time.time)
            {
                FreeHoverboardManager.instance.SendDropBoardRPC(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, GorillaTagger.Instance.rightHandTransform.forward * 30f, Vector3.zero, new Color(0, 0, 0));
                hoverboarddelay = Time.time + 0.5f;
            }
        }
        if (ControllerInputPoller.instance.leftGrab)
        {
            if (hoverboarddelay < Time.time)
            {
                FreeHoverboardManager.instance.SendDropBoardRPC(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.rotation, GorillaTagger.Instance.leftHandTransform.forward * 30f, Vector3.zero, new Color(0, 0, 0));
                hoverboarddelay = Time.time + 0.5f;
            }
        }
    }
    
    private static float waterdelay;
    public static void Watersplash()
    {
        if (Time.time > waterdelay)
        {
            waterdelay = Time.time + 0.1f;

            if (PhotonNetwork.InRoom)
            {
                if (ControllerInputPoller.instance.rightGrab)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, 100f, 100f, true, false);
                    Variables.RPCProtection();
                }
                if (ControllerInputPoller.instance.leftGrab)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.rotation, 100f, 100f, true, false);
                    Variables.RPCProtection();
                }
            }
        }
    }
    
    public static void Watergun()
    {
        start2guns(delegate ()
        {
            if (PhotonNetwork.InRoom)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = LockedRigOrPlayerOrwhatever.transform.position - new Vector3(0f, 1.9f, 0f);
                if (Time.time > waterdelay)
                {
                    waterdelay = Time.time + 0.3f;
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, LockedRigOrPlayerOrwhatever.transform.position, LockedRigOrPlayerOrwhatever.transform.rotation, 100f, 100f, true, false);
                    Variables.RPCProtection();
                }
            }
        }, true);
        VRRig.LocalRig.enabled = LockedRigOrPlayerOrwhatever == null;
    }
    
    private static float LagDelay;
    public static void StutterMaster()
    {
        if (Time.time > LagDelay)
        {
            var whackamole = GameObject.FindObjectOfType<WhackAMole>().GetView;
            LagDelay = Time.time + 11f;
            for (int i = 0; i < 3850; i++)
            {
                whackamole.RPC("WhackAMoleButtonPressed", RpcTarget.MasterClient, null);
            }
        }
        Variables.RPCProtection();
    }
    
    
    
    
    
}
