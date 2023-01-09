using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // 체력
    [SerializeField] private int hp; // 플레이어의 최대 체력
    private int currentHp;           // 현재 체력

    // 스태미나
    [SerializeField] private int sp; // 플레이어의 최대 스테미나
    private int currentSp;           // 현재 스테미나

    // 스태미나 증가량
    [SerializeField] private int spIncreaseSpeed; // SP 회복 속도

    // 스태미나 재회복 딜레이
    [SerializeField] private int spRechargeTime; // SP 회복 까지 걸리는 시간
    private int currentSpRechargeTime;           // SP 회복을 체크할 시간 변수

    // 스태미나 감소 여부
    private bool spUsed;

    [SerializeField]
    private Image[] image_Gauge;

    private const int HP = 0, DP = 1, SP = 2;

    void Start()
    {
        currentHp = hp;
        currentSp = sp;
    }

    void Update()
    {
        SPRechargeTime();
        SPRecover();
        GaugeUpdate();
    }

    // 재충전 시간 함수
    private void SPRechargeTime()
    {
        if (spUsed) // 스테미나를 사용했을 때 재충전 시간이 지나야 회복 가능
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
        if(!spUsed && currentSp < sp)     // spUsed 불값으로 회복 관리 + 최대 SP보다 낮을 때 
        {
            currentSp += spIncreaseSpeed; // SP 회복 속도만큼 현재 SP를 회복
        }
    }

    private void GaugeUpdate()
    {
        image_Gauge[HP].fillAmount = (float)currentHp / hp;
        image_Gauge[SP].fillAmount = (float)currentSp / sp;
    }

    public void DecreaseStamina(int _count) // 외부에서 Player SP를 소모하는ㄴ 메서드
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (currentSp - _count > 0)
            currentSp -= _count;
        else
            currentSp = 0;
    }

    public void DecreaseHP(int _count) // 외부에서 Player HP를 낮추는 메서드
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
