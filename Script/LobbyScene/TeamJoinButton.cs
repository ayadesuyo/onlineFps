using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamJoinButton : MonoBehaviour
{
    [SerializeField] TeamManager.team thisTeam;

    public void ChangeTeam()
    {
        //���̃I�u�W�F�N�g�̕ێ����Ă���`�[���ɓ���
        TeamManager.Instance.JoinTeam(thisTeam.ToString());
    }
}
