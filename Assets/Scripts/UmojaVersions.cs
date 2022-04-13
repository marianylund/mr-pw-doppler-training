using UnityEngine;
using UnityEngine.UI;
using Vuforia;

[RequireComponent(typeof(Text))]
public class UmojaVersions : MonoBehaviour
{
    private void Start()
    {
        var versions = $"<size=13><b>HoloUmoja</b>: {Application.version}</size>\n" +
                       $"Unity: {Application.unityVersion}\n" +
                       $"Vuforia: {VuforiaApplication.GetVuforiaLibraryVersion()}";
        GetComponent<Text>().text = versions;
    }
}
