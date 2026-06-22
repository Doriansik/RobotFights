using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private GameObject robotBlocker;

    public int maxTutNumber;

    public int tutNumber;

    public string tutorial1;
    public string tutorial2;
    public string tutorial3;
    public void ChangeText()
    {
        switch (tutNumber)
        {
            case 0:
                text.text = tutorial1;
                break;
            case 1:
                text.text = tutorial2;
                break;
            case 2:
                text.text = tutorial3;
                break;
        }

        tutNumber++;

        if (tutNumber >= maxTutNumber)
        {
            gameUIController.OnStartGameAfterStartOfTheRound();
            robotBlocker.SetActive(false);
        }
    }
}
