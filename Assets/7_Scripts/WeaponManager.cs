using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    // ���� �ߺ� ��ü ���� ����
    public bool isChangeWeapon = false;
    public bool isSniper = false;

    // ���� ���⿡ �ʿ��� ����
    public Transform currentWeapon;
    public Animator currentWeaponAnim;

    // ���� ������ Ÿ��
    [SerializeField] private string currentWeaponType;

    // ���� ��ü ������, ���� ��ü�� ������ ���� ����
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    [SerializeField]
    private Rifle[] guns;

    private Dictionary<string, Rifle> gunDictionary = new Dictionary<string, Rifle>();

    // �ʿ��� ������Ʈ
    [SerializeField]
    private HandController theGunController;

    [SerializeField] private PhotonView PV;

    private void Awake()
    {
        currentWeaponType = "GUN";
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isChangeWeapon)
        {
            if (!PV.IsMine) return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                isSniper = false;
                PV.RPC("RPCWeaponChange", RpcTarget.All,"GUN","Rifle1");
                //StartCoroutine(ChangeWeaponCoroutine("GUN", "Rifle1"));// ���� ����
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                isSniper = true;
                PV.RPC("RPCWeaponChange", RpcTarget.All,"GUN","Rifle2");
            }
        }
    }

    public void ChangeWeaponRPC(string _type, string _name)
    {
        StartCoroutine(ChangeWeaponCoroutine(_type, _name));
    }

    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction();
        WeaponChange(_type, _name);

        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        currentWeaponType = _type;
        isChangeWeapon = false; 
    }

    private void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                theGunController.CancelFineSight();
                break;
        }
    }

    private void WeaponChange(string _type, string _name)
    {
        if(_type == "GUN")
        {
            theGunController.GunChange(gunDictionary[_name]);
        }
    }
}
