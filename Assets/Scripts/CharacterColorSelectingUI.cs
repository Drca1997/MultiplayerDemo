using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectingUI : MonoBehaviour
{
    [SerializeField] private int colorID;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;


    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.ChangePlayerColor(colorID);
        });
    }
    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += OnColorChange;
        image.color = MultiplayerManager.Instance.GetPlayerColor(colorID);
        UpdateIsSelected();
    }

    private void OnColorChange(object sender, System.EventArgs args)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (MultiplayerManager.Instance.GetPlayerData().colorID == colorID)
        {
            selectedGameObject.SetActive(true);
        }
        else
        {
            selectedGameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged -= OnColorChange;
    }
}
