using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class NewNetwork : MonoBehaviourPunCallbacks
{
    public string email;
    public GameObject playerobj;
   
    private void Start()
    {
        // PhotonNetwork.ConnectUsingSettings();
        playerobj = GameObject.Find("profile");
        email = playerobj.GetComponent<Player_profile>().name_;
        // Destory();
        Invoke("spawn",1);
    }
    public void spawn()
    {
        PhotonNetwork.Instantiate("player_",new Vector3(Random.Range(-25f,25), 4, 0),Quaternion.identity);
        Debug.Log("spawn success");
    }
    public override void onDisconnected(DisconnectCause cause)
    {
        Debug.Log("실패");
        PhotonNetwork.ConnectUsingSettings();
    }

    IEnumerator Destory()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (GameObject GO in GameObject.FindGameObjectsWithTag("player")) GO.GetComponent<PhotonView>().RPC("Destroyplayer", RpcTarget.All);
    }
    [PunRPC]
    void Destroyplayer() => Destroy(playerobj);
}