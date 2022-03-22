using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Look : MonoBehaviour
{
    PhotonView PV;
    Shooter shooter;

    //“®‚©‚·ƒJƒƒ‰
    [SerializeField] Transform mainCamera;
    //e‚Ì‚İˆÚ‚·ƒJƒƒ‰
    [SerializeField] Transform myGunCamera;
    [SerializeField] Transform effects;

    //Œ»İ‚Ì‰ñ“]—Ê‚ğ•Û
    private float xRotation = 0, yRotation = 0;

    //Š´“x(ŠO‚©‚ç•ÏX‚Å‚«‚é‚æ‚¤‚É‚µ‚½‚¢‚©‚çstatic)
    public static float SensitivityX = 1;
    public static float SensitivityY = 1;

    bool isReaction = false;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        shooter = GetComponent<Shooter>();
        //©•ª‚ª¶¬‚µ‚½‚à‚Ì‚Å‚Í‚È‚¢ê‡‚Í‚¢‚ç‚È‚¢‚à‚Ì‚ğíœ
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

        //“ü—Í‚ğó‚¯æ‚é
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

        //‰ñ“]—Ê‚ğXV
        if (shooter.GetIsAiming())
        {
            //ƒGƒCƒ€‚ÍƒY[ƒ€”{—¦‚É‰‚¶‚Ä‰ñ“]—Ê‚ğŒ¸‚ç‚·
            xRotation += 5.0f * x / (shooter.GetZoomRate() * 2f);
            yRotation += 5.0f * y / (shooter.GetZoomRate() * 2f);
        }
        else
        {
            xRotation += 5 * x;
            yRotation += 5 * y;
        }
        

        //ŒÀŠE‚ğİ’è
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        //”½‰f
        mainCamera.transform.localRotation = Quaternion.Euler(-yRotation, 0, 0);
        transform.localRotation = Quaternion.Euler(0, xRotation, 0);
        effects.localRotation = Quaternion.Euler(0, 0, xRotation);
    }

    public void Recoil(Vector3 recoil)
    {
        //‰ñ“]—Ê‚ğXV
        xRotation += recoil.x;
        yRotation += recoil.y;

        //ŒÀŠE‚ğİ’è
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        //”½‰f
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
