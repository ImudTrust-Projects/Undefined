using BepInEx;
using GorillaExtensions;
using GorillaLocomotion;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;
using static Undefined.MENUSETTINGS.Settings;
using static Undefined.Mods.ModButtons;
using static Undefined.Utilities.Variables;

namespace Undefined.Menu;

public class Main : MonoBehaviour
{
    public static int activeCategory
    {
        get => categoryIndex;
        set
        {
            categoryIndex = value;
            activePage = 0;
        }
    }
    private static bool prevLeftTrigger;
    private static bool prevRightTrigger;
    private static readonly Dictionary<string, (int Cat, int Idx)> searchCache = new Dictionary<string, (int Cat, int Idx)>();

    private void Update()
    {
        try
        {
            if (InputHandler.Instance == null)
                return;

            bool openRequested = (!rightHanded && InputHandler.Instance.LeftSecondary.IsPressed) ||
                                 (rightHanded && InputHandler.Instance.RightSecondary.IsPressed);
            
            bool keyboardOpen = pcMenu && UnityInput.Current.GetKey(keyboardButton);

            if (activeMenu == null)
            {
                if (openRequested || keyboardOpen)
                {
                    BuildMenu();
                    AudioHandler.Play("Open", 0.5f);
                    PositionMenu(rightHanded, keyboardOpen);
                    if (handPointer == null)
                    {
                        BuildHandPointer(rightHanded);
                    }
                }
            }
            else
            {
                if (openRequested || keyboardOpen)
                {
                    PositionMenu(rightHanded, keyboardOpen);

                    bool leftTrig = InputHandler.Instance.LeftTrigger.WasPressed;
                    bool rightTrig = InputHandler.Instance.RightTrigger.WasPressed;

                    if (leftTrig && !prevLeftTrigger)
                    {
                        ChangePage(false);
                    }
                    prevLeftTrigger = leftTrig;

                    if (rightTrig && !prevRightTrigger)
                    {
                        ChangePage(true);
                    }
                    prevRightTrigger = rightTrig;
                }
                else
                {
                    AudioHandler.Play("Close", 0.5f);
                    GameObject.Find("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(true);
                    Rigidbody rb = activeMenu.AddComponent<Rigidbody>();
                    var tracker = rightHanded ? GTPlayer.Instance.LeftHand.velocityTracker : GTPlayer.Instance.RightHand.velocityTracker;
                    rb.linearVelocity = tracker.GetAverageVelocity(true, 0);

                    Destroy(activeMenu, 2f);
                    activeMenu = null;

                    Destroy(handPointer);
                    handPointer = null;

                    prevLeftTrigger = false;
                    prevRightTrigger = false;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"{Constants.PluginName} Menu Init Err: {ex.Message} at {ex.StackTrace}");
        }

        try
        {
            if (fpsLabel != null)
            {
                fpsLabel.text = "FPS: " + Mathf.Ceil(1f / Time.unscaledDeltaTime);
            }

            var activeMods = buttons.SelectMany(x => x).Where(b => b.enabled && b.method != null);
            foreach (var mod in activeMods)
            {
                try
                {
                    mod.method.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"{Constants.PluginName} Mod Exec Err ({mod.buttonText}): {ex.Message} at {ex.StackTrace}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"{Constants.PluginName} Mods Exec Flow Err: {ex.Message}");
        }

        try
        {
            if (GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom") == null)
                return;

            if (hasSetupFeaturedMapVideo && videoPlayer != null && !videoPlayer.isPlaying &&
                videoPlayer.gameObject.activeInHierarchy && videoPlayer.enabled)
            {
                videoPlayer.Play();
            }

            if (hasSetupFeaturedMapVideo)
                return;

            GameObject loadingText = GameObject.Find(
                "Environment Objects/LocalObjects_Prefab/TreeRoom/LoadingText");

            GameObject mapInfoText = GameObject.Find(
                "Environment Objects/LocalObjects_Prefab/TreeRoom/MapInfo_TMP");

            GameObject featuredMaps = GameObject.Find(
                "Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay");

            GameObject displayTextObj = GameObject.Find(
                "Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay/DisplayText");


            if (displayTextObj != null)
            {
                foreach (Transform child in displayTextObj.transform)
                {
                    if (child.name.ToLower().EndsWith("tmp"))
                        child.gameObject.SetActive(true);
                }
            }


            if (mapInfoText == null || featuredMaps == null)
                return;


            TextMeshPro text = mapInfoText.GetComponent<TextMeshPro>();

            if (text != null)
                text.text = "<color=black>Undefined</color>";


            if (loadingText != null)
                loadingText.Obliterate();


            GameObject featuredMapImage = featuredMaps.transform.Find("FeaturedMapImage")?.gameObject;

            if (featuredMapImage == null)
                return;


            if (featuredMapImage.TryGetComponent(out SpriteRenderer spriteRenderer))
                spriteRenderer.Obliterate();


            MeshFilter mf = featuredMapImage.GetOrAddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");


            MeshRenderer mr = featuredMapImage.GetOrAddComponent<MeshRenderer>();

            Material mat = new Material(Shader.Find("Unlit/Texture"));
            mr.material = mat;


            videoPlayer = featuredMapImage.GetComponent<VideoPlayer>();

            if (videoPlayer == null)
                videoPlayer = featuredMapImage.AddComponent<VideoPlayer>();


            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            videoPlayer.url = "https://github.com/ImudTrust/Mod-Resources/raw/refs/heads/main/%C3%B6.mp4";
            videoPlayer.isLooping = true;


            RenderTexture rt = new RenderTexture(512, 512, 0);

            videoPlayer.targetTexture = rt;
            mr.material.mainTexture = rt;


            featuredMapImage.transform.localScale = new Vector3(0.845f, 0.445f, 1f);

            featuredMapImage.SetActive(true);

            videoPlayer.Play();

            hasSetupFeaturedMapVideo = true;

        }
        catch (Exception ex)
        {
            Debug.LogError($"Promotion Video Error: {ex.Message}");
        }
    }

    public static void BuildMenu()
    {
        activeMenu = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(activeMenu.GetComponent<Rigidbody>());
        Destroy(activeMenu.GetComponent<BoxCollider>());
        Destroy(activeMenu.GetComponent<Renderer>());
        activeMenu.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);

        bgObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(bgObject.GetComponent<Rigidbody>());
        Destroy(bgObject.GetComponent<BoxCollider>());
        bgObject.transform.parent = activeMenu.transform;
        bgObject.transform.rotation = Quaternion.identity;
        bgObject.transform.localScale = menuSize;
        bgObject.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;
        bgObject.transform.position = new Vector3(0.05f, 0f, 0f);

        menuCanvas = new GameObject();
        menuCanvas.transform.parent = activeMenu.transform;
        Canvas canvasComp = menuCanvas.AddComponent<Canvas>();
        CanvasScaler scalerComp = menuCanvas.AddComponent<CanvasScaler>();
        menuCanvas.AddComponent<GraphicRaycaster>();
        canvasComp.renderMode = RenderMode.WorldSpace;
        scalerComp.dynamicPixelsPerUnit = 1000f;

        Text titleText = new GameObject { transform = { parent = menuCanvas.transform } }.AddComponent<Text>();
        titleText.font = currentFont;
        titleText.text = Constants.PluginName;
        titleText.fontSize = 1;
        titleText.color = textColors[0];
        titleText.supportRichText = true;
        titleText.fontStyle = FontStyle.Italic;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.resizeTextForBestFit = true;
        titleText.resizeTextMinSize = 0;
        RectTransform titleTrans = titleText.GetComponent<RectTransform>();
        titleTrans.localPosition = Vector3.zero;
        titleTrans.sizeDelta = new Vector2(0.28f, 0.05f);
        titleTrans.position = new Vector3(0.06f, 0f, 0.165f);
        titleTrans.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

        if (fpsCounter)
        {
            fpsLabel = new GameObject { transform = { parent = menuCanvas.transform } }.AddComponent<Text>();
            fpsLabel.font = currentFont;
            fpsLabel.text = "FPS: " + Mathf.Ceil(1f / Time.unscaledDeltaTime);
            fpsLabel.color = textColors[0];
            fpsLabel.fontSize = 1;
            fpsLabel.supportRichText = true;
            fpsLabel.fontStyle = FontStyle.Italic;
            fpsLabel.alignment = TextAnchor.MiddleCenter;
            fpsLabel.horizontalOverflow = HorizontalWrapMode.Overflow;
            fpsLabel.resizeTextForBestFit = true;
            fpsLabel.resizeTextMinSize = 0;
            RectTransform fpsTrans = fpsLabel.GetComponent<RectTransform>();
            fpsTrans.localPosition = Vector3.zero;
            fpsTrans.sizeDelta = new Vector2(0.28f, 0.02f);
            fpsTrans.position = new Vector3(0.06f, 0f, 0.135f);
            fpsTrans.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        if (disconnectButton)
        {
            GameObject discBtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(keyboardButton))
            {
                discBtn.layer = 2;
            }
            Destroy(discBtn.GetComponent<Rigidbody>());
            discBtn.GetComponent<BoxCollider>().isTrigger = true;
            discBtn.transform.parent = activeMenu.transform;
            discBtn.transform.rotation = Quaternion.identity;
            discBtn.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
            discBtn.transform.localPosition = new Vector3(0.56f, 0f, 0.6f);
            discBtn.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
            discBtn.AddComponent<Utilities.Button>().relatedText = "Disconnect";

            Text discText = new GameObject { transform = { parent = menuCanvas.transform } }.AddComponent<Text>();
            discText.text = "Disconnect";
            discText.font = currentFont;
            discText.fontSize = 1;
            discText.color = textColors[0];
            discText.alignment = TextAnchor.MiddleCenter;
            discText.resizeTextForBestFit = true;
            discText.resizeTextMinSize = 0;
            RectTransform discTextTrans = discText.GetComponent<RectTransform>();
            discTextTrans.localPosition = Vector3.zero;
            discTextTrans.sizeDelta = new Vector2(0.2f, 0.03f);
            discTextTrans.localPosition = new Vector3(0.064f, 0f, 0.23f);
            discTextTrans.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        ButtonInfo[] pageButtons = buttons[activeCategory].Skip(activePage * buttonsPerPage).Take(buttonsPerPage).ToArray();
        for (int i = 0; i < pageButtons.Length; i++)
        {
            BuildButton(i * 0.1f, pageButtons[i]);
        }
    }

    public static void BuildButton(float offset, ButtonInfo info)
    {
        GameObject btnObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (!UnityInput.Current.GetKey(keyboardButton))
        {
            btnObj.layer = 2;
        }

        Destroy(btnObj.GetComponent<Rigidbody>());
        btnObj.GetComponent<BoxCollider>().isTrigger = true;
        btnObj.transform.parent = activeMenu.transform;
        btnObj.transform.rotation = Quaternion.identity;
        btnObj.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
        btnObj.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
        btnObj.AddComponent<Utilities.Button>().relatedText = info.buttonText;

        Renderer renderer = btnObj.GetComponent<Renderer>();
        renderer.material.color = info.enabled
               ? buttonColors[1].colors[0].color
               : buttonColors[0].colors[0].color;

        Text btnText = new GameObject { transform = { parent = menuCanvas.transform } }.AddComponent<Text>();

        btnText.font = currentFont;
        btnText.text = info.overlapText ?? info.buttonText;
        btnText.supportRichText = true;
        btnText.fontSize = 1;
        btnText.color = info.enabled ? textColors[1] : textColors[0];
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.fontStyle = FontStyle.Italic;
        btnText.resizeTextForBestFit = true;
        btnText.resizeTextMinSize = 0;

        RectTransform textTrans = btnText.GetComponent<RectTransform>();
        textTrans.localPosition = Vector3.zero;
        textTrans.sizeDelta = new Vector2(0.2f, 0.03f);
        textTrans.localPosition = new Vector3(0.064f, 0f, 0.111f - offset / 2.6f - 0.0025f);
        textTrans.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
    }

    public static void RebuildMenu()
    {
        if (activeMenu != null)
        {
            Destroy(activeMenu);
            activeMenu = null;
            BuildMenu();
            PositionMenu(rightHanded, UnityInput.Current.GetKey(keyboardButton));
        }
    }

    public static void PositionMenu(bool isRightHanded, bool isKeyboardMode)
    {
        if (!isKeyboardMode)
        {
            if (!isRightHanded)
            {
                activeMenu.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                activeMenu.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
            }
            else
            {
                activeMenu.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                Vector3 euler = GorillaTagger.Instance.rightHandTransform.rotation.eulerAngles;
                euler += new Vector3(0f, 0f, 180f);
                activeMenu.transform.rotation = Quaternion.Euler(euler);
            }
        }
        else
        {
            try
            {
                spectatorCamera = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>();
            }
            catch { }

            GameObject.Find("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(false);

            if (spectatorCamera != null)
            {
                spectatorCamera.transform.position = new Vector3(-999f, -999f, -999f);
                spectatorCamera.transform.rotation = Quaternion.identity;

                GameObject backgroundBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                backgroundBlock.transform.localScale = new Vector3(10f, 10f, 0.01f);
                backgroundBlock.transform.position = spectatorCamera.transform.position + spectatorCamera.transform.forward;

                Color bgColor = backgroundColor.GetCurrentColor();
                backgroundBlock.GetComponent<Renderer>().material.color = new Color32((byte)(bgColor.r * 50), (byte)(bgColor.g * 50), (byte)(bgColor.b * 50), 255);
                Destroy(backgroundBlock, 0.05f);

                activeMenu.transform.parent = spectatorCamera.transform;
                activeMenu.transform.position = spectatorCamera.transform.position + (spectatorCamera.transform.forward * 0.5f) + (spectatorCamera.transform.up * -0.02f);
                activeMenu.transform.rotation = spectatorCamera.transform.rotation * Quaternion.Euler(-90f, 90f, 0f);

                if (handPointer != null)
                {
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = spectatorCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                        if (Physics.Raycast(ray, out RaycastHit hit, 100))
                        {
                            Utilities.Button button = hit.transform.gameObject.GetComponent<Utilities.Button>();
                            button?.OnTriggerEnter(triggerCollider);
                        }
                    }
                    else
                    {
                        handPointer.transform.position = new Vector3(999f, -999f, -999f);
                    }
                }
            }
        }
    }

    public static void BuildHandPointer(bool isRightHanded)
    {
        handPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        handPointer.transform.parent = isRightHanded ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;
        handPointer.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;
        handPointer.transform.localPosition = new Vector3(0f, -0.1f, 0f);
        handPointer.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        triggerCollider = handPointer.GetComponent<SphereCollider>();
    }

    public static void ChangePage(bool next)
    {
        int totalPages = (buttons[activeCategory].Length + buttonsPerPage - 1) / buttonsPerPage;
        if (totalPages <= 1) return;

        if (next)
        {
            activePage++;
            if (activePage >= totalPages) activePage = 0;
        }
        else
        {
            activePage--;
            if (activePage < 0) activePage = totalPages - 1;
        }
        RebuildMenu();
    }

    public static void ProcessClick(string text)
    {
        ButtonInfo target = FindButton(text);
        if (target != null)
        {
            if (target.isTogglable)
            {
                target.enabled = !target.enabled;
                if (target.enabled)
                {
                    target.enableMethod?.Invoke();

                    NotificationLib.SendNotification(
                        NotificationLib.NotificationType.Enabled, target.toolTip);
                }
                else
                {
                    target.disableMethod?.Invoke();

                    NotificationLib.SendNotification(
                        NotificationLib.NotificationType.Disabled,
                        target.toolTip);
                }
            }
            else
            {
                target.method?.Invoke();
                NotificationLib.SendNotification(
                    NotificationLib.NotificationType.Info,
                    target.toolTip);
            }
        }
        else
        {
            Debug.LogError($"{text} does not exist");
        }
        RebuildMenu();
    }

    public static ButtonInfo FindButton(string text)
    {
        if (text == null) return null;

        if (searchCache.TryGetValue(text, out var entry))
        {
            try
            {
                if (buttons[entry.Cat][entry.Idx].buttonText == text)
                {
                    return buttons[entry.Cat][entry.Idx];
                }
            }
            catch
            {
                searchCache.Remove(text);
            }
        }

        for (int cat = 0; cat < buttons.Length; cat++)
        {
            for (int idx = 0; idx < buttons[cat].Length; idx++)
            {
                if (buttons[cat][idx].buttonText == text)
                {
                    try
                    {
                        searchCache[text] = (cat, idx);
                    }
                    catch
                    {
                        if (searchCache.ContainsKey(text))
                        {
                            searchCache.Remove(text);
                        }
                    }
                    return buttons[cat][idx];
                }
            }
        }
        return null;
    }

    public static Vector3 GetRandomVector(float range = 1f)
    {
        return new Vector3(
            UnityEngine.Random.Range(-range, range),
            UnityEngine.Random.Range(-range, range),
            UnityEngine.Random.Range(-range, range)
        );
    }

    public static Quaternion GetRandomRotation(float range = 360f)
    {
        return Quaternion.Euler(
            UnityEngine.Random.Range(0f, range),
            UnityEngine.Random.Range(0f, range),
            UnityEngine.Random.Range(0f, range)
        );
    }

    public static Color GetRandomColor(byte limit = 255, byte alpha = 255)
    {
        return new Color32(
            (byte)UnityEngine.Random.Range(0, limit),
            (byte)UnityEngine.Random.Range(0, limit),
            (byte)UnityEngine.Random.Range(0, limit),
            alpha
        );
    }

    public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) GetLeftHand()
    {
        Quaternion rot = GorillaTagger.Instance.leftHandTransform.rotation * GTPlayer.Instance.LeftHand.handRotOffset;
        return (
            GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.rotation * GTPlayer.Instance.LeftHand.handOffset,
            rot,
            rot * Vector3.up,
            rot * Vector3.forward,
            rot * Vector3.right
        );
    }

    public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) GetRightHand()
    {
        Quaternion rot = GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handRotOffset;
        return (
            GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handOffset,
            rot,
            rot * Vector3.up,
            rot * Vector3.forward,
            rot * Vector3.right
        );
    }

    public static void ApplyScale(GameObject obj, Vector3 targetScale)
    {
        Vector3 lossy = obj.transform.parent.lossyScale;
        obj.transform.localScale = new Vector3(
            targetScale.x / lossy.x,
            targetScale.y / lossy.y,
            targetScale.z / lossy.z
        );
    }

    public static void PreventStickyPhysics(GameObject platform)
    {
        Vector3[] positions = new Vector3[]
        {
            new Vector3(0, 1f, 0),
            new Vector3(0, -1f, 0),
            new Vector3(1f, 0, 0),
            new Vector3(-1f, 0, 0),
            new Vector3(0, 0, 1f),
            new Vector3(0, 0, -1f)
        };
        Quaternion[] rotations = new Quaternion[]
        {
            Quaternion.Euler(90, 0, 0),
            Quaternion.Euler(-90, 0, 0),
            Quaternion.Euler(0, -90, 0),
            Quaternion.Euler(0, 90, 0),
            Quaternion.identity,
            Quaternion.Euler(0, 180, 0)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject side = GameObject.CreatePrimitive(PrimitiveType.Cube);
            try
            {
                if (platform.GetComponent<GorillaSurfaceOverride>() != null)
                {
                    side.AddComponent<GorillaSurfaceOverride>().overrideIndex = platform.GetComponent<GorillaSurfaceOverride>().overrideIndex;
                }
            }
            catch { }

            float size = 0.025f;
            side.transform.SetParent(platform.transform);
            side.transform.position = positions[i] * (size / 2);
            side.transform.rotation = rotations[i];
            ApplyScale(side, new Vector3(size, size, 0.01f));
            side.GetComponent<Renderer>().enabled = false;
        }
    }
}