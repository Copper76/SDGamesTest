using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    static PlayerController player;

    public static void SetPlayerController(PlayerController playerController)
    {
        player = playerController;
    }

    public static PlayerController GetPlayer()
    {
        return player;
    }
}
