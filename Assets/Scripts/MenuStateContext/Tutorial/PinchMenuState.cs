using System;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Tutorial
{
    public class PinchMenuState : MenuState
    {
        public override MenuType GetMenuType() => MenuType.Pinch;
        [SerializeField] private StepSlider sliderBob;
        [SerializeField]
        private Transform movePos;
        public override void Show()
        {
            gameObjectMenu.SetActive(true);
            Context.interactionHint.ShowHand(movePos.position, "Move");
            sliderBob.OnInteractionStarted.AddListener(StopHand);
        }

        private void StopHand(SliderEventData data)
        {
            Context.interactionHint.StopHand();
            sliderBob.OnInteractionStarted.RemoveListener(StopHand);
        }

        private void OnDisable()
        {
            sliderBob.OnInteractionStarted.RemoveListener(StopHand);
        }

        public override void Hide()
        {
            sliderBob.OnInteractionStarted.RemoveListener(StopHand);
            Context.interactionHint.StopHand();
            gameObjectMenu.SetActive(false);
        }
    }
}
