using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] public Interactable tutorialButton;
    [SerializeField] public Interactable trackingButton;
    [SerializeField] public Interactable bleButton;
    [SerializeField] public Interactable angleButton;
    [SerializeField] public Interactable prfButton;

    private bool _unlockedTutorial;

    private void Start()
    {
        tutorialButton.gameObject.SetActive(false);
        trackingButton.gameObject.SetActive(false);
        bleButton.gameObject.SetActive(false);
        angleButton.gameObject.SetActive(false);
        prfButton.gameObject.SetActive(false);
    }

    public void OnStateChange(MenuType menuState)
    {
        tutorialButton.IsToggled = false;
        trackingButton.IsToggled = false;
        bleButton.IsToggled = false;
        angleButton.IsToggled = false;
        prfButton.IsToggled = false;
        
        int state = (int) menuState;

        if (menuState == MenuType.TutorialFinished)
        {
            UnlockTutorial();
        }
        
        if(state >= 2 && state<=5)
        {
            if (_unlockedTutorial)
            {
                tutorialButton.IsToggled = true;
            }
        }else if (menuState == MenuType.Tracking)
        {
            trackingButton.gameObject.SetActive(true);
            trackingButton.IsToggled = true;
        }else if (menuState == MenuType.BLE)
        {
            bleButton.gameObject.SetActive(true);
            bleButton.IsToggled = true;
        }else if (menuState == MenuType.Angle)
        {
            angleButton.gameObject.SetActive(true);
            angleButton.IsToggled = true;
        }else if (menuState == MenuType.PRF)
        {
            prfButton.gameObject.SetActive(true);
            prfButton.IsToggled = true;
        }
    }

    private void UnlockTutorial()
    {
        _unlockedTutorial = true;
        tutorialButton.gameObject.SetActive(true);
    }
}
