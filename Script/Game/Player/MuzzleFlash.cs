using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject[] muzzleFlashs;

    public void muzzlePlay()
    {
        muzzleFlashs[0].SetActive(true);
        muzzleFlash.Play();
    }

    public void muzzleStop()
    {
        muzzleFlashs[0].SetActive(false);
        muzzleFlash.Stop();
    }

    /// <summary>
    /// ÉåÉCÉÑÅ[ÇïœçXÇ∑ÇÈ
    /// </summary>
    public void layerSet()
    {
        foreach(var m in muzzleFlashs)
        {
            m.layer = 6;
        }
    }
}
