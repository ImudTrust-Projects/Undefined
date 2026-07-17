using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using Undefined.Menu;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Undefined.Menu;

public class SearchAndKeyboard : MonoBehaviour
{
    public class KeyBounceHandler : MonoBehaviour
    {
        public string keyValue;
        public Vector3 originalScale;
        public GameObject roundedContainer;

        private Coroutine _activeBounce;
        private static float _lastFrameClick;

        private static float ElasticOut(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - 0.1f) * (MathF.PI * 2f) / 0.4f) + 1f;
        }

        private IEnumerator BounceRoutine()
        {
            Transform target = roundedContainer != null ? roundedContainer.transform : transform;
            Vector3 baseScale = roundedContainer != null ? Vector3.one : originalScale;
            Vector3 punched = new Vector3(baseScale.x * 1.25f, baseScale.y * 1.25f, baseScale.z * 0.7f);

            float half = 0.063f;
            float elapsed = 0f;

            while (elapsed < half)
            {
                if (this == null) yield break;
                float t = elapsed / half;
                Vector3 scale = Vector3.LerpUnclamped(baseScale, punched, 1f - Mathf.Pow(1f - t, 3f));
                target.localScale = scale;

                if (roundedContainer != null)
                {
                    transform.localScale = new Vector3(scale.x * originalScale.x,
                                                       scale.y * originalScale.y,
                                                       scale.z * originalScale.z);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            float rest = 0.18f - half;

            while (elapsed < rest)
            {
                if (this == null) yield break;
                float t = elapsed / rest;
                Vector3 scale = Vector3.LerpUnclamped(punched, baseScale, ElasticOut(t));
                target.localScale = scale;

                if (roundedContainer != null)
                {
                    transform.localScale = new Vector3(scale.x * originalScale.x,
                                                       scale.y * originalScale.y,
                                                       scale.z * originalScale.z);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (this != null)
            {
                target.localScale = baseScale;
                transform.localScale = originalScale;
            }
            _activeBounce = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name != "keyclicker") return;
            if (Time.frameCount < _lastFrameClick + 12.5f) return;

            _lastFrameClick = Time.frameCount;
            HandleKeyPress(keyValue);
        }

        public void TriggerBounce()
        {
            if (!gameObject.activeInHierarchy) return;

            if (_activeBounce != null)
                StopCoroutine(_activeBounce);

            _activeBounce = StartCoroutine(BounceRoutine());
        }
    }

    public static Text searchDisplay;
    public static string currentInput = "";
    public static GameObject keyboardObject;
    public static bool isSearching;
    public static bool showSearchText;
    public static bool isTyping;

    private static Material _backgroundMat;
    private static Material _borderMat;
    private static readonly List<Material> _materials = new List<Material>();
    private static readonly List<GameObject> _keyObjects = new List<GameObject>();
    private static readonly List<Mesh> _meshes = new List<Mesh>();

    public static List<KeyCode> lastKeys = new List<KeyCode>();
    private static readonly Dictionary<string, KeyBounceHandler> _keyHandlers = new Dictionary<string, KeyBounceHandler>();

    private const float BOUNCE_PUNCH = 1.25f;
    private const float BOUNCE_TIME = 0.18f;

    private static readonly KeyCode[] _allowedKeys = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
        KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
        KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z,
        KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
        KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,
        KeyCode.Space, KeyCode.Backspace, KeyCode.Delete
    };

    public static string placeholder = "Type to search...";
    public static float savedMenuScale = -1f;
    private static float _blinkTimer;
    private static bool _cursorVisible = true;

    public static Action<string> onComplete;
    public static Action onCancel;

    private static Material _clickerMat1;
    private static Material _clickerMat2;

    public static KeyCode[] AllowedKeys => _allowedKeys;

    private static GameObject MakePrimitive(Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(obj.GetComponent<Rigidbody>());
        Destroy(obj.GetComponent<BoxCollider>());
        obj.layer = LayerMask.NameToLayer("UI");
        obj.transform.SetParent(parent, false);
        obj.transform.localRotation = Quaternion.identity;
        _keyObjects.Add(obj);
        return obj;
    }

    private static void MakeClicker(ref GameObject clicker, Transform parent, ref Material mat)
    {
        if (clicker != null) return;

        clicker = new GameObject("keyclicker");
        clicker.AddComponent<BoxCollider>().isTrigger = true;
        clicker.layer = LayerMask.NameToLayer("UI");
        clicker.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");

        mat = new Material(Shader.Find("GorillaTag/UberShader")) { color = Color.white };
        clicker.AddComponent<MeshRenderer>().material = mat;

        if (parent != null)
        {
            clicker.transform.SetParent(parent);
            clicker.transform.localScale = new Vector3(0.0035f, 0.0035f, 0.0035f);
            clicker.transform.localPosition = new Vector3(0f, -0.1f, 0f);
        }
    }

    public static void CloseKeyboard()
    {
        isSearching = false;
        isTyping = false;
        onComplete = null;
        onCancel = null;
        showSearchText = false;

        Cleanup();
        Main.RebuildMenu();
    }

    private static Material MakeButtonMaterial()
    {
        Material m = new Material(Shader.Find("GorillaTag/UberShader"));
        m.color = Undefined.MENUSETTINGS.Settings.buttonColors[0].colors[0].color;
        _materials.Add(m);
        return m;
    }

    public static void HandleInput()
    {
        if (!Variables.InPcCondition || (!isSearching && !isTyping)) return;

        lastKeys.Clear();
        foreach (KeyCode key in AllowedKeys)
        {
            if (UnityInput.Current.GetKeyDown(key))
            {
                HandleKeyPress(KeyToString(key));
                lastKeys.Add(key);
            }
        }
    }

    public static void SetupClickerLeft(Transform parent)
    {
        MakeClicker(ref Variables.keyclickerObj1, parent, ref _clickerMat1);
    }

    public static int FuzzyScore(string text, string query)
    {
        if (string.IsNullOrEmpty(query)) return 0;
        if (string.IsNullOrEmpty(text)) return int.MinValue;

        string t = text.ToLowerInvariant();
        string q = query.ToLowerInvariant();

        int idx = t.IndexOf(q);
        if (idx == 0) return 10000;
        if (idx > 0) return 5000 - idx;

        int score = 0;
        int qPos = 0;
        int last = -1;
        int streak = 0;

        for (int i = 0; i < t.Length && qPos < q.Length; i++)
        {
            if (t[i] == q[qPos])
            {
                if (last == i - 1) streak++;
                else streak = 0;

                score += 10 + streak * 8;
                if (i == 0 || t[i - 1] == ' ') score += 15;

                last = i;
                qPos++;
            }
        }

        return qPos < q.Length ? int.MinValue : score - (t.Length - q.Length);
    }

    public static void SetupClickerRight(Transform parent)
    {
        MakeClicker(ref Variables.keyclickerObj2, parent, ref _clickerMat2);
    }

    public static void Cleanup()
    {
        if (keyboardObject != null)
        {
            Destroy(keyboardObject);
            keyboardObject = null;
        }

        if (Variables.keyclickerObj1 != null)
        {
            Destroy(Variables.keyclickerObj1);
            Variables.keyclickerObj1 = null;
        }

        if (Variables.keyclickerObj2 != null)
        {
            Destroy(Variables.keyclickerObj2);
            Variables.keyclickerObj2 = null;
        }

        foreach (Material m in _materials)
        {
            if (m != null) Destroy(m);
        }
        _materials.Clear();

        foreach (GameObject obj in _keyObjects)
        {
            if (obj != null) Destroy(obj);
        }
        _keyObjects.Clear();

        foreach (Mesh mesh in _meshes)
        {
            if (mesh != null) Destroy(mesh);
        }
        _meshes.Clear();

        if (_clickerMat1 != null)
        {
            Destroy(_clickerMat1);
            _clickerMat1 = null;
        }

        if (_clickerMat2 != null)
        {
            Destroy(_clickerMat2);
            _clickerMat2 = null;
        }

        _backgroundMat = null;
        _borderMat = null;
        _keyHandlers.Clear();
        currentInput = "";
    }

    public static void BuildKeyboard()
    {
        if (keyboardObject != null)
        {
            Destroy(keyboardObject);
            keyboardObject = null;
        }

        foreach (Material m in _materials)
        {
            if (m != null) Destroy(m);
        }
        _materials.Clear();

        foreach (GameObject obj in _keyObjects)
        {
            if (obj != null) Destroy(obj);
        }
        _keyObjects.Clear();

        foreach (Mesh mesh in _meshes)
        {
            if (mesh != null) Destroy(mesh);
        }
        _meshes.Clear();

        _keyHandlers.Clear();

        keyboardObject = new GameObject("Keyboard");

        GameObject bg = MakePrimitive(keyboardObject.transform);
        bg.name = "KeyboardBackground";
        bg.transform.localScale = new Vector3(0.485f, 0.24f, 0.005f);
        bg.transform.localPosition = new Vector3(0f, 0.002f, 0.005f);

        _backgroundMat = MakeBackgroundMaterial();
        bg.GetComponent<Renderer>().sharedMaterial = _backgroundMat;

        string[] rows = { "1234567890", "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM" };
        float keyW = 0.04f;
        float spacing = 0.005f;
        float yPos = 0.092f;

        foreach (string row in rows)
        {
            float totalW = row.Length * keyW + (row.Length - 1) * spacing;
            float startX = -totalW / 2f + keyW / 2f;

            foreach (char c in row)
            {
                MakeKey(c.ToString(), new Vector3(startX, yPos, 0f), keyW);
                startX += keyW + spacing;
            }
            yPos -= keyW + spacing;
        }

        float bottomY = yPos;
        float spaceW = keyW * 5f;
        float backW = keyW * 2.1f;
        float enterW = keyW * 2.1f;

        float totalBottom = spaceW + spacing + backW + spacing + enterW;
        float bottomStart = -totalBottom / 2f;

        MakeKey("SPACE", new Vector3(bottomStart + spaceW / 2f, bottomY, 0f), spaceW);
        MakeKey("BACK", new Vector3(bottomStart + spaceW + spacing + backW / 2f, bottomY, 0f), backW);
        MakeKey("ENTER", new Vector3(bottomStart + spaceW + spacing + backW + spacing + enterW / 2f, bottomY, 0f), enterW);
    }

    private static Material MakeBackgroundMaterial()
    {
        Material m = new Material(Shader.Find("GorillaTag/UberShader"));
        m.color = Undefined.MENUSETTINGS.Settings.backgroundColor.colors[0].color;
        _materials.Add(m);
        return m;
    }

    private static void MakeKey(string key, Vector3 pos, float width, float height = 0.04f)
    {
        GameObject keyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        keyObj.GetComponent<BoxCollider>().isTrigger = true;
        keyObj.layer = LayerMask.NameToLayer("UI");
        keyObj.transform.SetParent(keyboardObject.transform, false);
        keyObj.transform.localScale = new Vector3(width - 0.0025f, height - 0.0025f, 0.012f);
        keyObj.transform.localPosition = pos;
        keyObj.transform.localRotation = Quaternion.identity;
        _keyObjects.Add(keyObj);

        Rigidbody rb = keyObj.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        keyObj.GetComponent<Renderer>().sharedMaterial = MakeButtonMaterial();

        GameObject canvas = new GameObject("KeyCanvas");
        canvas.transform.SetParent(keyObj.transform, false);
        canvas.transform.localPosition = new Vector3(0f, 0f, -0.52f);
        canvas.transform.localRotation = Quaternion.identity;

        float scale = 0.01f / (width - 0.0025f) * (height - 0.0025f);
        canvas.transform.localScale = new Vector3(scale, 0.01f, 0.01f);
        canvas.layer = LayerMask.NameToLayer("UI");
        _keyObjects.Add(canvas);

        Canvas c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;
        c.sortingOrder = 10;

        Text label = new GameObject("Label").AddComponent<Text>();
        label.transform.SetParent(canvas.transform, false);
        label.font = Undefined.MENUSETTINGS.Settings.currentFont;
        label.text = key;
        label.fontSize = 50;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleCenter;
        label.resizeTextForBestFit = true;
        label.resizeTextMinSize = 10;
        label.resizeTextMaxSize = 50;

        RectTransform rect = label.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200f, 200f);
        rect.localPosition = Vector3.zero;
        rect.localRotation = Quaternion.identity;

        KeyBounceHandler handler = keyObj.AddComponent<KeyBounceHandler>();
        handler.keyValue = key;
        handler.originalScale = keyObj.transform.localScale;
        _keyHandlers[key] = handler;
    }

    public static string KeyToString(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Backspace: return "BACK";
            case KeyCode.Return: return "ENTER";
            case KeyCode.Space: return "SPACE";
            default:
                if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
                    return (key - KeyCode.Alpha0).ToString();
                return key.ToString();
        }
    }

    public static void CloseTyping(bool canceled = false)
    {
        string result = currentInput.Trim();
        isSearching = false;
        isTyping = false;
        showSearchText = false;

        Action<string> done = onComplete;
        Action cancel = onCancel;
        onComplete = null;
        onCancel = null;

        Cleanup();
        Main.RebuildMenu();

        if (canceled)
            cancel?.Invoke();
        else
            done?.Invoke(result);
    }

    public static void HandleKeyPress(string key)
    {
        if (searchDisplay == null) return;

        if (_keyHandlers.TryGetValue(key, out KeyBounceHandler handler) && handler != null)
            handler.TriggerBounce();

        switch (key)
        {
            case "SPACE":
                currentInput += " ";
                break;
            case "BACK":
                if (currentInput.Length > 0)
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                break;
            case "ENTER":
                if (isTyping)
                    CloseTyping();
                else
                    CloseKeyboard();
                return;
            default:
                currentInput += key;
                break;
        }

        if (GorillaTagger.Instance != null)
        {
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, true, 0.625f);
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, false, 0.625f);
        }

        searchDisplay.text = currentInput;
        Variables.activePage = 0;

        if (isSearching && !isTyping)
            Main.RebuildMenu();
    }

    public static void OpenTyping(string prefill = "", string placeholderText = "Type here...")
    {
        onComplete = null;
        onCancel = null;
        isSearching = true;
        isTyping = true;
        showSearchText = true;
        currentInput = prefill;
        placeholder = placeholderText;

        if (!Variables.InPcCondition)
        {
            BuildKeyboard();
            SetupClickerLeft(Variables.playerInstance.RightHand.controllerTransform);
            SetupClickerRight(Variables.playerInstance.LeftHand.controllerTransform);
        }

        Main.RebuildMenu();
    }

    public static void UpdateBlink()
    {
        if (!showSearchText || searchDisplay == null) return;

        _blinkTimer += Time.deltaTime;
        if (_blinkTimer >= 0.5f)
        {
            _blinkTimer = 0f;
            _cursorVisible = !_cursorVisible;

            string text = string.IsNullOrEmpty(currentInput) ? placeholder : currentInput;
            searchDisplay.text = text + (_cursorVisible ? "|" : "");
        }
    }

    public static void ToggleKeyboard()
    {
        if (isSearching)
            CloseKeyboard();
        else
            OpenKeyboard(false);
    }

    public static void OpenKeyboard(bool typingMode, string prefill = "", string placeholderText = "Type to search...")
    {
        isSearching = true;
        isTyping = typingMode;
        showSearchText = true;
        currentInput = prefill;
        placeholder = placeholderText;

        if (!Variables.InPcCondition)
        {
            BuildKeyboard();
            SetupClickerLeft(Variables.playerInstance.RightHand.controllerTransform);
            SetupClickerRight(Variables.playerInstance.LeftHand.controllerTransform);
        }

        Main.RebuildMenu();
    }
}