using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider xpSlider;
    public TMP_Text levelText;
    public TMP_Text debugXPText;

    [Header("XP Settings")]
    public int currentXP = 0;
    public int currentLevel = 1;
    public int maxLevel = 10;

    [Header("Animation")]
    public float fillSpeed = 10f;
    private float displayedXP = 0f;

    void Start()
    {
        displayedXP = currentXP;
        UpdateUI();
        ResetXP();
    }

    void Update()
    {
        // Animate the slider fill smoothly
        if (xpSlider != null)
        {
            displayedXP = Mathf.MoveTowards(displayedXP, currentXP, fillSpeed * Time.deltaTime);
            xpSlider.value = displayedXP;
        }
    }

    public void AddXP(int amount)
    {
    
        //Debug.Log($"[XPManager] Adding {amount} XP");

        if (currentLevel >= maxLevel) return;

        currentXP += amount;

        while (currentXP >= GetXPRequired(currentLevel) && currentLevel < maxLevel)
        {
            currentXP -= GetXPRequired(currentLevel);
            currentLevel++;
        }

        UpdateUI();
    }

    int GetXPRequired(int level)
    {
        return level * 10;
    }

    void UpdateUI()
    {
       

        int xpRequired = GetXPRequired(currentLevel);
        xpSlider.maxValue = xpRequired;
        levelText.text = $"Lv {currentLevel}";

        if (debugXPText != null)
            debugXPText.text = $"XP: {currentXP} / {xpRequired}\nLevel: {currentLevel}";

        //Debug.Log($"[UI Live] XPManager instance ID: {GetInstanceID()}, object: '{gameObject.name}' — XP: {currentXP}, Level: {currentLevel}");
    }

    public void ResetXP()
    {
        currentXP = 0;
        currentLevel = 1;
        displayedXP = 0f;
        UpdateUI();
    }
}