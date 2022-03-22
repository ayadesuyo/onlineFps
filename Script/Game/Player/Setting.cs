using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] Slider sliderX;
    [SerializeField] Slider sliderY;
    [SerializeField] Slider sliderFov;

    [SerializeField] Text sliderXValueText;
    [SerializeField] Text sliderYValueText;
    [SerializeField] Text fovValueText;

    public void Start()
    {
        sliderX.value = Look.SensitivityX;
        sliderY.value = Look.SensitivityY;
        sliderFov.value = Shooter.fov;

        sliderXValueText.text = sliderX.value.ToString();
        sliderYValueText.text = sliderY.value.ToString();
        fovValueText.text = sliderFov.value.ToString();
    }

    public void SensitivityXSet()
    {
        Look.SensitivityX = sliderX.value;
        sliderXValueText.text = sliderX.value.ToString();
    }

    public void SensitivityYSet()
    {
        Look.SensitivityY = sliderY.value;
        sliderYValueText.text = sliderY.value.ToString();
    }

    public void FovSet()
    {
        Shooter.fov = (int)sliderFov.value;
        fovValueText.text = ((int)sliderFov.value).ToString();
    }
}
