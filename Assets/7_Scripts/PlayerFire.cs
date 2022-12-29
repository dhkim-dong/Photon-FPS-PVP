using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

// ����ڰ� �߻� ��ư�� ������ ���� �߻��ϰ� �ʹ�.
public class PlayerFire : MonoBehaviourPun
{
    private Camera cam;

    [SerializeField]
    private GameObject appear;

    [SerializeField] private TextMeshProUGUI playerName;

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
            appear.SetActive(false);
        }
        // �׷��� ������ layer�� enemy�� ����
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
}
