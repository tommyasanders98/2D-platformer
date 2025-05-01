using UnityEngine;

public class ParallaxLooper : MonoBehaviour
{
    public float parallaxFactor = 0.5f;
    public bool loop = true;

    private float spriteWidth;
    private Transform cam;
    private Vector3 lastCamPosition;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPosition = cam.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPosition;
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0);
        lastCamPosition = cam.position;

        if (loop)
        {
            float camHorizontalExtent = Camera.main.orthographicSize * Screen.width / Screen.height;

            if (Mathf.Abs(cam.position.x - transform.position.x) >= spriteWidth)
            {
                float offset = (cam.position.x - transform.position.x) % spriteWidth;
                transform.position = new Vector3(cam.position.x + offset, transform.position.y, transform.position.z);
            }
        }
    }
}