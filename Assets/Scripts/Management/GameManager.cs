using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private static GameManager gameManager = null;
    public ManagementReferencing _ref;

    public static GameManager GetSingleton()
    {
        if(gameManager == null)
        {
            gameManager = new GameManager();

        }
        return gameManager;
    }
}
