using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MultiplayerManager.Instance.OnTryingToJoinGame += OnConnecting;
        MultiplayerManager.Instance.OnFailedToJoinGame += OnFailedConnection;
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnConnecting(object sender, System.EventArgs args)
    {
        Show();
    }

    private void OnFailedConnection(object sender, System.EventArgs args)
    {
        Hide();
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
        MultiplayerManager.Instance.OnTryingToJoinGame += OnConnecting;
        MultiplayerManager.Instance.OnFailedToJoinGame += OnFailedConnection;
    }
}
