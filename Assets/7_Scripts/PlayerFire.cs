using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

// ���� RPC�� �����ϴ� Player Class�̴�.
public class PlayerFire : MonoBehaviourPun
{
    private Camera cam;

    [SerializeField] private TextMeshProUGUI playerName;

    [SerializeField] private HandController theHandController;
    [SerializeField] private WeaponManager theWeaponManager;
    [SerializeField] private GameObject weapons;
    [SerializeField] private GameObject othersUI;
    // ����� �κ񿡼� �����ϰ� ���� �ٲٴ� �����̱� ������
    // �κ񿡼� ������ �� �����ϴ� ���� �� ���� ���� �ִ�.
    void Start()
    {
        // ���� �÷��̾ �� �ڽ��̶�� layer�� Player��
        if (photonView.IsMine)
        {
            Camera.main.gameObject.SetActive(false);
            cam = GetComponentInChildren<Camera>();
            cam.tag = "MainCamera";
            gameObject.layer = LayerMask.NameToLayer("Player");
            gameObject.name = "Player";
            Cursor.lockState = CursorLockMode.Locked;
        }
        // �׷��� ������ layer�� enemy�� ����
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
