using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoItem : MonoBehaviour
{
    [SerializeField] Text playerName;
    //���̃v���C���[��actorNum����P����������ێ����Ă���
    public int actorNum;

    public void Set(string _playerName, int _actorNum)
    {
        playerName.text = _playerName;
        actorNum = _actorNum-1;
    }
}
