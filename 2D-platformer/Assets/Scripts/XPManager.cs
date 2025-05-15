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
    //public int CurrentXP => currentXP;
    //public int CurrentLevel => currentLevel;
    public TMP_Text debugXPText;

    void Start()
    {
        UpdateUI();
        ResetXP();
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

        if (debugXPText != null)
        { debugXPText.text = $"XP: {currentXP} / {xpRequired}\nLevel: {currentLevel}"; }

        Debug.Log($"[UI Live] XPManager instance ID: {GetInstanceID()}, object: '{gameObject.name}' — XP: {currentXP}, Level: {currentLevel}");
    }

    public void ResetXP()
    {
        currentXP = 0;
        currentLevel = 1;
        UpdateUI();
    }
}