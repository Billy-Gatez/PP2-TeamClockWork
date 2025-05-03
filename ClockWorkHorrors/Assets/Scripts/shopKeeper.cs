using UnityEngine;
using UnityEngine.UI;
public class shopKeeper : MonoBehaviour
{
    public static shopKeeper instance;

    [Header("---Shop Settings---")]
    public GameObject shopUI;
    [Range(1, 50)][SerializeField] int healthPrice;
    [Range(1, 20)][SerializeField] int healthAmount;
    public Button buyHealthButton;
    public Button exitShopButton;
    public Text currencyText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (buyHealthButton != null)
        {
            buyHealthButton.onClick.AddListener(BuyHealth);
        }
        if (exitShopButton != null)
        {
            exitShopButton.onClick.AddListener(ExitShop);
        }
            UpdateCurrencyUI();
    }

    public void ToggleShop(bool isOpen)
    {
        shopUI.SetActive(isOpen);

        if (isOpen)
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
            UpdateCurrencyUI();
}
    public void ExitShop()
    {
        ToggleShop(false);
    }
    private void BuyHealth()
    {
        if (gamemanager.instance.currency >= healthPrice)
        {
            gamemanager.instance.currency -= healthPrice;
            gamemanager.instance.playerScript.PickupHealthItem(healthAmount);
            gamemanager.instance.updateCurrencyText();
            UpdateCurrencyUI();
            Debug.Log("Player bought HP!");
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }

    private void UpdateCurrencyUI()
    {
        if (currencyText != null)
        {
            currencyText.text = "Currency: " + gamemanager.instance.currency.ToString();
        }
    }
    void Update()
    {
         if (shopUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitShop();
        }
    }
    }
