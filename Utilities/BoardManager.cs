using GorillaNetworking;
using Photon.Pun;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Undefined.MENUSETTINGS;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Undefined.Utilities;

public class BoardManager : MonoBehaviour
{
    private static string WebsiteMOTD = "Loading MOTD...";

    private Coroutine updateRoutine;

    private readonly Dictionary<string, GameObject> boards = new();

    private readonly Dictionary<Renderer, Material> boardRenderers = new();
    private readonly Dictionary<TextMeshPro, Renderer> boardPlates = new();
    private readonly Dictionary<JoinTriggerUITemplate, Material[]> joinScreens = new();
    private Material boardMaterial;
    private float boardCheckTime;

    private static readonly Color DefaultBoardColor = new Color32(15, 15, 15, 255);


    private static string MenuColor =>
        ColorUtility.ToHtmlStringRGB(Settings.backgroundColor.colors[0].color);

    private static string MenuColorTag =>
        $"#{MenuColor}";


    private static string MOTDTitle =>
        $"[ <color={MenuColorTag}>{Constants.PluginName}</color> ]";


    private static string CoCTitle =>
        $"[ <color={MenuColorTag}>{Constants.PluginName}</color> ]";


    private static string CoCText =>
        $"<color={MenuColorTag}>{Constants.PluginName}</color>\n\n" +
        "================ Credits ================\n" +
        "Created by <color=white>ImudTrust-Projects</color>\n\n" +
        "Thanks to all GitHub contributors\n" +
        "who helped build, test, and improve\n" +
        "Undefined.\n" +
        "========================================\n\n" +
        "Thank you for supporting Undefined.";


    private static string RemoteText =>
        $"<color={MenuColorTag}>{Constants.PluginName}</color>\n" +
        "------------------------------------------\n" +
        "Location: <color=white>{0}</color>\n" +
        "Status: <color=green>Encrypted</color>";


    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        StartCoroutine(LoadWebsiteMOTD());
    }


    private void Start()
    {
        CreateBoards();
        StartUpdating();
    }


    private void StartUpdating()
    {
        if (updateRoutine != null)
            CoroutineManager.EndCoroutine(updateRoutine);

        updateRoutine = CoroutineManager.RunCoroutine(UpdateBoardText());
    }


    private IEnumerator UpdateBoardText()
    {
        for (int i = 0; i < 12; i++)
        {
            UpdateStumpBranding();

            yield return new WaitForSeconds(0.5f);
        }
    }


    private void Update()
    {
        if (!Variables.customBoardColor)
        {
            if (boardRenderers.Count > 0 || joinScreens.Count > 0 || boardPlates.Count > 0)
                ResetBoards();

            return;
        }

        if (boardMaterial == null)
            boardMaterial = new Material(Shader.Find("GorillaTag/UberShader"));

        Color color = Settings.backgroundColor.colors[0].color;
        boardMaterial.color = color;

        if (Time.time > boardCheckTime)
        {
            boardCheckTime = Time.time + 3f;

            try
            {
                ColorBoards();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{Constants.PluginName} Board Color Err: {ex.Message}");
            }
        }

        foreach (Renderer plate in boardPlates.Values)
        {
            if (plate != null)
                plate.material.color = color;
        }

        foreach (GameObject board in boards.Values)
        {
            if (board != null)
                board.GetComponent<Renderer>().material.color = color;
        }
    }

    private void ColorBoards()
    {
        foreach (MeshCollider collider in FindObjectsByType<MeshCollider>(FindObjectsSortMode.None))
        {
            string name = collider.name;

            if (name == "wallmonitorforestbg" || name == "wallmonitorscreen_small")
                ColorRenderer(collider.GetComponent<Renderer>());
        }

        foreach (TextMeshPro text in FindObjectsByType<TextMeshPro>(FindObjectsSortMode.None))
        {
            if (IsBoardText(text.name) && !boardPlates.ContainsKey(text))
                boardPlates[text] = CreatePlate(text);
        }

        GameObject monitor = Variables.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen");
        if (monitor != null)
            ColorRenderer(monitor.GetComponent<Renderer>());

        if (PhotonNetworkController.Instance == null)
            return;

        foreach (GorillaNetworkJoinTrigger joinTrigger in PhotonNetworkController.Instance.allJoinTriggers)
        {
            JoinTriggerUITemplate template = joinTrigger?.ui?.template;

            if (template == null || joinScreens.ContainsKey(template))
                continue;

            joinScreens[template] = new Material[]
            {
                template.ScreenBG_AbandonPartyAndSoloJoin,
                template.ScreenBG_AlreadyInRoom,
                template.ScreenBG_ChangingGameModeSoloJoin,
                template.ScreenBG_Error,
                template.ScreenBG_InPrivateRoom,
                template.ScreenBG_LeaveRoomAndGroupJoin,
                template.ScreenBG_LeaveRoomAndSoloJoin,
                template.ScreenBG_NotConnectedSoloJoin
            };

            SetJoinScreens(template, new Material[] { boardMaterial, boardMaterial, boardMaterial, boardMaterial, boardMaterial, boardMaterial, boardMaterial, boardMaterial });
        }

        PhotonNetworkController.Instance.UpdateTriggerScreens();
    }

    private static bool IsBoardText(string name)
    {
        return name == "motdHeadingText" || name == "CodeOfConductHeadingText" || name == "WelcomeToGorilllaTagHeadingText";
    }

    private static Renderer CreatePlate(TextMeshPro heading)
    {
        RectTransform root = heading.rectTransform;
        Vector3[] corners = new Vector3[4];
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        foreach (TMP_Text text in heading.GetComponentsInChildren<TMP_Text>(true))
        {
            text.rectTransform.GetWorldCorners(corners);

            foreach (Vector3 corner in corners)
            {
                Vector3 local = root.InverseTransformPoint(corner);
                min = Vector2.Min(min, local);
                max = Vector2.Max(max, local);
            }
        }

        float glyphBottom = float.MaxValue;
        float glyphTop = float.MinValue;

        foreach (TMP_Text text in heading.GetComponentsInChildren<TMP_Text>(true))
        {
            Bounds bounds = text.textBounds;

            if (bounds.size.sqrMagnitude <= 0f)
                continue;

            glyphBottom = Mathf.Min(glyphBottom, root.InverseTransformPoint(text.transform.TransformPoint(bounds.min)).y, root.InverseTransformPoint(text.transform.TransformPoint(bounds.max)).y);
            glyphTop = Mathf.Max(glyphTop, root.InverseTransformPoint(text.transform.TransformPoint(bounds.min)).y, root.InverseTransformPoint(text.transform.TransformPoint(bounds.max)).y);
        }

        Vector2 padding = (max - min) * 0.04f;
        min -= padding;
        max += padding;

        if (glyphBottom < float.MaxValue)
        {
            min.y = Mathf.Max(min.y, glyphBottom - padding.y);
            max.y = Mathf.Min(max.y, glyphTop + padding.y);
        }

        GameObject plate = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(plate.GetComponent<MeshCollider>());
        plate.name = $"{Constants.PluginName}BoardColor";
        plate.transform.SetParent(root, false);
        plate.transform.localRotation = Quaternion.identity;
        plate.transform.localPosition = (min + max) / 2f;
        plate.transform.localScale = new Vector3(max.x - min.x, max.y - min.y, 1f);
        plate.transform.position -= root.forward * 0.0005f;

        Renderer renderer = plate.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default")) { renderQueue = 2001 };
        return renderer;
    }

    private void ColorRenderer(Renderer renderer)
    {
        if (renderer == null || renderer.sharedMaterial == boardMaterial)
            return;

        if (!boardRenderers.ContainsKey(renderer))
            boardRenderers[renderer] = renderer.sharedMaterial;

        renderer.sharedMaterial = boardMaterial;
    }

    private static void SetJoinScreens(JoinTriggerUITemplate template, Material[] materials)
    {
        template.ScreenBG_AbandonPartyAndSoloJoin = materials[0];
        template.ScreenBG_AlreadyInRoom = materials[1];
        template.ScreenBG_ChangingGameModeSoloJoin = materials[2];
        template.ScreenBG_Error = materials[3];
        template.ScreenBG_InPrivateRoom = materials[4];
        template.ScreenBG_LeaveRoomAndGroupJoin = materials[5];
        template.ScreenBG_LeaveRoomAndSoloJoin = materials[6];
        template.ScreenBG_NotConnectedSoloJoin = materials[7];
    }

    private void ResetBoards()
    {
        foreach (var pair in boardRenderers)
        {
            if (pair.Key != null)
                pair.Key.sharedMaterial = pair.Value;
        }

        boardRenderers.Clear();

        foreach (Renderer plate in boardPlates.Values)
        {
            if (plate != null)
                Destroy(plate.gameObject);
        }

        boardPlates.Clear();

        foreach (var pair in joinScreens)
        {
            if (pair.Key != null)
                SetJoinScreens(pair.Key, pair.Value);
        }

        joinScreens.Clear();

        if (PhotonNetworkController.Instance != null)
            PhotonNetworkController.Instance.UpdateTriggerScreens();

        foreach (GameObject board in boards.Values)
        {
            if (board != null)
                board.GetComponent<Renderer>().material.color = DefaultBoardColor;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (updateRoutine != null)
            CoroutineManager.EndCoroutine(updateRoutine);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CreateBoards();
        StartUpdating();
    }


    private IEnumerator LoadWebsiteMOTD()
    {
        using UnityWebRequest request = UnityWebRequest.Get(Constants.UndefinedDataUrl);

        yield return request.SendWebRequest();


        if (request.result != UnityWebRequest.Result.Success)
        {
            WebsiteMOTD =
                $"Welcome to {Constants.PluginName}\n\n" +
                "Unable to load MOTD.";

            UpdateStumpBranding();
            yield break;
        }


        JObject data = JObject.Parse(request.downloadHandler.text);

        WebsiteMOTD = data["motd"]?.ToString();


        if (string.IsNullOrEmpty(WebsiteMOTD))
        {
            WebsiteMOTD =
                $"Welcome to {Constants.PluginName}\n\n" +
                "No MOTD has been set.";
        }


        UpdateStumpBranding();
    }


    private void CreateBoards()
    {
        UpdateStumpBranding();

        string scene = SceneManager.GetActiveScene().name;

        if (BoardInformations.TryGetValue(scene, out BoardInfo info))
            CreateBoard(scene, info);
    }


    private void UpdateStumpBranding()
    {
        SetText(
            "Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText",
            MOTDTitle
        );


        string motd = WebsiteMOTD;

        try
        {
            motd = string.Format(
                WebsiteMOTD,
                Constants.PluginVersion,
                Constants.PluginName,
                PhotonNetwork.LocalPlayer?.NickName ?? "Unknown",
                "ImudTrust-Projects"
            );
        }
        catch
        {
        }


        SetText(
            "Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText",
            motd
        );


        SetText(
            "Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText",
            CoCTitle
        );


        SetText(
            "Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData",
            CoCText
        );
    }


    private void SetText(string path, string text)
    {
        GameObject obj = GameObject.Find(path);

        if (!obj)
            return;


        TextMeshPro tmp = obj.GetComponent<TextMeshPro>();

        if (!tmp)
            return;


        tmp.richText = true;
        tmp.text = text;
    }
    
    private void CreateBoard(string scene, BoardInfo info)
    {
        RemoveBoard(scene);

        GameObject parent = GameObject.Find(info.Path);

        if (!parent)
            return;


        GameObject board = GameObject.CreatePrimitive(PrimitiveType.Plane);

        board.name = $"{Constants.PluginName}Board";

        board.transform.SetParent(parent.transform, false);
        board.transform.localPosition = info.Pos;
        board.transform.localRotation = Quaternion.Euler(info.Rot);
        board.transform.localScale = info.Scale;


        Destroy(board.GetComponent<Collider>());


        Renderer renderer = board.GetComponent<Renderer>();

        if (renderer)
        {
            renderer.material.shader = Shader.Find("GorillaTag/UberShader");
            renderer.material.color = new Color32(15, 15, 15, 255);
        }


        CreateBoardText(board, scene);

        boards[scene] = board;
    }


    private void CreateBoardText(GameObject board, string scene)
    {
        GameObject textObject = new($"{Constants.PluginName}Text");

        textObject.transform.SetParent(board.transform, false);
        textObject.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        textObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        textObject.transform.localScale = Vector3.one * 0.01f;


        TextMeshPro text = textObject.AddComponent<TextMeshPro>();

        text.richText = true;
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 2f;
        text.text = string.Format(RemoteText, scene);
    }


    private void RemoveBoard(string scene)
    {
        if (!boards.TryGetValue(scene, out GameObject board))
            return;


        if (board)
            Destroy(board);


        boards.Remove(scene);
    }


    private struct BoardInfo
    {
        public string Path;
        public Vector3 Pos;
        public Vector3 Rot;
        public Vector3 Scale;


        public BoardInfo(string path, Vector3 pos, Vector3 rot, Vector3 scale)
        {
            Path = path;
            Pos = pos;
            Rot = rot;
            Scale = scale;
        }
    }


    private static readonly Dictionary<string, BoardInfo> BoardInformations = new()
    {
        ["Canyon2"] = new(
            "Canyon/CanyonScoreboardAnchor/GorillaScoreBoard",
            new Vector3(-24.5f, -28.7f, 0.1f),
            new Vector3(270f, 0f, 0f),
            new Vector3(21.5f, 1f, 22.1f)
        ),


        ["Skyjungle"] = new(
            "skyjungle/UI/Scoreboard/GorillaScoreBoard",
            new Vector3(-21.2f, -32.1f, 0f),
            new Vector3(270f, 0f, 0f),
            new Vector3(21.6f, 0.1f, 20.4f)
        ),


        ["Beach"] = new(
            "BeachScoreboardAnchor/GorillaScoreBoard",
            new Vector3(-22.1f, -33.7f, 0.1f),
            new Vector3(270f, 0f, 0f),
            new Vector3(21.2f, 2f, 21.6f)
        ),


        ["City"] = new(
            "City_Pretty/CosmeticsScoreboardAnchor/GorillaScoreBoard",
            new Vector3(-22.1f, -34.9f, 0.5f),
            new Vector3(270f, 0f, 0f),
            new Vector3(21.6f, 2.4f, 22f)
        ),


        ["Basement"] = new(
            "Basement/BasementScoreboardAnchor/GorillaScoreBoard",
            new Vector3(-22.1f, -24.5f, 0.5f),
            new Vector3(270f, 0f, 0f),
            new Vector3(21.6f, 1.2f, 20.8f)
        )
    };
}