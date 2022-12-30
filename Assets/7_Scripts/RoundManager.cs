using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviourPunCallbacks
{
    public enum roundPhase{Begin,Battle, End};  // round�� ����, ����, ����� ���еȴ�.

    private roundPhase cur_Phase; // ���� Phase�� ��� ����

    public static RoundManager instance;

    [Header("���� ������ Bool")]
    public bool isBegin;
    public bool isStart;

    [Header("���� ����")]
    [SerializeField] TextMeshProUGUI redScore;
    [SerializeField] TextMeshProUGUI blueScore;

    public int redPoint;
    public int bluePoint;

    [Header("Battle Phase")]
    [SerializeField] TextMeshProUGUI startText;

    private float battleWaitTime;
    private float timeCount;

    void Start()
    {
        instance = this;
        cur_Phase = roundPhase.Begin;
    }



    private void Update()
    {
        redScore.text = redPoint.ToString();
        blueScore.text = bluePoint.ToString();

        if (isStart)
        {
            isStart = false;
            BeginPhase();
        }
    }

    // ���� �� N�� �� Battle Phase�� �����ϴ� �޼��� ���� ��

    public void BeginPhase()
    {
        // 5.. 4.. 3.. 2.. 1.. ��� �� ���� ����
        StartCoroutine(BeginText());
    }

    IEnumerator BeginText()
    {
        startText.text = "5";
        yield return new WaitForSeconds(1f);
        startText.text = "4";
        yield return new WaitForSeconds(1f);
        startText.text = "3";
        yield return new WaitForSeconds(1f);
        startText.text = "2";
        yield return new WaitForSeconds(1f);
        startText.text = "1";
        yield return new WaitForSeconds(1f);
        startText.text = "START!!";
        yield return new WaitForSeconds(1f);
        cur_Phase = roundPhase.Battle;
        startText.enabled = false;
        isBegin = false;
    }

    // ���� �� Player�� ��� �� End Phase�� ������ �� ������ 

    public void EndPhase()
    {
        
    }
}
