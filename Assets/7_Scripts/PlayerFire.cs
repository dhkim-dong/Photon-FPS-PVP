using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

// 포톤 RPC를 관리하는 Player Class이다.
public class PlayerFire : MonoBehaviourPun
{
    private Camera cam;

    [SerializeField] private TextMeshProUGUI playerName;

    [SerializeField] private HandController theHandController;
    [SerializeField] private WeaponManager theWeaponManager;
    [SerializeField] private GameObject weapons;
    [SerializeField] private GameObject othersUI;
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
            SetLayersRecursively(weapons.transform,"Enemy");
            othersUI.SetActive(false);
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            cam = GetComponentInChildren<Camera>();
            cam.gameObject.SetActive(false);
            gameObject.name = "Enemy";
        }
    }

    public void SetLayersRecursively(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach(Transform child in trans)
        {
            SetLayersRecursively(child, name);
        }
    }

    private void Update()
    {
        playerName.text = PhotonNetwork.LocalPlayer.NickName;
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

    [PunRPC]
    public void RPCWeaponChange(string _type, string _name)
    {
        theWeaponManager.ChangeWeaponRPC(_type, _name);
    }
}
