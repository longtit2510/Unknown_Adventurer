using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPoint;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float respawnTime;

    private float respawnTimeStart;

    private CameraFollow maincam;

    private bool respawn;

    private void Start()
    {
        maincam = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
    }
    private void Update()
    {
        CheckRespawn();
    }
    public void Respawn()
    {
        respawnTimeStart = Time.time;
        respawn = true;

    }
    private void CheckRespawn()
    {
        if(respawn && Time.time >= respawnTimeStart + respawnTime)
        {
            var playerTemp = Instantiate(player, respawnPoint);
            maincam.target = playerTemp.transform;
            respawn = false;
        }
    }
}
