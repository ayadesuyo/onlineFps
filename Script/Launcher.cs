using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class Launcher : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [SerializeField] LobbyUIContoroller lobbyUIContoroller;
    [SerializeField] TeamManager teamManager;
    [SerializeField] RuleManager ruleManager;

    void Start()
    {
        Cursor.visible = true;
        lobbyUIContoroller.LoadingUIPop();

        //すでにコネクト状態なら
        if (PhotonNetwork.IsConnected)
        {
            InitialSetting();
        }

        PhotonNetwork.ConnectUsingSettings();
        //仮の名前を付ける
        if (PhotonNetwork.NickName == "")
        {
            PhotonNetwork.NickName = "Player" + Random.Range(1000, 10000).ToString();
        }
        //ホストがシーンを読み込むと他のプレイヤーも自動的に読み込むようにする
        PhotonNetwork.AutomaticallySyncScene = true;

        lobbyUIContoroller.UpdatePlayerListView();

    }

    void InitialSetting()
    {
        lobbyUIContoroller.LoadingUIClose();

    }

    /// <summary>
    /// Photonのサーバに接続されると呼ばれる
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        lobbyUIContoroller.RoomDisplayOpen();
        //プレイヤー一覧を更新する
        lobbyUIContoroller.UpdatePlayerListView();

        //ルールを設定しなおす
        ruleManager.RuleSet();

        teamManager.UpdateTeams();
    }

    /// <summary>
    /// ロビーに入ると呼ばれる
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        lobbyUIContoroller.LoadingUIClose();
        //ロビーに入ったらまずはホーム画面を開く
        lobbyUIContoroller.HomeDisplayOpen();
    }

    /// <summary>
    /// 部屋を作る
    /// </summary>
    /// <param name="roomName">作成する部屋の名前</param>
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName ,new RoomOptions { IsVisible = true }, TypedLobby.Default);
        Debug.Log("CreateRoom : " + roomName);
    }

    /// <summary>
    /// 部屋に入ったときに呼ばれる
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        lobbyUIContoroller.RoomDisplayOpen();
        //プレイヤー一覧を更新する
        lobbyUIContoroller.UpdatePlayerListView();

        //ルールを設定しなおす
        ruleManager.RuleSet();

        //自動的にRedチームに参加
        teamManager.JoinTeam(TeamManager.team.Red.ToString());
    }

    /// <summary>
    /// 誰かが部屋から退出したときに呼ばれる
    /// </summary>
    /// <param name="otherPlayer">退出したプレイヤーの情報</param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //プレイヤー一覧を更新する
        lobbyUIContoroller.UpdatePlayerListView();
        //チームを更新する
        teamManager.UpdateTeams();
    }

    /// <summary>
    /// 誰かが部屋に入室したときに呼ばれる
    /// </summary>
    /// <param name="newPlayer">入室したプレイヤーの情報</param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //プレイヤー一覧を更新する
        lobbyUIContoroller.UpdatePlayerListView();
    }

    /// <summary>
    /// 部屋が作られたときや無くなったときに呼ばれる
    /// </summary>
    /// <param name="roomList">今ある部屋の一覧</param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room List Update");
        foreach(var room in roomList)
        {
            Debug.Log(room.Name);
        }
        lobbyUIContoroller.UpdateRoomListView(roomList);
    }

    /// <summary>
    /// 部屋に入る
    /// </summary>
    /// <param name="roomName">入る部屋の名前</param>
    public static void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// 今いる部屋から退出する
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        
    }

    /// <summary>
    /// 部屋から退出した時呼ばれる
    /// </summary>
    public override void OnLeftRoom()
    {
        //ホーム画面に戻る
        lobbyUIContoroller.HomeDisplayOpen();
    }

    /// <summary>
    /// ゲームシーンを読み込む
    /// </summary>
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
