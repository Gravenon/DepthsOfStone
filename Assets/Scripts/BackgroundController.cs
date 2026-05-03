using UnityEngine;

public class ParallaxGroup : MonoBehaviour
{
    public float parallaxFactor;
    private Transform cam;
    private Vector3 lastCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;
        transform.position += delta * parallaxFactor;
        lastCamPos = cam.position;
    }
}