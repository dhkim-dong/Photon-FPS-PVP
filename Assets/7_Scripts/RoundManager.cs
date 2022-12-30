using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviourPunCallbacks
{
    public enum roundPhase{Begin,Battle, End};  // round는 시작, 전투, 종료로 구분된다.

    private roundPhase cur_Phase; // 현재 Phase를 담는 변수

    public static RoundManager instance;

    [Header("게임 관리용 Bool")]
    public bool isBegin;
    public bool isStart;

    [Header("라운드 점수")]
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

    // 시작 후 N초 후 Battle Phase로 돌입하는 메서드 만들 기

    public void BeginPhase()
    {
        // 5.. 4.. 3.. 2.. 1.. 출력 후 게임 시작
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

    // 전투 중 Player가 사망 시 End Phase로 돌입한 후 점수를 

    public void EndPhase()
    {
        
    }
}
