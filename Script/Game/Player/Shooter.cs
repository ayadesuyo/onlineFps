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

    //UI�Ǘ��p
    [SerializeField] UIManager ui;

    //�e�����Đ�����
    [SerializeField] AudioSource audioSource;

    [SerializeField] GameObject testBullet;

    //�����������Ă�������ɒe�������߂ɃJ�������Q��
    [SerializeField] Camera mainCamera;
    //����p(�O����ύX�ł���悤�ɂ���������static)
    public static float fov = 90;
    //�e�����ʒu
    [SerializeField] Transform[] gunHolder = new Transform[2] { null,null};
    //�������g���e
    private GunStatus[] myGunStatus = new GunStatus[2] { null,null};
    private GameObject[] myGuns = new GameObject[2] { null, null };
    //�}�Y���t���b�V������
    private MuzzleFlash[] muzzleFlashes = new MuzzleFlash[2] { null, null };
    //�e�̃A�j���[�V����
    private GunAnimation[] gunAnis = new GunAnimation[2] { null, null };
    //�}�K�W����
    private int[] myGunCurrentMagazines = new int[2] { 0, 0 };
    //�g���Ă���e�̓Y����
    private int myUseGunIndex = 0;
    //���R�C���p�^�[���p�̓Y����
    private int recoilIndex = 0;

    //����ύX�����ǂ���
    private bool isSwaping = false;
    //���ˉ\���ǂ���(���[�g)
    private bool isShootable = true;
    //�����[�h�����ǂ���
    private bool isReloading = false;
    //�G�C�������ǂ���
    private bool isAiming = false;
    //�`�����������ǂ���
    private bool isAimed = false;

    //�X�R�[�v�����Ă���e�Ŕ`���������ꍇ�ɕ\������X�R�[�v�̉摜
    [SerializeField] private GameObject scope;

    //�`�[���ɑΉ������F
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
            //�������g���e�𓯊�����?
            PV.RPC("RPC_SetGun", RpcTarget.AllBufferedViaServer, GunManager.useGun1Index, 0);
            PV.RPC("RPC_SetGun", RpcTarget.AllBufferedViaServer, GunManager.useGun2Index, 1);
            PV.RPC("RPC_SetMuzzleFlash", RpcTarget.AllBufferedViaServer, 0);
            PV.RPC("RPC_SetMuzzleFlash", RpcTarget.AllBufferedViaServer, 1);
            //������Ray��������Ȃ��悤�Ƀ��C���[��ύX����
            transform.gameObject.layer = 2;

            //�A�j���[�V��������p�̃X�N���v�g���擾
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
    /// ����������ւ���
    /// </summary>
    private void Swap()
    {

        //����ύX���Ȃ畐���ς��Ȃ�
        if (isSwaping) return;

        //�����L�[�������Ƃ��̃L�[�̕���ɕύX����
        if (Input.GetButtonDown("swap1"))
        {
            Debug.Log("swap1");
            //���g���Ă���e�ɕς��悤�Ƃ��Ă�����ς���K�v���Ȃ��̂ŉ������Ȃ�
            if (myUseGunIndex == 0) return;
            RateTimerStop();
            StopReloadTimer();
            StartCoroutine(SwapTimer(0));
            return;
        }

        if (Input.GetButtonDown("swap2"))
        {
            Debug.Log("swap2");
            //���g���Ă���e�ɕς��悤�Ƃ��Ă�����ς���K�v���Ȃ��̂ŉ������Ȃ�
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
    /// ����������ւ���
    /// </summary>
    /// <param name="index">������������̓Y����</param>
    private IEnumerator SwapTimer(int index)
    {
        isSwaping = true;
        myGuns[myUseGunIndex].SetActive(false);

        //�w��b�o�����畐���ς��� UI�Ŏc�莞�Ԃ��o�����߂�for�����g��
        for(float i=0;i < myGunStatus[index].swapTime;)
        {
            i += Time.deltaTime;
            yield return null;
        }
        //�}�Y���t���b�V�����~�߂Ă���
        PV.RPC("RPC_MuzzleFlashStop", RpcTarget.AllBufferedViaServer);

        PV.RPC("RPC_ChangeGun", RpcTarget.AllBufferedViaServer, index);

        isSwaping = false;
    }

    /// <summary>
    /// �G�C������
    /// </summary>
    private void Aim()
    {
        //�E�N���b�N���Ă���Ȃ�
        if (Input.GetButton("Aim") && !isReloading && !isSwaping)
        {
            float targetFov = fov / myGunStatus[myUseGunIndex].zoomRate;
            isAiming = true;
            if (gunAnis[myUseGunIndex] != null)
            {
                gunAnis[myUseGunIndex].aim(myGunStatus[myUseGunIndex].aimTime);
            }
            

            //�Y�[������
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFov, 1 / myGunStatus[myUseGunIndex].aimTime * Time.deltaTime);
            //�^�[�Q�b�gfov�덷+-1�Ŕ`���������Ƃ݂Ȃ�
            if(mainCamera.fieldOfView >= targetFov - 1 && mainCamera.fieldOfView <= targetFov + 1)
            {
                isAimed = true;
            }
            else
            {
                isAimed = false;
            }

        }
        //���Ă��Ȃ��Ȃ�
        else
        {
            isAimed = false;
            isAiming = false;
            //�Y�[������߂�
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, fov,5 * Time.deltaTime);
            if(gunAnis[myUseGunIndex] != null)
            {
                gunAnis[myUseGunIndex].notAim();
            }
            
        }

        if (myGunStatus[myUseGunIndex] == null) return;
        //�X�R�[�v�t���̏e���`�������Ă���Ȃ�X�R�[�v�摜��\�����e���\���ɂ���
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
    /// ���� ray���΂��ăv���C���[�ɓ�����΃_���[�W��^����
    /// </summary>
    void Shoot()
    {
        //���N���b�N���Ă��Ȃ���Ό����Ȃ�
        if (!Input.GetButton("Shot"))
        {
            //RateTimer���~�߂�
            RateTimerStop();
            //���R�C�����Z�b�g
            recoilIndex = 0;
            return;
        }
        //�}�K�W���ɒe�������Ă��Ȃ��Ȃ猂�ĂȂ�
        if (myGunCurrentMagazines[myUseGunIndex] <= 0) return;
        //�����[�h���Ȃ猂�ĂȂ�
        if (isReloading) return;
        if (isSwaping) return;

        //�܂����ĂȂ��Ȃ�
        if (!isShootable) return;

        //�^�C�}�[�X�^�[�g
        StartCoroutine("RateTimer");

        //�e�����Đ�����
        PV.RPC("RPC_ShotAudioPlay", RpcTarget.AllBufferedViaServer);

        //�}�Y���t���b�V�����N����
        PV.RPC("RPC_MuzzleFlashPlay", RpcTarget.AllBufferedViaServer);

        //�}�b�v�ɐԓ_���o��
        miniMapManager.RedDotPop();

        //�e������
        myGunCurrentMagazines[myUseGunIndex]--;

        //�����𔭐�������
        look.Recoil(myGunStatus[myUseGunIndex].recoilPattern[recoilIndex]);
        //�Y�����X�V
        recoilIndex = (recoilIndex + 1) % myGunStatus[myUseGunIndex].recoilPattern.Length;

        //�e�̂΂����ݒ�
        Vector3 scatter = Vector3.zero;
        if (isAimed)
        {
            //�X�R�[�v�t���̏e���ړ����Ȃ���łƂ��Ƃ����Ƃ��͒e���΂���悤�ɂ���
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

        //�J�����������Ă��������ray���쐬
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f) + scatter);
        ray.origin = mainCamera.transform.position;
        RaycastHit hit;
        //ray���������Ă�����
        if(Physics.Raycast(ray,out hit))
        {
            //ray�����������I�u�W�F�N�g���_���[�W��^���邱�Ƃ��ł���Ȃ�
            Damagable damagable = hit.transform.gameObject.GetComponent<Damagable>();
            if (damagable != null)
            {
                bool applyDamage = false;
                //���̃Q�[�����[����FFA�Ȃ�_���[�W��^����
                if(RuleManager.currentRule == "FFA")
                {
                    applyDamage = true;
                }
                else
                {
                    //�`�[�����C�g�łȂ���΃_���[�W��^����
                    if(hit.transform.gameObject.GetComponent<MyTeam>().GetMyTeam() != myTeam.GetMyTeam())
                    {
                        applyDamage = true;
                    }
                }
                //�_���[�W��^����
                if (applyDamage)
                {
                    Health health = hit.transform.gameObject.GetComponent<Health>();
                    //�_���[�W��^���邱�Ƃő̗͂�0�ɂȂ�Ȃ�L���������̏������s��
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
                //player�ɓ�����Ȃ������瓖�������ꏊ�ɉΉԂ��U�炷
                GameObject fire = PhotonNetwork.Instantiate("BImpact", hit.point, Quaternion.identity);
                fire.transform.LookAt(transform);
            }

            
        }
    }

    /// <summary>
    /// �e�����Đ�����
    /// </summary>
    [PunRPC]
    private void RPC_ShotAudioPlay()
    {
        //�e�����Đ�
        audioSource.PlayOneShot(myGunStatus[myUseGunIndex].shotClip);
    }

    /// <summary>
    /// ���ˊԊu���Ǘ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator RateTimer()
    {
        isShootable = false;

        //�ݒ�b���������猂�Ă�悤�ɂ���
        yield return new WaitForSeconds(60.0f / (float)myGunStatus[myUseGunIndex].fireRate);

        isShootable = true;
    }

    /// <summary>
    /// RateTimer���~�߂�
    /// </summary>
    private void RateTimerStop()
    {
        //�^�C�}�[�N�����Ȃ�X�g�b�v
        if (!isShootable)
        {
            StopCoroutine("RateTimer");
            isShootable = true;
        }
    }

    /// <summary>
    /// �����[�h����
    /// </summary>
    private void Reload()
    {
        //R�L�[�������Ă��Ȃ���΃����[�h���Ȃ�
        if (!Input.GetButtonDown("Reload")) return;
        //����ύX���Ȃ烊���[�h���Ȃ�
        if (isSwaping) return;
        //�}�K�W�������^���������烊���[�h���Ȃ�
        if (myGunCurrentMagazines[myUseGunIndex] == myGunStatus[myUseGunIndex].maxMagazineSize) return;
        //�����[�h���Ȃ烊���[�h���Ȃ�
        if (isReloading) return;

        //�����[�h�J�n
        StartCoroutine("ReloadTimer");
    }

    /// <summary>
    /// �����[�h����
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadTimer()
    {
        isReloading = true;

        //�����[�h�����Đ�
        audioSource.PlayOneShot(myGunStatus[myUseGunIndex].reloadClip);

        //�w��b�����ă����[�h����
        yield return new WaitForSeconds(myGunStatus[myUseGunIndex].reloadTime);

        //�e���[
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
    /// �e�𐶐�������
    /// </summary>
    /// <param name="useGunIndex">�g������̓Y����</param>
    /// <param name="index">���Ԗڂ̃z���_�[�Ɏ���</param>
    [PunRPC]
    void RPC_SetGun(int useGunIndex, int index)
    {
        //��U�����Ă�e���폜
        foreach(Transform gun in gunHolder[index])
        {
            Destroy(gun.gameObject);
        }

        //�g���e�̃X�e�[�^�X���擾
        myGunStatus[index] = GunManager.guns[useGunIndex];

        //�e�𐶐�
        myGuns[index] = Instantiate(myGunStatus[index].gun, Vector3.zero, Quaternion.identity);
        //�q�ɂ��ăJ�����ɂ��Ă���悤�ɂ���
        myGuns[index].transform.parent = gunHolder[index];
        
        //�ʒu�p�x��ݒ�
        myGuns[index].transform.localPosition = myGunStatus[index].gun.transform.position;
        myGuns[index].transform.localRotation = myGunStatus[index].gun.transform.rotation;

        //�}�K�W���ʂ�ݒ�
        myGunCurrentMagazines[index] = myGunStatus[index].maxMagazineSize;


        //���s���̊֌W�ł܂�PhotonView���擾���Ă��Ȃ��ꍇ������
        if (PV != null)
        {
            if (PV.IsMine)
            {
                myGuns[index].layer = 6;
                //�����ōŏ��ɃG���[�������邪���͖������ۂ��H
                //�A�j���[�V��������p�̃X�N���v�g���擾
                gunAnis[0] = myGuns[0].GetComponent<GunAnimation>();
                gunAnis[1] = myGuns[1].GetComponent<GunAnimation>();
            }
        }

        //�g���e����������悤�ɂ���
        //���̂�start��������null�G���[���N���܂�
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
    /// �g���e��ς���
    /// </summary>
    /// <param name="changeGunIndex">�g���e�̓Y����</param>
    [PunRPC]
    void RPC_ChangeGun(int changeGunIndex)
    {
        myUseGunIndex = changeGunIndex;

        //�}�Y���t���b�V�����~�߂Ă���
        PV.RPC("RPC_MuzzleFlashStop",RpcTarget.AllBufferedViaServer);

        //�g���e����������悤�ɂ���
        foreach (GameObject gun in myGuns)
        {
            gun.SetActive(false);
        }
        myGuns[myUseGunIndex].SetActive(true);
    }

    /// <summary>
    /// �}�Y���t���b�V������p�̃X�N���v�g���擾
    /// </summary>
    /// <param name="index">����̓Y����</param>
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
        //�G�C�����Ă��Ȃ��Ȃ�ړ����x�͕ς��Ȃ�����1��Ԃ�;
        else return 1;
    }

    public float GetAimingJumpFprce()
    {
        if (isAiming) return myGunStatus[myUseGunIndex].aimingJumpForce;
        //�G�C�����Ă��Ȃ��Ȃ�W�����v�͕͂ς��Ȃ�����1��Ԃ�;
        else return 1;
    }

    public float GetZoomRate()
    {
        return myGunStatus[myUseGunIndex].zoomRate;
    }
}
