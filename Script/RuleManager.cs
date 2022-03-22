using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RuleManager : MonoBehaviour
{
    PhotonView PV;
    [SerializeField] Text ruleChangeButtonText;
    [SerializeField] LobbyUIContoroller lobbyUIContoroller;

    string[] rules = new string[2] { "FFA","TDM"};
    int ruleIndex = 1;
    public static string currentRule;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    public void RuleChangeButtonClick()
    {
        //部屋のホストで無いならルールを変更できない
        if (!PhotonNetwork.IsMasterClient) return;

        ruleIndex = (ruleIndex + 1) % rules.Length;
        RuleSet();
    }

    public void RuleSet()
    {
        PV.RPC("RPC_RuleSet", RpcTarget.AllBufferedViaServer, rules[ruleIndex]);
    }

    /// <summary>
    /// ルールをセットする
    /// </summary>
    /// <param name="rule"></param>
    [PunRPC]
    public void RPC_RuleSet(string rule)
    {
        ruleChangeButtonText.text = rule;
        currentRule = rule;

        //ルールがFFAならチーム関係のUIを見えなくする
        bool flag = true;
        if (currentRule == rules[0]) flag = false;
        lobbyUIContoroller.TeamViewSetActive(flag);
    }
}
