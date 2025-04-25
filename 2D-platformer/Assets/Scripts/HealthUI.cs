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
        Debug.Log("set max heart");
        foreach (UnityEngine.UI.Image heart in hearts)
        {
            Debug.Log("destroy heart");
            Destroy(heart.gameObject);
            
        }

        hearts.Clear();
        for (int i = 0; i < maxHearts; i++)
        {
            Debug.Log("Attempting to create heart #" + i);
            UnityEngine.UI.Image newHeart = Instantiate(heartPrefab, transform);
            if (newHeart == null)
            {
                Debug.LogWarning("Heart prefab instantiation failed!");
            }
            else
            {
                newHeart.sprite = fullHeartSprite;
                newHeart.color = Color.red;
                hearts.Add(newHeart);
            }
        }
        //for(int i = 0; i < maxHearts; i++)
        //{
        //    UnityEngine.UI.Image newHeart = Instantiate(heartPrefab, transform);
        //    newHeart.sprite = fullHeartSprite;
        //    newHeart.color = Color.red;
        //    hearts.Add(newHeart);
        //    Debug.Log("new heart");
        //}
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
