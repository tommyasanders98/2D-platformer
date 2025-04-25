using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HealthUI : MonoBehaviour
{
    public UnityEngine.UI.Image heartPrefab;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    public List<UnityEngine.UI.Image> hearts = new List<UnityEngine.UI.Image>();

    public void SetMaxHearts(int maxHearts)
    {
        
        foreach (UnityEngine.UI.Image heart in hearts)
        {
            ;
            Destroy(heart.gameObject);
            
        }

        hearts.Clear();
        for (int i = 0; i < maxHearts; i++)
        {
           
            UnityEngine.UI.Image newHeart = Instantiate(heartPrefab, transform);
            if (newHeart == null)
            {
                
            }
            else
            {
                newHeart.sprite = fullHeartSprite;
                newHeart.color = Color.red;
                hearts.Add(newHeart);
            }
        }
      
    }

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Count;i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeartSprite;
                hearts[i].color = Color.red;
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite;
                hearts[i].color = Color.white;
            }
        }
    }
}
