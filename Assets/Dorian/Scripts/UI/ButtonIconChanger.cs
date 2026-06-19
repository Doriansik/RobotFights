using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ButtonIconChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite pressedSprite;

    private Image targetImage;
    private bool isHovering;

    private void Awake()
    {
        targetImage = GetComponent<Image>();
        SetSprite(defaultSprite);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        SetSprite(hoverSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        SetSprite(defaultSprite);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetSprite(pressedSprite);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isHovering)
        {
            SetSprite(hoverSprite);
        }
        else
        {
            SetSprite(defaultSprite);
        }
    }

    private void SetSprite(Sprite sprite)
    {
        if (sprite != null && targetImage != null)
        {
            targetImage.sprite = sprite;
        }
    }
}