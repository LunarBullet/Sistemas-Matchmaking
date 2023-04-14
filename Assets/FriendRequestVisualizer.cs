using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using TMPro;
using Proyecto26;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Linq;

public class FriendRequestVisualizer : MonoBehaviour
{
    private string databaseURL = "https://snakeio-17b24-default-rtdb.firebaseio.com/users/";
    private string WebAPIKey = "AIzaSyB0Ez-Ht5kc-FTei8YEg89aHjbLFHf7IZE";

    [SerializeField] AutentificacionUsuario autentificacionUsuario;
    string friendRequestNames;

    List<string> friendRequestNamesList;
    List<string> friendNamesList;
    string friendsNames ="";

    [SerializeField] TextMeshProUGUI textMeshProUGUIFriendRequests;
    [SerializeField] TextMeshProUGUI textMeshProUGUIFriends;
    [SerializeField] TextMeshProUGUI textMeshProUGUIFriendToAddText;

    float timer, timerMax = 0.15f;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        //keeps the list updated in real time
        timer += Time.deltaTime;
        if (timer>timerMax)
        {
            ReWriteInTMP();
            timer = 0;
        }
    }

    void ReWriteInTMP()
    {
        RestClient.Get<User>(databaseURL + autentificacionUsuario.user.localId + ".json").Then(response =>
        {
            friendRequestNames = response.FriendsRequests;

            string[] namesArray = friendRequestNames.Split(',');
            friendRequestNamesList = namesArray.ToList();
            textMeshProUGUIFriendRequests.text = "";
            foreach (var item in friendRequestNamesList)
            {
                if (item=="") continue; //we ignore da first item;
                textMeshProUGUIFriendRequests.text += item + " te ha enviado una solicitud" + "\n";
            }

        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }

    public void ReadTextAndAcceptFriend()
    {
        AcceptFriendRequest(textMeshProUGUIFriendToAddText.text);
        //RestClient.Patch(databaseURL + autentificacionUsuario.originalUserBackup.localId + ".json", autentificacionUsuario.originalUserBackup);
    }

    void AcceptFriendRequest(string friendNameToAccept)
    {

        friendNameToAccept = Regex.Replace(friendNameToAccept, @"\p{C}+", "");
        RestClient.Get<User>(databaseURL + autentificacionUsuario.user.localId + ".json").Then(response =>
        {
            if (response.Friends.Contains(friendNameToAccept)) return;

            friendRequestNames = response.FriendsRequests;

            string[] namesArray = friendRequestNames.Split(',');
            friendRequestNamesList = namesArray.ToList();
            textMeshProUGUIFriendRequests.text = "";
            foreach (var item in friendRequestNamesList)
            {
                if (item == "") continue; //we ignore da first item;

                if (item== friendNameToAccept)
                {
                    //update local
                    friendsNames += "," + friendNameToAccept;
                    friendRequestNames = friendRequestNames.Replace($"{friendNameToAccept},", ""); //removes friendNameToAccept;

                    //parse new data and send to server 
                    User updatedUser = response;
                    updatedUser.Friends = friendsNames;
                    updatedUser.FriendsRequests = friendRequestNames;

                    RestClient.Patch<User>(databaseURL + autentificacionUsuario.user.localId + ".json", updatedUser).Catch(error =>
                    {
                        Debug.Log(error);
                    }); ;

                }
                textMeshProUGUIFriendRequests.text += item + "te ha enviado una solicitud" + "\n";
            }

            //add local changes
            string[] namesFriendsArray = response.Friends.Split(',');
            friendNamesList = namesFriendsArray.ToList();
            textMeshProUGUIFriends.text = "";

            foreach (var item in friendNamesList)
            {
                if (item == "") continue; //we ignore da first item;

                if (item == friendNameToAccept)
                {
                    textMeshProUGUIFriends.text += item + "\n";
                }
                
            }



        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
}


