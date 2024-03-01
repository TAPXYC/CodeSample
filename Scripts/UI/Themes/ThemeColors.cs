using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu]
public class ThemeColors : ScriptableObject
{
    [SerializeField] List<ThemeInfo<Color>> colors; 

    public List<ThemeInfo<Color>> Colors => colors;
}
