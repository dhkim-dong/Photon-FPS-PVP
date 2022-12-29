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


    // ���� �ð��� ������ ���¸� Move�� ��ȯ�Ѵ�.
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

    // Ÿ�� �������� �̵��ϰ� �ʹ�.
    public float speed = 5;
    public float turnSpeed = 5;
    public GameObject target;

    // ���� ���� �ȿ� ������ ���¸� attack����
    // �ʿ�Ӽ� : ���ݹ���
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
        float distance = dir.sqrMagnitude; // ��ǻ�Ͱ� �� �� ������ ���� �� ������ ������ �� �� ���� ����ؾ��Ѵ�.

        if(distance < attackRange)
        {
            m_state = EnemyState.Attack;
            currentTime = attackDelayTime;
            // ��ã�� ����
            agent.enabled = false;
            return;
        }

        // Agent�� �̿��� ��ã��
        agent.destination = target.transform.position;

        //dir.y = 0;
        //dir.Normalize();
        
        //cc.SimpleMove(dir * speed);
        //transform.LookAt(target.transform);
        //transform.forward = Vector3.Lerp(transform.forward,dir, turnSpeed * Time.deltaTime);
        // ������ : �ѹ��� ȸ���ϴ� ���� ����ϴ�. (�ε巴�� �ʴ�) 
        // �ε巯�� ȸ���� �����ϰ� �ʹ�.

        // �ڵ��󿡼��� �ִϸ����� ó�� ��� : tween �̶�� �Ѵ�. itween �÷����� ����� �����ϴ� �͵� (�Ÿ�, ���� ��ȭ ���)
        // ����� �ϴ� �̻��� ������ �߻��Ѵ�.
        // ������ ������ ����� �ؼ� ���� �������� ����� �۾��� �Ѵ�.
        // ������ : �����̴� ������ ���� ȸ���ϴ� ������ �޶�����.( �� ȸ���� ���ؼ� ������ �ִ�.)
        // ������ ���� �� �ִ�.
        // �ذ� ��� : ������ ���� �ϳ� �����ؼ� �װ��� �������� ������.
        // �� ����� Quaternion ����̴�.
        //transform.rotation = Quaternion.Lerp(transform.rotation,
        //    Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
        // �� �ٸ� ������ : ���� y�� ������� enemy�� ���� ������ �߻��Ѵ�.
        // ȸ�� ���Ͱ��� ������ �߻��Ѵ�. ��� �ذ��ϳ�? ������ �þ߰����� ������ �ִ� ���� Y�̴�. Y���� 0�̸� �ذ�ȴ�.
        // �Ÿ� ���
    }

    // Ÿ���� ���� ������ ����� ���¸� Move�� ��ȯ�Ѵ�.
    // ���� �ð��� �ѹ��� �����ϰ� �ʹ�.
    // �ʿ� �Ӽ� : ���� ��� �ð�, 
    // ������ �ٽ� ���� �ʴ� ������ �߻�.
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

    // ���� �ð� ���ϸ� ���¸� ��ȯ
    public float damageDelayTime = 2;
    private IEnumerator Damage()
    {
        // 1. ���¸� �������� ��ȯ
        m_state = EnemyState.Damage;
        // 2. �ִϸ��̼� ����
        anim.SetTrigger("Damage");
        // 3. ���� ���� ���
        yield return new WaitForSeconds(damageDelayTime);
        // 4. ���¸� ���� ��ȯ
        m_state = EnemyState.Idle;       
    }

    // �� �¾Ѿ�
    // �¾��� �� ȣ��Ǵ� �Լ�

    public int e_hp = 5;

    [PunRPC]
    public void OnDamageProcess() // �̺�Ʈ���� �Լ� = On�ӽñ�ӽñ�
    {
        if (m_state == EnemyState.Die) return;

        e_hp--;
        currentTime = 0;
        agent.enabled = false;
        StopAllCoroutines();   // �ش� ��ũ��Ʈ���� ����ϰ� �ִ� �ڷ�ƾ�� ���ִ� �޼���;

        if(e_hp <= 0)
        {
            StartCoroutine(Die());          
        }
        else
        {
            // �ڷ�ƾ ����
            StartCoroutine(Damage());
        }
    }

    private IEnumerator Die()
    {
        m_state = EnemyState.Die;
        anim.SetTrigger("Die");
        // ������ �浹ü�� ����.
        cc.enabled = false;

        yield return new WaitForSeconds(5);

        Destroy(gameObject);
    }

    

    // Visual Debugging �� ���� �Լ�
    // �ð��� ǥ���� �� �� ��ȹ�ڰ� �ľ��ϱ� �����ϴ�.
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
