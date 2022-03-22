using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// �e�����_�ړ������������������悤�ɂ���
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

        //�p�x��180����-180�ɐ��K��
        float defaultRotYForce = Mathf.Repeat(holder.transform.localRotation.eulerAngles.y + 180, 360) - 180;
        //���̊p�x�ɖ߂��͂�������
        holder.transform.Rotate(new Vector3(0, defaultRot.y - defaultRotYForce, 0) * Time.deltaTime * 10);

        //���͂��������Ɋp�x��������
        float mouseX = Input.GetAxis("Mouse X");
        holder.transform.Rotate(new Vector3(0, mouseX, 0) * Time.deltaTime * smooth);
    }
}
