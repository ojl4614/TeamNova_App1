using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  


public class Bullet : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PV;
    public float speed ;
    private Rigidbody2D rigid;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        Invoke("DestroyBullet",30);
    }
    void Update()
    {
        // Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rigid.velocity = transform.right * speed;
    }

     void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "BorderBullet")
        {
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }

        if (col.tag == "player" && col.GetComponent<PhotonView>().IsMine)   //플레이어 히트 판정
        {
            col.GetComponent<Player_multi>().hit();
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        
    }
     [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}