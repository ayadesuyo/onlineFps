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

    //プレイヤーのステータス
    [SerializeField] PlayerStatus status;
    //今の体力
    [SerializeField]private float currentHealth;
    //ダメージを受けたとき画面に表示する画像
    [SerializeField] private GameObject damageEffect;
    //damageEffectを表示する領域
    [SerializeField] private Transform damageEffects;
    //現在のHPを表示するスライダー
    [SerializeField] private Slider hpSlider;

    //体力を回復し始める時間を管理する ０になったら回復開始
    private float healbleTimer;

    //死んでいるかどうか
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
            //体力満タンなら回復しない
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
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <param name="enemyId">撃ってきた敵のID</param>
    /// <param name="enemyName">撃ってきた敵の名前</param>
    public void TakeDamage(float damage ,int enemyId,float enemyPosX, float enemyPosZ)
    {
        //ゲームが終わっているならダメージを受けない
        if (GameManager.isGameEnd) return;
        PV.RPC("RPC_TakeDamage", RpcTarget.AllBufferedViaServer, damage,enemyId,enemyPosX,enemyPosZ);
    }

    /// <summary>
    /// ダメージを同期する
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <param name="enemyId">撃ってきた敵のID</param>
    /// <param name="enemyName">撃ってきた敵の名前</param>
    [PunRPC]
    public void RPC_TakeDamage(float damage, int enemyId, float enemyPosX, float enemyPosZ)
    {
        currentHealth -= damage;

        if(PV.IsMine)
        {
            healbleTimer = status.healableTime;
            //look.Damaged();
            //自分が死んだら
            if (currentHealth <= 0)
            {
                GameManager.killDeathUpdate(enemyId, PhotonNetwork.LocalPlayer.ActorNumber);
                Death();
            }
            //ダメージエフェクトを追加
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

    //4秒後にリスポーン
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(4);
        Cursor.visible = true;
        //自分自身をdestroyしてゲームマネージャーを呼ぶ
        PhotonNetwork.Destroy(gameObject);
        GameManager.PlayerDeath();
    }

    /// <summary>
    /// ダメージを受けたことを表すエフェクトをUIとして生成
    /// </summary>
    private void UIDamageEffect(float enemyPosX, float enemyPosZ)
    {
        //敵と自分の座標の差を取る yは無視
        Vector3 diff = new Vector3(enemyPosX - transform.position.x, 0, enemyPosZ - transform.position.z).normalized;
        //撃たれた方向を向くように角度を計算
        float radian = Mathf.Atan2(diff.z, diff.x);
        float degree = radian * Mathf.Rad2Deg;
        //effectを生成
        GameObject effect = Instantiate(damageEffect, damageEffects);
        //位置角度を調整
        effect.transform.localPosition = new Vector3(100 * diff.x, 100 * diff.z,0);
        effect.transform.localRotation = Quaternion.Euler(new Vector3(0,0,degree + 45));
        //指定秒経ったらeffectを削除
        effect.GetComponent<DamageEffect>().Set(3);
    }

    /// <summary>
    /// 死んだときにレイヤーを変更し弾が当たらないようにする
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
