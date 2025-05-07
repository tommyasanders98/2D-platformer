using UnityEngine;
using UnityEngine.Rendering;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Vector2 parallaxEffectMultiplier;
    private Transform cameraTransform;
    private Vector3 startCameraPosition;
    private Vector3 startLayerPosition;
    private float pixelsPerUnit;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        startCameraPosition = cameraTransform.position;
        startLayerPosition = transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        pixelsPerUnit = sr.sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        // Parallax offset relative to where the camera started
        Vector3 camDelta = cameraTransform.position - startCameraPosition;

        Vector3 newPos = startLayerPosition + new Vector3(
            camDelta.x * parallaxEffectMultiplier.x,
            camDelta.y * parallaxEffectMultiplier.y,
            0f
        );

        // Snap to pixel grid
        newPos.x = Mathf.Round(newPos.x * pixelsPerUnit) / pixelsPerUnit;
        newPos.y = Mathf.Round(newPos.y * pixelsPerUnit) / pixelsPerUnit;

        transform.position = newPos;
    }




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

