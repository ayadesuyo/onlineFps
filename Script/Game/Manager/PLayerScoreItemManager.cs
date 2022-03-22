using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PLayerScoreItemManager : MonoBehaviour
{
    [SerializeField] private Text itemText;

    public void SetTDMText(string player, int kill, int death, string team)
    {
        itemText.text = "[" + team + "]" + player + " / " + kill + " kill / " + death + " death";
    }

    public void SetFFAText(string player, int kill, int death)
    {
        itemText.text = player + " / " + kill + " kill / " + death + " death";
    }
}
