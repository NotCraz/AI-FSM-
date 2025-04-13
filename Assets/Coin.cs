using UnityEngine;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CoinManager.instance.CollectCoin();
            Destroy(gameObject);
        }
    }
}
