using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Zombie : MonoBehaviour
{

    Rigidbody2D rigid;
    public int nextMove;//다음 행동지표를 결정할 변수
    Animator animator;
    SpriteRenderer spriteRenderer;
    public int nowHp;
    public Transform pos;

    public int trace = 0;
    // Start is called before the first frame update

    private void Start()
    {
        nowHp = 100;
    }

    private void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        // if (trace == 0)
        // {
        //     Invoke("Think", 5); // 초기화 함수 안에 넣어서 실행될 때 마다(최초 1회) nextMove변수가 초기화 되도록함 
        // }
    }

    
    //추적 할지말지의 여부 trace = 1이면 추적이고 trace = 0이면 추적이 아니다
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            trace = 1;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "player")
        {
            trace = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        trace = 0;
        Think();
    }

    private void Update()
    {
        if (nowHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "bullet")
            OnDamaged(other.transform.position);
    }
    //적에게 닿았을때
    void OnDamaged(Vector2 targetPos)
    {
        //player  damaged
        nowHp=nowHp-5;   //좀비가 뭐에 맞았는지에 따른 데미지 구별해야함 //FIXME
        // spriteRenderer.color = new Color(200, 1, 1, 0.4f);
        //reaction forc
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1), ForceMode2D.Impulse);
    }
    void FixedUpdate()
    {
        move();
    }
    void move()
    {
                //Move
                if (trace == 0)
                {
                    rigid.velocity = new Vector2(nextMove*2,rigid.velocity.y); //nextMove 에 0:멈춤 -1:왼쪽 1:오른쪽 으로 이동 
                }
                if (trace == 1)
                 {
                     Vector3 playerpos = pos.transform.position;
                     String dist = "";
                     if (playerpos.x < transform.position.x)
                     {
                         dist = "right";
                     }
                     else
                     {
                         dist = "left";
                     }
        
                     if (playerpos.x == transform.position.x)
                     {
                         dist = "right";
                     }
                
                     if (dist == "left")
                     {
                         rigid.velocity = new Vector2(2,rigid.velocity.y); //nextMove 에 0:멈춤 -1:왼쪽 1:오른쪽 으로 이동 
                         animator.SetInteger("WalkSpeed",1);
                         spriteRenderer.flipX = false;
                     }
                     if (dist == "right")
                     {
                         rigid.velocity = new Vector2(-2,rigid.velocity.y); //nextMove 에 0:멈춤 -1:왼쪽 1:오른쪽 으로 이동 
                         animator.SetInteger("WalkSpeed",-1);
                         spriteRenderer.flipX = true;
                     }
                 }
                //Platform check(맵 앞이 낭떨어지면 뒤돌기 위해서 지형을 탐색)
               //자신의 한 칸 앞 지형을 탐색해야하므로 position.x + nextMove(-1,1,0이므로 적절함)
               if (trace == 0)
               {
                   Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.4f, rigid.position.y);
        
                   //한칸 앞 부분아래 쪽으로 ray를 쏨
                   Debug.DrawRay(frontVec, Vector3.down, new Color(0,1,0));
        
                   //레이를 쏴서 맞은 오브젝트를 탐지 
                   RaycastHit2D raycast = Physics2D.Raycast(frontVec, Vector3.down,2,LayerMask.GetMask("Platform"));
        
                   //탐지된 오브젝트가 null : 그 앞에 지형이 없음
                   if(raycast.collider == null){
                       Turn();
                   }
                   if (nextMove == -1)
                   {
                       spriteRenderer.flipX = true;
                   }
                   if (nextMove == 1)
                   {
                       spriteRenderer.flipX = false;
                   }  
               }
    }
    void Think(){//몬스터가 스스로 생각해서 판단 (-1:왼쪽이동 ,1:오른쪽 이동 ,0:멈춤  으로 3가지 행동을 판단)
        if (trace == 0)
        {
        //Set Next Active
        //Random.Range : 최소<= 난수 <최대 /범위의 랜덤 수를 생성(최대는 제외이므로 주의해야함)
        nextMove = Random.Range(-1,2);
        //Sprite Animation
        //WalkSpeed변수를 nextMove로 초기화 
        animator.SetInteger("WalkSpeed",nextMove);
        //Flip Sprite 서있을 때 굳이 방향을 바꿀 필요가 없음 
        if (nextMove == 0)
        {
            if (spriteRenderer.flipX == true)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
        //Recursive (재귀함수는 가장 아래에 쓰는게 기본적) 
        float time = Random.Range(2f, 5f); //생각하는 시간을 랜덤으로 부여 
        //Think(); : 재귀함수 : 딜레이를 쓰지 않으면 CPU과부화 되므로 재귀함수쓸 때는 항상 주의 ->Think()를 직접 호출하는 대신 Invoke()사용
  
            Invoke("Think", time); //매개변수로 받은 함수를 time초의 딜레이를 부여하여 재실행   
        }
    }

    void Turn(){

        nextMove= nextMove*(-1); //우리가 직접 방향을 바꾸어 주었으니 Think는 잠시 멈추어야함
        if (nextMove == -1)
        {
            spriteRenderer.flipX = true;
        }

        CancelInvoke(); //think를 잠시 멈춘 후 재실행
        Invoke("Think",2);//  

    }

}