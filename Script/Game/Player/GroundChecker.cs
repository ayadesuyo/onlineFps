using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    bool isGround = false;

    public bool GetisGround()
    {
        return isGround;
    }

    private void OnTriggerStay(Collider other)
    {
        isGround = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isGround = false;
    }
}
