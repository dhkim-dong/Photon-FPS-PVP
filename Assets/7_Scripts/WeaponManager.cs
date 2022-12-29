using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    // 무기 중복 교체 실행 방지
    public static bool isChangeWeapon = false;

    public static int count;

    // 현재 무기에 필요한 정보
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

    [SerializeField]
    private Text count_2;

    // 현재 무기의 타입
    [SerializeField]
    private string currentWeaponType;

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


    private void Awake()
    {
        
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
        count_2.text = count.ToString();

        if (!isChangeWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                ;// 무기 교체 실행
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                ;// 무기 교체 실행
        }
    }

    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction();
    }

    private void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                theGunController.CancelFineSight();
                break;
            case "Sniper":
                break;
        }
    }
}
