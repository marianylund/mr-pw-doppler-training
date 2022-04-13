using UnityEngine;

public abstract class MenuState : MonoBehaviour
{
    [SerializeField] protected GameObject gameObjectMenu;
    
    protected MenuContext Context;
    public void SetContext(MenuContext context)
    {
        this.Context = context;
    }
    public abstract MenuType GetMenuType();
    public abstract void Show();
    public abstract void Hide();
    
    public override string ToString() => GetMenuType().ToString();
}
