using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider xpSlider;
    public TMP_Text levelText;

    [Header("XP Settings")]
    public int currentXP = 0;
    public int currentLevel = 1;
    public int maxLevel = 10;

    void Start()
    {
        UpdateUI();
    }

    public void AddXP(int amount)
    {
        Debug.Log($"[XPManager] Adding {amount} XP"); //  Debug line
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
        Debug.Log($"[UI] XP: {currentXP}/{xpRequired}, Level: {currentLevel}");
        xpSlider.maxValue = xpRequired;
        xpSlider.value = currentXP;
        levelText.text = $"Lv {currentLevel}";
    }

    public void ResetXP()
    {
        currentXP = 0;
        currentLevel = 1;
        UpdateUI();
    }
}