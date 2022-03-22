using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/PlayerStatus")]
//プレイヤーのステータスを格納する
public class PlayerStatus : ScriptableObject
{
    //移動スピード
    public float moveSpeed;
    //ジャンプする勢いの強さ
    public float jumpForce;
    //抗力 滑りにくさ
    public float dragForce;
    public float dragForce_y;

    //体力
    public float maxHealth;
    //1秒間に回復する量
    public float healAmount;
    //ダメージを受けてから体力が回復し始めるまでに秒数
    public float healableTime;
}
