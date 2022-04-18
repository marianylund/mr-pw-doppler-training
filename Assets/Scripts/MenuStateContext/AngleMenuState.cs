using UnityEngine;

public class AngleMenuState : MenuState
{
    [SerializeField] private AudioClip taskDoneAudioClip;
    [SerializeField] private Transform probeCoachPosition;
    public override MenuType GetMenuType() => MenuType.Angle;

    // TODO: Subscribe to valueUpdate from RaycastAngle the first time to play it
    private void IntersectedForTheFirstTime()
    {
        Context.myAudioSource.PlayOneShot(taskDoneAudioClip);
        Context.interactionHint.StopHand();
    }

    public override void Show()
    {
        gameObjectMenu.SetActive(true);
        Context.interactionHint.ShowProbe(probeCoachPosition);
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
