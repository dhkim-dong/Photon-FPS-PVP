using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.UI;

// 네트워크 플레이어간의 데이터 동기화 처리를 하고싶다.
public class PlayerMove : MonoBehaviourPun, IPunObservable
{
    public static PlayerMove instance;

    // 필요속성
    public float speed = 5;                      // 캐릭터 이동속도
    public float jumpPower = 10;                 // 캐릭터 점프힘
    public float gravity = -20;                  // 캐릭터 중력
    public float rotSpeed = 100;                 // 캐릭터 회전 속도
    float yVelocity = 0;                         // 캐릭터의 중력 속도를 받을 변수
    float camAngle;                              // 마우스에 따른 상하 회전 값을 받을 변수
    float bodyAngle;                             // 캐릭터의 좌우 회전 값을 변수
     
    CharacterController cc;                      // 캐릭터 이동에 사용할 속성
    Animator anim;                               // 애니메이션에 사용할 속성

    public Transform camera;                     // 카메라의 위치 값

    [SerializeField] private Camera cam;         // 플레이어 내부 카메라 

    // 네트워크 데이터 기억 변수
    Vector3 remotePos;
    Quaternion remoteRot;
    Quaternion remoteCamRot;

    [SerializeField] private HandController theGunController;

    [SerializeField] private GameObject appear;

    // 데이터 관리
    public float jumpTime;

    // 상태변수
    public bool isMove = false;
    public bool isJump = false;

    // 움직임 체크 변수

    private Vector3 lastPos;

    [SerializeField] private CrossHair theCrosshair;

    [SerializeField] private StatusController theStatusController;

    PhotonView PV;

    // 피격효과 구현
    private MeshRenderer mesh;

    [SerializeField] private GameObject hitUI;

    // 점수 체크용 Bool
    private bool blueWin;
    private bool redWin;

    [SerializeField] private GameObject Holder;

    void Start()
    {
        instance = this;

        cc = GetComponent<CharacterController>();

        anim = GetComponentInChildren<Animator>();

        PV = GetComponent<PhotonView>();

        mesh = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (RoundManager.instance.isBegin) return;

        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, remotePos, 10 * Time.deltaTime);
            transform.rotation= Quaternion.Lerp(transform.rotation, remoteRot, 10 * Time.deltaTime);
            camera.rotation= Quaternion.Lerp(camera.rotation, remoteCamRot, 10 * Time.deltaTime);
            return;
        }

        RotateCamera();
        RotateBody();
        MoveCheck();
        Move();

        if (cc.isGrounded)
        {
            isJump = false;
            theCrosshair.JumpAnimation(isJump);
        }
    }

    private void MoveCheck()
    {
        if (isJump)
            return;

        if (Vector3.Distance(lastPos, transform.position) >= 0.01f)
        {
            isMove = true;
        }
        else
        isMove = false;

        theCrosshair.WalkingAnimation(isMove);
        lastPos = transform.position;    
    }

    // 카메라 상하 이동
    void RotateCamera()
    {
        
        float value = Input.GetAxis("Mouse Y");
        camAngle += value * rotSpeed * Time.deltaTime;
        camAngle = Mathf.Clamp(camAngle, -60, 60);

        camera.localEulerAngles = new Vector3(-camAngle, 0, 0);
        Holder.transform.localEulerAngles = new Vector3(-camAngle, 0, 0);
    }

    // 플레이어 전체 회전
    void RotateBody()
    {
        float value = Input.GetAxis("Mouse X");
        bodyAngle += value * rotSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(0, bodyAngle, 0);
    }

    void Move()
    {
        // 사용자 입력
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        // 방향을 지정
        Vector3 dir = new Vector3(h, 0, v) * speed;
        // 카메라가 바라보는 방향으로 방향 전환
        dir = camera.TransformDirection(dir);

        // 바닥에 있으면 수직 속도를 0으로 하자(수직항력)
        if (cc.isGrounded)
        {
            yVelocity = 0;
        }

        //점프
        if (Input.GetButtonDown("Jump") && !isJump && theStatusController.GetSP() >= 200)
        {
            theStatusController.DecreaseStamina(200);
            isJump = true;
            theCrosshair.JumpAnimation(isJump);
            theGunController.CancelFineSight();
            yVelocity = jumpPower;
        }

        // 중력을 적용하고 싶다(CC에서)  
        // 중력을 적용하는 시점이 중요하다!(서순)
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;
        //이동한다.
       cc.Move(dir * Time.deltaTime);

    }

    // 데이터를 네트워크 사용자 간에 보내고 받는 콜백 함수
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 보내는 중
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(camera.rotation);
        }
        // 받는 중
        else
        {
            remotePos = (Vector3)stream.ReceiveNext();
            remoteRot = (Quaternion)stream.ReceiveNext();
            remoteCamRot = (Quaternion)stream.ReceiveNext();
        }

    }
 

    [PunRPC]
    public void PVPCallDamage()
    {
        if(theStatusController.GetHP() > 0)
        {
            StopCoroutine(HitMeshChange());
            StartCoroutine(HitMeshChange());
            theStatusController.DecreaseHP(5); // 무기 공격력 만큼 데미지 감소
        }
        else
        {
            PV.RPC("PlayerDebrisOut", RpcTarget.MasterClient);         
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            // 죽음
            if (PV.IsMine)
            {
                RoundManager.instance.redPoint++;
                Spawn();
            }
            else
            {
                RoundManager.instance.bluePoint++;
            }         
        }
    }

    [PunRPC]
    void PlayerDebrisOut()
    {
         var Debris = PhotonNetwork.Instantiate("P_Debris", transform.position, Quaternion.identity);
    }

    IEnumerator HitMeshChange()
    {
        mesh.material.color = Color.red;
        hitUI.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        mesh.material.color = Color.white;
        hitUI.SetActive(false);
    }

    [PunRPC]
    void DestroyRPC()
    {
        Destroy(gameObject);
    }

    private void Spawn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Player", NetworkManager.instance.createPos[0].position, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Player", NetworkManager.instance.createPos[1].position, Quaternion.identity);
        }
    }
}
