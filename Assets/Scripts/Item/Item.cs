using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField]private Animator _animator;

    private ItemType _type;
    public ItemType Type => _type;
    private int _score;
    
    public bool IsSwitch { get; set; }

    public void Init(ItemSO data)
    {
        _renderer.sprite = data.Sprite;
        _type = data.ItemType;
        _score = data.Score;
    }
    
}
