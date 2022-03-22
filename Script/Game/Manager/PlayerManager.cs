using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.IO;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    //ffa��player�𐶐�����ʒu
    private Vector3[] respawnPos = new Vector3[13] { new Vector3(-44, -6, -37), new Vector3(-21, -6, -41), new Vector3(-21, -5, -30), new Vector3(27, -6, -18), new Vector3(55, -6, -18), new Vector3(46, -6, 17), new Vector3(46, -6, 6), new Vector3(23, -6, 43), new Vector3(-17, -6, 40), new Vector3(-44, -6, 41),new Vector3(-55, -6, 4),new Vector3(-7, -6, 8),new Vector3(11, -6, -8) };

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        if (RuleManager.currentRule == "TDM")
        {
            Vector3 spwanPos;

            if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == TeamManager.team.Red.ToString())
            {
                Vector3[] redPoss = GameManager.redTeammatePos;
                var spawnablePos = new List<Vector3>();

                foreach (Vector3 pos in redPoss)
                {
                    // �߂��Ƀ��X�|�[���\�Ȗ����̈ʒu�̂ݎ擾
                    if (pos != Vector3.zero)
                    {
                        spawnablePos.Add(pos);
                    }
                }

                // ���X�|�[���\�Ȉʒu�������ꍇ�͏���̈ʒu�Ƀ��X�|�[��
                if (spawnablePos.Count == 0)
                {
                    spwanPos = new Vector3(-66, -6, -64);
                }
                else
                {
                    spwanPos = spawnablePos[Random.Range(0, spawnablePos.Count)];
                }

                GameObject player = PhotonNetwork.Instantiate("Player", spwanPos, Quaternion.identity);
                UIManager ui = player.GetComponent<UIManager>();
                GameManager.SetUI(ui);
            }
            else
            {
                Vector3[] bluePoss = GameManager.blueTeammatePos;
                var spawnablePos = new List<Vector3>();

                foreach (Vector3 pos in bluePoss)
                {
                    // �߂��Ƀ��X�|�[���\�Ȗ����̈ʒu�̂ݎ擾
                    if (pos != Vector3.zero)
                    {
                        spawnablePos.Add(pos);
                    }
                }

                // ���X�|�[���\�Ȉʒu�������ꍇ�͏���̈ʒu�Ƀ��X�|�[��
                if (spawnablePos.Count == 0)
                {
                    spwanPos = new Vector3(15, -6, 61);
                }
                else
                {
                    spwanPos = spawnablePos[Random.Range(0, spawnablePos.Count)];
                }

                GameObject player = PhotonNetwork.Instantiate("Player", spwanPos, Quaternion.identity);
                UIManager ui = player.GetComponent<UIManager>();
                Look look = player.GetComponent<Look>();
                look.SetRotation(180f, 0);

                GameManager.SetUI(ui);
            }
        }
        else if(RuleManager.currentRule == "FFA")
        {
            Vector3 spwanPos = respawnPos[Random.Range(0, respawnPos.Length)];
            GameObject player = PhotonNetwork.Instantiate("Player", spwanPos, Quaternion.identity);
            UIManager ui = player.GetComponent<UIManager>();
            GameManager.SetUI(ui);
        }
    }
}
