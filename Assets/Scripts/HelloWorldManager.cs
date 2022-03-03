using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldManager : MonoBehaviour
    {
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabel();
            }
            
            GUILayout.EndArea();
        }

        private static void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        }

        private static void StatusLabel()
        {
            GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        }
    }
}