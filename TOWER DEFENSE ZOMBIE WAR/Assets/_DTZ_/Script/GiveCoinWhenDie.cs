using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCoinWhenDie : MonoBehaviour
{
    [Range(0f, 1f)]
    public float chanceSpawnCoin = 0.35f;

    public int coinGiveMin = 5;
    public int coinGiveMax = 10;

    public void GiveCoin()
    {
        if (Random.Range(0f, 1f) < chanceSpawnCoin)
        {
            int random = Random.Range(coinGiveMin, coinGiveMax);
            var coinObj = (CoinController)Resources.Load("Coin", typeof(CoinController));
            var coin = (CoinController)Instantiate(coinObj, transform.position, Quaternion.identity);
            coin.Init(random);
        }
    }
}
