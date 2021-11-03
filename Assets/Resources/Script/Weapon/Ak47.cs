using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  

public class Ak47 : MonoBehaviour, IPunObservable
{
    private SpriteRenderer spriteRenderer; //방향전환을 위한 변수
    public GameObject bullet_a;
    public Transform pos;
    public float cooltime;
    private float curtime;
    public PhotonView PV;
    public float axis;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 초기화
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            if (curtime <= 0)
            {
                if (Input.GetMouseButton(0))
                {
                    PhotonNetwork.Instantiate("ak47_bullet",pos.position,transform.rotation);
                }
                curtime = cooltime;
            }
            curtime -= Time.deltaTime;  
        }
    }
    void FixedUpdate()
    {
        Move();
    }
    
    void Move()
    {
        if (PV.IsMine)
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.rotation = rotation;
            if (angle >= 0 && angle <= 90 || angle <= 0 && angle >= -90)
            {
                axis = -1;
                PV.RPC("FlipYRPC", RpcTarget.AllBuffered, axis); //재접속시 fipx를 동기화주기 위해서 allbuffered
            }
            else
            {
                axis = 1;
                PV.RPC("FlipYRPC", RpcTarget.AllBuffered, axis); //재접속시 fipx를 동기화주기 위해서 allbuffered
            }
        }

    }
    [PunRPC]
    void FlipYRPC(float axis) => spriteRenderer.flipY = axis == 1;
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
    
}