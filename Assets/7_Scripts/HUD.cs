using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    // �ʿ� ������Ʈ
    [SerializeField]
    private HandController theGun;
    private Rifle currentRifle;

    // �ʿ��ϸ� HUD Ȱ��ȭ, ��Ȱ��ȭ
    [SerializeField]
    private GameObject go_BulletHUD;

    // �Ѿ� ���� �ؽ�Ʈ�� �ݿ�
    [SerializeField]
    private Text[] text_Bullet;


    //private void Start()
    //{
    //    theGun = GameObject.FindGameObjectWithTag("Player").transform.GetComponentInChildren<HandController>();
    //}

    // Update is called once per frame
    void Update()
    {
        CheckBullet();
    }

    private void CheckBullet()
    {
        currentRifle = theGun.GetGun();
        text_Bullet[0].text = currentRifle.carryBulletCount.ToString();
        text_Bullet[1].text = currentRifle.reloadBulletCount.ToString();
        text_Bullet[2].text = currentRifle.currentBulletCount.ToString();

    }
}
