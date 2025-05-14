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

        float camDist = cam.position.x - transform.position.x;

        // Use 0.9f to reposition slightly before it's fully offscreen
        if (Mathf.Abs(camDist) >= spriteWidth * 0.9f)
        {
            float offset = spriteWidth * 3f * Mathf.Sign(camDist); // assumes 3-tile loop
            transform.position += new Vector3(offset, 0, 0);
        }
        //if (!loop) return;

        //float camHorizontalExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        //float distanceFromCamera = cam.position.x - transform.position.x;

        //if (Mathf.Abs(distanceFromCamera) >= spriteWidth)
        //{
        //    float offset = (distanceFromCamera % spriteWidth);
        //    transform.position += new Vector3(spriteWidth * 2 * Mathf.Sign(offset), 0, 0);
        //}
    }
}