using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // ü��
    [SerializeField]
    private int hp;
    private int currentHp;

    // ���¹̳�
    [SerializeField]
    private int sp;
    private int currentSp;

    // ���¹̳� ������
    [SerializeField]
    private int spIncreaseSpeed;

    // ���¹̳� ��ȸ�� ������
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // ���¹̳� ���� ����
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

    // ������ �ð� �Լ�
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

    // SP ������
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
