using Undefined.Mods.Categories;
using UnityEngine;
using static Undefined.Menu.Main;
using static Undefined.MENUSETTINGS.Settings;
using static Undefined.Utilities.Variables;

namespace Undefined.Utilities;

public class Button : MonoBehaviour
{
    public string relatedText;

    public static float buttonCooldown = 0f;

    public void OnTriggerEnter(Collider collider)
    {
        if (Time.time > buttonCooldown && collider == triggerCollider && activeMenu != null)
        {
            buttonCooldown = Time.time + 0.2f;
            GorillaTagger.Instance.StartVibration(rightHanded, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
            AudioHandler.Play(SoundSettings.currentButtonSound, 0.5f);
            ProcessClick(this.relatedText);
        }
    }
}