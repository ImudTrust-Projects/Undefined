using System.Text;
using GorillaNetworking;
using UnityEngine;
using Undefined.MENUSETTINGS;
using Undefined.Mods.Categories;

namespace Undefined.Utilities;

public class RoomJoiner : MonoBehaviour
{
    private const int MaxCodeLength = 12;
    private const string FieldName = "RoomCode";

    private static readonly Color PanelColor = new Color32(24, 24, 26, 245);
    private static readonly Color FieldColor = new Color32(40, 40, 44, 255);
    private static readonly Color FieldHoverColor = new Color32(52, 52, 58, 255);
    private static readonly Color BorderColor = new Color32(60, 60, 66, 255);
    private static readonly Color LeaveColor = new Color32(55, 55, 60, 255);

    private string roomCode = "";
    private Rect windowRect = new Rect(20, 20, 280, 136);

    private GUIStyle windowStyle;
    private GUIStyle labelStyle;
    private GUIStyle fieldStyle;
    private GUIStyle buttonTextStyle;
    private Texture2D white;

    private float joinHover;
    private float joinPress;
    private float leaveHover;
    private float leavePress;
    private float fieldHover;
    private float fieldFocus;
    private float fieldPulse;
    private bool dragging;
    private float dragAmount;
    private float dropBounce;

    private void OnGUI()
    {
        if (windowStyle == null)
            CreateStyles();

        if (Event.current.rawType == EventType.MouseUp && dragging)
        {
            dragging = false;
            dropBounce = 1f;
        }

        if (Event.current.type == EventType.Repaint)
        {
            float step = Time.unscaledDeltaTime;
            dragAmount = Mathf.MoveTowards(dragAmount, dragging ? 1f : 0f, step * 7f);
            dropBounce = Mathf.MoveTowards(dropBounce, 0f, step * 3f);
        }

        float scale = 1f - 0.08f * Ease(dragAmount) + 0.035f * Mathf.Sin(dropBounce * Mathf.PI);

        Matrix4x4 matrix = GUI.matrix;
        Color color = GUI.color;

        GUIUtility.ScaleAroundPivot(Vector2.one * scale, windowRect.center);
        GUI.color = new Color(1f, 1f, 1f, 1f - 0.15f * Ease(dragAmount));

        windowRect = GUI.Window(12345, windowRect, DrawWindow, "Room Joiner", windowStyle);

        GUI.matrix = matrix;
        GUI.color = color;
    }

    private static readonly Rect[] Controls =
    {
        new Rect(12, 52, 256, 28),
        new Rect(12, 92, 124, 32),
        new Rect(144, 92, 124, 32)
    };

    private void DrawWindow(int windowID)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !OverControl(Event.current.mousePosition))
            dragging = true;

        GUI.Label(new Rect(12, 26, 256, 20), Status(), labelStyle);

        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
        {
            JoinRoom();
            Event.current.Use();
        }

        DrawField(new Rect(12, 52, 256, 28));

        Color accent = Color.Lerp(Settings.backgroundColor.colors[0].color, Color.white, 0.25f);

        if (AnimatedButton(new Rect(12, 92, 124, 32), "Join", accent, ref joinHover, ref joinPress))
            JoinRoom();

        if (AnimatedButton(new Rect(144, 92, 124, 32), "Leave", LeaveColor, ref leaveHover, ref leavePress))
            Leave();

        GUI.DragWindow();
    }

    private void DrawField(Rect rect)
    {
        bool over = rect.Contains(Event.current.mousePosition);
        bool focused = GUI.GetNameOfFocusedControl() == FieldName;

        if (Event.current.type == EventType.Repaint)
        {
            float step = Time.unscaledDeltaTime;
            fieldHover = Mathf.MoveTowards(fieldHover, over ? 1f : 0f, step * 8f);
            fieldFocus = Mathf.MoveTowards(fieldFocus, focused ? 1f : 0f, step * 6f);
            fieldPulse = Mathf.MoveTowards(fieldPulse, 0f, step * 6f);
        }

        Color accent = Color.Lerp(Settings.backgroundColor.colors[0].color, Color.white, 0.25f);
        Rect box = Scale(rect, 1f + 0.035f * Mathf.Sin(fieldPulse * Mathf.PI));

        DrawRect(Grow(box, 1f + fieldHover), Color.Lerp(BorderColor, accent, Ease(fieldFocus)));
        DrawRect(box, Color.Lerp(FieldColor, FieldHoverColor, Ease(fieldHover)));

        float lineWidth = box.width * Ease(fieldFocus);
        DrawRect(new Rect(box.center.x - lineWidth / 2f, box.yMax - 2f, lineWidth, 2f), accent);

        GUI.SetNextControlName(FieldName);
        string cleaned = Clean(GUI.TextField(rect, roomCode, MaxCodeLength, fieldStyle));

        if (cleaned != roomCode)
        {
            roomCode = cleaned;
            fieldPulse = 1f;
            AudioHandler.Play(SoundSettings.currentButtonSound, 0.07f, Random.Range(1.2f, 1.4f));
        }
    }

    private bool AnimatedButton(Rect rect, string text, Color color, ref float hover, ref float press)
    {
        bool over = rect.Contains(Event.current.mousePosition);

        if (Event.current.type == EventType.Repaint)
        {
            float step = Time.unscaledDeltaTime;
            hover = Mathf.MoveTowards(hover, over ? 1f : 0f, step * 8f);
            press = Mathf.MoveTowards(press, 0f, step * 4f);
        }

        float scale = 1f + 0.05f * Ease(hover) - 0.1f * Mathf.Sin(press * Mathf.PI);
        Rect button = Scale(rect, scale);

        DrawRect(button, Color.Lerp(color, Color.white, 0.18f * Ease(hover) + 0.4f * press));

        float lineWidth = button.width * Ease(hover);
        DrawRect(new Rect(button.center.x - lineWidth / 2f, button.yMax - 2f, lineWidth, 2f), Color.Lerp(color, Color.white, 0.6f));

        GUI.Label(button, text, buttonTextStyle);

        if (!GUI.Button(rect, GUIContent.none, GUIStyle.none))
            return false;

        press = 1f;
        GUI.FocusControl(null);
        AudioHandler.Play(SoundSettings.currentButtonSound, 0.15f);
        return true;
    }

    private void DrawRect(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, white);
        GUI.color = previous;
    }

    private static bool OverControl(Vector2 point)
    {
        foreach (Rect control in Controls)
        {
            if (control.Contains(point))
                return true;
        }

        return false;
    }

    private static Rect Scale(Rect rect, float scale)
    {
        Vector2 size = rect.size * scale;
        return new Rect(rect.center - size / 2f, size);
    }

    private static Rect Grow(Rect rect, float amount)
    {
        return new Rect(rect.x - amount, rect.y - amount, rect.width + amount * 2f, rect.height + amount * 2f);
    }

    private static float Ease(float t)
    {
        return t * t * (3f - 2f * t);
    }

    private static string Status()
    {
        if (NetworkSystem.Instance == null || !NetworkSystem.Instance.InRoom)
            return "Not in a room";

        return $"{NetworkSystem.Instance.RoomName}  ({NetworkSystem.Instance.RoomPlayerCount})";
    }

    private static string Clean(string code)
    {
        StringBuilder clean = new StringBuilder(MaxCodeLength);

        foreach (char c in code.ToUpperInvariant())
        {
            if ((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                clean.Append(c);

            if (clean.Length >= MaxCodeLength)
                break;
        }

        return clean.ToString();
    }

    private void JoinRoom()
    {
        if (string.IsNullOrEmpty(roomCode) || PhotonNetworkController.Instance == null)
            return;

        PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomCode, JoinType.Solo);
    }

    private static void Leave()
    {
        if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom)
            NetworkSystem.Instance.ReturnToSinglePlayer();
    }

    private void CreateStyles()
    {
        white = Texture2D.whiteTexture;
        Texture2D background = MakeTexture(PanelColor);

        windowStyle = new GUIStyle(GUI.skin.window)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            padding = new RectOffset(12, 12, 24, 12)
        };
        windowStyle.normal.background = background;
        windowStyle.onNormal.background = background;
        windowStyle.normal.textColor = Color.white;
        windowStyle.onNormal.textColor = Color.white;

        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleLeft
        };
        labelStyle.normal.textColor = new Color32(170, 170, 176, 255);

        fieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = 15,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(8, 8, 4, 4)
        };
        fieldStyle.normal.background = null;
        fieldStyle.focused.background = null;
        fieldStyle.hover.background = null;
        fieldStyle.active.background = null;
        fieldStyle.normal.textColor = Color.white;
        fieldStyle.focused.textColor = Color.white;
        fieldStyle.hover.textColor = Color.white;

        buttonTextStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        buttonTextStyle.normal.textColor = Color.white;
    }

    private static Texture2D MakeTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}
