using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamJoinButton : MonoBehaviour
{
    [SerializeField] TeamManager.team thisTeam;

    public void ChangeTeam()
    {
        //このオブジェクトの保持しているチームに入る
        TeamManager.Instance.JoinTeam(thisTeam.ToString());
    }
}
