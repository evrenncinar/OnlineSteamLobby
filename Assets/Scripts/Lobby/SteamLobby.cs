using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SteamLobby : MonoBehaviour
{
   public static SteamLobby instance;
   //Callbacks
   protected Callback<LobbyCreated_t> lobbyCreated;
   protected Callback<GameLobbyJoinRequested_t> JoinRequest;
   protected Callback<LobbyEnter_t> lobbyEntered;

   //Variables

   public ulong CurrentLobbyID;
   private const string HostAddressKey = "HostAddress";
   private CustomNetworkManager manager;

   void Start()
   {
        if(!SteamManager.Initialized) { return; }
        MakeInstance();
        manager = GetComponent<CustomNetworkManager>();
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        
   }

    void MakeInstance()
    {
        if (instance == null)
            instance = this;
    }

   public void JoinLobby(CSteamID _lobbyID)
   {
        try
        {
            SteamMatchmaking.JoinLobby(_lobbyID);
        }
        catch (System.Exception)
        {
            print("Böyle bir Lobby kodu yok");
        }
   }
   

   public void HostLobby()
   {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);

   }
   private void OnLobbyCreated(LobbyCreated_t callback)
   {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("Lobby creation failed");
            return;
        }
        Debug.Log("Lobby Created successfully" + callback.m_ulSteamIDLobby);

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), 
            HostAddressKey, 
            SteamUser.GetSteamID().ToString()
        );

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "Name",
            SteamFriends.GetPersonaName().ToString() + "'s Room"
        );
   }

   private void OnJoinRequest(GameLobbyJoinRequested_t callback)
   {
        Debug.Log("Request To Join Lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
   }

   private void OnLobbyEnter(LobbyEnter_t callback)
   {
       //Everyone
       CurrentLobbyID = callback.m_ulSteamIDLobby;
       Debug.Log(CurrentLobbyID);

       //Clients
       if (NetworkServer.active){ return; }

       manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

       manager.StartClient();
   }
}
