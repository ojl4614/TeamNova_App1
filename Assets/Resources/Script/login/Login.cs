using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement; // 필요
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviourPunCallbacks
{
    
    public InputField Email_;
    public InputField Password_;
    public GameObject blankPanel;
    public GameObject incorrectPanel;
    public GameObject playerobj;
    string URL = "http://127.0.0.1/Unity/Unity_Login.php";

    public void Data ()
    {
        StartCoroutine(DataPost());
    }

    private void Start()
    {
        incorrectPanel.SetActive(false);
        blankPanel.SetActive(false);
    }

    IEnumerator DataPost(){
        string Email = Email_.text;
        string Pw = Password_.text;
        WWWForm form = new WWWForm();
        form.AddField("Email",Email_.text);
        form.AddField("Pw",Password_.text);
        // UnityWebRequest www = UnityWebRequest.Post(URL, form);
        // yield return www.SendWebRequest();
        WWW www = new WWW(URL, form);
        yield return www;
        
        if (www.text=="1")
        {
            Debug.Log(www.text);
            blankPanel.SetActive(true);
            Debug.Log("공란");
        }else if (www.text=="3")
        {
            Debug.Log(www.text);
            incorrectPanel.SetActive(true);
            Debug.Log("실패");
        }else if (www.text == "4")
        {
            incorrectPanel.SetActive(true);
            Debug.Log("d알없");
        }
        else
        {
            Debug.Log(www.text);
            playerobj = GameObject.Find("profile");
            playerobj.GetComponent<Player_profile>().name_= www.text;
            DontDestroyOnLoad(playerobj);
            SceneManager.LoadScene("lobby");
        }
    }
    
    
}