using UnityEngine;

public class FollowPlayerPosition : MonoBehaviour
{

    public PlayerMovement player;
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = player.positionTracker + offset;   //update position with offset to float above players head
        }     
    }
}
