using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // ü��
    [SerializeField] private int hp; // �÷��̾��� �ִ� ü��
    private int currentHp;           // ���� ü��

    // ���¹̳�
    [SerializeField] private int sp; // �÷��̾��� �ִ� ���׹̳�
    private int currentSp;           // ���� ���׹̳�

    // ���¹̳� ������
    [SerializeField] private int spIncreaseSpeed; // SP ȸ�� �ӵ�

    // ���¹̳� ��ȸ�� ������
    [SerializeField] private int spRechargeTime; // SP ȸ�� ���� �ɸ��� �ð�
    private int currentSpRechargeTime;           // SP ȸ���� üũ�� �ð� ����

    // ���¹̳� ���� ����
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

    // ������ �ð� �Լ�
    private void SPRechargeTime()
    {
        if (spUsed) // ���׹̳��� ������� �� ������ �ð��� ������ ȸ�� ����
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
        if(!spUsed && currentSp < sp)     // spUsed �Ұ����� ȸ�� ���� + �ִ� SP���� ���� �� 
        {
            currentSp += spIncreaseSpeed; // SP ȸ�� �ӵ���ŭ ���� SP�� ȸ��
        }
    }

    private void GaugeUpdate()
    {
        image_Gauge[HP].fillAmount = (float)currentHp / hp;
        image_Gauge[SP].fillAmount = (float)currentSp / sp;
    }

    public void DecreaseStamina(int _count) // �ܺο��� Player SP�� �Ҹ��ϴ¤� �޼���
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (currentSp - _count > 0)
            currentSp -= _count;
        else
            currentSp = 0;
    }

    public void DecreaseHP(int _count) // �ܺο��� Player HP�� ���ߴ� �޼���
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
