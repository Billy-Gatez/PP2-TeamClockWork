//Jeremy Cahill - Full Sail University - Portfolio 2 - Game Dev - Rod Moye

using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text xpText;

    public Image playerHPBar;
    public GameObject playerDamageScreen;
    

    public GameObject player;
    public playerController playerScript;


    public bool isPaused;

    float timeScaleOrig;

    int gameGoalCount;
    public int currency;
    

    
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        timeScaleOrig = Time.timeScale;
    }


    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }



        }

    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount, int cur)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        currency += cur;

        if (gameGoalCount <= 0)
        {
            
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }
    public void updateCurrency(int amount)
    {
        currency += amount;
        xpText.text = " " + currency.ToString("F0");

        if (currency < 0)
        {
            currency = 0; 
            xpText.text = " " + currency.ToString("F0");
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
