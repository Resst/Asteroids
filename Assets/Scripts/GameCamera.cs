using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public static float HalfHeight { get; private set; }
    public static float HalfWidth { get; private set; }

    private Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        HalfHeight = _camera.orthographicSize;
        HalfWidth = _camera.aspect * HalfHeight;
    }

    void Update()
    {
        HalfHeight = _camera.orthographicSize;
        HalfWidth = _camera.aspect * HalfHeight;
    }
}
