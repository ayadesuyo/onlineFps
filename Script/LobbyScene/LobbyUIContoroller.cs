using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyUIContoroller : MonoBehaviour
{
    [SerializeField] Launcher launcher;

    //LobbyUI���̉�ʂ��ׂ�
    [SerializeField] GameObject homeDisplay;
    [SerializeField] GameObject createRoomDisplay;
    [SerializeField] GameObject roomDisplay;//���������镔���̑ҋ@���
    [SerializeField] GameObject roomListViewDisplay;//���݂��镔���̈ꗗ���
    [SerializeField] GameObject nameChangeDisplay;//���O�ύX���
    [SerializeField] GameObject roading;

    //�����̖��O����͂���t�B�[���h
    [SerializeField] InputField roomNameInput;

    //�����镔���̖��O��\������e�L�X�g
    [SerializeField] Text currentRoomNameText;

    //���[�����ɂ���v���C���[�ꗗ�\
    [SerializeField] Transform playerListView;
    //Red �`�[���ɏ������Ă���v���C���[�ꗗ�\
    [SerializeField] Transform redTeamPlayerView;
    [SerializeField] Transform redTeamPlayerViewContent;
    //Blue�`�[���ɏ������Ă���v���C���[�ꗗ�\
    [SerializeField] Transform blueTeamPlayerView;
    [SerializeField] Transform blueTeamPlayerViewContent;
    //�`�[���ύX�Ɏg���{�^��
    [SerializeField] GameObject redTeamButton;
    [SerializeField] GameObject blueTeamButton;
    //�v���C���[�ꗗ�\�ɕ\������UI
    [SerializeField] GameObject playerInfo;
    //�Q�[���J�n�{�^��
    [SerializeField] GameObject gameStartButton;

    //�����ꗗ�\
    [SerializeField] Transform roomListView;
    //�����ꗗ�\�ɕ\������UI
    [SerializeField] GameObject roomInfo;

    //���O����͂���t�B�[���h
    [SerializeField] InputField nameInput;

    /// <summary>
    /// ���ׂẲ�ʂ����
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
    /// home��ʂ��J��
    /// </summary>
    public void HomeDisplayOpen()
    {
        //��x���ׂẲ�ʂ����
        AllDisplayClose();
        homeDisplay.SetActive(true);
    }

    /// <summary>
    /// createRoom��ʂ��J��
    /// </summary>
    public void CreateRoomDisplayOpen()
    {
        //��x���ׂẲ�ʂ����
        AllDisplayClose();
        createRoomDisplay.SetActive(true);
    }

    /// <summary>
    /// ���O�ύX��ʂ��J��
    /// </summary>
    public void NameChangeDisplayOpen()
    {
        //��x���ׂẲ�ʂ����
        AllDisplayClose();
        nameChangeDisplay.SetActive(true);
    }

    public void NameChange()
    {
        PhotonNetwork.NickName = nameInput.text;
    }

    /// <summary>
    /// ���������
    /// </summary>
    public void CreateRoom()
    {
        //���O���Ȃ������͍��Ȃ�
        if (roomNameInput.text == "") return;

        launcher.CreateRoom(roomNameInput.text);
        RoomDisplayOpen();
    }

    /// <summary>
    /// �Q�[���I��
    /// </summary>
    public void GameEnd()
    {
        Application.Quit();
    }

    public void RoomDisplayOpen()
    {
        //��x���ׂẲ�ʂ����
        AllDisplayClose();
        //�����̖��O���X�V
        currentRoomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomDisplay.SetActive(true);
    }

    public void RoomListViewDisplayOpen()
    {
        //��x���ׂẲ�ʂ����
        AllDisplayClose();
        roomListViewDisplay.SetActive(true);
    }

    /// <summary>
    /// �����镔������ޏo����
    /// </summary>
    public void LeaveRoom()
    {
        launcher.LeaveRoom();
    }

    /// <summary>
    /// �����ɂ���v���C���[�ꗗ�\���X�V����
    /// </summary>
    public void UpdatePlayerListView()
    {
        //�X�V���邽�߂Ɉ�x���\�����Ă���ꗗ�\�����ׂď���
        foreach (Transform child in playerListView)
        {
            Destroy(child.gameObject);
        }

        foreach(var player in PhotonNetwork.PlayerList)
        {
            //�v���C���[�����������{�^����ǉ����Ă���
            Instantiate(playerInfo,playerListView).GetComponent<PlayerInfoItem>().Set(player.NickName,player.ActorNumber);
        }
    }

    /// <summary>
    /// �����ꗗ�\���X�V����
    /// </summary>
    public void UpdateRoomListView(List<RoomInfo> roomList)
    {
        //�X�V���邽�߂Ɉ�x���\�����Ă���ꗗ�\�����ׂď���
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
        //�X�V���邽�߂Ɉ�x���\�����Ă���ꗗ�\�����ׂď���
        foreach (Transform child in redTeamPlayerViewContent)
        {
            Destroy(child.gameObject);
        }
        //�X�V���邽�߂Ɉ�x���\�����Ă���ꗗ�\�����ׂď���
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
    /// �`�[�����X�g��SetActive�𐧌�
    /// </summary>
    /// <param name="isSet">true��false��</param>
    public void TeamViewSetActive(bool isSet)
    {
        redTeamPlayerView.gameObject.SetActive(isSet);
        blueTeamPlayerView.gameObject.SetActive(isSet);
        redTeamButton.SetActive(isSet);
        blueTeamButton.SetActive(isSet);
    }

    public void StartGame()
    {
        //�����̃z�X�g�Ŗ����Ȃ�Q�[�����J�n�ł��Ȃ�
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
