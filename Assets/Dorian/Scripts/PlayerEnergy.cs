using System;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    public event Action<float> OnEnergyChanged;

    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyRegenRate = 15f;
    [SerializeField] private float minEnergyToRecover = 20f;
    [SerializeField] private float emptyEnergyValue = 0f;

    private float currentEnergy;
    private bool isExhausted;

    public bool IsExhausted => isExhausted;

    private void Awake()
    {
        currentEnergy = maxEnergy;
    }

    public void ConsumeEnergy(float amount)
    {
        if (currentEnergy > emptyEnergyValue)
        {
            currentEnergy -= amount;

            if (currentEnergy <= emptyEnergyValue)
            {
                currentEnergy = emptyEnergyValue;
                isExhausted = true;
            }

            OnEnergyChanged?.Invoke(currentEnergy / maxEnergy);
        }
    }

    public bool HasEnoughEnergy(float amount)
    {
        return currentEnergy >= amount && !isExhausted;
    }

}