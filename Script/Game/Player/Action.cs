using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Action : MonoBehaviour
{
    PhotonView PV;
    Rigidbody rb;
    Shooter shooter;
    Animator animator;
    MyTeam myTeam;

    private bool isDeath;

    //プレイヤーのステータス
    [SerializeField] PlayerStatus status;

    //地面があるか確認する用
    [SerializeField] GroundChecker groundChecker;

    //地面についていてかつエイムしているかどうか
    bool isAimGround = false;

    //移動しているかどうか
    bool isMoved = false;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        shooter = GetComponent<Shooter>();
        animator = GetComponent<Animator>();
        myTeam = GetComponent<MyTeam>();
        animator.enabled = false;

        if (PV.IsMine)
        {
            StartCoroutine("currentPosSet");
        }
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine) return;
        if (isDeath) return;

        // 地面についたかつエイムをしている時にisAimGroundをTrueにし、移動速度をAim時のものにする
        if(groundChecker.GetisGround() && shooter.GetIsAiming())
        {
            isAimGround = true;
        }else if (!shooter.GetIsAiming())
        {
            isAimGround = false;
        }

        Movement();
        Drag();
    }

    private void Update()
    {
        if (!PV.IsMine) return;
        if (isDeath) return;

        Jump();

        // マップの下に落下した場合は位置を変え正常な位置に戻す
        if (transform.position.y <= -30)
        {
            PV.RPC("PositionReset", RpcTarget.AllBufferedViaServer);
        }
    }

    /// <summary>
    /// マップの下に落下した時などの詰み状態になった時に呼ぶ
    /// </summary>
    [PunRPC]
    private void PositionReset()
    {
        transform.position = new Vector3(55,1,-8);
    }

    /// <summary>
    /// 一秒ごとに自分の現在地を知らせる
    /// </summary>
    /// <returns></returns>
    IEnumerator currentPosSet()
    {
        while (!isDeath)
        {
            PV.RPC("RPC_CurrentPosSet", RpcTarget.AllBufferedViaServer);

            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// 自分の近くに味方がリスポーンできるように自分のposをセットする
    /// </summary>
    [PunRPC]
    private void RPC_CurrentPosSet()
    {
        GameManager.CurrentPosSet(myTeam.GetMyTeam(), transform.position, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    /// <summary>
    /// 移動する
    /// </summary>
    private void Movement()
    {
        //入力を受け取る
        float x = Input.GetAxis("Vertical");
        float z = Input.GetAxis("Horizontal");

        if(!Input.GetButton("Vertical") && !Input.GetButton("Horizontal"))
        {
            isMoved = false;
        }
        else
        {
            isMoved = true;
        }
        
        float aimingMoveSpeed = 1;

        //エイムしているなら動く速度を変化させる
        if (isAimGround)
        {
             aimingMoveSpeed = shooter.GetAimingMoveSpeed();
        }

        //移動する
        rb.AddForce(transform.forward * x * status.moveSpeed * aimingMoveSpeed);
        rb.AddForce(transform.right   * z  * status.moveSpeed * aimingMoveSpeed);
    }

    /// <summary>
    /// 滑っていかないようにする
    /// </summary>
    private void Drag()
    {
        Vector3 drag_xz = new Vector3(-rb.velocity.x,0, -rb.velocity.z);
        rb.AddForce(drag_xz * status.dragForce);

        Vector3 drag_y = new Vector3(0, -rb.velocity.y, 0);
        rb.AddForce(drag_y * status.dragForce_y);
    }

    /// <summary>
    /// ジャンプする
    /// </summary>
    private void Jump()
    {
        //スペースを押していなければジャンプしない
        if (!Input.GetButtonDown("Jump")) return;
        //地面に立っていなければジャンプできない
        if (!groundChecker.GetisGround()) return;

        //エイムしているときはジャンプ力が変化する
        float aimigJumpForce = shooter.GetAimingJumpFprce();

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //ジャンプする
        rb.AddForce(transform.up * status.jumpForce * aimigJumpForce);

    }
    
    /// <summary>
    /// 死んだら倒れる
    /// </summary>
    public void DeathReaction()
    {
        animator.enabled = true;
        isDeath = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        animator.SetTrigger("death");
    }

    public bool GetIsMoved()
    {
        return isMoved;
    }
}
