using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

public class TeamManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    [SerializeField] LobbyUIContoroller lobbyUIContoroller;
    public static TeamManager Instance;

    //�`�[���ꗗ
    public enum team
    {
        Red,
        Blue
    }

    private void Awake()
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
    }

    /// <summary>
    /// �`�[���ɓ���
    /// </summary>
    /// <param name="teamName">���肽���`�[����</param>
    public void JoinTeam(string teamName)
    {
        //�ǂ������łɃ`�[���ɓ����Ă���Ȃ���肽���`�[���ɏ�����ς���
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam() != null)
        {
            PhotonNetwork.LocalPlayer.SwitchTeam(teamName);
        }
        else
        {
            PhotonNetwork.LocalPlayer.JoinTeam(teamName);
        }

        //�`�[���ɓ��������Ƃ������܂߃��[�����̑S�v���C���[�ɒm�点��
        UpdateTeams();
    }

    /// <summary>
    /// �`�[�����X�g���X�V
    /// </summary>
    public void UpdateTeams()
    {
        //ViaServer���g�p���Ȃ��Ǝ������_�Ń`�[�������f����Ȃ��H
        PV.RPC("RPC_UpdateTeams", RpcTarget.AllBufferedViaServer);
    }

    /// <summary>
    /// �`�[�����X�g���X�V
    /// </summary>
    [PunRPC]
    public void RPC_UpdateTeams()
    {
        Player[] redTeamPlayers;
        Player[] blueTeamPlayers;
        PhotonTeamsManager.Instance.TryGetTeamMembers(team.Red.ToString(),out redTeamPlayers);
        PhotonTeamsManager.Instance.TryGetTeamMembers(team.Blue.ToString(),out blueTeamPlayers);

        lobbyUIContoroller.UpdateTeamsPlayerView(redTeamPlayers, blueTeamPlayers);
    }

}
