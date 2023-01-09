using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private Transform[] playerPos; // 생성될 플레이어의 위치 정보

    private int index; 

    public Vector3 GetSpawnPosition()
    {
        Vector3 pos = playerPos[index++].position;
        if(index >= playerPos.Length)
        {
            index = 0;
        }
        return pos;
    }
}
