using UnityEngine;

[System.Serializable]
public class XPOrbType
{
    public GameObject prefab;
    public int value = 1;
    [Range(0f, 1f)] public float dropChance = 1f;
}

public class XPDropper : MonoBehaviour
{
    [Header("Orb Drop Settings")]
    public XPOrbType[] orbTypes;
    public float scatterForce = 3f;

    public void DropOrbs(int totalOrbs)
    {
        if (orbTypes == null || orbTypes.Length == 0)
        {
            Debug.LogWarning("No orb types assigned to XPOrbDropper.");
            return;
        }

        for (int i = 0; i < totalOrbs; i++)
        {
            XPOrbType selectedType = GetRandomOrbType();
            if (selectedType == null || selectedType.prefab == null) continue;

            GameObject orb = Instantiate(selectedType.prefab, transform.position, Quaternion.identity);

            IXPorb orbScript = orb.GetComponent<IXPorb>();
            if (orbScript != null)
            {
                orbScript.SetValue(selectedType.value);
            }

            Rigidbody2D rb = orb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                rb.AddForce(randomDir * scatterForce, ForceMode2D.Impulse);
            }
        }
    }

    private XPOrbType GetRandomOrbType()
    {
        float totalWeight = 0f;
        foreach (var type in orbTypes)
        {
            totalWeight += type.dropChance;
        }

        float randomValue = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var type in orbTypes)
        {
            cumulative += type.dropChance;
            if (randomValue <= cumulative)
                return type;
        }

        return null;
    }
}

public interface IXPorb
{
    void SetValue(int value);
}