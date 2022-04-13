namespace Tutorial
{
    public class TutorialFinishedState : MenuState
    {
        public override MenuType GetMenuType() => MenuType.TutorialFinished;

        public override void Show()
        {
            gameObjectMenu.SetActive(true);
            // Stop both hands in case some of them are still showing
            Context.interactionHint.StopHand(); 
            Context.interactionHint.StopHand(false); 

        }

        public override void Hide()
        {
            gameObjectMenu.SetActive(false);
        }
    }
}
