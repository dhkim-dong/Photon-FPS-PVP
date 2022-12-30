using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HandController : MonoBehaviourPun
{
    // 현재 장착된 총
    [SerializeField]
    private Rifle currentRifle;

    [SerializeField]
    private PhotonView PV;

    // 연사 속도 계산
    private float currentFireRate;


    // 상태 변수
    private bool isReload = false;
    [HideInInspector]
    public bool isFineSightMode = false;

    // 본래 포지션 값.
    private Vector3 originPos;


    // 히트 파악을 위한 변수

    private RaycastHit hitInfo;

    // 카메라 기반 총알 발사
    [SerializeField]
    private Camera theCam;

    [SerializeField]
    private CrossHair theCrossHair;

    // 피격 이팩트
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



    // 연산 속도 재계산
    private void GunFireRateCacl()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; // 
    }

    // 발사 시도
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

    // 재장전
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
            // 총알 없음 소리를 넣어준다.
            Debug.Log("소유한 총알이 없습니다.");
        }
    }

    public void ReloadForPun()
    {
        StartCoroutine(Reload());
    }

    // 재장전 시도
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentRifle.currentBulletCount < currentRifle.reloadBulletCount)
        {
            CancelFineSight();
            PV.RPC("PunReload", RpcTarget.All);
        }
    }

    // 발사 후 계산
    public void Shoot()
    {
        theCrossHair.FireAnimation();
        currentRifle.currentBulletCount--;
        currentFireRate = currentRifle.fireRate; // 연사 속도 재계산
        PV.RPC("PunMuzzleFlash", RpcTarget.All);
        Hit();
        AudioManager.instance.GunFire();

        // 총기 반동
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
    }

    public void MuzzleFlash()
    {
        currentRifle.muzzleFlash.Play();
    }

    // 히트 스캔 방법의 총알
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

    // 정조준 시도
    private void TryFineSight()
    {
        if (isReload)
            return;

        if (Input.GetButtonDown("Fire2"))
        {
            FineSight();
        }
    }

    // 정조준 취소
    public void CancelFineSight()
    {
        if (isFineSightMode)
            FineSight();
    }

    // 정조준 로직
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

    // 정조준 활성화
    IEnumerator FineSightActivateCoroutine()
    {
        while(currentRifle.transform.localPosition != currentRifle.fineSightOriginPos)
        {
            currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, currentRifle.fineSightOriginPos, 0.2f);

            yield return null;
        }
    }

    // 정조준 비활성화
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

            // 반동 시작
            while(currentRifle.transform.localPosition.x <= currentRifle.retroActionForce-0.02f)
            {
                currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            // 원 위치
            while(currentRifle.transform.localPosition != originPos)
            {
                currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else
        {
            currentRifle.transform.localPosition = currentRifle.fineSightOriginPos;

            // 반동 시작
            while (currentRifle.transform.localPosition.x <= currentRifle.retroActionFineSightForce - 0.02f)
            {
                currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            // 원 위치
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
