using UnityEngine;

public class ParallaxLooper : MonoBehaviour
{
    public bool loop = true;
    private float spriteWidth;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
    }

    void LateUpdate()
    {
        if (!loop) return;

        float camHorizontalExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        float distanceFromCamera = cam.position.x - transform.position.x;

        if (Mathf.Abs(distanceFromCamera) >= spriteWidth)
        {
            float offset = (distanceFromCamera % spriteWidth);
            transform.position += new Vector3(spriteWidth * 2 * Mathf.Sign(offset), 0, 0);
        }
    }
}