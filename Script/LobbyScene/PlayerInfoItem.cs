using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoItem : MonoBehaviour
{
    [SerializeField] Text playerName;
    //このプレイヤーのactorNumから１引いた数を保持しておく
    public int actorNum;

    public void Set(string _playerName, int _actorNum)
    {
        playerName.text = _playerName;
        actorNum = _actorNum-1;
    }
}
