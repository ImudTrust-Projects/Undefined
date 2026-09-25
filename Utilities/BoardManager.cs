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