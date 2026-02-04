using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
public class NetworkUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;
    public void StartHost()
    {
        if (NetworkManager.Singleton.StartHost())
            panel.SetActive(false);
    }
    public void StartClient()
    {
        if (NetworkManager.Singleton.StartClient())
            panel.SetActive(false);
    }
}
