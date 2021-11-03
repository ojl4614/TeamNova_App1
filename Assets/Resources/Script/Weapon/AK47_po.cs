using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AK47_po : MonoBehaviour
    

{

    public Transform pos;
    Vector2 vector; //새 변수 만듬
    Vector2 vectora; //새 변수 만듬
    Vector2 vectorb; //새 변수 만듬

    // Start is called before the first frame update

    public void Start()
    {
        vector= transform.localPosition; //현재 위치에 변수를 넣어줌
        vector.y *=(float) -1; //변수.x 이런식으로 원하는 값 넣음
        
        vectora = transform.localPosition;
        vectora.y *= 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        
    }
    void Move()
    {
        // Vector3 vector; //새 변수 만듬
        // vector = transform.position; //새 변수에 현재 위치 넣어줌
        // vector.y = 0.19f; //변수.x 이런식으로 원하는 값 넣음
        // transform.position = vector; //현재 위치에 변수를 넣어줌

        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        // transform.rotation = rotation;
        if (angle >= 0 && angle <= 90 || angle <= 0 && angle >= -90)
        {
            transform.localPosition = vectora; //현재 위치에 변수를 넣어줌
            // Debug.Log(vector);
            // transform.rotat ion = quaternion.Euler(0,0,0);
        }
        else
        {
            transform.localPosition = vector; //현재 위치에 변수를 넣어줌
            // Debug.Log(vector);
            // transform.rotation = quaternion.Euler(-90,0,0);
        }
    }
}