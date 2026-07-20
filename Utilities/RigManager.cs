using GorillaGameModes;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Undefined.Utilities;

public class RigManager
{

    public static VRRig GetVRRigFromNetPlayer(NetPlayer netPlayer)
    {
        if (netPlayer == null)
            return null;

        return GorillaGameManager.StaticFindRigForPlayer(netPlayer);
    }
    
    public static Player GetPlayerFromRig(VRRig rig)
    {
        return rig.OwningNetPlayer.GetPlayerRef();
    }

    public static VRRig GetRigFromPlayer(Player p) =>
            GorillaGameManager.instance.FindPlayerVRRig(p);

    public static VRRig GetVRRigFromPlayer(Player p) =>
        GorillaGameManager.instance.FindPlayerVRRig(p);

    public static NetPlayer GetNetPlayerFromVRRig(VRRig vrrig)
    {
        return vrrig.Creator ?? vrrig.OwningNetPlayer ?? NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(((Component)vrrig.rigSerializer).gameObject));
    }

    public static VRRig GetRandomVRRig(bool includeSelf)
    {
        VRRig random = VRRigCache.ActiveRigs[Random.Range(0, VRRigCache.ActiveRigs.Count - 1)];
        if (includeSelf)
            return random;
        else
        {
            if (random != VRRig.LocalRig)
                return random;
            else
                return GetRandomVRRig(includeSelf);
        }
    }

    public static VRRig GetClosestVRRig()
    {
        float num = float.MaxValue;
        VRRig outRig = null;
        foreach (VRRig vrrig in VRRigCache.ActiveRigs)
        {
            if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position) < num)
            {
                num = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position);
                outRig = vrrig;
            }
        }
        return outRig;
    }

    public static PhotonView GetPhotonViewFromVRRig(VRRig p) =>
        (PhotonView)Traverse.Create(p).Field("photonView").GetValue();

    public static Player GetRandomPlayer(bool includeSelf)
    {
        if (includeSelf)
            return PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length - 1)];
        else
            return PhotonNetwork.PlayerListOthers[Random.Range(0, PhotonNetwork.PlayerListOthers.Length - 1)];
    }

    public static Player GetPlayerFromVRRig(VRRig p) =>
        GetPhotonViewFromVRRig(p).Owner;

    public static NetPlayer GetPlayerFromVRRigg(VRRig p) =>
    p.Creator ?? NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(p.rigSerializer.gameObject));

    public static Player GetPlayerFromID(string id)
    {
        Player found = null;
        foreach (Player target in PhotonNetwork.PlayerList)
        {
            if (target.UserId == id)
            {
                found = target;
                break;
            }
        }
        return found;
    }

    public static Color GetPlayerColor(VRRig Player)
    {
        if (Player.bodyRenderer.cosmeticBodyType == GorillaBodyType.Skeleton)
            return Color.green;

        switch (Player.setMatIndex)
        {
            case 1:
                return Color.red;
            case 2:
            case 11:
                return new Color32(255, 128, 0, 255);
            case 3:
            case 7:
                return Color.blue;
            case 12:
                return Color.green;
            default:
                return Player.playerColor;
        }
    }
}

public static class extarstuff
{
    public static PhotonView GetViewFromRig(VRRig rig) =>
            rig2view(rig);

    public static PhotonView rig2view(VRRig p) =>
    p.netView.GetView;

    public static NetPlayer GetPlayerFromID(string id) =>
        PhotonNetwork.PlayerList.FirstOrDefault(player => player.UserId == id);

    public static VRRig GetVRRigFromPlayer(NetPlayer p) =>
    GorillaGameManager.StaticFindRigForPlayer(p);

    public static NetPlayer GetPlayerFromVRRig(VRRig p) =>
        p.Creator ?? NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(p.rigSerializer.gameObject));

    public static VRRig GhostRig;

    public static NetworkView GetNetViewFromVRRig(VRRig VRRig)
    {
        return (NetworkView)Traverse.Create(VRRig).Field("netView").GetValue();
    }

    public static NetPlayer GetPlayer(this VRRig rig) =>
    RigManager.GetPlayerFromVRRigg(rig);

    public static List<NetPlayer> InfectedList()
    {
        List<NetPlayer> infected = new List<NetPlayer>();

        if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
            return infected;

        switch (GorillaGameManager.instance.GameType())
        {
            case GameModeType.Infection:
            case GameModeType.InfectionCompetitive:
            case GameModeType.SuperInfect:
            case GameModeType.FreezeTag:
            case GameModeType.PropHunt:
                if (GorillaGameManager.instance is GorillaTagManager tagManager)
                {
                    if (tagManager.isCurrentlyTag)
                    {
                        if (tagManager.currentIt != null)
                            infected.Add(tagManager.currentIt);
                    }
                    else if (tagManager.currentInfected != null)
                    {
                        infected.AddRange(tagManager.currentInfected.Where(element => element != null));
                    }
                }
                break;

            case GameModeType.Ghost:
            case GameModeType.Ambush:
                if (GorillaGameManager.instance is GorillaAmbushManager ghostManager)
                {
                    if (ghostManager.isCurrentlyTag)
                    {
                        if (ghostManager.currentIt != null)
                            infected.Add(ghostManager.currentIt);
                    }
                    else if (ghostManager.currentInfected != null)
                    {
                        infected.AddRange(ghostManager.currentInfected.Where(element => element != null));
                    }
                }
                break;

            case GameModeType.Paintbrawl:
                if (GorillaGameManager.instance is GorillaPaintbrawlManager paintbrawlManager && paintbrawlManager.playerLives != null)
                {
                    infected.AddRange(
                        paintbrawlManager.playerLives
                            .Where(element => element.Value <= 0)
                            .Select(element => PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(element.Key))
                            .Where(dummy => dummy != null)
                            .Select(dummy => (NetPlayer)dummy)
                    );

                    if (NetworkSystem.Instance?.LocalPlayer != null && !infected.Contains(NetworkSystem.Instance.LocalPlayer))
                        infected.Add(NetworkSystem.Instance.LocalPlayer);
                }
                break;
        }

        return infected;
    }

    public static bool IsTagged(this VRRig rig)
    {
        if (rig == null) return false;
        List<NetPlayer> infectedPlayers = InfectedList();
        NetPlayer targetPlayer = rig.GetPlayer();

        return infectedPlayers.Contains(targetPlayer);
    }
}