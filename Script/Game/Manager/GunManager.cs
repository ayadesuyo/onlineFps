using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�������g���e�����߂�
public class GunManager : MonoBehaviour
{
    //���e�����߂�p�̃h���b�v���X�g
    [SerializeField] private Dropdown gun1Selecter;
    [SerializeField] private Dropdown gun2Selecter;

    //�g����e�̈ꗗ
    [SerializeField] private GunStatus[] gun;
    //���I�ɐ������ꂽ�v���C���[������e���Q�Ƃł���悤�ɂ������̂�static�Ƃ��Ď���
    static public GunStatus[] guns;
    //�g���e�̓Y����
    public static int useGun1Index = 0;
    public static int useGun2Index = 1;

    void Awake()
    {
        guns = gun;

        //DropDown���Z�b�g
        List<string> gunList = new List<string>();
        foreach(var gun in guns)
        {
            gunList.Add(gun.name);
        }
        gun1Selecter.AddOptions(gunList);
        gun2Selecter.AddOptions(gunList);
    }

    /// <summary>
    /// gun1Selecter�̒l���ς��ƌĂ΂�A������Z�b�g����
    /// </summary>
    public void SetUseGun1()
    {
        useGun1Index = gun1Selecter.value;
    }

    /// <summary>
    /// gun2Selecter�̒l���ς��ƌĂ΂�A������Z�b�g����
    /// </summary>
    public void SetUseGun2()
    {
        useGun2Index = gun2Selecter.value;
    }
}
