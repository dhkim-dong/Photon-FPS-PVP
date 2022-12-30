using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HandController : MonoBehaviourPun
{
    // ���� ������ ��
    [SerializeField]
    private Rifle currentRifle;

    [SerializeField]
    private PhotonView PV;

    // ���� �ӵ� ���
    private float currentFireRate;


    // ���� ����
    private bool isReload = false;
    [HideInInspector]
    public bool isFineSightMode = false;

    // ���� ������ ��.
    private Vector3 originPos;


    // ��Ʈ �ľ��� ���� ����

    private RaycastHit hitInfo;

    // ī�޶� ��� �Ѿ� �߻�
    [SerializeField]
    private Camera theCam;

    [SerializeField]
    private CrossHair theCrossHair;

    // �ǰ� ����Ʈ
    [SerializeField]
    private GameObject hit_effect_prefab;

    private void Start()
    {
        originPos = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (RoundManager.instance.isBegin) return;

        if (!PV.IsMine)
            return;


        GunFireRateCacl();
        TryFire();
        TryReload();
        TryFineSight();
    }



    // ���� �ӵ� ����
    private void GunFireRateCacl()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; // 
    }

    // �߻� �õ�
    private void TryFire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            Fire();
        }
    }
    
    public void Fire()
    {
        if (!isReload)
        {
            if (currentRifle.currentBulletCount > 0)
                Shoot();
            else
            {
                CancelFineSight();
                PV.RPC("PunReload", RpcTarget.All);
            }
        }
    }

    // ������
    IEnumerator Reload()
    {
        if(currentRifle.carryBulletCount > 0)
        {
            isReload = true;
            currentRifle.anim.SetTrigger("Reload");

            currentRifle.carryBulletCount += currentRifle.currentBulletCount;
            currentRifle.currentBulletCount = 0;

            yield return new WaitForSeconds(currentRifle.reloadTime);


            if (currentRifle.carryBulletCount >= currentRifle.reloadBulletCount)
            {
                currentRifle.currentBulletCount = currentRifle.reloadBulletCount;
                currentRifle.carryBulletCount -= currentRifle.reloadBulletCount;
            }
            else
            {
                currentRifle.currentBulletCount = currentRifle.carryBulletCount;
                currentRifle.carryBulletCount = 0;
            }


            isReload = false;
        }
        else
        {
            // �Ѿ� ���� �Ҹ��� �־��ش�.
            Debug.Log("������ �Ѿ��� �����ϴ�.");
        }
    }

    public void ReloadForPun()
    {
        StartCoroutine(Reload());
    }

    // ������ �õ�
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentRifle.currentBulletCount < currentRifle.reloadBulletCount)
        {
            CancelFineSight();
            PV.RPC("PunReload", RpcTarget.All);
        }
    }

    // �߻� �� ���
    public void Shoot()
    {
        theCrossHair.FireAnimation();
        currentRifle.currentBulletCount--;
        currentFireRate = currentRifle.fireRate; // ���� �ӵ� ����
        PV.RPC("PunMuzzleFlash", RpcTarget.All);
        Hit();
        AudioManager.instance.GunFire();

        // �ѱ� �ݵ�
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
    }

    public void MuzzleFlash()
    {
        currentRifle.muzzleFlash.Play();
    }

    // ��Ʈ ��ĵ ����� �Ѿ�
    private void Hit()
    {
        if(Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
            new Vector3(UnityEngine.Random.Range(-theCrossHair.GetAccuracy() - currentRifle.accuray, theCrossHair.GetAccuracy() + currentRifle.accuray),
                        UnityEngine.Random.Range(-theCrossHair.GetAccuracy() - currentRifle.accuray, theCrossHair.GetAccuracy() + currentRifle.accuray),
                         0)
            , out hitInfo, currentRifle.range))
        {
            PhotonView pv = hitInfo.transform.GetComponent<PhotonView>();

            var clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);
            if (hitInfo.transform.name.Contains("AI") && PV)
            {
                pv.RPC("OnDamageProcess", RpcTarget.All);
            }

            if(hitInfo.transform.name.Contains("Enemy") && PV)
            {
                pv.RPC("PVPCallDamage", RpcTarget.All);
            }
        }
    }

    // ������ �õ�
    private void TryFineSight()
    {
        if (isReload)
            return;

        if (Input.GetButtonDown("Fire2"))
        {
            FineSight();
        }
    }

    // ������ ���
    public void CancelFineSight()
    {
        if (isFineSightMode)
            FineSight();
    }

    // ������ ����
    private void FineSight()
    {
        isFineSightMode = !isFineSightMode;
        currentRifle.anim.SetBool("FineSightMode", isFineSightMode);
        theCrossHair.FineSightAnimation(isFineSightMode);

        if (isFineSightMode)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeActivateCoroutine());
        }
    }

    // ������ Ȱ��ȭ
    IEnumerator FineSightActivateCoroutine()
    {
        while(currentRifle.transform.localPosition != currentRifle.fineSightOriginPos)
        {
            currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, currentRifle.fineSightOriginPos, 0.2f);

            yield return null;
        }
    }

    // ������ ��Ȱ��ȭ
    IEnumerator FineSightDeActivateCoroutine()
    {
        while (currentRifle.transform.localPosition != originPos)
        {
            currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, originPos, 0.2f);

            yield return null;
        }
    }

    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(currentRifle.retroActionForce, originPos.y, originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(currentRifle.retroActionFineSightForce, currentRifle.fineSightOriginPos.y, currentRifle.fineSightOriginPos.z);

        if (!isFineSightMode)
        {
            currentRifle.transform.localPosition = originPos;

            // �ݵ� ����
            while(currentRifle.transform.localPosition.x <= currentRifle.retroActionForce-0.02f)
            {
                currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            // �� ��ġ
            while(currentRifle.transform.localPosition != originPos)
            {
                currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else
        {
            currentRifle.transform.localPosition = currentRifle.fineSightOriginPos;

            // �ݵ� ����
            while (currentRifle.transform.localPosition.x <= currentRifle.retroActionFineSightForce - 0.02f)
            {
                currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            // �� ��ġ
            while (currentRifle.transform.localPosition != currentRifle.fineSightOriginPos)
            {
                currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, currentRifle.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }

    public Rifle GetGun()
    {
        return currentRifle;
    }

    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

    Vector3 remotePos;
    Quaternion remoteRot;

}
