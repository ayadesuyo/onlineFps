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

    //�v���C���[�̃X�e�[�^�X
    [SerializeField] PlayerStatus status;

    //�n�ʂ����邩�m�F����p
    [SerializeField] GroundChecker groundChecker;

    //�n�ʂɂ��Ă��Ă��G�C�����Ă��邩�ǂ���
    bool isAimGround = false;

    //�ړ����Ă��邩�ǂ���
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

        // �n�ʂɂ������G�C�������Ă��鎞��isAimGround��True�ɂ��A�ړ����x��Aim���̂��̂ɂ���
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

        // �}�b�v�̉��ɗ��������ꍇ�͈ʒu��ς�����Ȉʒu�ɖ߂�
        if (transform.position.y <= -30)
        {
            PV.RPC("PositionReset", RpcTarget.AllBufferedViaServer);
        }
    }

    /// <summary>
    /// �}�b�v�̉��ɗ����������Ȃǂ̋l�ݏ�ԂɂȂ������ɌĂ�
    /// </summary>
    [PunRPC]
    private void PositionReset()
    {
        transform.position = new Vector3(55,1,-8);
    }

    /// <summary>
    /// ��b���ƂɎ����̌��ݒn��m�点��
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
    /// �����̋߂��ɖ��������X�|�[���ł���悤�Ɏ�����pos���Z�b�g����
    /// </summary>
    [PunRPC]
    private void RPC_CurrentPosSet()
    {
        GameManager.CurrentPosSet(myTeam.GetMyTeam(), transform.position, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    /// <summary>
    /// �ړ�����
    /// </summary>
    private void Movement()
    {
        //���͂��󂯎��
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

        //�G�C�����Ă���Ȃ瓮�����x��ω�������
        if (isAimGround)
        {
             aimingMoveSpeed = shooter.GetAimingMoveSpeed();
        }

        //�ړ�����
        rb.AddForce(transform.forward * x * status.moveSpeed * aimingMoveSpeed);
        rb.AddForce(transform.right   * z  * status.moveSpeed * aimingMoveSpeed);
    }

    /// <summary>
    /// �����Ă����Ȃ��悤�ɂ���
    /// </summary>
    private void Drag()
    {
        Vector3 drag_xz = new Vector3(-rb.velocity.x,0, -rb.velocity.z);
        rb.AddForce(drag_xz * status.dragForce);

        Vector3 drag_y = new Vector3(0, -rb.velocity.y, 0);
        rb.AddForce(drag_y * status.dragForce_y);
    }

    /// <summary>
    /// �W�����v����
    /// </summary>
    private void Jump()
    {
        //�X�y�[�X�������Ă��Ȃ���΃W�����v���Ȃ�
        if (!Input.GetButtonDown("Jump")) return;
        //�n�ʂɗ����Ă��Ȃ���΃W�����v�ł��Ȃ�
        if (!groundChecker.GetisGround()) return;

        //�G�C�����Ă���Ƃ��̓W�����v�͂��ω�����
        float aimigJumpForce = shooter.GetAimingJumpFprce();

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //�W�����v����
        rb.AddForce(transform.up * status.jumpForce * aimigJumpForce);

    }
    
    /// <summary>
    /// ���񂾂�|���
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
