using System;
using System.Collections;
using System.Text.RegularExpressions;
using BepInEx;
using Newtonsoft.Json.Linq;
using TMPro;
using Undefined.Mods.Categories;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using static Undefined.MENUSETTINGS.Settings;
using static Undefined.Utilities.Variables;

namespace Undefined.Menu;

public class UpdateWindow : MonoBehaviour
{
    public static bool TestingMode = false;
    public static string TestingText = "Undefined is such a good Gorilla Tag mod menu because it combines a huge variety of features with an interface that makes everything easy to access and experiment with. For people who enjoy the Gorilla Tag modding community, it fits into an ecosystem built around tools such as [BepInEx](https://github.com/BepInEx/BepInEx?utm_source=chatgpt.com), a widely used Unity modding framework that supports both Mono and IL2CPP games. Gorilla Tag itself is a VR multiplayer game developed and published by [Another Axiom](https://anotheraxiom.com/?utm_source=chatgpt.com), and its official Steam listing confirms its VR and multiplayer focus.\n\nWhat makes Undefined stand out is the sheer amount of functionality it can put into one menu. Instead of having to install and manage a bunch of completely separate mods, a menu can give players a central place to experiment with different features. This makes testing, messing around in private environments, and exploring Gorilla Tag's mechanics much more convenient. The organization of the menu is also important: having a large feature list means very little if the UI is confusing, slow, or difficult to navigate, so a clean layout and responsive controls make a major difference.\n\nAnother reason Undefined is appealing is that it fits naturally into the wider Unity modding scene. BepInEx provides plugin loading and modding infrastructure for Unity games, while its ecosystem also includes technologies such as HarmonyX and MonoMod. The official BepInEx documentation also provides installation and development information for Unity Mono and IL2CPP games. That means people interested in Undefined and similar projects can also learn about the underlying modding technologies rather than treating the menu as a completely isolated tool.\n\nUndefined is also appealing because it gives modders and players room to experiment. Gorilla Tag has a relatively simple core concept, but its movement system and VR interaction make it a particularly interesting game to modify. A feature-rich menu can turn that basic gameplay into a sandbox for testing different ideas, experimenting with movement, trying custom mechanics, and seeing what is possible with Unity modding. For someone interested in actually learning how mods work, projects surrounding tools like BepInEx can also provide useful references and documentation.\n\nOverall, Undefined stands out because it brings together a large feature set, an accessible interface, customization, and the freedom to experiment. It is especially interesting for people who enjoy the technical side of Gorilla Tag modding because it connects to the broader Unity modding ecosystem rather than existing completely on its own. With resources such as the official [BepInEx GitHub repository](https://github.com/BepInEx/BepInEx?utm_source=chatgpt.com) and [BepInEx releases](https://github.com/BepInEx/BepInEx/releases?utm_source=chatgpt.com) available to the community, there is also plenty of surrounding information for people who want to understand the technology behind Unity mods.\n";

    private GameObject window;
    private GameObject closeButton;
    private TextMeshPro notesText;
    private GameObject scrollBar;
    private GameObject scrollThumb;
    private int scrollLine;
    private int visibleLines;
    private int maxScroll;
    private float scrollTime;
    private bool leftTouching;
    private bool rightTouching;
    private string releaseTag;

    private void Start()
    {
        StartCoroutine(GetRelease());
    }

    private IEnumerator GetRelease()
    {
        if (TestingMode)
        {
            CreateWindow("Testing", AddLinks(TestingText.Replace("\r", "").Trim()));
            yield break;
        }

        using UnityWebRequest request = UnityWebRequest.Get("https://api.github.com/repos/ImudTrust-Projects/Undefined/releases/latest");
        request.SetRequestHeader("User-Agent", "Undefined");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            yield break;

        JObject release = JObject.Parse(request.downloadHandler.text);

        string version = release["tag_name"]?.ToString();
        string notes = release["body"]?.ToString().Replace("\r", "").Trim() ?? "";
        DateTime date = release["published_at"]?.ToObject<DateTime>() ?? DateTime.Now;

        if (version == PlayerPrefs.GetString("LastRelease"))
        {
            Destroy(this);
            yield break;
        }

        releaseTag = version;
        CreateWindow(version + "  |  " + date.ToString("MMM d, yyyy"), AddLinks(notes));
    }

    private static string AddLinks(string notes)
    {
        return Regex.Replace(notes, @"https?://[^\s)\]]*[^\s)\].,!?]", "<link=\"$0\"><color=#4DA3FF>$0</color></link>");
    }

    private void CreateWindow(string version, string notes)
    {
        window = new GameObject("UpdateWindow");

        Transform head = GorillaTagger.Instance.headCollider.transform;
        Vector3 forward = new Vector3(head.forward.x, 0f, head.forward.z).normalized;
        window.transform.position = head.position + forward * 0.6f - new Vector3(0f, 0.08f, 0f);
        window.transform.rotation = Quaternion.LookRotation(forward);

        GameObject background = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(background.GetComponent<Rigidbody>());
        Destroy(background.GetComponent<BoxCollider>());
        background.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;
        background.transform.parent = window.transform;
        background.transform.localRotation = Quaternion.identity;
        background.transform.localPosition = Vector3.zero;
        background.transform.localScale = new Vector3(0.56f, 0.4f, 0.01f);

        closeButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(closeButton.GetComponent<Rigidbody>());
        Destroy(closeButton.GetComponent<BoxCollider>());
        closeButton.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
        closeButton.transform.parent = window.transform;
        closeButton.transform.localRotation = Quaternion.identity;
        closeButton.transform.localPosition = new Vector3(0f, -0.16f, -0.008f);
        closeButton.transform.localScale = new Vector3(0.14f, 0.044f, 0.01f);

        if (backgroundTexture != null)
        {
            AddLogo(new Vector3(0f, 0f, -0.0055f), Quaternion.identity, 0.15f);
            AddLogo(new Vector3(0f, 0f, 0.0055f), Quaternion.Euler(0f, 180f, 0f), 0.8f);
        }

        scrollBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(scrollBar.GetComponent<Rigidbody>());
        Destroy(scrollBar.GetComponent<BoxCollider>());
        scrollBar.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
        scrollBar.transform.parent = window.transform;
        scrollBar.transform.localRotation = Quaternion.identity;
        scrollBar.transform.localPosition = new Vector3(0.265f, -0.005f, -0.006f);
        scrollBar.transform.localScale = new Vector3(0.006f, 0.22f, 0.002f);

        scrollThumb = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(scrollThumb.GetComponent<Rigidbody>());
        Destroy(scrollThumb.GetComponent<BoxCollider>());
        scrollThumb.GetComponent<Renderer>().material.color = buttonColors[1].colors[0].color;
        scrollThumb.transform.parent = window.transform;
        scrollThumb.transform.localRotation = Quaternion.identity;

        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);

        AddText(Constants.PluginName, 28f, FontStyles.Italic, TextAlignmentOptions.Center, new Vector3(0f, 0.165f, -0.006f), new Vector2(50f, 4f));
        AddText(version, 14f, FontStyles.Normal, TextAlignmentOptions.Center, new Vector3(0f, 0.135f, -0.006f), new Vector2(50f, 2f));
        notesText = AddText(notes, 17f, FontStyles.Normal, TextAlignmentOptions.TopLeft, new Vector3(0f, -0.005f, -0.006f), new Vector2(50f, 23f));
        notesText.overflowMode = TextOverflowModes.Overflow;
        notesText.ForceMeshUpdate();
        SetScroll(0);
        AddText("Close", 16f, FontStyles.Italic, TextAlignmentOptions.Center, new Vector3(0f, -0.16f, -0.014f), new Vector2(14f, 4.4f));
    }

    private void AddLogo(Vector3 position, Quaternion rotation, float alpha)
    {
        GameObject logo = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(logo.GetComponent<MeshCollider>());
        logo.transform.parent = window.transform;
        logo.transform.localPosition = position;
        logo.transform.localRotation = rotation;
        logo.transform.localScale = new Vector3(0.3f, 0.3f, 1f);

        Material material = new Material(Shader.Find("UI/Default"));
        material.mainTexture = backgroundTexture;
        material.color = new Color(1f, 1f, 1f, alpha);
        logo.GetComponent<Renderer>().material = material;
    }

    private TextMeshPro AddText(string text, float size, FontStyles style, TextAlignmentOptions alignment, Vector3 position, Vector2 area)
    {
        TextMeshPro label = new GameObject { transform = { parent = window.transform } }.AddComponent<TextMeshPro>();
        label.richText = true;
        label.text = text;
        label.fontSize = size;
        label.fontStyle = style;
        label.color = textColors[0];
        label.alignment = alignment;
        label.overflowMode = TextOverflowModes.Truncate;
        label.rectTransform.sizeDelta = area;

        label.transform.localRotation = Quaternion.identity;
        label.transform.localPosition = position;
        label.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        return label;
    }

    private void OnTextChanged(UnityEngine.Object text)
    {
        if (text == notesText)
            SetScroll(scrollLine);
    }

    private void SetScroll(int line)
    {
        TMP_TextInfo info = notesText.textInfo;
        float bottom = notesText.rectTransform.rect.yMin;

        visibleLines = 0;
        for (int i = 0; i < info.lineCount; i++)
        {
            if (info.lineInfo[i].descender >= bottom)
                visibleLines++;
        }
        visibleLines = Mathf.Max(1, visibleLines);

        maxScroll = Mathf.Max(0, info.lineCount - visibleLines);
        scrollLine = Mathf.Clamp(line, 0, maxScroll);

        float offset = info.lineCount > 0 ? info.lineInfo[0].ascender - info.lineInfo[scrollLine].ascender : 0f;
        notesText.transform.localPosition = new Vector3(0f, -0.005f + offset * 0.01f, -0.006f);

        for (int i = 0; i < info.characterCount; i++)
        {
            TMP_CharacterInfo character = info.characterInfo[i];

            if (!character.isVisible)
                continue;

            byte alpha = IsLineShown(character.lineNumber) ? (byte)255 : (byte)0;
            Color32[] colors = info.meshInfo[character.materialReferenceIndex].colors32;

            for (int v = 0; v < 4; v++)
                colors[character.vertexIndex + v].a = alpha;
        }

        notesText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        scrollBar.SetActive(maxScroll > 0);
        scrollThumb.SetActive(maxScroll > 0);

        if (maxScroll > 0)
        {
            float height = Mathf.Max(0.02f, 0.22f * visibleLines / info.lineCount);
            float y = 0.105f - height / 2f - (0.22f - height) * scrollLine / maxScroll;
            scrollThumb.transform.localPosition = new Vector3(0.265f, y, -0.008f);
            scrollThumb.transform.localScale = new Vector3(0.008f, height, 0.002f);
        }
    }

    private bool IsLineShown(int line)
    {
        return line >= scrollLine && line < scrollLine + visibleLines;
    }

    private void CheckScroll()
    {
        if (maxScroll == 0)
            return;

        float stick = 0f;

        if (ControllerInputPoller.instance != null)
            stick = ControllerInputPoller.instance.leftControllerPrimary2DAxis.y + ControllerInputPoller.instance.rightControllerPrimary2DAxis.y;

        if (Mathf.Abs(stick) > 0.5f && Time.time > scrollTime)
        {
            scrollTime = Time.time + 0.12f;
            SetScroll(scrollLine + (stick > 0f ? -1 : 1));
        }

        float wheel = Mouse.current != null ? Mouse.current.scroll.ReadValue().y : 0f;

        if (wheel != 0f)
            SetScroll(scrollLine + (wheel > 0f ? -3 : 3));
    }

    private void Update()
    {
        if (window != null)
            CheckScroll();

        if (window != null)
            leftTouching = CheckFinger(GorillaTagger.Instance.leftHandTriggerCollider, true, leftTouching);

        if (window != null)
            rightTouching = CheckFinger(GorillaTagger.Instance.rightHandTriggerCollider, false, rightTouching);

        if (window != null)
            CheckMouse();

        if (window != null && UnityInput.Current.GetKeyDown(KeyCode.Escape))
            CloseWindow();
    }

    private bool CheckFinger(GameObject finger, bool isLeft, bool wasTouching)
    {
        if (finger == null)
            return false;

        Vector3 tip = finger.transform.position;
        Vector3 local = window.transform.InverseTransformPoint(tip);

        if (local.z < -0.025f || local.z > 0.02f)
            return false;

        if (maxScroll > 0 && Mathf.Abs(local.x - 0.265f) < 0.015f && local.y < 0.105f && local.y > -0.115f)
        {
            SetScroll(Mathf.RoundToInt((0.105f - local.y) / 0.22f * maxScroll));
            return true;
        }

        string target = GetTarget(tip);

        if (target == null)
            return false;

        if (!wasTouching)
        {
            GorillaTagger.Instance.StartVibration(isLeft, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
            Press(target);
        }

        return true;
    }

    private void CheckMouse()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Camera camera = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera")?.GetComponent<Camera>() ?? Camera.main;

        if (camera == null)
            return;

        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(window.transform.forward, window.transform.TransformPoint(0f, 0f, -0.01f));

        if (!plane.Raycast(ray, out float distance))
            return;

        string target = GetTarget(ray.GetPoint(distance));

        if (target != null)
            Press(target);
    }

    private string GetTarget(Vector3 point)
    {
        Vector3 local = window.transform.InverseTransformPoint(point);
        Vector3 button = closeButton.transform.localPosition;

        if (Mathf.Abs(local.x - button.x) < 0.07f && Mathf.Abs(local.y - button.y) < 0.022f)
            return "Close";

        return GetLink(point);
    }

    private void Press(string target)
    {
        AudioHandler.Play(SoundSettings.currentButtonSound, 0.5f);

        if (target == "Close")
            CloseWindow();
        else
            Application.OpenURL(target);
    }

    private string GetLink(Vector3 tip)
    {
        Vector3 point = notesText.transform.InverseTransformPoint(tip);
        TMP_TextInfo info = notesText.textInfo;

        for (int i = 0; i < info.linkCount; i++)
        {
            TMP_LinkInfo link = info.linkInfo[i];

            for (int c = link.linkTextfirstCharacterIndex; c < link.linkTextfirstCharacterIndex + link.linkTextLength && c < info.characterCount; c++)
            {
                TMP_CharacterInfo character = info.characterInfo[c];

                if (!IsLineShown(character.lineNumber))
                    continue;

                if (point.x >= character.bottomLeft.x - 0.3f && point.x <= character.topRight.x + 0.3f &&
                    point.y >= character.descender - 0.3f && point.y <= character.ascender + 0.3f)
                    return link.GetLinkID();
            }
        }

        return null;
    }

    private void CloseWindow()
    {
        if (releaseTag != null)
        {
            PlayerPrefs.SetString("LastRelease", releaseTag);
            PlayerPrefs.Save();
        }

        AudioHandler.Play("Close", 0.5f);
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        Destroy(window);
        window = null;
        Destroy(this);
    }
}
