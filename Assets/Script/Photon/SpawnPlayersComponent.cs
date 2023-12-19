using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayersComponent : MonoBehaviour
{
    [SerializeField] GameObject player1Prebaf;
    [SerializeField] GameObject player2Prebaf;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Vector3 spawnPoint;
    [SerializeField] Vector3 spawnPointEnemy;

    float spawnCooldown = 5;
    float elapsedTime = 0;

    private void Start()
    {
        elapsedTime = Time.deltaTime;

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

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if(elapsedTime < spawnCooldown)
        {
            Instantiate(enemyPrefab, spawnPointEnemy, Quaternion.identity);
            elapsedTime = 0;
        }
    }
}
