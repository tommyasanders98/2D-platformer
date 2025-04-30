using UnityEngine;

public class LockScale : MonoBehaviour
{
    [Header("Locked Scale")]
    public Vector3 lockedScale = new Vector3(1f, 1f, 1f); // Match this to your intended character size

    void LateUpdate()
    {
        // Forcefully reset the scale after animations are applied
        if (transform.localScale != lockedScale)
        {
            transform.localScale = lockedScale;
        }
    }
}
