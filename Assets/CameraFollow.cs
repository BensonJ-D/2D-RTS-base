using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] 
    private CinemachineVirtualCamera camera;
    private Camera _mainCam;
    
    [SerializeField] 
    private float minFov = 1f;
    [SerializeField] 
    private float maxFov = 15f;
    public float sensitivity = 10f;
    // Start is called before the first frame update
    private void Start()
    {    
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        var target = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        target.z = transform.position.z;
        
        var newPos = _mainCam.WorldToViewportPoint(target);
        newPos.x = Mathf.Clamp01(newPos.x);
        newPos.y = Mathf.Clamp01(newPos.y);
        transform.position = _mainCam.ViewportToWorldPoint(newPos);
        
        var fov = camera.m_Lens.OrthographicSize;
        var fovChange = Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov += fovChange;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        camera.m_Lens.OrthographicSize = fov;
    }
}
