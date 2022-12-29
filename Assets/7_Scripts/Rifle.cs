using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    public string gunName; // ���� �̸�
    public float range; // ���� �����Ÿ�
    public float accuray; // ���� ��Ȯ��
    public float fireRate; // ���� ����ӵ�;
    public float reloadTime; // ������ �ӵ�;

    public int damage; // ���� ������

    public int reloadBulletCount; // �Ѿ� ������ ����
    public int currentBulletCount; // ���� ź������ �����ִ� �Ѿ��� ����
    public int maxBulletCount; // �ִ� ���� ���� �Ѿ� ����
    public int carryBulletCount; // ���� ���� �ϰ� �ִ� �Ѿ� ����

    public float retroActionForce; // �ݵ� ����
    public float retroActionFineSightForce; // ������ �� �ݵ� ����

    public Vector3 fineSightOriginPos;

    public Animator anim;

    public ParticleSystem muzzleFlash; // �ѱ� ����
}
