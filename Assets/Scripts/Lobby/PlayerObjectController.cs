using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int _connectionID;
    [SyncVar] public int _playerIDNumber;
    [SyncVar] public ulong _playerSteamID;
    [SyncVar] public bool IsGameLeader = false;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string _playerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool _playerReady;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if(manager != null) 
            { 
                return manager; 
            }
            return manager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Start() 
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    { 
        if(isServer)
        {
            this._playerReady = newValue;
        }
        if(isClient)
        {
            LobbyController.instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this._playerReady,!this._playerReady);
    }

    public void ChangeReady()
    {
        if(isOwned)
        {
            CmdSetPlayerReady();   
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.instance.FindLocalPlayer();
        LobbyController.instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.instance.UpdateLobbyName();
        LobbyController.instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        ChatManager.instance.SendChatMessage("Oyuncu çıkış yaptı", _playerName, "red");
        Manager.GamePlayers.Remove(this);
        LobbyController.instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this._playerName, PlayerName);
    }
    
    public void PlayerNameUpdate(string oldName, string newName)
    {
        if(isServer) // Host
        {
            this._playerName = newName;
        }
        if(isClient)
        {
            LobbyController.instance.UpdatePlayerList();
        }
    }

    public void QuitLobby()
    {
        if(isOwned)
        {
            if(IsGameLeader)
            {
                Manager.StopHost();
            }
            else
            {
                Manager.StopClient();
            }
        }
    }

    private void OnDestroy() 
    {
        if(isOwned)
        {
            LobbyController.instance.RemovePlayerItem();
            SteamMatchmaking.LeaveLobby((CSteamID)LobbyController.instance._currentLobbyID);
        }
    }

    
}

