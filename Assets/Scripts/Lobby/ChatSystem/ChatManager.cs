using System;
using TMPro;
using UnityEngine;
using Mirror;
using Steamworks;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager instance;

    [SerializeField] private ChatMessage _chatMessagePrefab;
    [SerializeField] private CanvasGroup _chatContent;
    [SerializeField] private TMP_InputField _chatInputField;

    private string _playerName;

    private void Awake()
    {
        if (SteamManager.Initialized)
        {
            _playerName = SteamFriends.GetPersonaName();
        }
        instance = this;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        ChatManager.instance.SendChatMessage("Oyuncu giriş yaptı", _playerName, "green");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SendChatMessage(_chatInputField.text, _playerName);
            _chatInputField.text = "";
        }
    }

    public void setPlayerName(string _name)
    {
        _playerName = _name;
    }

    public void SendChatMessage(string _message, string _who = null, string _color = null) 
    {
        if(string.IsNullOrWhiteSpace(_message)){return;}
        string s = _who + " : " + _message;
        if(_color != null) s = "<color=" + _color + ">" + s + "</color>";
        CmdSendChatMessage(s);
    }

    void AddMessage(string message)
    {
        ChatMessage chatMessage = Instantiate(_chatMessagePrefab, _chatContent.transform);
        chatMessage.SetText(message);
    }

    [Command(requiresAuthority = false)]
    void CmdSendChatMessage(string _msg)
    {
        ReceiveChatMessage(_msg);
    }

    [ClientRpc]
    void ReceiveChatMessage(string _msg)
    {
        ChatManager.instance.AddMessage(_msg);
    }
}
