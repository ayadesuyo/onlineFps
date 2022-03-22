using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Look : MonoBehaviour
{
    PhotonView PV;
    Shooter shooter;

    //�������J����
    [SerializeField] Transform mainCamera;
    //�e�݈̂ڂ��J����
    [SerializeField] Transform myGunCamera;
    [SerializeField] Transform effects;

    //���݂̉�]�ʂ�ێ�
    private float xRotation = 0, yRotation = 0;

    //���x(�O����ύX�ł���悤�ɂ���������static)
    public static float SensitivityX = 1;
    public static float SensitivityY = 1;

    bool isReaction = false;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        shooter = GetComponent<Shooter>();
        //�����������������̂ł͂Ȃ��ꍇ�͂���Ȃ����̂��폜
        if (!PV.IsMine)
        {
            Destroy(mainCamera.GetComponent<Camera>());
            Destroy(mainCamera.GetComponent<AudioListener>());
            Destroy(myGunCamera.GetComponent<Camera>());
            Destroy(myGunCamera.GetComponent<AudioListener>());
            Destroy(this);
        }
    }

    private void Update()
    {
        if (!PV.IsMine) return;

        //���͂��󂯎��
        float x = 0;
        float y = 0;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float controllerX = Input.GetAxis("Controller X") / 2;
        float controllerY = Input.GetAxis("Controller Y") / 2;

        //if(mouseX + mouseY != 0.0f)
        //{
        //    Debug.Log("use Mouse");
        //}
        //else
        //{
        //    Debug.Log("use Controller");
        //}

        x = (mouseX - controllerX) * SensitivityX;
        y = (mouseY + controllerY) * SensitivityY;

        //��]�ʂ��X�V
        if (shooter.GetIsAiming())
        {
            //�G�C�����̓Y�[���{���ɉ����ĉ�]�ʂ����炷
            xRotation += 5.0f * x / (shooter.GetZoomRate() * 2f);
            yRotation += 5.0f * y / (shooter.GetZoomRate() * 2f);
        }
        else
        {
            xRotation += 5 * x;
            yRotation += 5 * y;
        }
        

        //���E��ݒ�
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        //���f
        mainCamera.transform.localRotation = Quaternion.Euler(-yRotation, 0, 0);
        transform.localRotation = Quaternion.Euler(0, xRotation, 0);
        effects.localRotation = Quaternion.Euler(0, 0, xRotation);
    }

    public void Recoil(Vector3 recoil)
    {
        //��]�ʂ��X�V
        xRotation += recoil.x;
        yRotation += recoil.y;

        //���E��ݒ�
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        //���f
        mainCamera.transform.localRotation = Quaternion.Euler(-yRotation, 0, 0);
        transform.localRotation = Quaternion.Euler(0, xRotation, 0);
        effects.localRotation = Quaternion.Euler(0, 0, xRotation);
    }

    public void Damaged()
    {
        if (isReaction)
        {
            StopCoroutine("DamageReation");
        }

        StartCoroutine("DamageReation");
    }

    public IEnumerator DamageReation()
    {
        isReaction = true;

        for(float i = 0;i <= 0.1f;i = i + Time.deltaTime)
        {
            yRotation += Random.Range(50,80)*Time.deltaTime;
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);
            mainCamera.transform.localRotation = Quaternion.Euler(-yRotation, 0, 0);

            yield return null;
        }

        isReaction = false;
    }

    public void SetRotation(float x,float y)
    {
        xRotation = x;
        yRotation = y;
    }
}
