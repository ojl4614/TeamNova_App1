using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  
using Cinemachine;
public class Player_multi : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D RB;
    public Animator AN; 
    public SpriteRenderer SR;
    public PhotonView PV;
    public Image HealthImage;
    public Text NickNameText;
    bool isGround;
    public float axis;
    public GameObject HPOBJ;
    Vector3 curPOS;
    private void Awake()
    {
        HPOBJ = GameObject.Find("hp");
        HealthImage = HPOBJ.GetComponent<Image>();
        //닉네임 설정 본인 그린 다른사람 빨강
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
        if (PV.IsMine)
        {
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
        }
    }
    
    private void Update()
    {
        trun();
        move();
        jump();
    }
    void FixedUpdate()
    {
        jumpcheck();
    }


    public void hit()
    {
        HealthImage.fillAmount -= 0.1f;
    }
    void jump()
    {
        if (PV.IsMine&&Input.GetButtonDown("Jump") && !AN.GetBool("isJumping"))// 스테미나 조건걸기
        {
            // nowSt=(float) (nowSt-10); // 스테미나 조건걸기
            RB.AddForce(Vector2.up * 10, ForceMode2D.Impulse);  
            AN.SetBool("isJumping", true);
        }
    }
    void jumpcheck()
    {
        //Landing Platform
        if (PV.IsMine&&RB.velocity.y < 0)
        {
            // Debug.DrawRay(RB.position, Vector3.down, new Color(0, 1, 0));
            //Landing Paltform
            Debug.DrawRay(RB.position, Vector3.down, new Color(0, 1, 0)); //빔을 쏨(디버그는 게임상에서보이지 않음 ) 시작위치, 어디로 쏠지, 빔의 색

            RaycastHit2D rayHit = Physics2D.Raycast(RB.position, Vector3.down, (float) 2.0, LayerMask.GetMask("Platform"));
            //빔의 시작위치, 빔의 방향 , 1:distance , ( 빔에 맞은 오브젝트를 특정 레이어로 한정 지어야할 때 사용 ) // RaycastHit2D : Ray에 닿은 오브젝트 클래스

            //rayHit는 여러개 맞더라도 처음 맞은 오브젝트의 정보만을 저장(?)

            //점프판정
            if (RB.velocity.y < 0)
            { // 뛰어올랐다가 아래로 떨어질 때만 빔을 쏨
                if (rayHit.collider != null)
                { //빔을 맞은 오브젝트가 있을때  -> 맞지않으면 collider도 생성되지않음
                    if (rayHit.distance < 1.3f)
                        AN.SetBool("isJumping", false); //거리가 0.5보다 작아지면 변경
                }
            }
        }
    }
    void move()
    {
        if (PV.IsMine)
        {
            float axis = Input.GetAxisRaw("Horizontal");
            RB.velocity = new Vector2(4 * axis, RB.velocity.y);
            if (axis != 0)
            {
                AN.SetBool("isWalking", true);
            }
            else
            {
                AN.SetBool("isWalking", false);
            }
        }
    }
    void trun()
    {
        if (PV.IsMine)
        {
            //먼저 계산을 위해 마우스와 게임 오브젝트의 현재의 좌표를 임시로 저장합니다.
            Vector3 mPosition = Input.mousePosition; //마우스 좌표 저장
            Vector3 oPosition = transform.position; //게임 오브젝트 좌표 저장
            //카메라가 앞면에서 뒤로 보고 있기 때문에, 마우스 position의 z축 정보에
            //게임 오브젝트와 카메라와의 z축의 차이를 입력시켜줘야 합니다.
            mPosition.z = oPosition.z - Camera.main.transform.position.z;
            //화면의 픽셀별로 변화되는 마우스의 좌표를 유니티의 좌표로 변화해 줘야 합니다.
            //그래야, 위치를 찾아갈 수 있겠습니다.
            Vector3 target = Camera.main.ScreenToWorldPoint(mPosition);
            //다음은 아크탄젠트(arctan, 역탄젠트)로 게임 오브젝트의 좌표와 마우스 포인트의 좌표를
            //이용하여 각도를 구한 후, 오일러(Euler)회전 함수를 사용하여 게임 오브젝트를 회전시키기
            //위해, 각 축의 거리차를 구한 후 오일러 회전함수에 적용시킵니다.
            //우선 각 축의 거리를 계산하여, dy, dx에 저장해 둡니다.
            float dy = target.y - oPosition.y;
            float dx = target.x - oPosition.x;
            //오릴러 회전 함수를 0에서 180 또는 0에서 -180의 각도를 입력 받는데 반하여
            //(물론 270과 같은 값의 입력도 전혀 문제없습니다.) 아크탄젠트 Atan2()함수의 결과 값은
            //라디안 값(180도가 파이(3.141592654...)로)으로 출력되므로
            //라디안 값을 각도로 변화하기 위해 Rad2Deg를 곱해주어야 각도가 됩니다.

            float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
            // Debug.Log(rotateDegree);
            //구해진 각도를 오일러 회전 함수에 적용하여 z축을 기준으로 게임 오브젝트를 회전시킵니다.
            // transform.rotation = Quaternion.Euler (0f, 0f, rotateDegree);
            if (rotateDegree >= 0 && rotateDegree <= 90 || rotateDegree <= 0 && rotateDegree >= -90)
            {
                axis = -1;
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered, axis); //재접속시 fipx를 동기화주기 위해서 allbuffered
            }
            else
            {
                axis = 1;
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered, axis); //재접속시 fipx를 동기화주기 위해서 allbuffered
            }
        }
    }
    
    [PunRPC]
    void FlipXRPC(float axis) => SR.flipX = axis == 1;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
    
    
    
    
}
