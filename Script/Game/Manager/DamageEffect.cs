using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] Image effect;
    float time;

    public void Set(float _time)
    {
        time = _time;
        StartCoroutine("destroy");
    }

    private IEnumerator destroy()
    {
        for (float i = 0; i < time;)
        {
            i += Time.deltaTime;
            if(i >= time / 2)
            {
                effect.color -= new Color(0,0,0,Time.deltaTime/time*2);
            }
            
            yield return null;
        }
        Destroy(gameObject);

    }
}
