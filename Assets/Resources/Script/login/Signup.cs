using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.SceneManagement; // 필요
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class Signup : MonoBehaviour {
    public InputField Email_;
    public InputField Nickname_;
    public InputField Password_;
    public InputField Password_Check_;
    private bool isOn = false;
    string URL = "http://127.0.0.1/Unity/Unity_Signup.php";
    public GameObject emailPanel;
    public GameObject nickPanel;
    public GameObject pwPanel;
    public GameObject blankPanel;
    

    public void Data ()
    {
        StartCoroutine(DataPost());
    }

    private void Start()
    {
        emailPanel.SetActive(false);
        nickPanel.SetActive(false);
        pwPanel.SetActive(false);
        blankPanel.SetActive(false);
    }

    IEnumerator DataPost(){
        string Email = Email_.text;
        string Nick = Nickname_.text;
        string Pw = Password_.text;
        string Pw2 = Password_Check_.text;
        if (Email == "" || Nick == "")
        {
            blankPanel.SetActive(true);
        }else if(Pw==Pw2 && Pw!="")
        {
            WWWForm form = new WWWForm();
            form.AddField("Email",Email_.text);
            form.AddField("Nick",Nickname_.text);
            form.AddField("Pw",Password_.text);
            // UnityWebRequest www = UnityWebRequest.Post(URL, form);
            // yield return www.SendWebRequest();
            WWW www = new WWW(URL, form);
            yield return www;
            if (www.text=="1")
            {
                emailPanel.SetActive(true);
                Debug.Log("이메일 중복");
            }
            if (www.text=="2")
            {
                nickPanel.SetActive(true);
                Debug.Log("닉네임 중복");
            }
            if (www.text=="3")
            {
                Debug.Log("회원가입 성공");
                SceneManager.LoadScene("Login");
            }
        }else{
            pwPanel.SetActive(true);
            Debug.Log("비밀번호 불일치");
        }
    }

}
