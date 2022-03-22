using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Game/Gun")]
public class GunStatus : ScriptableObject
{
    //�e�̃��f��
    public GameObject gun;
    //�e�̖��O
    public string gunName;
    //�e��
    public AudioClip shotClip;
    //�����[�h��
    public AudioClip reloadClip;

    //�_���[�W
    public float damage;
    //���̏e�����܂łɂ����鎞��
    public float swapTime;
    //�}�K�W���T�C�Y
    public int maxMagazineSize;
    //�����[�h�ɂ�����b��
    public float reloadTime;
    //���˃��[�g 1���ԂɌ��Ă�e�̐�
    public int fireRate;
    //��G�C�����̒e�̂΂��
    public float scatter;
    //�G�C�����̒e�̂΂��
    public float aimingScatter;

    //�X�R�[�v�����邩�ǂ���
    public bool isScope;
    //�Y�[��������܂łɂ����鎞�� �G�C������̂ɂ����鎞��
    public float aimTime;
    //�G�C�����̃Y�[���{��
    public float zoomRate;
    //�G�C�����̈ړ����x 1���ƕς�炸0���Ɠ����Ȃ��Ȃ�
    public float aimingMoveSpeed;
    //�G�C�����̃W�����v�� 1���ƕς�炸0���ƃW�����v���Ȃ��Ȃ�
    public float aimingJumpForce;

    //���R�C���p�^�[��
    public Vector2[] recoilPattern;
}
