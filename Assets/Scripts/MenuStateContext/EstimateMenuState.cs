using System;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

public class EstimateMenuState : MenuState
{
    public override MenuType GetMenuType() => MenuType.Estimate;
    [SerializeField] private PressableButton newValue;
    [SerializeField] private PressableButton showAnswer;
    [SerializeField] private PressableButton submitValue;
    [SerializeField] private DopplerUI dopplerUI;
    [SerializeField] private Text descriptionText;

    private string startText;
    private float InputValue => Context.slidersStateController.GetCurrentInputValue();
    
    private void Start()
    {
        newValue.ButtonPressed.AddListener(NewValue);
        showAnswer.ButtonPressed.AddListener(ShowAnswer);
        submitValue.ButtonPressed.AddListener(SubmitValue);
        startText = descriptionText.text;
    }

    private void ShowAnswer()
    {
        var actualValue = dopplerUI.GetBloodVelocity();
         var str = $"The arterial velocity is around <b>{Mathf.RoundToInt(actualValue)}</b> cm/s, the input chosen value is <b>{Mathf.RoundToInt(InputValue)}</b> cm/s.";

        if (Math.Abs(InputValue - actualValue) < 5)
        {
            str += "So you were <b><color=green>correct</color></b>!";
        }
        else if (Math.Abs(InputValue - actualValue) < 15)
        {
            str += "So it was quite close!";
        }

        descriptionText.text = str;

    }

    private void NewValue()
    {
        dopplerUI.SetRandomBloodVelocityWithinRange();
        descriptionText.text =
            "The arterial velocity is updated to a <b><color=yellow>new</color></b> value. Change the input velocity slider and press <b><color=green>Submit Velocity</color></b> when you are ready to check your input value.";
    }
    
    private void SubmitValue()
    {
        var actualValue = dopplerUI.GetBloodVelocity();
        int input = Mathf.RoundToInt(InputValue);
        if (Math.Abs(InputValue - actualValue) < 5)
        {
            Context.myAudioSource.PlayOneShot(Context.clipVelocitySuccess);

            descriptionText.text =
                $"You submitted <b>{input}</b> cm/s and it is <b><color=green>correct</color></b> since the arterial velocity was <b>{Mathf.RoundToInt(actualValue)}</b> cm/s. Well done! Feel free to try again by pressing <b><color=yellow>New Value</color></b> button";
        }
        else
        {
            descriptionText.text = $"You submitted <b>{input}</b> cm/s and it is not quite there yet. Try again? Or press <b><color=yellow>See Answer</color></b> to check the actual value";
        }
    }

    public override void Show()
    {
        gameObjectMenu.SetActive(true);
        Context.slidersStateController.SetEstimateState();
        descriptionText.text = startText;
    }

    public override void Hide()
    {
        Context.slidersStateController.HideAll();
        gameObjectMenu.SetActive(false);
    }
}
