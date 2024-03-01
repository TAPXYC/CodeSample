using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ThemeSprites : ScriptableObject
{
    [SerializeField] List<ThemeInfo<Sprite>> sprites;

    public List<ThemeInfo<Sprite>> Sprites => sprites;
}
