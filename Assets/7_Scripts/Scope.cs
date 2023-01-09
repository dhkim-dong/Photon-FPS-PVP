using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour
{
    // 내용 해석 및 주석 리펙토링 필요
    public GameObject scopeOverlay;
    public GameObject weaponCamera;
    public Camera mainCamera;

    [SerializeField] private float scopedFOV = 15f;
    private float normalFOV;

    private bool isScoped = false;

    private WeaponManager theWeaponManager;

    private void Awake()
    {
        theWeaponManager = GetComponent<WeaponManager>();
        normalFOV = 60f;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire2") && theWeaponManager.isSniper)
        {
            isScoped = !isScoped;

            if (isScoped)
                StartCoroutine(OnScoped());
            else
                OnUnscoped();
        }
    }

    public void OnUnscoped()
    {
        scopeOverlay.SetActive(false);
        weaponCamera.SetActive(true);

        mainCamera.fieldOfView = normalFOV;
    }

    private IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(0.15f);

        scopeOverlay.SetActive(true);
        weaponCamera.SetActive(false);

        mainCamera.fieldOfView = scopedFOV;
    }
}
