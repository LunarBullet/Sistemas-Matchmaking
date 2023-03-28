using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Proyecto26;

public class MainPage : MonoBehaviour
{
    public TextMeshProUGUI profileName;
    public TextMeshProUGUI profileNameMatch;

    public static void NameMessageStatic()
    {
    }

    public void NameMessage()
    {
        profileName.text = AutentificacionUsuario.playerName;
        profileNameMatch.text = AutentificacionUsuario.playerName;
    }
}
