using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    public int totalCoins;
    private int collectedCoins = 0;

    public TextMeshProUGUI coinText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateCoinUI();
    }

    public void CollectCoin()
    {
        collectedCoins++;
        UpdateCoinUI();

        if (collectedCoins >= totalCoins)
        {
            Debug.Log("You Win!");
            SceneManager.LoadScene("New Scene");
        }
    }

    void UpdateCoinUI()
    {
        coinText.text = $"Coins: {collectedCoins} / {totalCoins}";
    }
}
