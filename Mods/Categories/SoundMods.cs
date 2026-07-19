using GorillaLocomotion;
using Photon.Pun;
using System.Collections.Generic;
using Undefined.Utilities;
using UnityEngine;
using static Undefined.Patches.EffectDataPatch;
using static Undefined.Plugin;

namespace Undefined.Mods.Categories;

public class SoundMods
{
    private static float Sound_delay;

    public static List<string> soundOptions = new List<string>();
    public static int currentSoundId = 1;

    public static void PopulateSoundOptions()
    {
        soundOptions.Clear();
        for (int i = 1; i <= allsoundsids; i++) soundOptions.Add(i.ToString());
    }

    public static void SetSound(string value) { if (int.TryParse(value, out int id)) currentSoundId = id; }

    public static void PlaySelectedSound() => Sound(currentSoundId);

    public static void Sound(int soundid)
    {
        if (PhotonNetwork.InRoom)
        {
            if (InputHandler.Instance.RightGrip.IsPressed)
            {
                if (Time.time > Sound_delay)
                {
                    Sound_delay = Time.time + 0.1f;
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, soundid, false, 999999f);
                }
            }
            else if (InputHandler.Instance.LeftGrip.IsPressed)
            {
                if (Time.time > Sound_delay)
                {
                    Sound_delay = Time.time + 0.1f;
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, soundid, true, 999999f);
                }
            }

            Utilities.Variables.RPCProtection();
        }
        else
        {
            if (InputHandler.Instance.RightGrip.IsPressed)
            {
                if (Time.time > Sound_delay)
                {
                    Sound_delay = Time.time + 0.1f;
                    VRRig.LocalRig.PlayHandTapLocal(soundid, false, 15f);
                }
            }
            else if (InputHandler.Instance.LeftGrip.IsPressed)
            {
                if (Time.time > Sound_delay)
                {
                    Sound_delay = Time.time + 0.1f;
                    VRRig.LocalRig.PlayHandTapLocal(soundid, true, 15f);
                }
            }

            Utilities.Variables.RPCProtection();
        }
    }

    public static void jmancurly_Soundspam() => Sound(Random.Range(336, 338));

    public static void RandomSoundspam() => Sound(Random.Range(0, GTPlayer.Instance.materialData.Count));

    public static void Shiny_Rocks() => Sound(269);

    public static void Fireworks() => Sound(316);

    public static void Lemming() => Sound(Random.Range(309, 310));

    public static void Bouncythings() => Sound(Random.Range(256, 257));

    public static void Crystal() => Sound(Random.Range(40, 54));

    public static void Wolf() => Sound(Random.Range(156, 157));

    public static void Voting_Rock() => Sound(287);

    public static void AK_47()
    {
        Sound(39);
        Sound(30);
    }

    public static void Override_HandTap_Sounds(bool disable)
    {
        if (!disable)
        {
            enabled = true;
            material = currentSoundId;
        }
        else
        {
            enabled = false;
            material = -1;
        }
    }

    public static void No_hand_taps(bool disable)
    {
        if (!disable)
        {
            enabled = true;
            material = -1;
            tapsEnabled = false;
        }
        else
        {
            enabled = false;
            material = -1;
            tapsEnabled = true;
        }
    }
}