using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyUIContoroller : MonoBehaviour
{
    [SerializeField] Launcher launcher;

    //LobbyUI内の画面すべて
    [SerializeField] GameObject homeDisplay;
    [SerializeField] GameObject createRoomDisplay;
    [SerializeField] GameObject roomDisplay;//自分がいる部屋の待機画面
    [SerializeField] GameObject roomListViewDisplay;//存在する部屋の一覧画面
    [SerializeField] GameObject nameChangeDisplay;//名前変更画面
    [SerializeField] GameObject roading;

    //部屋の名前を入力するフィールド
    [SerializeField] InputField roomNameInput;

    //今いる部屋の名前を表示するテキスト
    [SerializeField] Text currentRoomNameText;

    //ルーム内にいるプレイヤー一覧表
    [SerializeField] Transform playerListView;
    //Red チームに所属しているプレイヤー一覧表
    [SerializeField] Transform redTeamPlayerView;
    [SerializeField] Transform redTeamPlayerViewContent;
    //Blueチームに所属しているプレイヤー一覧表
    [SerializeField] Transform blueTeamPlayerView;
    [SerializeField] Transform blueTeamPlayerViewContent;
    //チーム変更に使うボタン
    [SerializeField] GameObject redTeamButton;
    [SerializeField] GameObject blueTeamButton;
    //プレイヤー一覧表に表示するUI
    [SerializeField] GameObject playerInfo;
    //ゲーム開始ボタン
    [SerializeField] GameObject gameStartButton;

    //部屋一覧表
    [SerializeField] Transform roomListView;
    //部屋一覧表に表示するUI
    [SerializeField] GameObject roomInfo;

    //名前を入力するフィールド
    [SerializeField] InputField nameInput;

    /// <summary>
    /// すべての画面を閉じる
    /// </summary>
    private void AllDisplayClose()
    {
        homeDisplay.SetActive(false);
        createRoomDisplay.SetActive(false);
        roomDisplay.SetActive(false);
        roomListViewDisplay.SetActive(false);
        nameChangeDisplay.SetActive(false);

    }

    /// <summary>
    /// home画面を開く
    /// </summary>
    public void HomeDisplayOpen()
    {
        //一度すべての画面を閉じる
        AllDisplayClose();
        homeDisplay.SetActive(true);
    }

    /// <summary>
    /// createRoom画面を開く
    /// </summary>
    public void CreateRoomDisplayOpen()
    {
        //一度すべての画面を閉じる
        AllDisplayClose();
        createRoomDisplay.SetActive(true);
    }

    /// <summary>
    /// 名前変更画面を開く
    /// </summary>
    public void NameChangeDisplayOpen()
    {
        //一度すべての画面を閉じる
        AllDisplayClose();
        nameChangeDisplay.SetActive(true);
    }

    public void NameChange()
    {
        PhotonNetwork.NickName = nameInput.text;
    }

    /// <summary>
    /// 部屋を作る
    /// </summary>
    public void CreateRoom()
    {
        //名前がない部屋は作らない
        if (roomNameInput.text == "") return;

        launcher.CreateRoom(roomNameInput.text);
        RoomDisplayOpen();
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void GameEnd()
    {
        Application.Quit();
    }

    public void RoomDisplayOpen()
    {
        //一度すべての画面を閉じる
        AllDisplayClose();
        //部屋の名前を更新
        currentRoomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomDisplay.SetActive(true);
    }

    public void RoomListViewDisplayOpen()
    {
        //一度すべての画面を閉じる
        AllDisplayClose();
        roomListViewDisplay.SetActive(true);
    }

    /// <summary>
    /// 今いる部屋から退出する
    /// </summary>
    public void LeaveRoom()
    {
        launcher.LeaveRoom();
    }

    /// <summary>
    /// 部屋にいるプレイヤー一覧表を更新する
    /// </summary>
    public void UpdatePlayerListView()
    {
        //更新するために一度今表示している一覧表をすべて消す
        foreach (Transform child in playerListView)
        {
            Destroy(child.gameObject);
        }

        foreach(var player in PhotonNetwork.PlayerList)
        {
            //プレイヤー名を書いたボタンを追加していく
            Instantiate(playerInfo,playerListView).GetComponent<PlayerInfoItem>().Set(player.NickName,player.ActorNumber);
        }
    }

    /// <summary>
    /// 部屋一覧表を更新する
    /// </summary>
    public void UpdateRoomListView(List<RoomInfo> roomList)
    {
        //更新するために一度今表示している一覧表をすべて消す
        foreach (Transform child in roomListView)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            Instantiate(roomInfo, roomListView).GetComponent<RoomInfoItem>().Set(roomList[i].Name);
        }
    }



    public void UpdateTeamsPlayerView(Player[] redPlayers,Player[] bluePlayers)
    {
        //更新するために一度今表示している一覧表をすべて消す
        foreach (Transform child in redTeamPlayerViewContent)
        {
            Destroy(child.gameObject);
        }
        //更新するために一度今表示している一覧表をすべて消す
        foreach (Transform child in blueTeamPlayerViewContent)
        {
            Destroy(child.gameObject);
        }

        if(redPlayers.Length > 0)
        {
            foreach(Player player in redPlayers)
            {
                Instantiate(playerInfo, redTeamPlayerViewContent).GetComponent<PlayerInfoItem>().Set(player.NickName, player.ActorNumber);
            }
        }

        if (bluePlayers.Length > 0)
        {
            foreach (Player player in bluePlayers)
            {
                Instantiate(playerInfo, blueTeamPlayerViewContent).GetComponent<PlayerInfoItem>().Set(player.NickName, player.ActorNumber);
            }
        }
    }

    /// <summary>
    /// チームリストのSetActiveを制御
    /// </summary>
    /// <param name="isSet">trueかfalseか</param>
    public void TeamViewSetActive(bool isSet)
    {
        redTeamPlayerView.gameObject.SetActive(isSet);
        blueTeamPlayerView.gameObject.SetActive(isSet);
        redTeamButton.SetActive(isSet);
        blueTeamButton.SetActive(isSet);
    }

    public void StartGame()
    {
        //部屋のホストで無いならゲームを開始できない
        if (!PhotonNetwork.IsMasterClient) return;

        launcher.StartGame();
    }

    public void LoadingUIPop()
    {
        roading.SetActive(true);
    }

    public void LoadingUIClose()
    {
        roading.SetActive(false);
    }
}
