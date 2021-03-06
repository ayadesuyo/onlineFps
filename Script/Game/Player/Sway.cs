using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 銃を視点移動させた方向を向くようにする
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

        //角度を180から-180に正規化
        float defaultRotYForce = Mathf.Repeat(holder.transform.localRotation.eulerAngles.y + 180, 360) - 180;
        //元の角度に戻す力を加える
        holder.transform.Rotate(new Vector3(0, defaultRot.y - defaultRotYForce, 0) * Time.deltaTime * 10);

        //入力した方向に角度を加える
        float mouseX = Input.GetAxis("Mouse X");
        holder.transform.Rotate(new Vector3(0, mouseX, 0) * Time.deltaTime * smooth);
    }
}
