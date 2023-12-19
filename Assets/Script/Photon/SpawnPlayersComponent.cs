using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayersComponent : MonoBehaviour
{
    [SerializeField] GameObject player1Prebaf;
    [SerializeField] GameObject player2Prebaf;

    [SerializeField] Vector3 spawnPoint;

    private void Start()
    {
        player1Prebaf.layer = 3;
        player2Prebaf.layer = 3;

        if(PhotonNetwork.IsConnected)
        {
            if(PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                    PhotonNetwork.Instantiate(player2Prebaf.name, spawnPoint, Quaternion.identity);
                else
                    PhotonNetwork.Instantiate(player1Prebaf.name, spawnPoint, Quaternion.identity);
            }
        }
        
    }
}
