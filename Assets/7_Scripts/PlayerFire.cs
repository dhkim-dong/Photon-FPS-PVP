using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

// 사용자가 발사 버튼을 누르면 총을 발사하고 싶다.
public class PlayerFire : MonoBehaviourPun
{
    private Camera cam;

    [SerializeField] private TextMeshProUGUI playerName;

    [SerializeField] private HandController theHandController;

    // 여기는 로비에서 접속하고 나서 바꾸는 로직이기 때문에
    // 로비에서 접속할 때 관리하는 것이 더 편할 수도 있다.
    void Start()
    {
        // 현재 플레이어가 나 자신이라면 layer를 Player로
        if (photonView.IsMine)
        {
            Camera.main.gameObject.SetActive(false);
            cam = GetComponentInChildren<Camera>();
            cam.tag = "MainCamera";
            gameObject.layer = LayerMask.NameToLayer("Player");
            gameObject.name = "Player";
            Cursor.lockState = CursorLockMode.Locked;
        }
        // 그렇지 않으면 layer를 enemy로 지정
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            cam = GetComponentInChildren<Camera>();
            cam.gameObject.SetActive(false);
            gameObject.name = "Enemy";
        }
    }

    private void Update()
    {
        playerName.text = "Player" + photonView.ViewID;
    }

    [PunRPC]
    public void Damage()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void PunMuzzleFlash()
    {
        theHandController.MuzzleFlash();
    }

    [PunRPC]
    public void PunReload()
    {
        theHandController.ReloadForPun();
    }
}
