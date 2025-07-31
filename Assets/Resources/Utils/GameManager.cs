using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Camera mainCamera;
    private Light sunLight;
    private Vector3 mouseScreenPosition;
    private Vector3 mouseWorldPosition;

    public Camera MainCamera => mainCamera;
    public Light SunLight => sunLight;

    public float MouseScreenX => mouseScreenPosition.x;
    public float MouseScreenY => mouseScreenPosition.y;
    public float MouseScreenZ => mouseScreenPosition.z;

    public float MouseWorldX => mouseWorldPosition.x;
    public float MouseWorldY => mouseWorldPosition.y;
    public float MouseWorldZ => mouseWorldPosition.z;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = GetComponentInChildren<Camera>();
        sunLight = GetComponentInChildren<Light>();
    }

    private void FixedUpdate()
    {
        UpdateMousePosition();
    }

    private void UpdateMousePosition()
    {
        mouseScreenPosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float enter))
            mouseWorldPosition = ray.GetPoint(enter);
    }
}