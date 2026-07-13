using UnityEngine;

namespace MonkeSpellbook.Behaviours.Wand;

public class GestureCreatorUI : MonoBehaviour
{
    public bool CreationMode { get; private set; }
    public string NewGestureName { get; private set; } = string.Empty;

    private Rect _windowRect = new(10, 10, 250, 150);

    private void OnGUI()
    {
        _windowRect = GUI.Window(0, _windowRect, DrawWindow, "Gesture Creator");
    }

    private void DrawWindow(int id)
    {
        CreationMode = GUI.Toggle(new Rect(10, 25, 200, 25), CreationMode, "Creation Mode");

        GUI.Label(new Rect(10, 55, 200, 20), "New Gesture Name:");
        NewGestureName = GUI.TextField(new Rect(10, 75, 200, 25), NewGestureName);

        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}