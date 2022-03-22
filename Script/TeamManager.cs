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

    //チーム一覧
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
    /// チームに入る
    /// </summary>
    /// <param name="teamName">入りたいチーム名</param>
    public void JoinTeam(string teamName)
    {
        //どこかすでにチームに入っているなら入りたいチームに所属を変える
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam() != null)
        {
            PhotonNetwork.LocalPlayer.SwitchTeam(teamName);
        }
        else
        {
            PhotonNetwork.LocalPlayer.JoinTeam(teamName);
        }

        //チームに入ったことを自分含めルーム内の全プレイヤーに知らせる
        UpdateTeams();
    }

    /// <summary>
    /// チームリストを更新
    /// </summary>
    public void UpdateTeams()
    {
        //ViaServerを使用しないと自分視点でチームが反映されない？
        PV.RPC("RPC_UpdateTeams", RpcTarget.AllBufferedViaServer);
    }

    /// <summary>
    /// チームリストを更新
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
