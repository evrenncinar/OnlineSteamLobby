using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Steamworks;
public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            bool IsGameLeader = GamePlayers.Count == 0;

            PlayerObjectController GamePlayerInstantiate = Instantiate(GamePlayerPrefab);
            GamePlayerInstantiate.IsGameLeader = IsGameLeader;
            GamePlayerInstantiate._connectionID = conn.connectionId;
            GamePlayerInstantiate._playerIDNumber = GamePlayers.Count + 1;
            GamePlayerInstantiate._playerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.CurrentLobbyID, GamePlayers.Count);
            NetworkServer.AddPlayerForConnection(conn,GamePlayerInstantiate.gameObject);
        }
    } 

    public override void OnStopServer()
    {
        GamePlayers.Clear();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if(conn.identity != null)
        {
            PlayerObjectController player = conn.identity.GetComponent<PlayerObjectController>();
            GamePlayers.Remove(player);
        }
        base.OnServerDisconnect(conn);
    }

    public void HostShutDownServer()
    {
        GameObject NetworkManagerObject = GameObject.Find("NetworkManager");
        Destroy(this.GetComponent<SteamManager>());
        Destroy(NetworkManagerObject);
        ResetStatics();
        SceneManager.LoadScene("MainMenu");

        Start();

    }
}
