using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// e‚ğ‹“_ˆÚ“®‚³‚¹‚½•ûŒü‚ğŒü‚­‚æ‚¤‚É‚·‚é
/// </summary>
public class Sway : MonoBehaviour
{
    PhotonView PV;

    [SerializeField] private float smooth;
    [SerializeField] private GameObject holder;

    private Vector3 defaultRot;
    private void Start()
    {
        PV = GetComponent<PhotonView>();

        defaultRot = holder.transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        if (!PV.IsMine) return;

        //Šp“x‚ğ180‚©‚ç-180‚É³‹K‰»
        float defaultRotYForce = Mathf.Repeat(holder.transform.localRotation.eulerAngles.y + 180, 360) - 180;
        //Œ³‚ÌŠp“x‚É–ß‚·—Í‚ğ‰Á‚¦‚é
        holder.transform.Rotate(new Vector3(0, defaultRot.y - defaultRotYForce, 0) * Time.deltaTime * 10);

        //“ü—Í‚µ‚½•ûŒü‚ÉŠp“x‚ğ‰Á‚¦‚é
        float mouseX = Input.GetAxis("Mouse X");
        holder.transform.Rotate(new Vector3(0, mouseX, 0) * Time.deltaTime * smooth);
    }
}
