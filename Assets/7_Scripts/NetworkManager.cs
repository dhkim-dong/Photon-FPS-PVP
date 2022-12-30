using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text StatusText;       // 현재 네트워크 상태를 알려주는 텍스트
    public Transform[] createPos; // 0은 호스트 위치 1은 클라이언트 위치

    public static NetworkManager instance;

    public int playerCount;

    private void Awake()
    {
        Screen.SetResolution(1080, 1920, false);
        PhotonNetwork.ConnectUsingSettings();
        instance = this;
    }
    public PhotonView PV;

    public override void OnConnectedToMaster()
    {
        Debug.Log(" 마스터에 연결");
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom() // 방에 입장과 동시에 Player 캐릭터를 생성한다.
    {
        Debug.Log("조인룸체크");

        PV.RPC("PlayerCountUp", RpcTarget.All);

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("Player", createPos[0].position, Quaternion.identity);
        else
            PhotonNetwork.Instantiate("Player", createPos[1].position, Quaternion.identity);

        RoundManager.instance.isBegin = true;   
    }

    [PunRPC]
    private void PlayerCountUp()
    {
        playerCount++;
    }

    [PunRPC]
    private void GameIsStartRPC()
    {
        RoundManager.instance.isStart = true;
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("에러");
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("마스터");
        }
    }

    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();

        if (playerCount == 2)
        {
            PV.RPC("GameIsStartRPC", RpcTarget.All);
            playerCount = 0;
        }
    }
}
