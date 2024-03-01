using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ThemeGradient : ScriptableObject
{
    [SerializeField] List<ThemeInfo<Gradient>> gradients;

    public List<ThemeInfo<Gradient>> Gradients => gradients;
}

