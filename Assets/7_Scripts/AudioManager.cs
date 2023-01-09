using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource audio;
    // 효과음을 원하는 위치에서 사용하고 싶다.
    // 필요속성 : 오디오 클립
    public AudioClip gun;
    public AudioClip zombie;
    public AudioClip reload;

    void Start()
    {
        instance = this;
        audio = GetComponent<AudioSource>();
    }

    public void GunFire()
    {
        audio.PlayOneShot(gun);
    }

    public void zomAtk()
    {
        audio.PlayOneShot(zombie);
    }

    public void GunReload()
    {
        audio.PlayOneShot(reload);
    }
}
