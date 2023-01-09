using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    // ũ�ν� ��� ���¿� ���� ��Ȯ��
    private float gunAccuracy;

    [SerializeField]
    private GameObject go_CrosshairHUD;

    [SerializeField]
    private HandController theGun;

    [SerializeField] private GameObject my_cam;

    public void WalkingAnimation(bool _flag)
    {
        anim.SetBool("Move", _flag);
    }

    public void JumpAnimation(bool _flag)
    {
        anim.SetBool("Jump", _flag);
    }

    public void FineSightAnimation(bool _flag)
    {
        anim.SetBool("FineSight", _flag);
    }

    public void FireAnimation()
    {
        if (anim.GetBool("Move"))
        {
            anim.SetTrigger("move_Fire");
        }
        else
        {
            anim.SetTrigger("idle_Fire");
        }
    }

    public float GetAccuracy()
    {
        if (anim.GetBool("Move"))
        {
            anim.SetTrigger("move_Fire");
            gunAccuracy = 0.06f;
        }
        else if(theGun.GetFineSightMode())
        {
            gunAccuracy = 0.001f;
        }
        else
        {
            anim.SetTrigger("idle_Fire");
            gunAccuracy = 0.015f;
        }

        return gunAccuracy;
    }

    public void CrossModeOff()
    {
        go_CrosshairHUD.SetActive(false);
    }

    public void CrossModeOn()
    {
        go_CrosshairHUD.SetActive(true);
    }
}
