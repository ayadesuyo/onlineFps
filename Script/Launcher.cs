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

        //���łɃR�l�N�g��ԂȂ�
        if (PhotonNetwork.IsConnected)
        {
            InitialSetting();
        }

        PhotonNetwork.ConnectUsingSettings();
        //���̖��O��t����
        if (PhotonNetwork.NickName == "")
        {
            PhotonNetwork.NickName = "Player" + Random.Range(1000, 10000).ToString();
        }
        //�z�X�g���V�[����ǂݍ��ނƑ��̃v���C���[�������I�ɓǂݍ��ނ悤�ɂ���
        PhotonNetwork.AutomaticallySyncScene = true;

        lobbyUIContoroller.UpdatePlayerListView();

    }

    void InitialSetting()
    {
        lobbyUIContoroller.LoadingUIClose();

    }

    /// <summary>
    /// Photon�̃T�[�o�ɐڑ������ƌĂ΂��
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        lobbyUIContoroller.RoomDisplayOpen();
        //�v���C���[�ꗗ���X�V����
        lobbyUIContoroller.UpdatePlayerListView();

        //���[����ݒ肵�Ȃ���
        ruleManager.RuleSet();

        teamManager.UpdateTeams();
    }

    /// <summary>
    /// ���r�[�ɓ���ƌĂ΂��
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        lobbyUIContoroller.LoadingUIClose();
        //���r�[�ɓ�������܂��̓z�[����ʂ��J��
        lobbyUIContoroller.HomeDisplayOpen();
    }

    /// <summary>
    /// ���������
    /// </summary>
    /// <param name="roomName">�쐬���镔���̖��O</param>
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName ,new RoomOptions { IsVisible = true }, TypedLobby.Default);
        Debug.Log("CreateRoom : " + roomName);
    }

    /// <summary>
    /// �����ɓ������Ƃ��ɌĂ΂��
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        lobbyUIContoroller.RoomDisplayOpen();
        //�v���C���[�ꗗ���X�V����
        lobbyUIContoroller.UpdatePlayerListView();

        //���[����ݒ肵�Ȃ���
        ruleManager.RuleSet();

        //�����I��Red�`�[���ɎQ��
        teamManager.JoinTeam(TeamManager.team.Red.ToString());
    }

    /// <summary>
    /// �N������������ޏo�����Ƃ��ɌĂ΂��
    /// </summary>
    /// <param name="otherPlayer">�ޏo�����v���C���[�̏��</param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //�v���C���[�ꗗ���X�V����
        lobbyUIContoroller.UpdatePlayerListView();
        //�`�[�����X�V����
        teamManager.UpdateTeams();
    }

    /// <summary>
    /// �N���������ɓ��������Ƃ��ɌĂ΂��
    /// </summary>
    /// <param name="newPlayer">���������v���C���[�̏��</param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //�v���C���[�ꗗ���X�V����
        lobbyUIContoroller.UpdatePlayerListView();
    }

    /// <summary>
    /// ���������ꂽ�Ƃ��△���Ȃ����Ƃ��ɌĂ΂��
    /// </summary>
    /// <param name="roomList">�����镔���̈ꗗ</param>
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
    /// �����ɓ���
    /// </summary>
    /// <param name="roomName">���镔���̖��O</param>
    public static void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// �����镔������ޏo����
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        
    }

    /// <summary>
    /// ��������ޏo�������Ă΂��
    /// </summary>
    public override void OnLeftRoom()
    {
        //�z�[����ʂɖ߂�
        lobbyUIContoroller.HomeDisplayOpen();
    }

    /// <summary>
    /// �Q�[���V�[����ǂݍ���
    /// </summary>
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
