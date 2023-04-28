using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private ConnectionMethod connectionMethod;
    [SerializeField] private Button hostBttn; 
    [SerializeField] private Button joinBttn;
    [SerializeField] private InputField joinCodeInput;
    [SerializeField] private TestRelay relayScript;
    [SerializeField] private UsernameUI usernameUI; 


    private void Awake()
    {
        hostBttn.onClick.AddListener(() => 
        {
            if (connectionMethod == ConnectionMethod.NETCODE)
            {
                MultiplayerManager.Instance.StartHost();
            }
            else if(connectionMethod == ConnectionMethod.RELAY)
            {
                relayScript.CreateRelay();
            }
            hostBttn.gameObject.SetActive(false);
            joinBttn.gameObject.SetActive(false);
            joinCodeInput.gameObject.SetActive(false);
            usernameUI.gameObject.SetActive(false);
        });
        joinBttn.onClick.AddListener(() =>
        {
            if (connectionMethod == ConnectionMethod.NETCODE)
            {
                MultiplayerManager.Instance.StartClient();
            }
            else if (connectionMethod == ConnectionMethod.RELAY)
            {
                relayScript.JoinRelay(joinCodeInput.text);
            }
            joinBttn.gameObject.SetActive(false);
            hostBttn.gameObject.SetActive(false);
            joinCodeInput.gameObject.SetActive(false);
            usernameUI.gameObject.SetActive(false);
        });
        if (connectionMethod == ConnectionMethod.RELAY)
        {
            joinCodeInput.gameObject.SetActive(true);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum ConnectionMethod
{
    NETCODE,
    RELAY
}