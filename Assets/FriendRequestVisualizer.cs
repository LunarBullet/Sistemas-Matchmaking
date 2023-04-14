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
    [SerializeField] string friendRequestNames;
    [SerializeField] string friendNamesList;

    List<string> friendRequestNamesList;
    List<string> friendsNamesList;
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

            friendNamesList = response.Friends;
            string[] namesFriendArray = friendNamesList.Split(',');
            friendsNamesList = namesFriendArray.ToList();
            textMeshProUGUIFriends.text = "";
            foreach (var item in friendsNamesList)
            {
                if (item == "") continue;
                textMeshProUGUIFriends.text += item + "\n";
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
        friendNameToAccept = friendNameToAccept.Trim();
        RestClient.Get<User>(databaseURL + autentificacionUsuario.user.localId + ".json").Then(response =>
        {
            if (response.Friends.Contains(friendNameToAccept)) return;

            if (response.FriendsRequests.Contains(friendNameToAccept))
            {
                User updatedUser = response;
                friendNameToAccept = friendNameToAccept.Trim();
                updatedUser.FriendsRequests = updatedUser.FriendsRequests.Trim();

                updatedUser.FriendsRequests = Regex.Replace(updatedUser.FriendsRequests, @"\p{C}+", "");
                updatedUser.FriendsRequests = updatedUser.FriendsRequests.Replace(friendNameToAccept + ",", "");
                updatedUser.FriendsRequests = updatedUser.FriendsRequests.Replace("," + friendNameToAccept, "");
                updatedUser.FriendsRequests = updatedUser.FriendsRequests.Replace( friendNameToAccept, "");

                updatedUser.Friends += "," + friendNameToAccept;

                RestClient.Patch<User>(databaseURL + autentificacionUsuario.user.localId + ".json", updatedUser);


            }
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
}
