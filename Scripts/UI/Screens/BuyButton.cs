using System;
using TMPro;
using UnityEngine;

public class BuyButton : MonoBehaviour
{
    [SerializeField] TMP_Text rewardAmount;
    [SerializeField] GameObject DiamondImage;
    [SerializeField] GameObject CardImage;
    [SerializeField] TMP_Text price;

    public event Action<int, double> OnClick;

    private int _rewardAmount;
    private double _price;

    public void Init(int rewardAmount, double price)
    {
        this.rewardAmount.text = rewardAmount.ToString();
        this.price.text = Math.Round(price, 2) + "$";
        _rewardAmount = rewardAmount;
        _price = Math.Round(price, 2);
    }

    public void SetDiamondImage()
    {
        CardImage.SetActive(false);
        DiamondImage.SetActive(true);
    }

    public void SetCardImage()
    {
        DiamondImage.SetActive(false);
        CardImage.SetActive(true);        
    }

    public void BuyClick()
    {
        OnClick?.Invoke(_rewardAmount, _price);
    }
}
