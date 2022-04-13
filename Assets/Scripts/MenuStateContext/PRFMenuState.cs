public class PRFMenuState : MenuState
{
    public override MenuType GetMenuType() => MenuType.PRF;

    public override void Show()
    {
        gameObjectMenu.SetActive(true);
    }

    public override void Hide()
    {
        gameObjectMenu.SetActive(false);
    }
}
