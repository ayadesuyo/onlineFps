using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Rendering.PostProcessing;

public class Health : MonoBehaviour, Damagable
{
    PhotonView PV;
    Action action;
    Shooter shooter;
    Sway sway;
    Look look;

    //�v���C���[�̃X�e�[�^�X
    [SerializeField] PlayerStatus status;
    //���̗̑�
    [SerializeField]private float currentHealth;
    //�_���[�W���󂯂��Ƃ���ʂɕ\������摜
    [SerializeField] private GameObject damageEffect;
    //damageEffect��\������̈�
    [SerializeField] private Transform damageEffects;
    //���݂�HP��\������X���C�_�[
    [SerializeField] private Slider hpSlider;

    //�̗͂��񕜂��n�߂鎞�Ԃ��Ǘ����� �O�ɂȂ�����񕜊J�n
    private float healbleTimer;

    //����ł��邩�ǂ���
    private bool isDeath = false;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        action = GetComponent<Action>();
        shooter = GetComponent<Shooter>();
        sway = GetComponent<Sway>();
        look = GetComponent<Look>();
        currentHealth = status.maxHealth;
        healbleTimer = 0;
    }

    private void Update()
    {
        if (!PV.IsMine) return;

        hpSlider.value = (status.maxHealth - currentHealth) / status.maxHealth;

        if (isDeath) return;
        if(currentHealth <= 0)return;

        if (healbleTimer <= 0)
        {
            //�̗͖��^���Ȃ�񕜂��Ȃ�
            if (currentHealth >= status.maxHealth) return;
            PV.RPC("Heal", RpcTarget.All);
        }
        else
        {
            healbleTimer -= Time.deltaTime;
        }
    }

    [PunRPC]
    public void Heal()
    {
        currentHealth = Mathf.Min(currentHealth + status.healAmount * Time.deltaTime, status.maxHealth);
    }

    /// <summary>
    /// �_���[�W���󂯂�
    /// </summary>
    /// <param name="damage">�_���[�W��</param>
    /// <param name="enemyId">�����Ă����G��ID</param>
    /// <param name="enemyName">�����Ă����G�̖��O</param>
    public void TakeDamage(float damage ,int enemyId,float enemyPosX, float enemyPosZ)
    {
        //�Q�[�����I����Ă���Ȃ�_���[�W���󂯂Ȃ�
        if (GameManager.isGameEnd) return;
        PV.RPC("RPC_TakeDamage", RpcTarget.AllBufferedViaServer, damage,enemyId,enemyPosX,enemyPosZ);
    }

    /// <summary>
    /// �_���[�W�𓯊�����
    /// </summary>
    /// <param name="damage">�_���[�W��</param>
    /// <param name="enemyId">�����Ă����G��ID</param>
    /// <param name="enemyName">�����Ă����G�̖��O</param>
    [PunRPC]
    public void RPC_TakeDamage(float damage, int enemyId, float enemyPosX, float enemyPosZ)
    {
        currentHealth -= damage;

        if(PV.IsMine)
        {
            healbleTimer = status.healableTime;
            //look.Damaged();
            //���������񂾂�
            if (currentHealth <= 0)
            {
                GameManager.killDeathUpdate(enemyId, PhotonNetwork.LocalPlayer.ActorNumber);
                Death();
            }
            //�_���[�W�G�t�F�N�g��ǉ�
            UIDamageEffect(enemyPosX,enemyPosZ);
        }
    }

    private void Death()
    {
        isDeath = true;
        PV.RPC("RPC_CurrentPosReset", RpcTarget.AllBufferedViaServer);
        PV.RPC("RPC_LayerDeathSet", RpcTarget.AllBufferedViaServer);

        shooter.enabled = false;
        look.enabled = false;
        action.enabled = false;
        sway.enabled = false;

        action.DeathReaction();
        StartCoroutine("Respawn");
    }

    //4�b��Ƀ��X�|�[��
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(4);
        Cursor.visible = true;
        //�������g��destroy���ăQ�[���}�l�[�W���[���Ă�
        PhotonNetwork.Destroy(gameObject);
        GameManager.PlayerDeath();
    }

    /// <summary>
    /// �_���[�W���󂯂����Ƃ�\���G�t�F�N�g��UI�Ƃ��Đ���
    /// </summary>
    private void UIDamageEffect(float enemyPosX, float enemyPosZ)
    {
        //�G�Ǝ����̍��W�̍������ y�͖���
        Vector3 diff = new Vector3(enemyPosX - transform.position.x, 0, enemyPosZ - transform.position.z).normalized;
        //�����ꂽ�����������悤�Ɋp�x���v�Z
        float radian = Mathf.Atan2(diff.z, diff.x);
        float degree = radian * Mathf.Rad2Deg;
        //effect�𐶐�
        GameObject effect = Instantiate(damageEffect, damageEffects);
        //�ʒu�p�x�𒲐�
        effect.transform.localPosition = new Vector3(100 * diff.x, 100 * diff.z,0);
        effect.transform.localRotation = Quaternion.Euler(new Vector3(0,0,degree + 45));
        //�w��b�o������effect���폜
        effect.GetComponent<DamageEffect>().Set(3);
    }

    /// <summary>
    /// ���񂾂Ƃ��Ƀ��C���[��ύX���e��������Ȃ��悤�ɂ���
    /// </summary>
    [PunRPC]
    private void RPC_LayerDeathSet()
    {
        gameObject.layer = 2;
    }

    [PunRPC]
    private void RPC_CurrentPosReset()
    {
        GameManager.CurrentPosReset(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

}
