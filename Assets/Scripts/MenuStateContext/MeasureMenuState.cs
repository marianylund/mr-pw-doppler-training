using UnityEngine;

public class MeasureMenuState : MenuState
{

    [SerializeField] private Transform probeCoachPosition;
    [SerializeField] private RaycastAngle raycastAngle;
    public override MenuType GetMenuType() => MenuType.Measure;
    
    private void IntersectedForTheFirstTime(int newAngle)
    {
        Context.myAudioSource.PlayOneShot(Context.clipTrackingSuccess);
        Context.interactionHint.StopHand();
    }

    public override void Show()
    {
        gameObjectMenu.SetActive(true);
        Context.interactionHint.ShowProbe(probeCoachPosition);
        Context.slidersStateController.SetMeasureState();
        raycastAngle.valueUpdate += IntersectedForTheFirstTime;
    }

    public override void Hide()
    {
        raycastAngle.valueUpdate -= IntersectedForTheFirstTime;
        Context.slidersStateController.HideAll();
        gameObjectMenu.SetActive(false);
        Context.interactionHint.StopHand();
    }

    private void OnDisable()
    {
        raycastAngle.valueUpdate -= IntersectedForTheFirstTime;
        Context.interactionHint.StopHand();
    }
}
