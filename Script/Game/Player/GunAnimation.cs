using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    Vector3 defaultPos;
    Vector3 defaultRot;

    [SerializeField] Vector3 aimPos;
    [SerializeField] Vector3 aimRot;

    private void Start()
    {
        defaultPos = transform.localPosition;
        defaultRot = transform.localRotation.eulerAngles;
    }

    public void aim(float smooth)
    {
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Lerp(pos.x,aimPos.x, 1 / smooth * Time.deltaTime);
        pos.y = Mathf.Lerp(pos.y,aimPos.y, 1 / smooth * Time.deltaTime);
        pos.z = Mathf.Lerp(pos.z,aimPos.z, 1 / smooth * Time.deltaTime);

        Vector3 rot = transform.localRotation.eulerAngles;
        rot.x = Mathf.Lerp(rot.x, aimRot.x, 1 / smooth * Time.deltaTime);
        rot.y = Mathf.Lerp(rot.y, aimRot.y, 1 / smooth * Time.deltaTime);
        rot.z = Mathf.Lerp(rot.z, aimRot.z, 1 / smooth * Time.deltaTime);

        transform.localPosition = pos;
        transform.localRotation = Quaternion.Euler(rot.x,rot.y,rot.z);
    }

    public void notAim()
    {
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Lerp(pos.x, defaultPos.x, 5 * Time.deltaTime);
        pos.y = Mathf.Lerp(pos.y, defaultPos.y, 5 * Time.deltaTime);
        pos.z = Mathf.Lerp(pos.z, defaultPos.z, 5 * Time.deltaTime);

        Vector3 rot = transform.localRotation.eulerAngles;
        rot.x = Mathf.Lerp(rot.x, defaultRot.x, 5 * Time.deltaTime);
        rot.y = Mathf.Lerp(rot.y, defaultRot.y, 5 * Time.deltaTime);
        rot.z = Mathf.Lerp(rot.z, defaultRot.z, 5 * Time.deltaTime);

        transform.localPosition = pos;
        transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
    }
}
