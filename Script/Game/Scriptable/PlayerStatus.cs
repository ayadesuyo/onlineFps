using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/PlayerStatus")]
//�v���C���[�̃X�e�[�^�X���i�[����
public class PlayerStatus : ScriptableObject
{
    //�ړ��X�s�[�h
    public float moveSpeed;
    //�W�����v���鐨���̋���
    public float jumpForce;
    //�R�� ����ɂ���
    public float dragForce;
    public float dragForce_y;

    //�̗�
    public float maxHealth;
    //1�b�Ԃɉ񕜂����
    public float healAmount;
    //�_���[�W���󂯂Ă���̗͂��񕜂��n�߂�܂łɕb��
    public float healableTime;
}
