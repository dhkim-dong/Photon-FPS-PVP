using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


// 랜덤 시간에 랜덤 장소에 적이 생성하고 싶다.
// 필요속성 : 랜덤시간 간격, spawn 위치
public class EnemyManager : MonoBehaviourPun
{
    public float minTime = 120f;
    public float maxTime = 180f;
    float createTime = 0;

    public Transform[] spawnPoints;
    public GameObject enemyFactory;
    // Start is called before the first frame update
    void Start()
    {
        createTime = Random.RandomRange(minTime, maxTime);

        Invoke("CreateEnemy", createTime);
    }

    void CreateEnemy()
    {
        int index = Random.Range(0, spawnPoints.Length);
        Vector3 pos = spawnPoints[index].position;

        PhotonNetwork.Instantiate("AI", pos, Quaternion.identity);

        createTime = Random.Range(minTime, maxTime);

        Invoke("CreateEnemy", createTime);
    }
}
