using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text StatusText;

    private void Awake()
    {
        Screen.SetResolution(1080, 1920, false);
        PhotonNetwork.ConnectUsingSettings();
    }
    public PhotonView PV;

    public override void OnConnectedToMaster()
    {
        Debug.Log(" �����Ϳ� ����");
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("���η�üũ");
        Vector3 randPos = Random.insideUnitSphere * 10;
        randPos.y = 1;
        PhotonNetwork.Instantiate("Player", randPos, Quaternion.identity);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("����");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("������");
        }
    }

    // Update is called once per frame
    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();

    }
}
