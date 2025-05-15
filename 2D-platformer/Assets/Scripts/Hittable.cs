using UnityEngine;

public interface IHittable
{
    void Hit(Vector2 hitDirection, float damage);
}
