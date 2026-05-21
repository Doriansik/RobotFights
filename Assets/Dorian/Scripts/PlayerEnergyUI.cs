using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergyUI : MonoBehaviour
{
    [SerializeField] private PlayerEnergy playerEnergy;
    [SerializeField] private Slider energySlider;
    [SerializeField] private float minSliderValue = 0f;
    [SerializeField] private float maxSliderValue = 1f;

    private void OnEnable()
    {
        if (playerEnergy != null)
        {
            playerEnergy.OnEnergyChanged += UpdateEnergyUI;
        }
    }

    private void OnDisable()
    {
        if (playerEnergy != null)
        {
            playerEnergy.OnEnergyChanged -= UpdateEnergyUI;
        }
    }

    private void Start()
    {
        if (energySlider != null)
        {
            energySlider.minValue = minSliderValue;
            energySlider.maxValue = maxSliderValue;
            energySlider.value = maxSliderValue;
        }
    }

    private void UpdateEnergyUI(float normalizedEnergy)
    {
        if (energySlider != null)
        {
            energySlider.value = normalizedEnergy;
        }
    }
}