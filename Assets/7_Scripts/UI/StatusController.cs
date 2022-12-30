using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // 체력
    [SerializeField]
    private int hp;
    private int currentHp;

    // 스태미나
    [SerializeField]
    private int sp;
    private int currentSp;

    // 스태미나 증가량
    [SerializeField]
    private int spIncreaseSpeed;

    // 스태미나 재회복 딜레이
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // 스태미나 감소 여부
    private bool spUsed;

    [SerializeField]
    private int dp;
    private int currentDp;

    [SerializeField]
    private Image[] image_Gauge;

    private const int HP = 0, DP = 1, SP = 2;

    // Start is called before the first frame update
    void Start()
    {
        currentDp = dp;
        currentHp = hp;
        currentSp = sp;
    }

    // Update is called once per frame
    void Update()
    {
        SPRechargeTime();
        SPRecover();
        GaugeUpdate();
    }

    // 재충전 시간 함수
    private void SPRechargeTime()
    {
        if (spUsed)
        {
            if (currentSpRechargeTime < spRechargeTime)
                currentSpRechargeTime++;
            else
                spUsed = false;
        }
    }

    // SP 재충전
    private void SPRecover()
    {
        if(!spUsed && currentSp < sp)
        {
            currentSp += spIncreaseSpeed;
        }
    }

    private void GaugeUpdate()
    {
        image_Gauge[HP].fillAmount = (float)currentHp / hp;
        image_Gauge[SP].fillAmount = (float)currentSp / sp;
        image_Gauge[DP].fillAmount = (float)currentDp / dp;
    }

    public void DecreaseStamina(int _count)
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (currentSp - _count > 0)
            currentSp -= _count;
        else
            currentSp = 0;
    }

    public void DecreaseHP(int _count)
    {
        if (currentHp - _count > 0)
            currentHp -= _count;
        else
            currentHp = 0;
    }

    public int GetHP()
    {
        return currentHp;
    }

    public int GetSP()
    {
        return currentSp;
    }
}
