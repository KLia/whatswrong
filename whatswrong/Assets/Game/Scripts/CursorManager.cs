using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private float size = 0.15f;
    
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D hoverCursor;
    [SerializeField] private Texture2D dragCursor;

    public Vector2 hotspot = Vector2.zero;//new (31, 16);
    public CursorMode cursorMode = CursorMode.Auto;
    Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
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
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || Mouse.current == null)
            return;

        if (_camera == null)
            _camera = Camera.main;

        if (_camera == null)
            return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();

        Vector3 world = _camera.ScreenToWorldPoint(mouseScreen);
        world.z = 0f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(world + Vector3.left * size, world + Vector3.right * size);
        Gizmos.DrawLine(world + Vector3.down * size, world + Vector3.up * size);
        Gizmos.DrawSphere(world, size * 0.25f);
    }
}