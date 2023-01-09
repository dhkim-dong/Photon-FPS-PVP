using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text StatusText;       // ���� ��Ʈ��ũ ���¸� �˷��ִ� �ؽ�Ʈ
    public Transform[] createPos; // 0�� ȣ��Ʈ ��ġ 1�� Ŭ���̾�Ʈ ��ġ
    public InputField NickNameInput; // InputField�� �Է��� ���� 

    public static NetworkManager instance;

    public int playerCount;

    public PhotonView PV;

    [SerializeField] private GameObject loginPhase;  // �α��� �� ���� ������Ʈ 
    [SerializeField] private GameObject waitingText; // �α��� �� �ߴ� �ȳ� �޽���

    private void Awake()
    {
        Screen.SetResolution(1080, 1920, false);
        instance = this;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        Debug.Log(" �����Ϳ� ����");
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        JoinOrCreateRoom();
    }

    public void JoinOrCreateRoom() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);

    public override void OnJoinedRoom() // �濡 ����� ���ÿ� Player ĳ���͸� �����Ѵ�.
    {
        Debug.Log("���η�üũ");

        PV.RPC("PlayerCountUp", RpcTarget.All);

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("Player", createPos[0].position, Quaternion.identity);
        else
            PhotonNetwork.Instantiate("Player", createPos[1].position, Quaternion.identity);

        RoundManager.instance.isBegin = true;
        loginPhase.SetActive(false);
        waitingText.SetActive(true);
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
        Debug.Log("����");
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("������");
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
