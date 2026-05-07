using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject losePanel;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;
            playerHealth.OnDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
            playerHealth.OnDeath -= HandleDeath;
        }
    }

    private void Start()
    {
        if (slider != null && playerHealth != null)
        {
            slider.maxValue = playerHealth.CurrentHp;
            slider.value = playerHealth.CurrentHp;
        }

        if (animator == null && playerHealth != null)
        {
            animator = playerHealth.GetComponent<Animator>();
        }
    }

    private void UpdateHealthUI(int currentHp)
    {
        if (slider != null)
        {
            slider.value = currentHp;
        }
    }

    private void HandleDeath()
    {
        if (animator != null) animator.SetTrigger("Death");
        if (player != null) player.enabled = false;
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f;
    }
}