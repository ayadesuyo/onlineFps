using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MiniMapManager : MonoBehaviour
{
    PhotonView PV;
    MyTeam myTeam;

    [SerializeField] GameObject miniMapCamera;
    [SerializeField] Camera iconCamera;

    [SerializeField] GameObject icon;
    [SerializeField] GameObject redDotIcon;
    bool isRedDotPop = false;
    void Start()
    {
        PV = GetComponent<PhotonView>();
        myTeam = GetComponent<MyTeam>();
        redDotIcon.SetActive(false);

        if (!PV.IsMine)
        {
            Destroy(iconCamera);
            Destroy(miniMapCamera);

            // 自身のチームに合わせてレイヤーを設定
            if(myTeam.GetMyTeam() == TeamManager.team.Red.ToString())
            {
                icon.layer = 14;
            }
            if (myTeam.GetMyTeam() == TeamManager.team.Blue.ToString())
            {
                icon.layer = 15;
            }
        }
        else
        {
            icon.layer = 13;
            
            // FFAでなければ敵チームの位置がミニマップに表示されないよう設定する
            if(RuleManager.currentRule != "FFA")
            {
                if (myTeam.GetMyTeam() == TeamManager.team.Red.ToString())
                {
                    iconCamera.cullingMask |= 1 << 14;
                }
                if (myTeam.GetMyTeam() == TeamManager.team.Blue.ToString())
                {
                    iconCamera.cullingMask |= 1 << 15;
                }
            }
        }
    }

    public void RedDotPop()
    {
        if (!PV.IsMine) return;
        PV.RPC("RPC_RedDotPop", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void RPC_RedDotPop()
    {
        if (isRedDotPop)
        {
            StopCoroutine("RedDotPopTimer");
        }
        StartCoroutine("RedDotPopTimer");
    }

    IEnumerator RedDotPopTimer()
    {
        isRedDotPop = true;
        redDotIcon.SetActive(isRedDotPop);

        yield return new WaitForSeconds(1f);

        isRedDotPop = false;
        redDotIcon.SetActive(isRedDotPop);
    }
}
