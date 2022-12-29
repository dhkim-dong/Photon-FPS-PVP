using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    // 필요 컴포넌트
    [SerializeField]
    private HandController theGun;
    private Rifle currentRifle;

    // 필요하면 HUD 활성화, 비활성화
    [SerializeField]
    private GameObject go_BulletHUD;

    // 총알 개수 텍스트에 반영
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
