using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using TMPro;
using Proyecto26;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

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

    [Header("Add Friend Variables")]
    public TextMeshProUGUI AddFriendInputField;
    public string AddFriendName;

    public static string playerName;
    public User user;

    private string idToken;
    public static string localId;
    public static string status = "offline";

    private string databaseURL = "https://snakeio-17b24-default-rtdb.firebaseio.com/users/";
    private string WebAPIKey = "AIzaSyB0Ez-Ht5kc-FTei8YEg89aHjbLFHf7IZE";

    public static fsSerializer serializer = new fsSerializer();

    //friends logic variables
    string idFromName;
    [SerializeField] string OurLocalFriendsRequests="";


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

    //Send a friend request logic
    public void SendFriendRequest()
    {
       StartCoroutine(IE_SendFriendRequest());

    }

    IEnumerator IE_SendFriendRequest()
    {
        AddFriendName = AddFriendInputField.text;
        User userToModify = new User();
        string targetLocalID = "";
        //get the users local id through his name
        yield return new WaitForSeconds(0.1f);
        GetAnIDFromName(AddFriendName);
        yield return new WaitForSeconds(2f);
        RestClient.Get<User>(databaseURL + idFromName + ".json").Then(response =>
        {
            
            userToModify.Friends = response.Friends;
            userToModify.FriendsRequests = response.FriendsRequests;
            userToModify.localId = response.localId;
            userToModify.IsUserMatchmaking = response.IsUserMatchmaking;
            userToModify.status = response.status;
            userToModify.userName = response.userName;


            targetLocalID = response.localId;
            print(userToModify.localId+":"+userToModify.localId.Length);
            print(idFromName+":"+idFromName.Length);
            userToModify.FriendsRequests = response.FriendsRequests + "," + user.userName;

            RestClient.Patch(databaseURL + targetLocalID + ".json", userToModify).Catch(error =>
            {
                print("error when trying to patch: " + error);
            });

        }).Catch(error =>
        {
            Debug.Log(error);
        });
        //then we send 
        yield return new WaitForSeconds(0.25f);
        print("tried to send friend requesto to id:" + idFromName+ ":name:"+ userToModify.userName + " from:" + user.userName);

        //after this we have to make some type of function that refreshes our local friendrequests 
        //and then, when we hit accept a friend request, we have to send that new info to the server, remove the old info too, and change (update) our local info
    }

    private void Start()
    {
        StartCoroutine(CheckForServerChanges());
    }
    IEnumerator CheckForServerChanges()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.15f);
            CheckAndUpdateFriendRequests();
            yield return new WaitForSeconds(0.15f);
        }
    }

    //We use this to update our local friend requests lists, this is summoned whenever theres a change in the server, we could even periodically check the server or something
    private void CheckAndUpdateFriendRequests()
    {
        RestClient.Get<User>(databaseURL + localId + ".json").Then(response =>
        {
            if (response.FriendsRequests!=OurLocalFriendsRequests)
            {
                string oldLocalFriendsRequests = (string)OurLocalFriendsRequests.Clone();
                OurLocalFriendsRequests = response.FriendsRequests;
                print("updated local friend requests from:" + oldLocalFriendsRequests + ":to:" + response.FriendsRequests);
            }
        }).Catch(error =>
        {
            Debug.Log(error);
        });
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
        string Friends="";
        string FriendsRequests = "";
        string IsUserMatchmaking="false";

        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"Friends\":\"" + Friends + "\",\"FriendsRequests\":\"" + FriendsRequests + "\",\"IsUserMatchmaking\":\"" + IsUserMatchmaking + "\",\"returnSecureToken\":true}";
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

    //couldve done better with an asyc method or similar, but im experienced with that
    public void GetAnIDFromName(string _playerName)
    {
        _playerName = Regex.Replace(_playerName, @"\p{C}+", "");
        //foreach (char c in _playerName) //for debudding as i had a weird error
        //{
        //    Debug.Log($"Character '{c}' has Unicode value {Convert.ToUInt16(c)}");
        //}

        RestClient.Get("https://snakeio-17b24-default-rtdb.firebaseio.com/users" + ".json").Then(response =>
        {
            fsData userData = fsJsonParser.Parse(response.Text);
            Dictionary<string, User> users = null;
            serializer.TryDeserialize(userData, ref users);

            foreach (var user in users.Values)
            {
                //print("looking for " + _playerName + " in " + user.userName + " with id: " + user.localId);
                print("_playerName:" + _playerName+","+_playerName.Length+",user.username:"+user.userName+","+user.userName.Length);

                if (_playerName.Equals(user.userName))
                {
                    print("user " + _playerName + " found!");
                    idFromName = user.localId;
                    return;
                }

            }

        }).Catch(error =>
        {
            Debug.Log(error);
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

    public string Friends;
    public string FriendsRequests;
    public string IsUserMatchmaking;
    public User()
    {
        status = AutentificacionUsuario.status;
        userName = AutentificacionUsuario.playerName;
        localId = AutentificacionUsuario.localId;
        Friends = "";
        FriendsRequests = "";
        IsUserMatchmaking = "false";

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
