using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class MyTeam : MonoBehaviour
{
    PhotonView PV;

    [SerializeField]private TeamManager.team myTeam;
    //É`Å[ÉÄÇ…ëŒâûÇµÇΩêF
    [SerializeField] private Material[] teamColors;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            PV.RPC("RPC_SetMyTeam", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.GetPhotonTeam().Name);
        }
    }

    [PunRPC]
    private void RPC_SetMyTeam(string teamName)
    {
        if(teamName == TeamManager.team.Red.ToString())
        {
            myTeam = TeamManager.team.Red;
            gameObject.GetComponent<Renderer>().material = teamColors[0];
        }
        else
        {
            myTeam = TeamManager.team.Blue;
            gameObject.GetComponent<Renderer>().material = teamColors[1];
        }
    }

    public string GetMyTeam()
    {
        return myTeam.ToString();
    }
}
