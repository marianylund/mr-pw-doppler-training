using UnityEngine;

public class AngleMenuState : MenuState
{
    [SerializeField] private AudioClip taskDoneAudioClip;
    
    public override MenuType GetMenuType() => MenuType.Angle;

    // If intersecting then good job!
    private void IntersectedForTheFirstTime()
    {
        Context.myAudioSource.PlayOneShot(taskDoneAudioClip);
        Context.interactionHint.StopHand();
    }

    public override void Show()
    {
        gameObjectMenu.SetActive(true);
        // TODO: find a relevant position to show the hand at 
        Context.interactionHint.ShowHand(this.transform.position);
    }

    public override void Hide()
    {
        gameObjectMenu.SetActive(false);
        Context.interactionHint.StopHand();
    }

    private void OnDisable()
    {
        Context.interactionHint.StopHand();
    }
}
