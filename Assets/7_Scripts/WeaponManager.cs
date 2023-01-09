using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    // 무기 중복 교체 실행 방지
    public bool isChangeWeapon = false;
    public bool isSniper = false;

    // 현재 무기에 필요한 정보
    public Transform currentWeapon;
    public Animator currentWeaponAnim;

    // 현재 무기의 타입
    [SerializeField] private string currentWeaponType;

    // 무기 교체 딜레이, 무기 교체가 완전히 끝난 시점
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    [SerializeField]
    private Rifle[] guns;

    private Dictionary<string, Rifle> gunDictionary = new Dictionary<string, Rifle>();

    // 필요한 컴포넌트
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
                //StartCoroutine(ChangeWeaponCoroutine("GUN", "Rifle1"));// 돌격 소총
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
