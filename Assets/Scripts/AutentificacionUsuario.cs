using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using TMPro;
using Proyecto26;

public class AutentificacionUsuario : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField inputEmailLI;
    public TMP_InputField inputPasswordLI;
    public TMP_InputField inputEmailSU;
    public TMP_InputField inputNameSU;
    public TMP_InputField inputPasswordSU;

    [Header("Texts")]
    public TextMeshProUGUI passwordSignUpError;
    public TextMeshProUGUI passwordLogInError;
    public TextMeshProUGUI profileName;
    public TextMeshProUGUI profileNameMatch;
    public TextMeshProUGUI userStatusText;

    [Header("Gameo Objects")]
    public GameObject logInPanel;
    public GameObject signUpPanel;
    public GameObject mainPanel;

    public static string playerName;
    public User user;

    private string idToken;
    public static string localId;
    public static string status = "offline";

    private string databaseURL = "https://snakeio-17b24-default-rtdb.firebaseio.com/users/";
    private string WebAPIKey = "AIzaSyB0Ez-Ht5kc-FTei8YEg89aHjbLFHf7IZE";

    public static fsSerializer serializer = new fsSerializer();

    //Envia el registro a firebase 
    private void PostToDatabase()
    {
        User user = new User();
        RestClient.Put(databaseURL+ localId+ ".json", user);
    }
    private void PostOnlineStatus()
    {
        user.status = "online";
        user.userName = playerName;
        RestClient.Patch(databaseURL + localId + ".json", user);
    }
    private void LogOut()
    {
        user.status = "offline";
        user.userName = playerName;
        RestClient.Patch(databaseURL + localId + ".json", user);
    }
    // Metodos para botones
    public void SignUpButton()
    {
        SignUp(inputEmailSU.text, inputNameSU.text, inputPasswordSU.text);
    }
    public void GetUsersStatusButton()
    {
        GetUsersStatus();
    }
    public void LogInButton()
    {
        LogIn(inputEmailLI.text, inputPasswordLI.text);
    }
    public void LogOutButtom()
    {
        LogOut();
    }
    public void DeleteErrorMessage()
    {
        passwordSignUpError.text = "";
        passwordLogInError.text = "";
    }
    public void DeleteUserStatus()
    {
        userStatusText.text = "";
    }
    // Metodo para mostrar el nombre en la interfaz
    public void NameMessage()
    {
        profileName.text = playerName;
        profileNameMatch.text = playerName;
    }
    //Post a firebase para el registro
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
                logInPanel.SetActive(true);
                signUpPanel.SetActive(false);

            }).Catch(error =>
            {
                Debug.Log(error);
                passwordSignUpError.text = "El correo o contraseña es incorrecto " + error; 
            });
            
    }
    //Post a firebase para el login
    private void LogIn(string email, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignUpResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + WebAPIKey, userData).Then(response =>
        {
            idToken = response.idToken;
            localId = response.localId;
            user.localId = localId;
            GetUsername();
            mainPanel.SetActive(true);
            logInPanel.SetActive(false);           

        }).Catch(error =>
        {
            Debug.Log(error);
            passwordLogInError.text = "El correo o contraseña es incorrecto " + error;
        });
    }

    //Get para ver el nombre de usuario
    private void GetUsername()
    {
        RestClient.Get<User>(databaseURL + localId + ".json").Then(response =>
        {
            playerName = response.userName;
            user.userName = playerName;
            Debug.Log(playerName + " iniciaste sesion");
            NameMessage();
            PostOnlineStatus();

        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    private void GetUsersStatus()
    {
        RestClient.Get("https://snakeio-17b24-default-rtdb.firebaseio.com/users" + ".json").Then(response =>
        {
            fsData userData = fsJsonParser.Parse(response.Text);
            Dictionary<string,User> users = null;
            serializer.TryDeserialize(userData, ref users);
            foreach(var user in users.Values)
            {
                userStatusText.text += user.userName + " está " + user.status + "\r\n";
            }
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
    public string status;

    public User()
    {
        status = AutentificacionUsuario.status;
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
