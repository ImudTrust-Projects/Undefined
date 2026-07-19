using UnityEngine;
using System.Linq;

namespace Undefined.Mods.Categories;

public class Safety
{
    private static float delay;

    public static void AntiReport()
    {
        if (!NetworkSystem.Instance.InRoom) return;

        foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
        {
            if (line.linePlayer != NetworkSystem.Instance.LocalPlayer) continue;

            Transform report = line.reportButton.gameObject.transform;

            foreach (VRRig vrrig in VRRigCache.ActiveRigs.Where(v =>
                         !v.isLocal &&
                         (Vector3.Distance(v.rightHandTransform.position, report.position) < 0.35f ||
                          Vector3.Distance(v.leftHandTransform.position, report.position) < 0.35f)))
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();

                if (!(Time.time > delay)) return;
                delay = Time.time + 1f;
            }
        }
    }
}