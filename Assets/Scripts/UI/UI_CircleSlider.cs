using UnityEngine.UI;
using UnityEngine;

public class UI_CircleSlider : MonoBehaviour
{
    [SerializeField] private Image circleImage;

    private void Awake()
    {
        circleImage = GetComponent<Image>();
    }

    public void SetFillAmount(float fillAmount)
    {
        if(circleImage) circleImage.fillAmount = fillAmount;
    }
}
