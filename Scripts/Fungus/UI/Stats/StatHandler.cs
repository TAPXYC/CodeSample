using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statName;
    [SerializeField] TextMeshProUGUI statValue;

    public void Init(Stat stat)
    {
        statName.text = stat.Name;
        statValue.text = stat.Value.Value.ToString();
    }
}
