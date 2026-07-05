using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D hoverCursor;
    [SerializeField] private Texture2D dragCursor;

    public Vector2 hotspot = new (30, 15);
    public CursorMode cursorMode = CursorMode.Auto;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Cursor.SetCursor(defaultCursor, hotspot, cursorMode);
    }

    public void SetHoverCursor()
    {
        Cursor.SetCursor(hoverCursor, hotspot, cursorMode);
    }

    public void SetDragCursor()
    {
        Cursor.SetCursor(dragCursor, hotspot, cursorMode);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, hotspot, cursorMode);
    }
}