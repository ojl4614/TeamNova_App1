using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 필요
public class Signup_to_Login : MonoBehaviour
{
    public void SceneChange()
    {       
        SceneManager.LoadScene("Login");
    }
}
    