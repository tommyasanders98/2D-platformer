using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Combat/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int damage = 1;
    public float knockbackForce = 7f;
    public float hitCooldown = 0.3f; // cooldown per enemy hit
}