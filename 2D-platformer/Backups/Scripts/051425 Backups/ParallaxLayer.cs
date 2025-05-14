using UnityEngine;
using UnityEngine.Rendering;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Vector2 parallaxMultiplier = new Vector2(0.5f, 0f);
    private Transform cam;
    private Vector3 startCamPos;
    private Vector3 startLayerPos;

    void Start()
    {
        cam = Camera.main.transform;
        startCamPos = cam.position;
        startLayerPos = transform.position;
    }

    void LateUpdate()
    {
        Vector3 camOffset = cam.position - startCamPos;

        transform.position = startLayerPos + new Vector3(
            camOffset.x * parallaxMultiplier.x,
            camOffset.y * parallaxMultiplier.y,
            0f
        );
    }
    //    [SerializeField] private Vector2 parallaxMultiplier = Vector2.one;
    //private Transform cam;
    //private Vector3 lastCamPos;

    //void Start()
    //{
    //    cam = Camera.main.transform;
    //    lastCamPos = cam.position;
    //}

    //void LateUpdate()
    //{
    //    Vector3 delta = cam.position - lastCamPos;

    //    // Move this layer by a fraction of the camera’s movement
    //    transform.position += new Vector3(
    //        delta.x * parallaxMultiplier.x,
    //        delta.y * parallaxMultiplier.y,
    //        0f
    //    );

    //    lastCamPos = cam.position;
    //}

    //[SerializeField] private Vector2 parallaxEffectMultiplier;
    //private Transform cameraTransform;
    //private Vector3 startCameraPosition;
    //private Vector3 startLayerPosition;
    //private float pixelsPerUnit;

    //private void Start()
    //{
    //    cameraTransform = Camera.main.transform;
    //    startCameraPosition = cameraTransform.position;
    //    startLayerPosition = transform.position;

    //    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    //    pixelsPerUnit = sr.sprite.pixelsPerUnit;
    //}

    //private void LateUpdate()
    //{
    //    // Parallax offset relative to where the camera started
    //    Vector3 camDelta = cameraTransform.position - startCameraPosition;

    //    Vector3 newPos = startLayerPosition + new Vector3(
    //        camDelta.x * parallaxEffectMultiplier.x,
    //        camDelta.y * parallaxEffectMultiplier.y,
    //        0f
    //    );

    //    // Snap to pixel grid
    //    newPos.x = Mathf.Round(newPos.x * pixelsPerUnit) / pixelsPerUnit;
    //    newPos.y = Mathf.Round(newPos.y * pixelsPerUnit) / pixelsPerUnit;

    //    transform.position = newPos;
    //}




    //[SerializeField] private Vector2 parallaxEffectMultiplier;
    //private Transform cameraTransform;
    //private Vector3 lastCameraPosition;
    //private float textureUnitSizeX;

    //private void Start()
    //{
    //    cameraTransform = Camera.main.transform;
    //    lastCameraPosition = cameraTransform.position;
    //    Sprite sprite = GetComponent<SpriteRenderer>().sprite;
    //    Texture2D texture = sprite.texture;
    //    textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    //}

    //private void LateUpdate()
    //{


    //    Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
    //    transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y, 0f);
    //    lastCameraPosition = cameraTransform.position;

    //    if (cameraTransform.position.x - transform.position.x >= textureUnitSizeX)
    //    {
    //        float offsetPositonX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
    //        transform.position = new Vector3(cameraTransform.position.x + offsetPositonX, transform.position.y);
    //    }
    //}
}

