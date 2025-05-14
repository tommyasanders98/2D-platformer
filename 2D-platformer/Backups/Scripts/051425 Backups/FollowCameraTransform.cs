using UnityEngine;

public class FollowCameraTransform : MonoBehaviour
{
    public Transform cameraToFollow;

    void LateUpdate()
    {
        if (cameraToFollow == null && Camera.main != null)
        {
            cameraToFollow = Camera.main.transform;
        }

        if (cameraToFollow != null)
        {
            transform.position = new Vector3(
                cameraToFollow.position.x,
                cameraToFollow.position.y,
                transform.position.z // preserve depth
            );
        }
    }
}
