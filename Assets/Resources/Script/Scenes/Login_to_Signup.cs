using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 필요

public class Login_to_Signup : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("Signup");
    }
}
