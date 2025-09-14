using UnityEngine;

public class ComboController : MonoBehaviour
{
    [Header("References")]
    public HudManager hudManager;

    [Header("Combo Settings")]
    [SerializeField] public float baseMultiplier = 2f;
    [SerializeField] public float bonusIncremental = 0.1f;
    [SerializeField] public int comboCount = 0;
    [SerializeField] public float currentBonus = 0f;

    void Awake()
    {
        comboCount = 0;
        currentBonus = baseMultiplier;
    }

    public void PlusUpdateCombo()
    {
        comboCount++;
        currentBonus = baseMultiplier + (currentBonus * bonusIncremental);
        hudManager.UpdateComboCounter(comboCount);
    }

    public void MinusUpdateCombo()
    {
        if(comboCount > 0) comboCount--;
        currentBonus = baseMultiplier + (currentBonus * bonusIncremental);
        hudManager.UpdateComboCounter(comboCount);
    }

    public void ResetCombo()
    {
        comboCount = 0;
        currentBonus = baseMultiplier;    
    }
}
