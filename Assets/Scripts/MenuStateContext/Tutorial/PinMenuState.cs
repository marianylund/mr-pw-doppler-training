
using Microsoft.MixedReality.Toolkit.UI;

namespace Tutorial
{
    public class PinMenuState : MenuState
    {
        private PressableButtonHoloLens2 pinButton;
        public override MenuType GetMenuType() => MenuType.Pin;
        public override void Show()
        {
            gameObjectMenu.SetActive(true);
            Context.interactionHint.ShowHand(Context.pinButton.transform.position, "NearSelect");
            pinButton = Context.pinButton.GetComponent<PressableButtonHoloLens2>();
            pinButton.ButtonReleased.AddListener(StopHand);
        }
        
        // If the pin button clicked
        public void StopHand()
        {
            Context.interactionHint.StopHand(); 
            pinButton.ButtonReleased.RemoveListener(StopHand);
        }
        
        private void OnDisable()
        {
            if (pinButton != null) pinButton.ButtonReleased.RemoveListener(StopHand);
        }

        public override void Hide()
        {
            if (pinButton != null) pinButton.ButtonReleased.RemoveListener(StopHand);
            Context.interactionHint.StopHand();
            gameObjectMenu.SetActive(false);
        }
    }
}
