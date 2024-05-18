using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipper : MonoBehaviour
{
    public void OnEquip(int itemID)
    {
        Statistics playerStat = GameManager.GetPlayer().playerStat;
        switch (itemID)
        {
            case 3:
                playerStat.ATK += 1;
                break;
            case 4:
                playerStat.DEF += 1;
                break;
            case 5:
                playerStat.DEF += 2;
                break;
            case 6:
                playerStat.MAXHP += 1;
                break;
            case 7:
                playerStat.MOV += 1;
                break;
        }
    }

    public void OnUnequip(int itemID)
    {
        Statistics playerStat = GameManager.GetPlayer().playerStat;
        switch (itemID)
        {
            case 3:
                playerStat.ATK -= 1;
                break;
            case 4:
                playerStat.DEF -= 1;
                break;
            case 5:
                playerStat.DEF -= 2;
                break;
            case 6:
                playerStat.MAXHP -= 1;
                if (playerStat.HP > playerStat.MAXHP)
                {
                    playerStat.HP -= 1;
                }
                break;
            case 7:
                playerStat.MOV -= 1;
                break;
        }
    }
}
