using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;
using System;
public class LobbyListItemScript : MonoBehaviour
{
    
    public string _playerName;
    public int _connectionID;
    public ulong _playerSteamID;
    private bool  _avatarReceived;

    public TMP_Text _playerNameText;
    public RawImage _playerAvatarImage;
    public TMP_Text _playerReadyText;
    public bool _isReady;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    public void ChangeReadyState()
    {
        if(_isReady) // Ready
        {
            _playerReadyText.text = "Ready";
            _playerReadyText.color = Color.green;
        }
        else // Not Ready
        {
            _playerReadyText.text = "UnReady";
            _playerReadyText.color = Color.red;
        }
    }

    private void Start() 
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if(callback.m_steamID.m_SteamID == _playerSteamID)
        {
            _playerAvatarImage.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else // another player
        {
            return;
        }
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if(isValid)
        {
            byte[] imageData = new byte[width * height * 4];
            isValid = SteamUtils.GetImageRGBA(iImage, imageData, (int)(width * height * 4));
            if(isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(imageData);
                texture.Apply();
            }
        }
        _avatarReceived = true;
        return texture;
    }

    void GetPlayerAvatar()
    {
        int ImageId = SteamFriends.GetLargeFriendAvatar((CSteamID)_playerSteamID);
        if(ImageId == -1){ return; }
        _playerAvatarImage.texture = GetSteamImageAsTexture(ImageId);
    }

    public void SetPlayerValues()
    {
        _playerNameText.text = _playerName;
        ChangeReadyState();
        if(!_avatarReceived) {GetPlayerAvatar();}
    }
}
