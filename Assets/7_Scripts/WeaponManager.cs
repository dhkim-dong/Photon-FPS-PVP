using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    // ���� �ߺ� ��ü ���� ����
    public static bool isChangeWeapon = false;

    public static int count;

    // ���� ���⿡ �ʿ��� ����
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

    [SerializeField]
    private Text count_2;

    // ���� ������ Ÿ��
    [SerializeField]
    private string currentWeaponType;

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
                ;// ���� ��ü ����
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                ;// ���� ��ü ����
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
