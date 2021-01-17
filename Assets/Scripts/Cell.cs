using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{

    [SerializeField]private bool _isFree;
    public bool IsFree
    {
        get { return _isFree; }
        set { _isFree = value; }
    }

    private int _row;
    public int Row => _row;

    private int _column;
    public int Column=> _column;

    [SerializeField]private Item _item;
    public Item Item
    {
        get { return _item; }
        set { _item = value; }
    }

    [SerializeField] private SpriteRenderer _renderer;
    public SpriteRenderer Renderer => _renderer;

    [SerializeField] private TextMeshPro _textMesh;

    
    public void Init(int row , int coulmn)
    {
        _row = row;
        _column = coulmn;
        _textMesh.text = $"{_row} : {_column}";
        gameObject.name = $"Cell_{_row} : {_column}";
    }


}
