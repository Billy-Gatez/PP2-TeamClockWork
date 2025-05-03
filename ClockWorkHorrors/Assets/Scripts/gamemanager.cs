using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;
    [Header("---Components---")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text currencyText;

    [Header("---   ---")]
    public TMP_Text ammoCur, ammoMax;
    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public GameObject checkpointPopup;


    [Header("---   ---")]
    public GameObject playerSpawnPos;
    public GameObject player;
    public playerController playerScript;
    public GameObject miniMapIcon;

    public bool isPaused;

    float timeScaleOrig;

    int gameGoalCount;
    public int currency;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");

        timeScaleOrig = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
        Vector3 newPosition = player.transform.position;
        newPosition.y = miniMapIcon.transform.position.y;
        miniMapIcon.transform.position = newPosition;
    }
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void ToggleShop(bool isOpen, GameObject shopUI)
    {
        shopUI.SetActive(isOpen);

        if (isOpen)
        {
            isPaused = true;
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }
        else
        {
            isPaused = false;
            Time.timeScale = timeScaleOrig;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
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
        currency += cur;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        currencyText.text = currency.ToString("F0");


        if (gameGoalCount <= 0)
        {
            // You won!
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }
    public void updateCurrencyText()
    {
        currencyText.text = currency.ToString("F0");
    }
    public void youlose()
    {
        // You lose!
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    public void BuyHealth(int healthPrice, int healthAmount)
    {
        if (currency >= healthPrice)
        {
            currency -= healthPrice;
            playerScript.PickupHealthItem(healthAmount);
            updateCurrencyText();
            Debug.Log("Player bought HP!");
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }
}