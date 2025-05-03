using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gamemanager.instance.stateUnpause();
    }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
    }
    public void quit()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false ;
    #else
        Application.Quit();
    #endif
    }

    public void respawn()
    {
        gamemanager.instance.playerScript.spawnPlayer();
        gamemanager.instance.stateUnpause();
    }

    public void exitShop()
    {
        shopKeeper.instance.ExitShop();
    }
    public void increaseHP(int cost)
    {
        if (gamemanager.instance.currency >= cost)
        {

        //gamemanager.instance.playerScript.HP += 1;
        gamemanager.instance.currency -= cost;
        gamemanager.instance.playerScript.PickupHealthItem(1);
        gamemanager.instance.updateCurrencyText();
        gamemanager.instance.playerScript.updatePlayerUI();
        }
    }
}
