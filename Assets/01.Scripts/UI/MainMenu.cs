using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _txtUsername;


    public void ConnectedToServer()
    {
        string name = _txtUsername.text;
        UserData userData = new UserData
        {
            username = name
            ,color = Random.ColorHSV()
        };

        ClientSingleton.Instance.StartClient(userData);
    }
}
