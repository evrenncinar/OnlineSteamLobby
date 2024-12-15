using System;
using Steamworks;
using TMPro;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lobbyCode;

    public void HostLobby()
    {
        SteamLobby.instance.HostLobby();
    }

    public void JoinLobby()
    {
        try
        {
            CSteamID steamLobbyID = new CSteamID(ulong.Parse(_lobbyCode.text)); // Lobby ID'yi CSteamID'ye dönüştür
            SteamLobby.instance.JoinLobby(steamLobbyID);
        }
        catch (System.Exception)
        {
            
            print("Böyle bir Lobby kodu yok");
        }
    }
}
