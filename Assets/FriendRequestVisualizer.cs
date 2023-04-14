using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class FriendRequestVisualizer : MonoBehaviour
{
    string friendRequestNames;
    List<string> friendRequestNamesList;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void WriteInTMP()
    {
        string[] namesArray = friendRequestNames.Split(',');
        friendRequestNamesList = namesArray.ToList();
    }

}
