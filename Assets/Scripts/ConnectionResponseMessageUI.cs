using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    void Start()
    {
        MultiplayerManager.Instance.OnFailedToJoinGame += OnFailedConnection;
        Hide();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnFailedConnection(object sender, System.EventArgs args)
    {
        Show();
        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if (messageText.text == "")
        {
            messageText.text = "Failed to connect";
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
       
    }

    private void Hide()
    {
        gameObject.SetActive(false);
      
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnFailedToJoinGame -= OnFailedConnection;
        
    }
}
