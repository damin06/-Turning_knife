using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ConnectedToServer(string username)
    {
        UserData userData = new UserData
        {
            username = username,
            color = Random.ColorHSV()
        };

        ClientSingleton.Instance.StartClient(userData);
    }
}
