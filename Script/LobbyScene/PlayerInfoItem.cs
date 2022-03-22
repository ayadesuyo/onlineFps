using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoItem : MonoBehaviour
{
    [SerializeField] Text playerName;
    //‚±‚ÌƒvƒŒƒCƒ„[‚ÌactorNum‚©‚ç‚Pˆø‚¢‚½”‚ğ•Û‚µ‚Ä‚¨‚­
    public int actorNum;

    public void Set(string _playerName, int _actorNum)
    {
        playerName.text = _playerName;
        actorNum = _actorNum-1;
    }
}
