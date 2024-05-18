using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statText;

    public void UpdateStatText()
    {
        Statistics playerStat = GameManager.GetPlayer().playerStat;

        string text = "";
        text += string.Format("Max HP: {0}\n\n", playerStat.MAXHP);
        text += string.Format("Current HP: {0}\n\n", playerStat.HP);
        text += string.Format("Attack: {0}\n\n", playerStat.ATK);
        text += string.Format("Defense: {0}\n\n", playerStat.DEF);
        text += string.Format("Speed: {0}", playerStat.MOV);

        statText.text = text;
    }
}
