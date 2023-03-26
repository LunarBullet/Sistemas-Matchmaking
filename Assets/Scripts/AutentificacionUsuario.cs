using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Proyecto26;

public class AutentificacionUsuario : MonoBehaviour
{
    public TextMeshProUGUI userNameText;
    public TMP_InputField inputName;

    public string playerName;
    public User user;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Ingresar()
    {
        playerName = inputName.text;
        PostToDatabase();
    }

    public void GetUserData()
    {
        // Esta parte solo es para testear que sí este recibiendo los datos de firebase
        RetriveFromeDatabase();
    }
    public void UpdateData()
    {
        userNameText.text = "Nombre: " + user.userName;
    }

    private void PostToDatabase()
    {
        User user = new User(playerName);
        RestClient.Put("https://snakeio-17b24-default-rtdb.firebaseio.com/"+ playerName+ ".json", user);
    }
    private void RetriveFromeDatabase()
    {
        RestClient.Get<User>("https://snakeio-17b24-default-rtdb.firebaseio.com/" + inputName.text + ".json").Then(response =>
        {
            user = response;
            UpdateData();
        });
    }

}
[System.Serializable] 
public class User
{
    public string userName;
    
    public User(string userName)
    {
        this.userName = userName; 
    }
}
