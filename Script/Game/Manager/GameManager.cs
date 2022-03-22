using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviourPunCallbacks
{
    bool getComponent = false;

    static PhotonView PV;
    public static UIManager ui;

    //playerの戦績 id-1がキー
    public static int[] kills;
    public static int[] deaths;
    public static string[] playerNames;
    public static string[] teams;

    //redチームの人の現在地
    public static Vector3[] redTeammatePos;
    //blueチームの人の現在地
    public static Vector3[] blueTeammatePos;

    //ゲームが終わったかどうか
    public static bool isGameEnd;

    //UI関係
    [SerializeField] GameObject setGunUI;
    static GameObject _setGunUI;

    [SerializeField] GameObject tdmScoreBoard;
    static GameObject _tdmScoreBoard;
    [SerializeField] GameObject playerScoreItem;
    static GameObject _playerScoreItem;
    [SerializeField] GameObject redTeamScore;
    static GameObject _redTeamScore;
    [SerializeField] Text redTeamTotalScore;
    static Text _redTeamTotalScore;
    [SerializeField] GameObject blueTeamScore;
    static GameObject _blueTeamScore;
    [SerializeField] Text blueTeamTotalScore;
    static Text _blueTeamTotalScore;
    [SerializeField] Text winTeam;
    static Text _winTeam;
    [SerializeField] GameObject ffaScoreBoard;
    static GameObject _ffaScoreBoard;
    [SerializeField] GameObject ffaScores;
    static GameObject _ffaScores;
    [SerializeField] Text ffaWinner;
    static Text _ffaWinner;
    private void Start()
    {
        if (!getComponent)
        {
            getComponent = true;
            isGameEnd = false;

            PV = GetComponent<PhotonView>();
            var players = PhotonNetwork.PlayerList;
            kills = new int[players.Length];
            deaths = new int[players.Length];
            playerNames = new string[players.Length];
            teams = new string[players.Length];
            redTeammatePos = new Vector3[players.Length];
            blueTeammatePos = new Vector3[players.Length];
            for(int i=0;i < players.Length; i++)
            {
                playerNames[i] = players[i].NickName;
                teams[i] = players[i].GetPhotonTeam().Name;
            }
            _setGunUI = setGunUI;
            _playerScoreItem = playerScoreItem;
            _redTeamScore = redTeamScore;
            _redTeamTotalScore = redTeamTotalScore;
            _blueTeamScore = blueTeamScore;
            _blueTeamTotalScore = blueTeamTotalScore;
            _winTeam = winTeam;
            _tdmScoreBoard = tdmScoreBoard;
            _ffaScoreBoard = ffaScoreBoard;
            _ffaScores = ffaScores;
            _ffaWinner = ffaWinner;
        }
        
    }

    public void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (isGameEnd) return;

        if (RuleManager.currentRule == "TDM")
        {
            int redTeamTotalKills = 0;
            int blueTeamTotalKills = 0;
            for (int i = 0; i < kills.Length; i++)
            {
                if (teams[i] == TeamManager.team.Red.ToString())
                {
                    redTeamTotalKills += kills[i];
                }
                else
                {
                    blueTeamTotalKills += kills[i];
                }
            }

            if (redTeamTotalKills >= 20 || blueTeamTotalKills >= 20)
            {
                PV.RPC("TDMGameFinish", RpcTarget.AllBufferedViaServer, redTeamTotalKills, blueTeamTotalKills);
            }
        }
        else if(RuleManager.currentRule == "FFA")
        {
            for(int i = 0; i < kills.Length; i++)
            {
                if(kills[i] >= 10)
                {
                    PV.RPC("FFAGameFinish", RpcTarget.AllBufferedViaServer,i);
                }
            }
        }
    }

    public static void SetUI(UIManager _ui)
    {
        ui = _ui;
        if(RuleManager.currentRule == "TDM")
        {
            TDMScoreUpdate();
        }else if(RuleManager.currentRule == "FFA")
        {
            FFAScoreUpdate();
        }
        
    }

    public static void PlayerSpawn()
    {
        //使う銃が設定されてなければリスポーンしない
        if (GunManager.useGun1Index == -1) return;
        if (GunManager.useGun2Index == -1) return;
        _setGunUI.SetActive(false);
        PhotonNetwork.Instantiate("PlayerManager", Vector3.zero, Quaternion.identity);
    }

    public static void PlayerDeath()
    {
        _setGunUI.SetActive(true);
    }

    public static void killDeathUpdate(int killId,int deathId)
    {
        PV.RPC("RPC_killDeathUpdate", RpcTarget.All,killId, deathId);
    }

    [PunRPC]
    public void RPC_killDeathUpdate(int killId, int deathId)
    {
        kills[killId - 1]++;
        deaths[deathId - 1]++;
        if(RuleManager.currentRule == "TDM")
        {
            TDMScoreUpdate();
        }else if(RuleManager.currentRule == "FFA")
        {
            FFAScoreUpdate();
        }
    }

    public static void TDMScoreUpdate()
    {
        if (ui == null) return;
        ui.SetTeamScore(playerNames, kills, deaths, teams);
    }

    public static void FFAScoreUpdate()
    {
        if (ui == null) return;
        ui.SetFFAScore(playerNames, kills, deaths);
    }

    [PunRPC]
    public void TDMGameFinish(int redTotalKill,int blueTotalKill)
    {
        if (isGameEnd) return;
        isGameEnd = true;

        foreach (Transform item in _redTeamScore.transform)
        {
            Destroy(item);
        }
        foreach (Transform item in _blueTeamScore.transform)
        {
            Destroy(item);
        }

        for (int i=0;i < playerNames.Length; i++)
        {
            GameObject item;
            if(teams[i] == TeamManager.team.Red.ToString())
            {
                item = Instantiate(_playerScoreItem, _redTeamScore.transform);
            }
            else
            {
                item = Instantiate(_playerScoreItem, _blueTeamScore.transform);
            }
            item.GetComponent<PLayerScoreItemManager>().SetTDMText(playerNames[i], kills[i], deaths[i], teams[i]);
        }
        _redTeamTotalScore.text = redTotalKill.ToString();
        _blueTeamTotalScore.text = blueTotalKill.ToString();

        if(redTotalKill > blueTotalKill)
        {
            _winTeam.text = TeamManager.team.Red.ToString() + "チームの勝利";
        }else if(redTotalKill < blueTotalKill)
        {
            _winTeam.text = TeamManager.team.Blue.ToString() + "チームの勝利";
        }
        else//引き分けありえないはずだけど一応
        {
            _winTeam.text = "引き分け";
        }
        Debug.Log("Finish");
        _setGunUI.SetActive(false);
        _tdmScoreBoard.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine("LobbySceneLoad");
        }
    }

    [PunRPC]
    public void FFAGameFinish(int winnerIndex)
    {
        if (isGameEnd) return;
        isGameEnd = true;

        foreach (Transform item in _ffaScores.transform)
        {
            Destroy(item);
        }

        for (int i = 0; i < playerNames.Length; i++)
        {
            GameObject item = Instantiate(_playerScoreItem, _ffaScores.transform);
            item.GetComponent<PLayerScoreItemManager>().SetFFAText(playerNames[i], kills[i], deaths[i]);
        }

        _ffaWinner.text = playerNames[winnerIndex] + " の勝ち!";

        _setGunUI.SetActive(false);
        _ffaScoreBoard.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine("LobbySceneLoad");
        }
    }

    public static void CurrentPosSet(string team, Vector3 pos, int id)
    {
        // 自身のIDに対応した添え字に自分の位置を記録する
        if(team == TeamManager.team.Red.ToString())
        {
            redTeammatePos[id - 1] = pos;
        }
        else
        {
            blueTeammatePos[id - 1] = pos;
        }
    }

    public static void CurrentPosReset(int id)
    {
        redTeammatePos[id - 1] = Vector3.zero;
        blueTeammatePos[id - 1] = Vector3.zero;
    }

    private IEnumerator LobbySceneLoad()
    {
        yield return new WaitForSeconds(5);
        PhotonNetwork.LoadLevel(0);
    }


}
