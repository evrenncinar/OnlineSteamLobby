using UnityEngine;
using TMPro;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;

    public void SetText(string str)
    {
        _messageText.text = str;
    }
}
