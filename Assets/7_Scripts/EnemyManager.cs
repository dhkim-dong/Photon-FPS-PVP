using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


// ���� �ð��� ���� ��ҿ� ���� �����ϰ� �ʹ�.
// �ʿ�Ӽ� : �����ð� ����, spawn ��ġ
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
