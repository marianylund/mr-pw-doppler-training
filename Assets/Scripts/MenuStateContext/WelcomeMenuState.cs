using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class WelcomeMenuState: MenuState
{
    public override MenuType GetMenuType() => MenuType.Welcome;
    [SerializeField] private PressableButton skipButton;
    
    public override void Show()
    {
        gameObjectMenu.SetActive(true);
        skipButton.ButtonPressed.AddListener(SkipToAfterTutorial);
        Context.interactionHint.ShowHand(Context.nextButton.GetWorldPositionAlongPushDirection(0.0f), "NearSelect");
    }

    public override void Hide()
    {
        Context.interactionHint.StopHand(); 
        skipButton.ButtonPressed.RemoveListener(SkipToAfterTutorial);
        gameObjectMenu.SetActive(false);
    }

    private void SkipToAfterTutorial()
    {
        Context.SetState((MenuType) (((int) MenuType.TutorialFinished) + 1));
    }

    private void OnDestroy()
    {
        skipButton.ButtonPressed.RemoveListener(SkipToAfterTutorial);
    }


}
