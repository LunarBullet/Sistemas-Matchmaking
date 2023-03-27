using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Proyecto26;

public class AutentificacionUsuario : MonoBehaviour
{
    private TextMeshProUGUI userNameText;
    private TMP_InputField inputName;

    public TMP_InputField inputEmailLI;
    public TMP_InputField inputPasswordLI;

    public TMP_InputField inputEmailSU;
    public TMP_InputField inputNameSU;
    public TMP_InputField inputPasswordSU;


    public static string playerName;
    public User user;

    private string idToken;
    public static string localId;

    private string databaseURL = "https://snakeio-17b24-default-rtdb.firebaseio.com/users/";
    private string WebAPIKey = "AIzaSyB0Ez-Ht5kc-FTei8YEg89aHjbLFHf7IZE";

    void Start()
    {

    }

    // Esta parte solo es para testear que s� este recibiendo los datos de firebase
    /*public void IngresarNombreUsuario()
    {
        PostToDatabase();
    }
    public void GetUserData()
    {
        RetriveFromeDatabase();
    }
    public void UpdateData()
    {
        userNameText.text = "Nombre: " + user.userName;
    }
    private void RetriveFromeDatabase()
    {
        RestClient.Get<User>(databaseURL + inputName.text + ".json").Then(response =>
        {
            user = response;
            UpdateData();
        });
    }*/

    private void PostToDatabase()
    {
        User user = new User();
        RestClient.Put(databaseURL+ localId+ ".json", user);
    }
    public void SignUpButton()
    {
        SignUp(inputEmailSU.text, inputNameSU.text, inputPasswordSU.text);
    }
    public void LogInButton()
    {
        LogIn(inputEmailLI.text, inputPasswordLI.text);
    }
    private void SignUp(string email, string username, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignUpResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + WebAPIKey, userData).Then(
            response =>
            {
                idToken = response.idToken;
                localId = response.localId;
                playerName = username;
                PostToDatabase();

            }).Catch(error =>
            {
                Debug.Log(error);
            });
        
    }
    private void LogIn(string email, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignUpResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + WebAPIKey, userData).Then(response =>
        {
            idToken = response.idToken;
            localId = response.localId;
            GetUsername();

        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    private void GetUsername()
    {
        RestClient.Get<User>(databaseURL + localId + ".json").Then(response =>
        {
            playerName = response.userName;
            Debug.Log(playerName + " iniciaste sesion");

        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    

}
[System.Serializable] 
public class User
{
    public string userName;
    public string localId;

    public User()
    {
        userName = AutentificacionUsuario.playerName;
        localId = AutentificacionUsuario.localId;
    }
    public User(string userName)
    {
        this.userName = userName; 
    }
}
[System.Serializable]
public class SignUpResponse
{
    public string localId;
    public string idToken;

}
