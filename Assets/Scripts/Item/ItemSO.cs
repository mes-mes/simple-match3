using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Apple , Banana , Orange
}
[CreateAssetMenu(fileName = "Item" , menuName = "Create new item")]
public class ItemSO : ScriptableObject
{
    public ItemType ItemType;
    public Sprite Sprite;
    public int Score;
}
