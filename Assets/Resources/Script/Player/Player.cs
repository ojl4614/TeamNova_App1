using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float maxSpeed; //최대 속력 변수
    public float jumpPower; // 점프
    public int maxHp = 100;
    public int nowHp = 100;
    public float maxSt = 100;
    public float nowSt = 100;
    public Image nowHpbar;
    public Image nowStbar;

    private Rigidbody2D rigid; //물리이동을 위한 변수 선언
    private SpriteRenderer spriteRenderer; //방향전환을 위한 변수
    private Animator animator; //애니메이터 조작을 위한 변수

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>(); //변수 초기화
        spriteRenderer = GetComponent<SpriteRenderer>(); // 초기화
        animator = GetComponent<Animator>();
    }
    void OnDamaged(Vector2 targetPos)
    {
        //player  damaged

        //뭐에 맞았는지에 따른 데미지 구별
        nowHp=nowHp-5;
        
        // spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //reaction forc
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*2.5f, ForceMode2D.Impulse);

        // Invoke("OffDamaged", 2);
    }

    void OffDamaged()
    {
        // 피격시 무적?
        // spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //적에게 닿았을때
        if(collision.gameObject.tag == "zombie")
            OnDamaged(collision.transform.position);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "zombie")
            OnDamaged(collision.transform.position);
    }

    void Update()
    {
        nowHpbar.fillAmount = (float) nowHp / (float) maxHp;
        nowStbar.fillAmount = (float) nowSt / (float) maxSt;
        if (nowSt > 100)
        {
            nowSt = 100;
        }
        trun();
        move();
    }
    
    
    private void FixedUpdate()
    {
        maxspeed();
        jump();
    }

    void jump()
    {
        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            //Landing Paltform
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); //빔을 쏨(디버그는 게임상에서보이지 않음 ) 시작위치, 어디로 쏠지, 빔의 색

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            //빔의 시작위치, 빔의 방향 , 1:distance , ( 빔에 맞은 오브젝트를 특정 레이어로 한정 지어야할 때 사용 ) // RaycastHit2D : Ray에 닿은 오브젝트 클래스

            //rayHit는 여러개 맞더라도 처음 맞은 오브젝트의 정보만을 저장(?)

            //점프판정
            if (rigid.velocity.y < 0)
            { // 뛰어올랐다가 아래로 떨어질 때만 빔을 쏨
                if (rayHit.collider != null)
                { //빔을 맞은 오브젝트가 있을때  -> 맞지않으면 collider도 생성되지않음
                    if (rayHit.distance < 1.5f)
                        animator.SetBool("isJumping", false); //거리가 0.5보다 작아지면 변경
                }
            }
        }
    }
    void maxspeed()
    {
        // movespeed
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); //빔을 쏨(디버그는 게임상에서보이지 않음 ) 시작위치, 어디로 쏠지, 빔의 색
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (Input.GetAxis("Horizontal") != 0 && Input.GetKey(KeyCode.LeftShift) && rayHit.collider != null && nowSt >= 0.3)
        {
            nowSt=(float) (nowSt-0.3);
            float h = Input.GetAxisRaw("Horizontal");
            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
            //max speed
            if (rigid.velocity.x > maxSpeed)  //오른쪽으로 이동 (+) , 최대 속력을 넘으면
                rigid.velocity = new Vector2(maxSpeed* 2, rigid.velocity.y ); //해당 오브젝트의 속력은 maxSpeed
            else if (rigid.velocity.x < maxSpeed * (-1)) // 왼쪽으로 이동 (-)
                rigid.velocity = new Vector2(maxSpeed* 2 * (-1), rigid.velocity.y ); //y값은 점프의 영향이므로 0으로 제한을 두면 안됨
        }else if (Input.GetAxis("Horizontal") != 0)
        {
            nowSt=(float) (nowSt+0.03);
            float h = Input.GetAxisRaw("Horizontal");
            rigid.AddForce(Vector2.right * h * 1 , ForceMode2D.Impulse);
            //max speed
            if (rigid.velocity.x > maxSpeed)  //오른쪽으로 이동 (+) , 최대 속력을 넘으면
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y * 1); //해당 오브젝트의 속력은 maxSpeed
            else if (rigid.velocity.x < maxSpeed * (-1)) // 왼쪽으로 이동 (-)
                rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y * 1); //y값은 점프의 영향이므로 0으로 제한을 두면 안됨
        }
        else if(Input.GetAxis("Horizontal") == 0)
        {
            nowSt=(float) (nowSt+0.2);
        }
    }
    void move()
    {
        // 버튼에서 손을 떄는 a등의 단발적인 키보드 입력은 FixedUpdate보다 Update에 쓰는게 키보드 입력이 누락될 확률이 낮아짐
        //Jump
        if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping")&&nowSt >= 10)
        {
            nowSt=(float) (nowSt-10);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            // animator.SetBool("isJumping", true);
        }
        //Stop speed
        if (Input.GetButtonUp("Horizontal"))
        { 
            // 버튼에서 손을 때는 경우
            // normalized : 벡터 크기를 1로 만든 상태 (단위벡터 : 크기가 1인 벡터)
            // 벡터는 방향과 크기를 동시에 가지는데 크기(- : 왼 , + : 오)를 구별하기 위하여 단위벡터(1,-1)로 방향을 알수 있도록 단위벡터를 곱함
            rigid.velocity = new Vector2(0.01f * rigid.velocity.normalized.x, rigid.velocity.y);
        }
        //Direction Sprite

        //속도가 0 == 멈춤
        if (Mathf.Abs(rigid.velocity.x) < 1)
        {
            animator.SetBool("isWalking", false); //isWalking 변수 : false
        }
        else
        {
            //이동중
            animator.SetBool("isWalking", true);
        }
    }
    void trun()
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
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

}