using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FinanceManager : MonoBehaviour
{
    #region Instance Method
    public static FinanceManager Instance;
    private void InstanceMethod()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion
    
    public int totalCoin;
    
    [System.Serializable]
    public class Products
    { 
        public enum Product
        {
            Sock,
            Bra,
            Short,
            Slip
        }
        public Product product;
        public float sewTime;
        public float coloringTime;
        public int productCost;
    }
    
    public List<Products> products = new List<Products>();

    public Transform coinObject;
    private void Awake()
    {
        InstanceMethod();
    }

    private void Start()
    {
        totalCoin = PlayerPrefs.GetInt("TotalCoin");

        if (totalCoin < 500)
        {
            PlayerPrefs.SetInt("TotalCoin", 500);
            totalCoin = PlayerPrefs.GetInt("TotalCoin");
            PlayerPrefs.Save();
        }
    }

    private void Update()
    {
        CalculateCoins();
    }

    public void GainCoin(int amount)
    {
        UIManager.Instance.coinHolder.transform.GetChild(0).DOShakeScale(0.1f, 0.25f).OnComplete(() =>
        {
            UIManager.Instance.coinHolder.transform.GetChild(0).DOScale(1, 0.2f);
        });
        
        totalCoin += amount;
        
        PlayerPrefs.SetInt("TotalCoin", totalCoin);
        PlayerPrefs.Save();
        
        SoundManager.Instance.PlaySound("CoinPickup", 0.7f);
    }
    
    private static string SetCurrency(float amount)
    {
        string[] currencies = { "", "K", "M", "B", "T" };

        var currencyIndex = 0;
        while (amount >= 1000f && currencyIndex < currencies.Length - 1)
        {
            amount /= 1000f;
            currencyIndex++;
        }

        var format = (currencyIndex == 0 && amount < 1000f) ? "f0" : "f2"; 

        return amount.ToString(format) + currencies[currencyIndex];
    }

    private void CalculateCoins()
    {
        UIManager.Instance.coinText.text = SetCurrency(totalCoin);
        UIManager.Instance.fabricCostText.text = SetCurrency(LevelSpecial.Instance.fabricCost);
    }
}
