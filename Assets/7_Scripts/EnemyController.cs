using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviourPun
{
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Damage,
        Die
    };

    EnemyState m_state;
    CharacterController cc;
    Animator anim;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        m_state = EnemyState.Idle;   
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            StopAllCoroutines();
            target = GameObject.FindGameObjectWithTag("Player");
        }

        switch (m_state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Damage:
                //Damage();
                break;
            case EnemyState.Die:
                //Die();
                break;
        }   
    }


    // 일정 시간이 지나면 상태를 Move로 전환한다.
    public float idleDelayTime = 2;
    private float currentTime = 0;
    private void Idle()
    {
        currentTime += Time.deltaTime;
        if(currentTime > idleDelayTime)
        {
            m_state = EnemyState.Move;
            currentTime = 0;
        }
    }

    // 타겟 방향으로 이동하고 싶다.
    public float speed = 5;
    public float turnSpeed = 5;
    public GameObject target;

    // 공격 범위 안에 들어오면 상태를 attack으로
    // 필요속성 : 공격범위
    public float attackRange = 2;

    float runSpeed = 0;
    private void Move()
    {
        if(agent.enabled == false)
        {
            agent.enabled = true;
        }
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        runSpeed = Mathf.Lerp(runSpeed, 1, 10 * Time.deltaTime);
        anim.SetFloat("Speed", runSpeed);
        Vector3 dir = target.transform.position - transform.position;
        float distance = dir.sqrMagnitude; // 컴퓨터가 볼 때 성능은 향상될 수 있지만 실제로 볼 때 값을 계산해야한다.

        if(distance < attackRange)
        {
            m_state = EnemyState.Attack;
            currentTime = attackDelayTime;
            // 길찾기 종료
            agent.enabled = false;
            return;
        }

        // Agent를 이용한 길찾기
        agent.destination = target.transform.position;

        //dir.y = 0;
        //dir.Normalize();
        
        //cc.SimpleMove(dir * speed);
        //transform.LookAt(target.transform);
        //transform.forward = Vector3.Lerp(transform.forward,dir, turnSpeed * Time.deltaTime);
        // 문제점 : 한번에 회전하는 것이 어색하다. (부드럽지 않다) 
        // 부드러운 회전을 구현하고 싶다.

        // 코딩상에서의 애니메이터 처리 기법 : tween 이라고 한다. itween 플러그인 무료로 제공하는 것들 (거리, 색상 변화 등등)
        // 백덤블링 하는 이상한 현상이 발생한다.
        // 백터의 외적을 계산을 해서 전부 직각으로 만드는 작업을 한다.
        // 문제점 : 움직이는 순서에 따라서 회전하는 방향이 달라진다.( 즉 회전에 대해서 약점이 있다.)
        // 짐벌락 등의 용어가 있다.
        // 해결 방법 : 임의의 축을 하나 생성해서 그것을 기준으로 돌린다.
        // 이 방식이 Quaternion 방식이다.
        //transform.rotation = Quaternion.Lerp(transform.rotation,
        //    Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
        // 또 다른 문제점 : 높은 y을 대상으로 enemy가 눕는 현상이 발생한다.
        // 회전 백터값의 문제가 발생한다. 어떻게 해결하나? 백터의 시야각도에 영향을 주는 값이 Y이다. Y값이 0이면 해결된다.
        // 거리 계산
    }

    // 타겟이 공격 범위를 벗어나면 상태를 Move로 전환한다.
    // 일정 시간에 한번씩 공격하고 싶다.
    // 필요 속성 : 공격 대기 시간, 
    // 공격을 다시 하지 않는 현상이 발생.
    public float attackDelayTime = 2;
    private void Attack()
    {
        currentTime += Time.deltaTime;
        if(currentTime >= attackDelayTime)
        {
            StartCoroutine(E_Attack());
        }

        Vector3 dir = target.transform.position - transform.position;
        float distance = dir.sqrMagnitude;
        if (distance > attackRange && !isAttack)
        {
            m_state = EnemyState.Move;
            anim.SetTrigger("Move");
            return;          
        }
    }

    private bool isAttack;
    private IEnumerator E_Attack()
    {
        AudioManager.instance.zomAtk();
        currentTime = 0;
        anim.SetTrigger("Attack");
        isAttack = true;
        yield return new WaitForSeconds(1);
        isAttack = false;
    }

    // 일정 시간 변하면 상태를 변환
    public float damageDelayTime = 2;
    private IEnumerator Damage()
    {
        // 1. 상태를 데미지로 전환
        m_state = EnemyState.Damage;
        // 2. 애니메이션 동작
        anim.SetTrigger("Damage");
        // 3. 다음 동작 대기
        yield return new WaitForSeconds(damageDelayTime);
        // 4. 상태를 대기로 전환
        m_state = EnemyState.Idle;       
    }

    // 너 맞앗어
    // 맞았을 때 호출되는 함수

    public int e_hp = 5;

    [PunRPC]
    public void OnDamageProcess() // 이벤트적인 함수 = On머시기머시기
    {
        if (m_state == EnemyState.Die) return;

        e_hp--;
        currentTime = 0;
        agent.enabled = false;
        StopAllCoroutines();   // 해당 스크립트에서 사용하고 있는 코루틴을 없애는 메서드;

        if(e_hp <= 0)
        {
            StartCoroutine(Die());          
        }
        else
        {
            // 코루틴 시작
            StartCoroutine(Damage());
        }
    }

    private IEnumerator Die()
    {
        m_state = EnemyState.Die;
        anim.SetTrigger("Die");
        // 죽으면 충돌체를 끈다.
        cc.enabled = false;

        yield return new WaitForSeconds(5);

        Destroy(gameObject);
    }

    

    // Visual Debugging 을 위한 함수
    // 시각적 표현을 할 때 기획자가 파악하기 수월하다.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void bite()
    {
        float height = anim.GetFloat("Height");
        cc.height = height;
    }
}
