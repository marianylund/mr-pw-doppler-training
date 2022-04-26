using System;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class EstimateMenuState : MenuState
{
    public override MenuType GetMenuType() => MenuType.Estimate;
    [SerializeField] private PressableButton newValue;
    [SerializeField] private PressableButton showAnswer;
    [SerializeField] private PressableButton submitValue;
    [SerializeField] private DopplerUI dopplerUI;

    private float InputValue => Context.slidersStateController.GetCurrentInputValue();
    
    private void Start()
    {
        newValue.ButtonPressed.AddListener(dopplerUI.SetRandomBloodVelocityWithinRange);
        showAnswer.ButtonPressed.AddListener(ShowAnswer);
        submitValue.ButtonPressed.AddListener(SubmitValue);
    }

    private void ShowAnswer()
    {
        var actualValue = dopplerUI.GetBloodVelocity();
        var isCorrect = dopplerUI.IsBloodVelocityEqual(InputValue);
        Debug.Log("The blood velocity estimate was :" + isCorrect + " true value is: " + actualValue + " but was: " + InputValue);

    }
    
    private void SubmitValue()
    {
        Debug.Log("The blood velocity estimate was :" + dopplerUI.IsBloodVelocityEqual(InputValue));
    }

    public override void Show()
    {
        gameObjectMenu.SetActive(true);
        Context.slidersStateController.SetEstimateState();
    }

    public override void Hide()
    {
        Context.slidersStateController.HideAll();
        gameObjectMenu.SetActive(false);
    }
}
