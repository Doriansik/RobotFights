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

    private void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;

            if (currentEnergy > maxEnergy)
            {
                currentEnergy = maxEnergy;
            }

            if (isExhausted && currentEnergy >= minEnergyToRecover)
            {
                isExhausted = false;
            }

            OnEnergyChanged?.Invoke(currentEnergy / maxEnergy);
        }
    }

    public void ConsumeEnergy(float amount)
    {
        currentEnergy -= amount;

        if (currentEnergy <= emptyEnergyValue)
        {
            currentEnergy = emptyEnergyValue;
            isExhausted = true;
        }

        OnEnergyChanged?.Invoke(currentEnergy / maxEnergy);
    }

    public bool HasEnoughEnergy(float amount)
    {
        return true;
    }
}