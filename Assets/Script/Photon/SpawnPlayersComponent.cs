using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayersComponent : MonoBehaviour
{
    [SerializeField] GameObject player1Prebaf;

    [SerializeField] Vector3 p1SpawnPoint;

    private void Start()
    {
        PhotonNetwork.Instantiate(player1Prebaf.name, p1SpawnPoint, Quaternion.identity);
    }
}
