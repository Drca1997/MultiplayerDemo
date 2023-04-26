using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton; 
    [SerializeField] private Button readyButton;


    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        readyButton.onClick.AddListener(() => 
        {
            CharacterSelection.Instance.SetPlayerReady();
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "READY";
            readyButton.gameObject.SetActive(false);
        });
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
