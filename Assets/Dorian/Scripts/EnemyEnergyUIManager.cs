using UnityEngine;
using UnityEngine.UI;

public class EnemyEnergyUIManager : MonoBehaviour
{
    public static EnemyEnergyUIManager Instance { get; private set; }

    [SerializeField] private Slider energySlider;
    [SerializeField] private float minSliderValue = 0f;
    [SerializeField] private float maxSliderValue = 1f;

    private EnemyEnergy currentEnemyEnergy;

    private void Awake()
    {
        Instance = this;
        if (energySlider)
        {
            energySlider.minValue = minSliderValue;
            energySlider.maxValue = maxSliderValue;
            energySlider.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        AttackCoordinator.OnTargetChanged += SetCurrentEnemy;
    }

    private void OnDisable()
    {
        AttackCoordinator.OnTargetChanged -= SetCurrentEnemy;
        if (currentEnemyEnergy != null)
        {
            currentEnemyEnergy.OnEnergyChanged -= UpdateEnergyUI;
        }
    }

    private void SetCurrentEnemy(GameObject targetObj)
    {
        if (currentEnemyEnergy != null)
        {
            currentEnemyEnergy.OnEnergyChanged -= UpdateEnergyUI;
            currentEnemyEnergy = null;
        }

        if (targetObj == null)
        {
            if (energySlider) energySlider.gameObject.SetActive(false);
            return;
        }

        EnemyEnergy enemyEnergy = targetObj.GetComponent<EnemyEnergy>();
        if (enemyEnergy != null && energySlider != null)
        {
            currentEnemyEnergy = enemyEnergy;
            currentEnemyEnergy.OnEnergyChanged += UpdateEnergyUI;

            energySlider.value = currentEnemyEnergy.CurrentEnergy / currentEnemyEnergy.MaxEnergy;
            energySlider.gameObject.SetActive(true);
        }
        else if (energySlider != null)
        {
            energySlider.gameObject.SetActive(false);
        }
    }

    private void UpdateEnergyUI(float normalizedEnergy)
    {
        if (energySlider)
        {
            energySlider.value = normalizedEnergy;
        }
    }
}