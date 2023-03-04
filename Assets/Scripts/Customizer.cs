using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Customizer : NetworkBehaviour
{
    [SerializeField] Color[] allColors;
    [SerializeField] Button[] colorButtons;
    int lastClicked = -1;
    [SerializeField] GameObject errorText;

    public void SetColor(int colorIndex)
    {
        NetworkManager.LocalClient.PlayerObject.GetComponent<CharacterController>().SetColor(allColors[colorIndex]);
        if (IsClient)
        {
            BlockColorServerRpc(colorIndex, lastClicked);
        }
        lastClicked = colorIndex;


    }

    public void SetName(string name)
    {
        NetworkManager.LocalClient.PlayerObject.GetComponent<CharacterController>().SetName(name);
        errorText.SetActive(false);
    }

    public void ReadyUp(int sceneIndex)
    {
        
        if(NetworkManager.LocalClient.PlayerObject.GetComponent<CharacterController>().GetName().Length < 4)
        {
            errorText.SetActive(true);
            return;
        }
        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }
    [ClientRpc]
    private void BlockColorClientRpc(int idx, int previous)
    {
        colorButtons[idx].interactable = false;
        if (previous != -1)
        {
            colorButtons[previous].interactable = true;
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void BlockColorServerRpc(int idx, int previous)
    {
        BlockColorClientRpc(idx, previous);
    }
}
