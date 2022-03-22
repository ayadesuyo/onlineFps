using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    PhotonView PV;
    Action action;
    Shooter shooter;
    Look look;
    Sway sway;

    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject confUI;
    [SerializeField] private Text magazineText;
    [SerializeField] private Text gunNameText;
    [SerializeField] private GameObject hitMarker;
    [SerializeField] private GameObject killMarker;
    [SerializeField] private GameObject crossHair;
    [SerializeField] private GameObject dot;
    [SerializeField] private GameObject reloadText;
    [SerializeField] private GameObject swapText;
    private bool isHitMarker = false;
    private bool isKillMarker = false;

    [SerializeField] private GameObject tdmScoreBorad;
    [SerializeField] private GameObject playerScoreItem;
    [SerializeField] private Transform redTeamScoreBoard;
    [SerializeField] private Transform blueTeamScoreBoard;
    [SerializeField] private Text redTotalKillText;
    [SerializeField] private Text blueTotalKillText;
    [SerializeField] private GameObject ffaScoreBoard;
    [SerializeField] private GameObject scoreBoard;
    private bool isScorePopup = false;
    private bool isPause = false;
    private bool isConf = false;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        Cursor.visible = false;
        if (!PV.IsMine)
        {
            Destroy(ui);
            Destroy(this);
        }
        else
        {
            action = GetComponent<Action>();
            shooter = GetComponent<Shooter>();
            look = GetComponent<Look>();
            sway = GetComponent<Sway>();
        }
    }

    private void Update()
    {
        if (!PV.IsMine) return;

        if (Input.GetButtonDown("tab"))
        {
            isScorePopup = !isScorePopup;
            inGameUI.SetActive(!isScorePopup);
            if(RuleManager.currentRule == "TDM")
            {
                tdmScoreBorad.SetActive(isScorePopup);
            }else if(RuleManager.currentRule == "FFA")
            {
                ffaScoreBoard.SetActive(isScorePopup);
            }
        }

        if (!isScorePopup && !isConf && Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = !isPause;
            pauseUI.SetActive(isPause);
            Cursor.visible = isPause;

            shooter.enabled = !isPause;
            look.enabled = !isPause;
            sway.enabled = !isPause;
        }
    }

    public void ConfPop()
    {
        isConf = true;
        confUI.SetActive(isConf);
    }

    public void ConfClose()
    {
        isConf = false;
        confUI.SetActive(isConf);
    }

    public void SetMagagine(string _text)
    {
        magazineText.text = _text;
    }

    public void SetGunName(string _text)
    {
        gunNameText.text = _text;
    }

    public void SetCrossHair(bool isAiming)
    {
        if (isAiming)
        {
            crossHair.SetActive(false);
            dot.SetActive(true);
        }
        else
        {
            crossHair.SetActive(true);
            dot.SetActive(false);
        }
    }

    public void SetReload(bool flag)
    {
        reloadText.SetActive(flag);
    }

    public void SetSwap(bool flag)
    {
        swapText.SetActive(flag);
    }

    private IEnumerator HitMarkerTimer()
    {
        hitMarker.SetActive(true);
        isHitMarker = true;

        yield return new WaitForSeconds(0.5f);

        hitMarker.SetActive(false);
        isHitMarker = false;
    }
    
    public void SetHitMarker()
    {
        if (isHitMarker)
        {
            StopCoroutine("HitMarkerTimer");
        }
        StartCoroutine("HitMarkerTimer");
    }

    private IEnumerator KillMarkerTimer()
    {
        killMarker.SetActive(true);
        isKillMarker = true;

        Debug.Log("SetKillMarkerTimer");

        yield return new WaitForSeconds(1f);

        killMarker.SetActive(false);
        isKillMarker = false;
    }

    public void SetKillMarker()
    {
        if (isKillMarker)
        {
            StopCoroutine("KillMarkerTimer");
        }
        Debug.Log("SetKillMarker");
        StartCoroutine("KillMarkerTimer");
    }

    public void SetTeamScore(string[] players,int[] kills, int[] deaths, string[] teams)
    {
        foreach(Transform item in redTeamScoreBoard)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in blueTeamScoreBoard)
        {
            Destroy(item.gameObject);
        }

        int redTotalKill = 0;
        int blueTotalKill = 0;

        for(int i= 0;i < players.Length; i++)
        {
            GameObject item;
            if (teams[i] == TeamManager.team.Red.ToString()) 
            {
                redTotalKill += kills[i];
                item = Instantiate(playerScoreItem, redTeamScoreBoard);
            }
            else
            {
                blueTotalKill += kills[i];
                item = Instantiate(playerScoreItem, blueTeamScoreBoard);
            }
            redTotalKillText.text = redTotalKill.ToString();
            blueTotalKillText.text = blueTotalKill.ToString();
            item.GetComponent<PLayerScoreItemManager>().SetTDMText(players[i], kills[i], deaths[i], teams[i]);
        }
    }

    public void SetFFAScore(string[] players, int[] kills, int[] deaths)
    {
        foreach (Transform item in scoreBoard.transform)
        {
            Destroy(item.gameObject);
        }
        for (int i = 0; i < players.Length; i++)
        {
            GameObject item = Instantiate(playerScoreItem, scoreBoard.transform);
            item.GetComponent<PLayerScoreItemManager>().SetFFAText(players[i], kills[i], deaths[i]);
        }
    }

    /// <summary>
    /// ÉzÅ[ÉÄâÊñ Ç…ñﬂÇÈ
    /// </summary>
    public void HomeDisplayBack()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
