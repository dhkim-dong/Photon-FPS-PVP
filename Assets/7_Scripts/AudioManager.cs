using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource audio;
    // ȿ������ ���ϴ� ��ġ���� ����ϰ� �ʹ�.
    // �ʿ�Ӽ� : ����� Ŭ��
    public AudioClip gun;
    public AudioClip zombie;
    public AudioClip reload;

    // Start is called before the first frame update
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
}
