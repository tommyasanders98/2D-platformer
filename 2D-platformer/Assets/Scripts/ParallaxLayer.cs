using UnityEngine;
using UnityEngine.Rendering;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Vector2 parallaxEffectMultiplier;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y, 0f);
        lastCameraPosition = cameraTransform.position;

        if (cameraTransform.position.x - transform.position.x >= textureUnitSizeX)
        {
            float offsetPositonX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offsetPositonX, transform.position.y);
        }
    }

    //public float parallaxFactor = 0.5f;
    //private Vector3 startLocalPos;

    //void Start()
    //{
    //    startLocalPos = transform.localPosition;
    //}

    //void LateUpdate()
    //{
    //    Vector3 camOffset = transform.parent.position;
    //    transform.localPosition = startLocalPos + new Vector3(camOffset.x * parallaxFactor, camOffset.y * parallaxFactor, 0);
    //}
}