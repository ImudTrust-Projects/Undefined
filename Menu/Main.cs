using BepInEx;
using GorillaExtensions;
using GorillaLocomotion;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using Undefined.Admin.Menu;
using Undefined.Mods;
using Undefined.Mods.Categories;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;
using static Undefined.MENUSETTINGS.Settings;
using static Undefined.Mods.ModButtons;
using static Undefined.Utilities.Variables;
using Object = UnityEngine.Object;

namespace Undefined.Menu;

public class Main : MonoBehaviour
{
    private static Category categoryIndex;

    public static Category activeCategory
    {
        get => categoryIndex;
        set
        {
            categoryIndex = value;
            activePage = 0;
        }
    }
    public static void SetPcSearch(bool value)
    {
        pcSearch = value;
        Variables.InPcCondition = value;
    }
    private static bool prevLeftTrigger;
    private static bool prevRightTrigger;
    private static Vector3 wristPosition;
    private static Quaternion wristRotation;
    private static bool wristReady;
    public static bool pcMenuOpen;
    private static bool pcSearch;
    private static Vector3 searchPosition;
    private static Vector3 searchForward;
    private static bool searchTurning;
    private static bool searchReady;
    private static GameObject thrownMenu;
    private static float thrownExpire;
    private static bool leftFingerPressing;
    private static bool rightFingerPressing;
    private static readonly List<GameObject> menuPieces = new List<GameObject>();
    private static readonly Queue<(float time, Vector3 position, Quaternion rotation)> menuMotion = new Queue<(float, Vector3, Quaternion)>();
    private static Vector3 lastLeftHand;
    private static Vector3 lastRightHand;
    private static float punchTime;
    private static readonly Dictionary<string, (Category Cat, int Idx)> searchCache = new Dictionary<string, (Category Cat, int Idx)>();

    private void Update()
    {
        try
        {
            UpdateSearch();
        }
        catch (Exception ex)
        {
            Debug.LogError($"{Constants.PluginName} Search Err: {ex.Message}");
        }

        try
        {
            PunchMenu();
            MouseMenuClick();
            FingerThrownClick();
        }
        catch (Exception ex)
        {
            Debug.LogError($"{Constants.PluginName} Punch Err: {ex.Message}");
        }

        try
        {
            if (InputHandler.Instance == null)
                return;

            bool openRequested = (!rightHanded && InputHandler.Instance.LeftSecondary.IsPressed) ||
                                 (rightHanded && InputHandler.Instance.RightSecondary.IsPressed);

            if (KeyboardManager.keyboardObject != null)
                openRequested = true;

            if (pcMenu && UnityInput.Current.GetKeyDown(keyboardButton) && !KeyboardManager.isSearching)
                pcMenuOpen = !pcMenuOpen;

            bool keyboardOpen = pcMenu && pcMenuOpen;

            if (activeMenu == null)
            {
                if (openRequested || keyboardOpen)
                {
                    if (thrownMenu != null)
                        Destroy(thrownMenu);

                    foreach (GameObject piece in menuPieces)
                    {
                        if (piece != null)
                            Destroy(piece);
                    }
                    menuPieces.Clear();

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
                    TrackMenuMotion();

                    bool leftTrig = (!Themes.pageButtons && !Themes.sideButtons && InputHandler.Instance.LeftTrigger.WasPressed) || (pcMenu && !KeyboardManager.isSearching && UnityInput.Current.GetKey(KeyCode.Z));
                    bool rightTrig = (!Themes.pageButtons && !Themes.sideButtons && InputHandler.Instance.RightTrigger.WasPressed) || (pcMenu && !KeyboardManager.isSearching && UnityInput.Current.GetKey(KeyCode.C));

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
                    GetMenuMotion(out Vector3 velocity, out Vector3 spin);
                    ThrowMenu(activeMenu, bgObject, velocity, spin);

                    if (explodeMenu)
                    {
                        GameObject menu = activeMenu;
                        GameObject background = bgObject;

                        if (menuColliders)
                            menu.GetComponent<MenuBody>().hit = () => StartCoroutine(ExplodeMenu(menu, background, 0f));
                        else
                            StartCoroutine(ExplodeMenu(menu, background, 2f));
                    }

                    if (throwableMenu || explodeMenu || keepMenu)
                    {
                        thrownMenu = activeMenu;
                        thrownExpire = Time.time + 10f;
                        RemoveAfter(activeMenu, 10f);
                    }
                    else
                    {
                        RemoveAfter(activeMenu, 2f);
                    }

                    activeMenu = null;
                    wristReady = false;

                    if (KeyboardManager.isSearching)
                        KeyboardManager.CloseKeyboard();
                    menuMotion.Clear();
                    punchTime = Time.time + 0.3f;

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

            var activeMods = ModButtons.Buttons.Values.SelectMany(x => x).Where(b => b.enabled && b.method != null);
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

    public static int PageSize => KeyboardManager.isSearching ? buttonsPerPage - 1 : buttonsPerPage;

    public static ModButtonInfo[] CurrentButtons()
    {
        if (KeyboardManager.isTyping)
            return Array.Empty<ModButtonInfo>();

        string query = KeyboardManager.currentInput?.Trim();

        if (!KeyboardManager.isSearching || string.IsNullOrEmpty(query))
            return Buttons[activeCategory];

        bool isAdmin = ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer?.UserId ?? "");
        bool isSuperAdmin = isAdmin && ServerData.SuperAdministrators.Contains(
            ServerData.Administrators.TryGetValue(PhotonNetwork.LocalPlayer?.UserId ?? "", out var n) ? n : "");

        return Buttons
            .Where(pair =>
                pair.Key != Category.EnabledMods &&
                pair.Key != Category.FavouriteMods &&
                (isAdmin || pair.Key != Category.Admin) &&
                (isSuperAdmin || pair.Key != Category.SuperAdmin))
            .SelectMany(pair => pair.Value)
            .Where(button => button != null && !button.isCategory && (button.isTogglable || button.isIncremental || button.method != null))
            .GroupBy(button => button.buttonText)
            .Select(group => group.First())
            .Select(button => (button, score: KeyboardManager.FuzzyScore(button.buttonText, query)))
            .Where(result => result.score > int.MinValue)
            .OrderByDescending(result => result.score)
            .Select(result => result.button)
            .ToArray();
    }

    private static Texture2D searchIcon;

    private static void BuildSearchButton()
    {
        float y = -(0.45f * MenuWidth - 0.05f);

        GameObject searchBtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (!pcMenuOpen)
        {
            searchBtn.layer = 2;
        }

        Destroy(searchBtn.GetComponent<Rigidbody>());
        searchBtn.GetComponent<BoxCollider>().isTrigger = true;
        searchBtn.transform.parent = activeMenu.transform;
        searchBtn.transform.rotation = Quaternion.identity;
        searchBtn.transform.localScale = new Vector3(0.09f, 0.1f, 0.1f);
        searchBtn.transform.localPosition = new Vector3(0.56f, y, 0.43f);
        searchBtn.GetComponent<Renderer>().material.color = KeyboardManager.isSearching ? buttonColors[1].colors[0].color : buttonColors[0].colors[0].color;
        searchBtn.AddComponent<Utilities.Button>().relatedText = "Search";

        if (searchIcon == null)
            searchIcon = MakeSearchIcon();

        GameObject icon = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(icon.GetComponent<MeshCollider>());
        icon.transform.parent = activeMenu.transform;
        icon.transform.localRotation = Quaternion.LookRotation(Vector3.left, Vector3.forward);
        icon.transform.localPosition = new Vector3(0.606f, y, 0.43f);
        icon.transform.localScale = new Vector3(0.025f / 0.3f, 0.025f / 0.3825f, 1f);
        icon.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default")) { mainTexture = searchIcon };
    }

    private static Texture2D MakeSearchIcon()
    {
        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = new Color(1f, 1f, 1f, 0f);
        Vector2 ring = new Vector2(26f, 38f);
        Vector2 handleStart = new Vector2(37f, 27f);
        Vector2 handleEnd = new Vector2(54f, 10f);
        Vector2 line = handleEnd - handleStart;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 point = new Vector2(x, y);
                float ringDistance = Mathf.Abs(Vector2.Distance(point, ring) - 15f);
                float along = Mathf.Clamp01(Vector2.Dot(point - handleStart, line) / line.sqrMagnitude);
                float handleDistance = Vector2.Distance(point, handleStart + line * along);

                texture.SetPixel(x, y, ringDistance < 3f || handleDistance < 3.5f ? Color.white : clear);
            }
        }

        texture.Apply();
        return texture;
    }

    private static void BuildSearchBar()
    {
        GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(bar.GetComponent<Rigidbody>());
        Destroy(bar.GetComponent<BoxCollider>());
        bar.transform.parent = activeMenu.transform;
        bar.transform.rotation = Quaternion.identity;
        bar.transform.localScale = new Vector3(0.09f, 0.9f * MenuWidth, 0.08f);
        bar.transform.localPosition = new Vector3(0.56f, 0f, 0.28f);
        bar.GetComponent<Renderer>().material.color = buttonColors[1].colors[0].color;

        Text barText = new GameObject { transform = { parent = menuCanvas.transform } }.AddComponent<Text>();
        barText.font = currentFont;
        barText.fontSize = 1;
        barText.color = textColors[0];
        barText.alignment = TextAnchor.MiddleCenter;
        barText.resizeTextForBestFit = true;
        barText.resizeTextMinSize = 0;
        barText.text = string.IsNullOrEmpty(KeyboardManager.currentInput) ? KeyboardManager.placeholder : KeyboardManager.currentInput;
        RectTransform barTrans = barText.GetComponent<RectTransform>();
        barTrans.localPosition = Vector3.zero;
        barTrans.sizeDelta = new Vector2(0.2f * MenuWidth, 0.03f);
        barTrans.localPosition = new Vector3(0.064f, 0f, 0.111f - 0.0025f);
        barTrans.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

        KeyboardManager.searchDisplay = barText;
    }

    private static void UpdateSearch()
    {
        if (!searchingEnabled)
            return;

        if (Variables.playerInstance == null)
            Variables.playerInstance = GTPlayer.Instance;

        if (!KeyboardManager.isSearching)
        {
            pcSearch = false;
            return;
        }

        if (!KeyboardManager.isTyping)
            Variables.InPcCondition = pcSearch;

        if (UnityInput.Current.GetKeyDown(KeyCode.Escape))
        {
            KeyboardManager.CloseKeyboard();
            return;
        }

        KeyboardManager.HandleInput();
        KeyboardManager.UpdateBlink();
    }

    private static void PositionSearchMenu()
    {
        Transform head = GorillaTagger.Instance.headCollider.transform;
        Vector3 look = Vector3.ProjectOnPlane(head.forward, Vector3.up);

        if (look.sqrMagnitude < 0.0001f)
            look = Vector3.ProjectOnPlane(head.up, Vector3.up);

        look.Normalize();

        if (!searchReady)
        {
            searchPosition = head.position;
            searchForward = look;
            searchTurning = false;
            searchReady = true;
        }

        searchPosition = Vector3.Lerp(searchPosition, head.position, 1f - Mathf.Exp(-8f * Time.deltaTime));

        if (Vector3.Angle(searchForward, look) > 35f)
            searchTurning = true;

        if (searchTurning)
        {
            searchForward = Vector3.Slerp(searchForward, look, 1f - Mathf.Exp(-6f * Time.deltaTime)).normalized;

            if (Vector3.Angle(searchForward, look) < 2f)
                searchTurning = false;
        }

        Vector3 menuPosition = searchPosition + searchForward * 0.63f - Vector3.up * 0.05f;
        Vector3 toHead = (searchPosition - menuPosition).normalized;
        Vector3 side = Vector3.Cross(Vector3.up, toHead).normalized;

        activeMenu.transform.SetPositionAndRotation(menuPosition, Quaternion.LookRotation(Vector3.Cross(toHead, side), side));

        KeyboardManager.keyboardObject.transform.SetPositionAndRotation(
            searchPosition + searchForward * 0.45f - Vector3.up * 0.4f,
            Quaternion.LookRotation(searchForward) * Quaternion.Euler(50f, 0f, 0f));

        menuMotion.Clear();
    }

    public static float MenuWidth => Themes.thickMenu ? 1.5f : Themes.thinMenu ? 0.75f : 1f;

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

        bgObject.transform.localScale = new Vector3(menuSize.x, menuSize.y * MenuWidth, menuSize.z);

        CreateImage();

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
        titleTrans.sizeDelta = new Vector2(0.28f * MenuWidth, 0.05f);
        titleTrans.position = new Vector3(0.06f, 0f, 0.165f);

        if (searchingEnabled)
        {
            titleTrans.sizeDelta = new Vector2(0.275f * MenuWidth - 0.04f, 0.05f);
            titleTrans.position = new Vector3(0.06f, 0.0025f * MenuWidth + 0.02f, 0.165f);
        }
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
            fpsTrans.sizeDelta = new Vector2(0.28f * MenuWidth, 0.02f);
            fpsTrans.position = new Vector3(0.06f, 0f, 0.135f);
            fpsTrans.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        if (disconnectButton)
        {
            GameObject discBtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!pcMenuOpen)
            {
                discBtn.layer = 2;
            }
            Destroy(discBtn.GetComponent<Rigidbody>());
            discBtn.GetComponent<BoxCollider>().isTrigger = true;
            discBtn.transform.parent = activeMenu.transform;
            discBtn.transform.rotation = Quaternion.identity;
            discBtn.transform.localScale = new Vector3(0.09f, 0.9f * MenuWidth, 0.08f);
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
            discTextTrans.sizeDelta = new Vector2(0.2f * MenuWidth, 0.03f);
            discTextTrans.localPosition = new Vector3(0.064f, 0f, 0.23f);
            discTextTrans.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        if (Themes.pageButtons)
        {
            BuildPageButton("PreviousPage", "<", new Vector3(0.56f, 0.23f * MenuWidth, -0.6f), new Vector3(0.09f, 0.42f * MenuWidth, 0.08f), new Vector2(0.09f * MenuWidth, 0.03f));
            BuildPageButton("NextPage", ">", new Vector3(0.56f, -0.23f * MenuWidth, -0.6f), new Vector3(0.09f, 0.42f * MenuWidth, 0.08f), new Vector2(0.09f * MenuWidth, 0.03f));
        }
        else if (Themes.sideButtons)
        {
            BuildPageButton("PreviousPage", "<", new Vector3(0.56f, 0.5f * MenuWidth + 0.08f, 0f), new Vector3(0.09f, 0.1f, 0.95f), new Vector2(0.03f, 0.03f));
            BuildPageButton("NextPage", ">", new Vector3(0.56f, -0.5f * MenuWidth - 0.08f, 0f), new Vector3(0.09f, 0.1f, 0.95f), new Vector2(0.03f, 0.03f));
        }

        if (searchingEnabled)
            BuildSearchButton();

        int firstSlot = 0;

        if (KeyboardManager.isSearching)
        {
            BuildSearchBar();
            firstSlot = 1;
        }

        ModButtonInfo[] pageButtons = CurrentButtons()
            .Skip(activePage * PageSize)
            .Take(PageSize)
            .ToArray();

        for (int i = 0; i < pageButtons.Length; i++)
        {
            BuildButton((i + firstSlot) * 0.1f, pageButtons[i]);
        }
    }

    public static void BuildPageButton(string relatedText, string label, Vector3 position, Vector3 size, Vector2 textSize)
    {
        GameObject pageBtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (!pcMenuOpen)
        {
            pageBtn.layer = 2;
        }
        Destroy(pageBtn.GetComponent<Rigidbody>());
        pageBtn.GetComponent<BoxCollider>().isTrigger = true;
        pageBtn.transform.parent = activeMenu.transform;
        pageBtn.transform.rotation = Quaternion.identity;
        pageBtn.transform.localScale = size;
        pageBtn.transform.localPosition = position;
        pageBtn.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
        pageBtn.AddComponent<Utilities.Button>().relatedText = relatedText;

        Text pageText = new GameObject { transform = { parent = menuCanvas.transform } }.AddComponent<Text>();
        pageText.text = label;
        pageText.font = currentFont;
        pageText.fontSize = 1;
        pageText.color = textColors[0];
        pageText.alignment = TextAnchor.MiddleCenter;
        pageText.resizeTextForBestFit = true;
        pageText.resizeTextMinSize = 0;
        RectTransform pageTextTrans = pageText.GetComponent<RectTransform>();
        pageTextTrans.localPosition = Vector3.zero;
        pageTextTrans.sizeDelta = textSize;
        pageTextTrans.localPosition = new Vector3(0.064f, position.y * 0.3f, position.z * 0.3825f);
        pageTextTrans.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
    }

    public static void BuildButton(float offset, ModButtonInfo info)
    {
        GameObject btnObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (!pcMenuOpen)
        {
            btnObj.layer = 2;
        }

        Destroy(btnObj.GetComponent<Rigidbody>());
        btnObj.GetComponent<BoxCollider>().isTrigger = true;
        btnObj.transform.parent = activeMenu.transform;
        btnObj.transform.rotation = Quaternion.identity;
        btnObj.transform.localScale = new Vector3(0.09f, 0.9f * MenuWidth, 0.08f);
        btnObj.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
        btnObj.AddComponent<Utilities.Button>().relatedText = info.buttonText;

        Renderer renderer = btnObj.GetComponent<Renderer>();
        renderer.material.color = info.enabled
               ? buttonColors[1].colors[0].color
               : buttonColors[0].colors[0].color;

        Text btnText = new GameObject { transform = { parent = menuCanvas.transform } }.AddComponent<Text>();

        btnText.font = currentFont;

        string baseText;

        if (info.isIncremental)
        {
            string value = SoundSettings.GetDisplayName(info.GetCurrentIncrementalValue());
            baseText = $"{info.buttonText} [<color=#00FFFF>{value}</color>]";
        }
        else
        {
            baseText = info.overlapText ?? info.buttonText;
        }

        btnText.text = baseText;

        btnText.supportRichText = true;
        btnText.fontSize = 1;
        btnText.color = info.enabled ? textColors[1] : textColors[0];
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.fontStyle = FontStyle.Italic;
        btnText.resizeTextForBestFit = true;
        btnText.resizeTextMinSize = 0;

        RectTransform textTrans = btnText.GetComponent<RectTransform>();
        textTrans.localPosition = Vector3.zero;
        textTrans.sizeDelta = new Vector2(0.2f * MenuWidth, 0.03f);
        textTrans.localPosition = new Vector3(0.064f, 0f, 0.111f - offset / 2.6f - 0.0025f);
        textTrans.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

        if (FavouriteMods.IsFavourite(info))
        {
            textTrans.sizeDelta = new Vector2(0.15f * MenuWidth, 0.03f);
            FavouriteMods.AddStar(textTrans, 0.2f * MenuWidth);
        }

        if (activeCategory == Category.CustomColors && !info.isCategory)
        {
            float width = 0.9f * MenuWidth;

            btnObj.transform.localScale = new Vector3(0.09f, width - 0.12f, 0.08f);
            btnObj.transform.localPosition = new Vector3(0.56f, 0.06f, 0.28f - offset);
            textTrans.sizeDelta = new Vector2(0.2f * MenuWidth - 0.03f, 0.03f);
            textTrans.localPosition = new Vector3(0.064f, 0.06f * 0.3f, textTrans.localPosition.z);

            BuildDeleteButton(info.buttonText, -(width / 2f - 0.05f), 0.28f - offset, textTrans.localPosition.z);
        }
    }

    private static void BuildDeleteButton(string colorName, float y, float z, float textZ)
    {
        GameObject deleteBtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (!pcMenuOpen)
        {
            deleteBtn.layer = 2;
        }

        Destroy(deleteBtn.GetComponent<Rigidbody>());
        deleteBtn.GetComponent<BoxCollider>().isTrigger = true;
        deleteBtn.transform.parent = activeMenu.transform;
        deleteBtn.transform.rotation = Quaternion.identity;
        deleteBtn.transform.localScale = new Vector3(0.09f, 0.08f, 0.08f);
        deleteBtn.transform.localPosition = new Vector3(0.56f, y, z);
        deleteBtn.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
        deleteBtn.AddComponent<Utilities.Button>().relatedText = "DeleteColor:" + colorName;

        Text deleteText = new GameObject { transform = { parent = menuCanvas.transform } }.AddComponent<Text>();
        deleteText.text = "X";
        deleteText.font = currentFont;
        deleteText.fontSize = 1;
        deleteText.fontStyle = FontStyle.Bold;
        deleteText.color = Color.red;
        deleteText.alignment = TextAnchor.MiddleCenter;
        deleteText.resizeTextForBestFit = true;
        deleteText.resizeTextMinSize = 0;
        RectTransform deleteTextTrans = deleteText.GetComponent<RectTransform>();
        deleteTextTrans.localPosition = Vector3.zero;
        deleteTextTrans.sizeDelta = new Vector2(0.024f, 0.024f);
        deleteTextTrans.localPosition = new Vector3(0.064f, y * 0.3f, textZ);
        deleteTextTrans.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
    }

    public static void RebuildMenu()
    {
        if (activeCategory == Category.EnabledMods)
        {
            EnabledMods.UpdateCategory();
        }

        if (activeCategory == Category.FavouriteMods)
        {
            FavouriteMods.UpdateCategory();
        }

        if (activeCategory == Category.CustomColors)
        {
            Themes.UpdateCustomColors();
        }
        
        KeyboardManager.RefreshColors();

        if (activeMenu != null)
        {
            searchCache.Clear();

            Destroy(activeMenu);
            activeMenu = null;
            BuildMenu();
            PositionMenu(rightHanded, pcMenuOpen);
        }
    }

    public static void PositionMenu(bool isRightHanded, bool isKeyboardMode)
    {
        if (KeyboardManager.keyboardObject != null && !isKeyboardMode)
        {
            PositionSearchMenu();
            return;
        }

        searchReady = false;

        if (Themes.wristMenu && !isKeyboardMode)
        {
            PositionWristMenu(isRightHanded);
        }
        else if (!isKeyboardMode)
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

    private IEnumerator ExplodeMenu(GameObject menu, GameObject background, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (menu == null)
            yield break;

        Color color = background != null ? background.GetComponent<Renderer>().material.color : backgroundColor.colors[0].color;

        if (background != null)
            BreakBackground(background);

        Rigidbody menuBody = menu.GetComponent<Rigidbody>();
        Vector3 velocity = menuBody != null ? menuBody.linearVelocity : Vector3.zero;
        Vector3 center = menu.transform.position;

        StartCoroutine(Flash(center, color));
        SpawnSparks(center, velocity, color);

        List<Transform> parts = new List<Transform>();
        foreach (Transform child in menu.transform)
        {
            if (child.GetComponent<Renderer>() != null && child.gameObject != background)
                parts.Add(child);
        }

        Text[] texts = menu.GetComponentsInChildren<Text>();
        List<Transform> pieces = new List<Transform>();

        foreach (Transform part in parts)
        {
            GameObject piece = new GameObject("MenuPiece");
            piece.transform.SetPositionAndRotation(part.position, part.rotation);
            part.SetParent(piece.transform, true);
            pieces.Add(piece.transform);
            menuPieces.Add(piece);
            RemoveAfter(piece, 8f);
        }

        foreach (Text text in texts)
        {
            Transform nearest = pieces.OrderBy(piece => (piece.position - text.transform.position).sqrMagnitude).FirstOrDefault();

            if (nearest == null)
                continue;

            text.gameObject.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            text.transform.SetParent(nearest, true);
        }

        foreach (Transform piece in pieces)
        {
            Rigidbody body = piece.gameObject.AddComponent<Rigidbody>();
            body.useGravity = !zeroGravityMenu;
            body.interpolation = RigidbodyInterpolation.Interpolate;

            if (zeroGravityMenu)
            {
                body.linearDamping = 0f;
                body.angularDamping = 0f;
            }

            body.linearVelocity = velocity;

            Vector3 push = (piece.position - center).normalized + UnityEngine.Random.insideUnitSphere * 0.5f;
            body.AddForce(push * UnityEngine.Random.Range(3f, 6f), ForceMode.VelocityChange);
            body.AddTorque(UnityEngine.Random.insideUnitSphere * 20f, ForceMode.VelocityChange);

            if (menuColliders)
            {
                UseMenuLayer(piece.gameObject);
                piece.gameObject.AddComponent<MenuBody>();
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
        }

        Destroy(menu);
    }

    private IEnumerator Flash(Vector3 position, Color color)
    {
        GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(flash.GetComponent<Collider>());
        flash.transform.position = position;

        Material material = new Material(Shader.Find("Sprites/Default"));
        flash.GetComponent<Renderer>().material = material;

        Color bright = Color.Lerp(color, Color.white, 0.5f);
        float time = 0f;

        while (time < 0.25f)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / 0.25f);

            flash.transform.localScale = Vector3.one * Mathf.Lerp(0.05f, 0.6f, t);
            bright.a = 1f - t;
            material.color = bright;
            yield return null;
        }

        Destroy(flash);
    }

    private void SpawnSparks(Vector3 position, Vector3 velocity, Color color)
    {
        Color bright = Color.Lerp(color, Color.white, 0.35f);

        for (int i = 0; i < 18; i++)
        {
            GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(spark.GetComponent<Collider>());
            spark.transform.position = position + UnityEngine.Random.insideUnitSphere * 0.05f;
            spark.transform.rotation = UnityEngine.Random.rotation;
            spark.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.012f, 0.03f);
            spark.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default")) { color = bright };

            Rigidbody body = spark.AddComponent<Rigidbody>();
            body.useGravity = !zeroGravityMenu;
            body.linearVelocity = velocity + UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(3f, 7f);
            body.angularVelocity = UnityEngine.Random.insideUnitSphere * 30f;

            StartCoroutine(ShrinkAway(spark, UnityEngine.Random.Range(0.6f, 1.2f)));
        }
    }

    private static int menuLayer = -1;

    private static void UseMenuLayer(GameObject root)
    {
        if (menuLayer < 0)
        {
            int world = GTPlayer.Instance.locomotionEnabledLayers.value;
            menuLayer = 0;

            for (int i = 31; i >= 8; i--)
            {
                if (string.IsNullOrEmpty(LayerMask.LayerToName(i)) && (world & (1 << i)) == 0)
                {
                    menuLayer = i;
                    break;
                }
            }

            if (menuLayer != 0)
            {
                for (int i = 0; i < 32; i++)
                    Physics.IgnoreLayerCollision(menuLayer, i, (world & (1 << i)) == 0);
            }
        }

        foreach (Collider collider in root.GetComponentsInChildren<Collider>(true))
        {
            collider.isTrigger = false;
            collider.gameObject.layer = menuLayer;
        }
    }

    private void RemoveAfter(GameObject target, float delay)
    {
        if (keepMenu)
            return;

        if (disappearAnimation)
            StartCoroutine(ShrinkAway(target, delay));
        else
            Destroy(target, delay);
    }

    private IEnumerator ShrinkAway(GameObject target, float delay)
    {
        const float duration = 0.4f;

        yield return new WaitForSeconds(Mathf.Max(0f, delay - duration));

        if (target == null)
            yield break;

        Vector3 startScale = target.transform.localScale;
        float time = 0f;

        while (time < duration && target != null)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            target.transform.localScale = startScale * (1f - t * t);
            yield return null;
        }

        if (target != null)
            Destroy(target);
    }

    private static void BreakBackground(GameObject background)
    {
        Transform bg = background.transform;
        Color color = background.GetComponent<Renderer>().material.color;

        for (int y = -1; y <= 1; y += 2)
        {
            for (int z = -1; z <= 1; z += 2)
            {
                GameObject quarter = GameObject.CreatePrimitive(PrimitiveType.Cube);

                if (!menuColliders)
                    Destroy(quarter.GetComponent<BoxCollider>());

                quarter.GetComponent<Renderer>().material.color = color;
                quarter.transform.SetParent(bg.parent, false);
                quarter.transform.localRotation = bg.localRotation;
                quarter.transform.localScale = new Vector3(bg.localScale.x, bg.localScale.y / 2f, bg.localScale.z / 2f);
                quarter.transform.localPosition = bg.localPosition + new Vector3(0f, y * bg.localScale.y / 4f, z * bg.localScale.z / 4f);
            }
        }

        Destroy(background);
    }

    private static void ThrowMenu(GameObject menu, GameObject background, Vector3 velocity, Vector3 spin)
    {
        menu.transform.SetParent(null, true);

        Rigidbody rb = menu.AddComponent<Rigidbody>();
        rb.useGravity = !zeroGravityMenu;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.linearVelocity = velocity;
        rb.angularVelocity = spin;

        if (zeroGravityMenu)
        {
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
        }

        if (menuColliders)
        {
            background.AddComponent<BoxCollider>();
            UseMenuLayer(menu);
            menu.AddComponent<MenuBody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }

    private static bool CanClickThrown => thrownMenu != null && throwableMenu && !explodeMenu;

    private void MouseMenuClick()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame || (pcMenuOpen && !Themes.wristMenu))
            return;

        Camera camera = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera")?.GetComponent<Camera>();

        if (camera == null)
            return;

        RaycastHit[] hits = Physics.RaycastAll(camera.ScreenPointToRay(Mouse.current.position.ReadValue()), 100f, ~0, QueryTriggerInteraction.Collide);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            Utilities.Button button = hit.collider.GetComponent<Utilities.Button>();

            if (button == null)
                continue;

            if (activeMenu != null && button.transform.IsChildOf(activeMenu.transform) && triggerCollider != null)
                button.OnTriggerEnter(triggerCollider);
            else if (CanClickThrown && button.transform.IsChildOf(thrownMenu.transform))
                ClickThrown(button.relatedText, rightHanded);

            return;
        }
    }

    private void FingerThrownClick()
    {
        if (!CanClickThrown || GorillaTagger.Instance == null)
        {
            leftFingerPressing = false;
            rightFingerPressing = false;
            return;
        }

        leftFingerPressing = CheckFinger(GorillaTagger.Instance.leftHandTriggerCollider, true, leftFingerPressing);

        if (CanClickThrown)
            rightFingerPressing = CheckFinger(GorillaTagger.Instance.rightHandTriggerCollider, false, rightFingerPressing);
    }

    private bool CheckFinger(GameObject finger, bool isLeft, bool wasPressing)
    {
        if (finger == null)
            return false;

        Vector3 tip = finger.transform.position;

        foreach (Utilities.Button button in thrownMenu.GetComponentsInChildren<Utilities.Button>())
        {
            Collider collider = button.GetComponent<Collider>();

            if (collider == null || (collider.ClosestPoint(tip) - tip).sqrMagnitude > 0.01f * 0.01f)
                continue;

            if (!wasPressing)
                ClickThrown(button.relatedText, isLeft);

            return true;
        }

        return false;
    }

    private void ClickThrown(string text, bool isLeft)
    {
        if (Time.time < Utilities.Button.buttonCooldown)
            return;

        Utilities.Button.buttonCooldown = Time.time + 0.2f;
        GorillaTagger.Instance.StartVibration(isLeft, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
        AudioHandler.Play(SoundSettings.currentButtonSound, 0.5f);

        ProcessClick(text);
        RebuildThrownMenu();
    }

    private void RebuildThrownMenu()
    {
        if (thrownMenu == null || activeMenu != null)
            return;

        Rigidbody body = thrownMenu.GetComponent<Rigidbody>();
        Vector3 velocity = body != null ? body.linearVelocity : Vector3.zero;
        Vector3 spin = body != null ? body.angularVelocity : Vector3.zero;
        Vector3 position = thrownMenu.transform.position;
        Quaternion rotation = thrownMenu.transform.rotation;

        Destroy(thrownMenu);

        BuildMenu();
        GameObject menu = activeMenu;
        activeMenu = null;

        menu.transform.SetPositionAndRotation(position, rotation);
        ThrowMenu(menu, bgObject, velocity, spin);

        thrownMenu = menu;
        RemoveAfter(menu, Mathf.Max(0.5f, thrownExpire - Time.time));
    }

    private static void PunchMenu()
    {
        if (GorillaTagger.Instance == null)
            return;

        Vector3 left = GorillaTagger.Instance.leftHandTransform.position;
        Vector3 right = GorillaTagger.Instance.rightHandTransform.position;

        if (zeroGravityMenu && menuColliders && Time.deltaTime > 0f && Time.time > punchTime)
        {
            Vector3 leftVelocity = (left - lastLeftHand) / Time.deltaTime;
            Vector3 rightVelocity = (right - lastRightHand) / Time.deltaTime;

            List<Rigidbody> bodies = new List<Rigidbody>();

            if (thrownMenu != null && thrownMenu.TryGetComponent(out Rigidbody menuBody))
                bodies.Add(menuBody);

            foreach (GameObject piece in menuPieces)
            {
                if (piece != null && piece.TryGetComponent(out Rigidbody pieceBody))
                    bodies.Add(pieceBody);
            }

            foreach (Rigidbody body in bodies)
            {
                if (TryPunch(body, left, leftVelocity) || TryPunch(body, right, rightVelocity))
                    break;
            }
        }

        lastLeftHand = left;
        lastRightHand = right;
    }

    private static bool TryPunch(Rigidbody body, Vector3 hand, Vector3 handVelocity)
    {
        if (handVelocity.magnitude < 1f)
            return false;

        foreach (Collider collider in body.GetComponentsInChildren<Collider>())
        {
            if (collider.isTrigger || (collider.ClosestPoint(hand) - hand).sqrMagnitude > 0.08f * 0.08f)
                continue;

            body.linearVelocity = handVelocity * 1.5f;
            body.AddTorque(UnityEngine.Random.insideUnitSphere * 5f, ForceMode.VelocityChange);
            punchTime = Time.time + 0.15f;
            return true;
        }

        return false;
    }

    private static void TrackMenuMotion()
    {
        menuMotion.Enqueue((Time.time, activeMenu.transform.position, activeMenu.transform.rotation));

        while (menuMotion.Count > 2 && Time.time - menuMotion.Peek().time > 0.1f)
            menuMotion.Dequeue();
    }

    private static void GetMenuMotion(out Vector3 velocity, out Vector3 spin)
    {
        velocity = Vector3.zero;
        spin = Vector3.zero;

        if (menuMotion.Count < 2)
            return;

        var first = menuMotion.Peek();
        var last = menuMotion.Last();
        float time = last.time - first.time;

        if (time <= 0f)
            return;

        velocity = (last.position - first.position) / time;

        (last.rotation * Quaternion.Inverse(first.rotation)).ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
            angle -= 360f;

        if (!float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
            spin = axis * (angle * Mathf.Deg2Rad / time);
    }

    public static void PositionWristMenu(bool isRightHanded)
    {
        if (activeMenu == null || GTPlayer.Instance == null || GorillaTagger.Instance == null)
            return;

        GTPlayer player = GTPlayer.Instance;
        float scale = player.scale;

        var hand = isRightHanded
            ? ControllerUtilities.GetTrueRightHand()
            : ControllerUtilities.GetTrueLeftHand();

        Vector3 handPosition = hand.position + new Vector3(0.08f, 0.25f * scale, 0f);

        Vector3 headPos = GorillaTagger.Instance.headCollider.transform.position;

        Quaternion faceUser = Quaternion.LookRotation(handPosition - headPos, Vector3.up);
        Quaternion targetRotation = faceUser * Quaternion.Euler(-90f, 180f, -90f);

        if (!wristReady)
        {
            wristPosition = handPosition;
            wristRotation = targetRotation;
            wristReady = true;
        }

        float t = 1f - Mathf.Exp(-14f * Time.deltaTime);
        wristPosition = Vector3.Lerp(wristPosition, handPosition, t);
        wristRotation = Quaternion.Slerp(wristRotation, targetRotation, t);

        activeMenu.transform.SetPositionAndRotation(wristPosition, wristRotation);
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

    public static void CreateImage()
    {
        if (Variables.backgroundTexture != null)
        {
            GameObject iconBack = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(iconBack.GetComponent<Rigidbody>());
            Destroy(iconBack.GetComponent<BoxCollider>());
            Destroy(iconBack.GetComponent<MeshCollider>());

            iconBack.transform.parent = bgObject.transform;
            iconBack.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
            iconBack.transform.localPosition = new Vector3(-0.51f, 0f, 0f);
            iconBack.transform.localScale = new Vector3(0.5f / MenuWidth, 0.5f, 1f);

            Renderer iconRenderer = iconBack.GetComponent<Renderer>();
            Material iconMaterial = new Material(Shader.Find("UI/Default"));
            iconMaterial.mainTexture = Variables.backgroundTexture;
            iconRenderer.material = iconMaterial;
            iconRenderer.material.color = new Color(1, 1, 1, 0.6f);
            iconRenderer.sortingOrder = 1;
        }

        menuCanvas = new GameObject();
        menuCanvas.transform.parent = activeMenu.transform;
        Canvas canvasComp = menuCanvas.AddComponent<Canvas>();
        CanvasScaler scalerComp = menuCanvas.AddComponent<CanvasScaler>();
        menuCanvas.AddComponent<GraphicRaycaster>();
        canvasComp.renderMode = RenderMode.WorldSpace;
        scalerComp.dynamicPixelsPerUnit = 1000f;
    }

    public static void ChangePage(bool next)
    {
        int totalPages = (CurrentButtons().Length + PageSize - 1) / PageSize;
        if (totalPages <= 1)
            return;

        if (next)
        {
            activePage++;
            if (activePage >= totalPages)
                activePage = 0;
        }
        else
        {
            activePage--;
            if (activePage < 0)
                activePage = totalPages - 1;
        }

        RebuildMenu();
    }

    public static void ProcessClick(string text)
    {
        if (text == "Search")
        {
            Variables.playerInstance = GTPlayer.Instance;
            pcSearch = !KeyboardManager.isSearching && (pcMenuOpen || (Mouse.current != null && Mouse.current.leftButton.isPressed));
            Variables.InPcCondition = pcSearch;
            KeyboardManager.ToggleKeyboard();
            return;
        }

        if (text.StartsWith("DeleteColor:"))
        {
            Themes.DeleteColor(text.Substring("DeleteColor:".Length));
            RebuildMenu();
            return;
        }

        if (text == "PreviousPage" || text == "NextPage")
        {
            ChangePage(text == "NextPage");
            return;
        }

        if (text == "Disconnect")
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.Disconnect();
            }
            return;
        }

        ModButtonInfo target = FindButton(text);
        if (target != null && FavouriteMods.HoldingFavouriteGrip() && FavouriteMods.CanFavourite(target))
        {
            FavouriteMods.Toggle(target);
            RebuildMenu();
            SettingsSaver.Save();
            return;
        }

        if (target != null)
        {
            if (target.isIncremental)
            {
                target.CycleIncrementalValue();

                NotificationLib.SendNotification(
                    NotificationLib.NotificationType.Info,
                    $"Changed to: {target.GetCurrentIncrementalValue()}"
                );
            }
            else if (target.isTogglable)
            {
                target.enabled = !target.enabled;
                if (target.enabled)
                {
                    target.enableMethod?.Invoke();

                    if (!string.IsNullOrEmpty(target.toolTip))
                    {
                        NotificationLib.SendNotification(
                            NotificationLib.NotificationType.Enabled,
                            target.toolTip
                        );
                    }
                }
                else
                {
                    target.disableMethod?.Invoke();

                    if (!string.IsNullOrEmpty(target.toolTip))
                    {
                        NotificationLib.SendNotification(
                            NotificationLib.NotificationType.Disabled,
                            target.toolTip
                        );
                    }
                }
            }
            else
            {
                target.method?.Invoke();
            }
        }
        else
        {
            Debug.LogError($"{text} does not exist");
        }

        RebuildMenu();
        SettingsSaver.Save();
    }

    public static ModButtonInfo FindButton(string text)
    {
        if (text == null)
            return null;

        foreach (ModButtonInfo result in CurrentButtons())
        {
            if (result != null && result.buttonText == text)
                return result;
        }

        if (Buttons.TryGetValue(activeCategory, out var categoryButtons))
        {
            foreach (var button in categoryButtons)
            {
                if (button != null && button.buttonText == text)
                    return button;
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

    static Material mat = null;
    static bool mat1;
    public static VRRig ghostRig;

    public static void Overseer()
    {
        if (Variables.Overseer)
        {
            if (mat == null)
                mat = new Material(Shader.Find("GUI/Text Shader")) { color = new Color32(255, 255, 255, 90) };

            if (ghostRig == null)
            {
                ghostRig = Object.Instantiate<VRRig>(
                    GorillaTagger.Instance.offlineVRRig,
                    GorillaLocomotion.GTPlayer.Instance.transform.position,
                    GorillaLocomotion.GTPlayer.Instance.transform.rotation
                );
                ghostRig.enabled = false;
                ghostRig.transform.position = Vector3.zero;

                ghostRig.transform.Find("VR Constraints/LeftArm/Left Arm IK/SlideAudio").gameObject.SetActive(false);
                ghostRig.transform.Find("VR Constraints/RightArm/Right Arm IK/SlideAudio").gameObject.SetActive(false);
            }

            if (!GorillaTagger.Instance.offlineVRRig.enabled)
            {
                mat1 = false;
                ghostRig.enabled = true;
                ghostRig.rightHandTransform.position = GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position;
                ghostRig.leftHandTransform.position = GorillaLocomotion.GTPlayer.Instance.LeftHand.controllerTransform.position;
                ghostRig.mainSkin.material = mat;
            }
            else
            {
                if (!mat1)
                {
                    ghostRig.enabled = false;
                    ghostRig.mainSkin.material = null;
                    ghostRig.transform.position = Vector3.zero;
                    mat1 = true;
                }
            }
        }
        else if (ghostRig != null)
        {
            Destroy(ghostRig);
            ghostRig = null;
        }
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

    private class MenuBody : MonoBehaviour
    {
        private const float Radius = 0.04f;

        public Action hit;
        private Rigidbody body;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Hit();
        }

        private void FixedUpdate()
        {
            if (body == null || GTPlayer.Instance == null)
                return;

            Vector3 velocity = body.linearVelocity;
            float distance = velocity.magnitude * Time.fixedDeltaTime;

            if (distance < 0.0001f)
                return;

            Vector3 direction = velocity.normalized;

            if (!Physics.SphereCast(body.position, Radius, direction, out RaycastHit ground, distance + 0.01f, GTPlayer.Instance.locomotionEnabledLayers, QueryTriggerInteraction.Ignore))
                return;

            body.position += direction * Mathf.Max(0f, ground.distance - 0.005f);

            Vector3 normalPart = Vector3.Project(velocity, ground.normal);
            Vector3 tangent = velocity - normalPart;
            float bounce = normalPart.magnitude > 0.5f ? 0.3f : 0f;

            body.linearVelocity = tangent * 0.8f - normalPart * bounce;
            body.angularVelocity *= 0.8f;

            Hit();
        }

        private void Hit()
        {
            Action action = hit;
            hit = null;
            action?.Invoke();
        }
    }
}