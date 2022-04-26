public class EstimateMenuState : MenuState
{
    public override MenuType GetMenuType() => MenuType.Estimate;

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
