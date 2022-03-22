using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Shooter : MonoBehaviour
{
    PhotonView PV;
    Look look;
    Action action;
    MyTeam myTeam;
    MiniMapManager miniMapManager;

    //UI管理用
    [SerializeField] UIManager ui;

    //銃声を再生する
    [SerializeField] AudioSource audioSource;

    [SerializeField] GameObject testBullet;

    //自分が向いている方向に弾を撃つためにカメラを参照
    [SerializeField] Camera mainCamera;
    //視野角(外から変更できるようにしたいからstatic)
    public static float fov = 90;
    //銃を持つ位置
    [SerializeField] Transform[] gunHolder = new Transform[2] { null,null};
    //自分が使う銃
    private GunStatus[] myGunStatus = new GunStatus[2] { null,null};
    private GameObject[] myGuns = new GameObject[2] { null, null };
    //マズルフラッシュ制御
    private MuzzleFlash[] muzzleFlashes = new MuzzleFlash[2] { null, null };
    //銃のアニメーション
    private GunAnimation[] gunAnis = new GunAnimation[2] { null, null };
    //マガジン量
    private int[] myGunCurrentMagazines = new int[2] { 0, 0 };
    //使っている銃の添え字
    private int myUseGunIndex = 0;
    //リコイルパターン用の添え字
    private int recoilIndex = 0;

    //武器変更中かどうか
    private bool isSwaping = false;
    //発射可能かどうか(レート)
    private bool isShootable = true;
    //リロード中かどうか
    private bool isReloading = false;
    //エイム中かどうか
    private bool isAiming = false;
    //覗ききったかどうか
    private bool isAimed = false;

    //スコープがついている銃で覗ききった場合に表示するスコープの画像
    [SerializeField] private GameObject scope;

    //チームに対応した色
    [SerializeField] private Material[] teamColors;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        myTeam = GetComponent<MyTeam>();
        if (PV.IsMine)
        {
            look = GetComponent<Look>();
            action = GetComponent<Action>();
            miniMapManager = GetComponent<MiniMapManager>();
            //自分が使う銃を同期する?
            PV.RPC("RPC_SetGun", RpcTarget.AllBufferedViaServer, GunManager.useGun1Index, 0);
            PV.RPC("RPC_SetGun", RpcTarget.AllBufferedViaServer, GunManager.useGun2Index, 1);
            PV.RPC("RPC_SetMuzzleFlash", RpcTarget.AllBufferedViaServer, 0);
            PV.RPC("RPC_SetMuzzleFlash", RpcTarget.AllBufferedViaServer, 1);
            //自分のRayが当たらないようにレイヤーを変更する
            transform.gameObject.layer = 2;

            //アニメーション制御用のスクリプトを取得
            //gunAnis[0] = myGuns[0].GetComponent<GunAnimation>();
            //gunAnis[1] = myGuns[1].GetComponent<GunAnimation>();
        }
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine) return;
        Aim();
        Shoot();

        UISet();
    }

    private void UISet()
    {
        ui.SetMagagine(myGunCurrentMagazines[myUseGunIndex].ToString());

        if (myGunStatus[myUseGunIndex] != null)
        {
            ui.SetGunName(myGunStatus[myUseGunIndex].gunName);
        }

        ui.SetCrossHair(isAiming);
        ui.SetReload(isReloading);
        ui.SetSwap(isSwaping);
    }

    private void Update()
    {
        if (!PV.IsMine) return;

        Swap();
        Reload();

    }

    /// <summary>
    /// 武器を持ち替える
    /// </summary>
    private void Swap()
    {

        //武器変更中なら武器を変えない
        if (isSwaping) return;

        //数字キーを押すとそのキーの武器に変更する
        if (Input.GetButtonDown("swap1"))
        {
            Debug.Log("swap1");
            //今使っている銃に変えようとしていたら変える必要がないので何もしない
            if (myUseGunIndex == 0) return;
            RateTimerStop();
            StopReloadTimer();
            StartCoroutine(SwapTimer(0));
            return;
        }

        if (Input.GetButtonDown("swap2"))
        {
            Debug.Log("swap2");
            //今使っている銃に変えようとしていたら変える必要がないので何もしない
            if (myUseGunIndex == 1) return;
            RateTimerStop();
            StopReloadTimer();
            StartCoroutine(SwapTimer(1));
            return;
        }

        if (Input.GetButtonDown("swap"))
        {
            Debug.Log("swap");
            RateTimerStop();
            StopReloadTimer();
            StartCoroutine(SwapTimer((myUseGunIndex + 1) % 2));
            return;
        }
    }

    /// <summary>
    /// 武器を持ち替える
    /// </summary>
    /// <param name="index">持ちたい武器の添え字</param>
    private IEnumerator SwapTimer(int index)
    {
        isSwaping = true;
        myGuns[myUseGunIndex].SetActive(false);

        //指定秒経ったら武器を変える UIで残り時間を出すためにfor文を使う
        for(float i=0;i < myGunStatus[index].swapTime;)
        {
            i += Time.deltaTime;
            yield return null;
        }
        //マズルフラッシュを止めておく
        PV.RPC("RPC_MuzzleFlashStop", RpcTarget.AllBufferedViaServer);

        PV.RPC("RPC_ChangeGun", RpcTarget.AllBufferedViaServer, index);

        isSwaping = false;
    }

    /// <summary>
    /// エイムする
    /// </summary>
    private void Aim()
    {
        //右クリックしているなら
        if (Input.GetButton("Aim") && !isReloading && !isSwaping)
        {
            float targetFov = fov / myGunStatus[myUseGunIndex].zoomRate;
            isAiming = true;
            if (gunAnis[myUseGunIndex] != null)
            {
                gunAnis[myUseGunIndex].aim(myGunStatus[myUseGunIndex].aimTime);
            }
            

            //ズームする
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFov, 1 / myGunStatus[myUseGunIndex].aimTime * Time.deltaTime);
            //ターゲットfov誤差+-1で覗ききったとみなす
            if(mainCamera.fieldOfView >= targetFov - 1 && mainCamera.fieldOfView <= targetFov + 1)
            {
                isAimed = true;
            }
            else
            {
                isAimed = false;
            }

        }
        //していないなら
        else
        {
            isAimed = false;
            isAiming = false;
            //ズームをやめる
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, fov,5 * Time.deltaTime);
            if(gunAnis[myUseGunIndex] != null)
            {
                gunAnis[myUseGunIndex].notAim();
            }
            
        }

        if (myGunStatus[myUseGunIndex] == null) return;
        //スコープ付きの銃が覗ききっているならスコープ画像を表示し銃を非表示にする
        if (isAimed && myGunStatus[myUseGunIndex].isScope)
        {
            scope.SetActive(isAimed);
            myGuns[myUseGunIndex].layer = 11;
        }
        else
        {
            scope.SetActive(false);
            myGuns[myUseGunIndex].layer = 6;
        }
    }

    /// <summary>
    /// 撃つ rayを飛ばしてプレイヤーに当たればダメージを与える
    /// </summary>
    void Shoot()
    {
        //左クリックしていなければ撃たない
        if (!Input.GetButton("Shot"))
        {
            //RateTimerを止める
            RateTimerStop();
            //リコイルリセット
            recoilIndex = 0;
            return;
        }
        //マガジンに弾が入っていないなら撃てない
        if (myGunCurrentMagazines[myUseGunIndex] <= 0) return;
        //リロード中なら撃てない
        if (isReloading) return;
        if (isSwaping) return;

        //まだ撃てないなら
        if (!isShootable) return;

        //タイマースタート
        StartCoroutine("RateTimer");

        //銃声を再生する
        PV.RPC("RPC_ShotAudioPlay", RpcTarget.AllBufferedViaServer);

        //マズルフラッシュを起こす
        PV.RPC("RPC_MuzzleFlashPlay", RpcTarget.AllBufferedViaServer);

        //マップに赤点を出す
        miniMapManager.RedDotPop();

        //弾を消費
        myGunCurrentMagazines[myUseGunIndex]--;

        //反動を発生させる
        look.Recoil(myGunStatus[myUseGunIndex].recoilPattern[recoilIndex]);
        //添え字更新
        recoilIndex = (recoilIndex + 1) % myGunStatus[myUseGunIndex].recoilPattern.Length;

        //弾のばらつきを設定
        Vector3 scatter = Vector3.zero;
        if (isAimed)
        {
            //スコープ付きの銃を移動しながら打とうとしたときは弾がばらつくようにする
            if(myGunStatus[myUseGunIndex].isScope && action.GetIsMoved())
            {
                scatter = new Vector3(Random.Range(-myGunStatus[myUseGunIndex].scatter, myGunStatus[myUseGunIndex].scatter),
                                      Random.Range(-myGunStatus[myUseGunIndex].scatter, myGunStatus[myUseGunIndex].scatter),
                                      0);
            }
            else
            {
                scatter = new Vector3(Random.Range(-myGunStatus[myUseGunIndex].aimingScatter, myGunStatus[myUseGunIndex].aimingScatter),
                                      Random.Range(-myGunStatus[myUseGunIndex].aimingScatter, myGunStatus[myUseGunIndex].aimingScatter),
                                      0);
            }
        }
        else
        {
            scatter = new Vector3(Random.Range(-myGunStatus[myUseGunIndex].scatter, myGunStatus[myUseGunIndex].scatter),
                                  Random.Range(-myGunStatus[myUseGunIndex].scatter, myGunStatus[myUseGunIndex].scatter),
                                  0);
        }

        //カメラが向いている方向にrayを作成
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f) + scatter);
        ray.origin = mainCamera.transform.position;
        RaycastHit hit;
        //rayが当たっていたら
        if(Physics.Raycast(ray,out hit))
        {
            //rayが当たったオブジェクトがダメージを与えることができるなら
            Damagable damagable = hit.transform.gameObject.GetComponent<Damagable>();
            if (damagable != null)
            {
                bool applyDamage = false;
                //今のゲームルールがFFAならダメージを与える
                if(RuleManager.currentRule == "FFA")
                {
                    applyDamage = true;
                }
                else
                {
                    //チームメイトでなければダメージを与える
                    if(hit.transform.gameObject.GetComponent<MyTeam>().GetMyTeam() != myTeam.GetMyTeam())
                    {
                        applyDamage = true;
                    }
                }
                //ダメージを与える
                if (applyDamage)
                {
                    Health health = hit.transform.gameObject.GetComponent<Health>();
                    //ダメージを与えることで体力が0になるならキルした時の処理を行う
                    if(health.GetCurrentHealth() - myGunStatus[myUseGunIndex].damage <= 0)
                    {
                        KillEffect();
                    }
                    health.TakeDamage(myGunStatus[myUseGunIndex].damage, PhotonNetwork.LocalPlayer.ActorNumber, transform.position.x, transform.position.z);

                    ui.SetHitMarker();
                }
            }
            else
            {
                //playerに当たらなかったら当たった場所に火花を散らす
                GameObject fire = PhotonNetwork.Instantiate("BImpact", hit.point, Quaternion.identity);
                fire.transform.LookAt(transform);
            }

            
        }
    }

    /// <summary>
    /// 銃声を再生する
    /// </summary>
    [PunRPC]
    private void RPC_ShotAudioPlay()
    {
        //銃声を再生
        audioSource.PlayOneShot(myGunStatus[myUseGunIndex].shotClip);
    }

    /// <summary>
    /// 発射間隔を管理
    /// </summary>
    /// <returns></returns>
    private IEnumerator RateTimer()
    {
        isShootable = false;

        //設定秒数たったら撃てるようにする
        yield return new WaitForSeconds(60.0f / (float)myGunStatus[myUseGunIndex].fireRate);

        isShootable = true;
    }

    /// <summary>
    /// RateTimerを止める
    /// </summary>
    private void RateTimerStop()
    {
        //タイマー起動中ならストップ
        if (!isShootable)
        {
            StopCoroutine("RateTimer");
            isShootable = true;
        }
    }

    /// <summary>
    /// リロードする
    /// </summary>
    private void Reload()
    {
        //Rキーを押していなければリロードしない
        if (!Input.GetButtonDown("Reload")) return;
        //武器変更中ならリロードしない
        if (isSwaping) return;
        //マガジンが満タンだったらリロードしない
        if (myGunCurrentMagazines[myUseGunIndex] == myGunStatus[myUseGunIndex].maxMagazineSize) return;
        //リロード中ならリロードしない
        if (isReloading) return;

        //リロード開始
        StartCoroutine("ReloadTimer");
    }

    /// <summary>
    /// リロードする
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadTimer()
    {
        isReloading = true;

        //リロード音を再生
        audioSource.PlayOneShot(myGunStatus[myUseGunIndex].reloadClip);

        //指定秒かけてリロードする
        yield return new WaitForSeconds(myGunStatus[myUseGunIndex].reloadTime);

        //弾を補充
        myGunCurrentMagazines[myUseGunIndex] = myGunStatus[myUseGunIndex].maxMagazineSize;
        isReloading = false;
    }

    private void StopReloadTimer()
    {
        if (isReloading)
        {
            StopCoroutine("ReloadTimer");
            isReloading = false;
        }
    }

    /// <summary>
    /// 銃を生成し持つ
    /// </summary>
    /// <param name="useGunIndex">使う武器の添え字</param>
    /// <param name="index">何番目のホルダーに持つか</param>
    [PunRPC]
    void RPC_SetGun(int useGunIndex, int index)
    {
        //一旦持ってる銃を削除
        foreach(Transform gun in gunHolder[index])
        {
            Destroy(gun.gameObject);
        }

        //使う銃のステータスを取得
        myGunStatus[index] = GunManager.guns[useGunIndex];

        //銃を生成
        myGuns[index] = Instantiate(myGunStatus[index].gun, Vector3.zero, Quaternion.identity);
        //子にしてカメラについてくるようにする
        myGuns[index].transform.parent = gunHolder[index];
        
        //位置角度を設定
        myGuns[index].transform.localPosition = myGunStatus[index].gun.transform.position;
        myGuns[index].transform.localRotation = myGunStatus[index].gun.transform.rotation;

        //マガジン量を設定
        myGunCurrentMagazines[index] = myGunStatus[index].maxMagazineSize;


        //実行順の関係でまだPhotonViewを取得していない場合がある
        if (PV != null)
        {
            if (PV.IsMine)
            {
                myGuns[index].layer = 6;
                //ここで最初にエラーがおこるが問題は無いっぽい？
                //アニメーション制御用のスクリプトを取得
                gunAnis[0] = myGuns[0].GetComponent<GunAnimation>();
                gunAnis[1] = myGuns[1].GetComponent<GunAnimation>();
            }
        }

        //使う銃だけ見えるようにする
        //何故かstart時ここでnullエラーが起きます
        for(int i=0;i < myGuns.Length; i++)
        {
            if(myGuns[i] != null)
            {
                myGuns[i].SetActive(false);
            }
            
        }
        myGuns[myUseGunIndex].SetActive(true);
    }

    /// <summary>
    /// 使う銃を変える
    /// </summary>
    /// <param name="changeGunIndex">使う銃の添え字</param>
    [PunRPC]
    void RPC_ChangeGun(int changeGunIndex)
    {
        myUseGunIndex = changeGunIndex;

        //マズルフラッシュを止めておく
        PV.RPC("RPC_MuzzleFlashStop",RpcTarget.AllBufferedViaServer);

        //使う銃だけ見えるようにする
        foreach (GameObject gun in myGuns)
        {
            gun.SetActive(false);
        }
        myGuns[myUseGunIndex].SetActive(true);
    }

    /// <summary>
    /// マズルフラッシュ制御用のスクリプトを取得
    /// </summary>
    /// <param name="index">武器の添え字</param>
    [PunRPC]
    void RPC_SetMuzzleFlash(int index)
    {
        muzzleFlashes[index] = myGuns[index].GetComponent<MuzzleFlash>();
        if (PV.IsMine)
        {
            muzzleFlashes[index].layerSet();
        }
    }

    [PunRPC]
    void RPC_MuzzleFlashPlay()
    {
        muzzleFlashes[myUseGunIndex].muzzlePlay();
    }

    [PunRPC]
    void RPC_MuzzleFlashStop()
    {
        muzzleFlashes[myUseGunIndex].muzzleStop();
    }


    public void KillEffect()
    {
        ui.SetKillMarker();
    }

    public bool GetIsAiming()
    {
        return isAiming;
    }

    public float GetAimingMoveSpeed()
    {
        if (isAiming) return myGunStatus[myUseGunIndex].aimingMoveSpeed;
        //エイムしていないなら移動速度は変わらないから1を返す;
        else return 1;
    }

    public float GetAimingJumpFprce()
    {
        if (isAiming) return myGunStatus[myUseGunIndex].aimingJumpForce;
        //エイムしていないならジャンプ力は変わらないから1を返す;
        else return 1;
    }

    public float GetZoomRate()
    {
        return myGunStatus[myUseGunIndex].zoomRate;
    }
}
