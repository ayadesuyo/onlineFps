using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Game/Gun")]
public class GunStatus : ScriptableObject
{
    //銃のモデル
    public GameObject gun;
    //銃の名前
    public string gunName;
    //銃声
    public AudioClip shotClip;
    //リロード音
    public AudioClip reloadClip;

    //ダメージ
    public float damage;
    //この銃を持つまでにかかる時間
    public float swapTime;
    //マガジンサイズ
    public int maxMagazineSize;
    //リロードにかかる秒数
    public float reloadTime;
    //発射レート 1分間に撃てる弾の数
    public int fireRate;
    //非エイム時の弾のばらつき
    public float scatter;
    //エイム時の弾のばらつき
    public float aimingScatter;

    //スコープがあるかどうか
    public bool isScope;
    //ズームしきるまでにかかる時間 エイムするのにかかる時間
    public float aimTime;
    //エイム時のズーム倍率
    public float zoomRate;
    //エイム時の移動速度 1だと変わらず0だと動けなくなる
    public float aimingMoveSpeed;
    //エイム時のジャンプ力 1だと変わらず0だとジャンプしなくなる
    public float aimingJumpForce;

    //リコイルパターン
    public Vector2[] recoilPattern;
}
