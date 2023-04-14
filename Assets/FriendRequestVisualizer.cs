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
    [SerializeField]TextMeshProUGUI textMeshProUGUI;

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
            textMeshProUGUI.text = "";
            foreach (var item in friendRequestNamesList)
            {
                if (item=="") continue; //we ignore da first item;
                textMeshProUGUI.text += item + "te ha enviado una solicitud" + "\n";
            }

        }).Catch(error =>
        {
            Debug.Log(error);
        });




        
    }

}
