using UnityEngine;
using UnityEngine.UI;

public class BLEMenuState : MenuState
{
    public override MenuType GetMenuType() => MenuType.BLE;
    // Reference to the BLE Component
    
    [SerializeField] private Text textIsScanning;
    [SerializeField] private Text textTargetDeviceConnection;
    [SerializeField] private Text textTargetDeviceData;
    [SerializeField] private Text textDiscoveredDevices;
    
    public override void Show()
    {
        gameObjectMenu.SetActive(true);
    }

    public override void Hide()
    {
        gameObjectMenu.SetActive(false);
    }
}
