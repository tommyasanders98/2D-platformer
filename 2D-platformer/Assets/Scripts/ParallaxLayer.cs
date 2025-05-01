using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor = 0.5f;
    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        Vector3 camOffset = transform.parent.position;
        transform.localPosition = startLocalPos + new Vector3(camOffset.x * parallaxFactor, camOffset.y * parallaxFactor, 0);
    }
}