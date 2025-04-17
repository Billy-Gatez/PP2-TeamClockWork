//Jeremy Cahill - Full Sail University - Portfolio 2 - Game Dev - Rod Moye

using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;


public class buttonFunctions : MonoBehaviour
{
    private float XP;

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
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void increaseHP(int cost)
    {
      
        int healthPerCost = 10; 

       
        if (gamemanager.instance.currency >= cost)
        {
           
            int healthGained = cost * healthPerCost;

           
            gamemanager.instance.playerScript.HP += healthGained;

        
            if (gamemanager.instance.playerScript.HP > gamemanager.instance.playerScript.HPOrig)
            {
                gamemanager.instance.playerScript.HP = gamemanager.instance.playerScript.HPOrig;
            }

           
            gamemanager.instance.currency -= cost;

            gamemanager.instance.playerHPBar.fillAmount = (float)gamemanager.instance.playerScript.HP / gamemanager.instance.playerScript.HPOrig;
            
        gamemanager.instance.updateCurrency(-cost); // Call the method to update currency display

        }
        else
        {
            Debug.Log("Not enough currency to increase HP!");
        }
    }

}