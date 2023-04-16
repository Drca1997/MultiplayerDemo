using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnowballUI : MonoBehaviour
{

    [SerializeField] private Image background;
    [SerializeField] private Image foreground;
    [SerializeField] private Color availableColor;
    [SerializeField] private Color unavailableColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SetFillAmount(float originalCooldown, float currentCooldown)
    {
        background.fillAmount = currentCooldown / originalCooldown;
        if (currentCooldown <= 0)
        {
            foreground.color = availableColor;
        }
    }

    public void ResetCooldown()
    {
        foreground.color = unavailableColor;
        background.fillAmount = 1;
    }
}
