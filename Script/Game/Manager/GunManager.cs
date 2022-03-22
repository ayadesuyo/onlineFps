using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//自分が使う銃を決める
public class GunManager : MonoBehaviour
{
    //持つ銃を決める用のドロップリスト
    [SerializeField] private Dropdown gun1Selecter;
    [SerializeField] private Dropdown gun2Selecter;

    //使える銃の一覧
    [SerializeField] private GunStatus[] gun;
    //動的に生成されたプレイヤーからも銃を参照できるようにしたいのでstaticとして持つ
    static public GunStatus[] guns;
    //使う銃の添え字
    public static int useGun1Index = 0;
    public static int useGun2Index = 1;

    void Awake()
    {
        guns = gun;

        //DropDownをセット
        List<string> gunList = new List<string>();
        foreach(var gun in guns)
        {
            gunList.Add(gun.name);
        }
        gun1Selecter.AddOptions(gunList);
        gun2Selecter.AddOptions(gunList);
    }

    /// <summary>
    /// gun1Selecterの値が変わると呼ばれ、武器をセットする
    /// </summary>
    public void SetUseGun1()
    {
        useGun1Index = gun1Selecter.value;
    }

    /// <summary>
    /// gun2Selecterの値が変わると呼ばれ、武器をセットする
    /// </summary>
    public void SetUseGun2()
    {
        useGun2Index = gun2Selecter.value;
    }
}
