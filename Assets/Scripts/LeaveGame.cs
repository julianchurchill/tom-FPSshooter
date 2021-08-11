using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LeaveGame : MonoBehaviour
{
    public void OnClick()
    {
        PhotonNetwork.LeaveRoom(true);
        SceneManager.LoadScene("MainMenu");

    }
}
