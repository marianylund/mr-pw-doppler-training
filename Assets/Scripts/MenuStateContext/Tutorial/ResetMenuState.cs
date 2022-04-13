using System;
using UnityEngine;

namespace Tutorial
{
    public class ResetMenuState : MenuState
    {
        public override MenuType GetMenuType() => MenuType.Reset;

        [SerializeField]
        private Transform flipPos;

        public override void Show()
        {
            gameObjectMenu.SetActive(true);
            // If Animator is not able to play HandFlip, add it to the animator controller manually
            Context.interactionHint.ShowHand(flipPos.position, "HandFlip", rightHand:false);
            Context.resetButton.ButtonReleased.AddListener(StopHand);
        }
        
        // If the reset button clicked
        public void StopHand()
        {
            Context.interactionHint.StopHand(rightHand:false);
            Context.resetButton.ButtonReleased.RemoveListener(StopHand);
        }

        private void OnDisable()
        {
            Context.resetButton.ButtonReleased.RemoveListener(StopHand);
        }

        public override void Hide()
        {
            Context.resetButton.ButtonReleased.RemoveListener(StopHand); 
            Context.interactionHint.StopHand(rightHand:false);
            gameObjectMenu.SetActive(false);
        }
    }
}
