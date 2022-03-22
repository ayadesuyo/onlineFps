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
        //�����̃z�X�g�Ŗ����Ȃ烋�[����ύX�ł��Ȃ�
        if (!PhotonNetwork.IsMasterClient) return;

        ruleIndex = (ruleIndex + 1) % rules.Length;
        RuleSet();
    }

    public void RuleSet()
    {
        PV.RPC("RPC_RuleSet", RpcTarget.AllBufferedViaServer, rules[ruleIndex]);
    }

    /// <summary>
    /// ���[�����Z�b�g����
    /// </summary>
    /// <param name="rule"></param>
    [PunRPC]
    public void RPC_RuleSet(string rule)
    {
        ruleChangeButtonText.text = rule;
        currentRule = rule;

        //���[����FFA�Ȃ�`�[���֌W��UI�������Ȃ�����
        bool flag = true;
        if (currentRule == rules[0]) flag = false;
        lobbyUIContoroller.TeamViewSetActive(flag);
    }
}
