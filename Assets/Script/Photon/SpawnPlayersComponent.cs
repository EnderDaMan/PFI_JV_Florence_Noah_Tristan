using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayersComponent : MonoBehaviour
{
    [SerializeField] GameObject player1Prebaf;
    [SerializeField] GameObject player2Prebaf;

    [SerializeField] Vector3 spawnPoint;

    bool player1Connected = false;
    bool player2Connected = false;  

    private void Start()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        { 
            PhotonNetwork.Instantiate(player1Prebaf.name, spawnPoint, Quaternion.identity);
        }
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.Instantiate(player2Prebaf.name, spawnPoint, Quaternion.identity);
        }
        
    }
}
