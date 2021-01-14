using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject _framePrefab;
    [SerializeField] private GameObject _cellPrefab;

    [SerializeField] private float _xCrop;
    [SerializeField] private float _yCrop;
    
    [SerializeField] private int _row;
    [SerializeField] private int _column;
    
    private SpriteRenderer _frameSprite;
    private SpriteRenderer[,] _cells;


    private void Start()
    {
        _frameSprite = CreateFrame(_framePrefab);
        Crop(_xCrop,_yCrop , ref _frameSprite);
        CreateCells(_cellPrefab , _frameSprite);
    }

    private SpriteRenderer CreateFrame(GameObject framePrefab)
    {
        var frame = Instantiate(framePrefab, Vector3.zero, quaternion.identity).GetComponent<SpriteRenderer>();
        var width = Camera.main.aspect * Camera.main.orthographicSize * 2;
        var height = Camera.main.orthographicSize *2;
        
        frame.transform.localScale = 
            new Vector2( width / frame.bounds.size.x , height / frame.bounds.size.y);
        return frame;
    }

    private void Crop(float x , float y , ref SpriteRenderer rnd)
    {
        var xScale = rnd.transform.localScale.x -  x * rnd.transform.localScale.x / 100;
        var yScale = rnd.transform.localScale.y -  y * rnd.transform.localScale.y / 100;
        rnd.transform.localScale = new Vector3(xScale , yScale);
    }

    private void CreateCells(GameObject cellPrefab , SpriteRenderer frame)
    {
        _cells = new SpriteRenderer[_row,_column];
        
        var width = frame.bounds.size.x / _column;
        var height = frame.bounds.size.y / _row;

        var cellSprite = cellPrefab.GetComponent<SpriteRenderer>();
        var scale  = new Vector3(width / cellSprite.bounds.size.x , height / cellSprite.bounds.size.y );

        var leftDownPoint = new Vector2(-frame.bounds.size.x / 2 + width / 2
            , -frame.bounds.size.y / 2 + height / 2);

        for (var row = 0; row < _row; row++)
        {
            for (var column = 0; column < _column; column++)
            {
                var cell = Instantiate(cellPrefab, Vector3.zero, quaternion.identity).GetComponent<SpriteRenderer>();
                cell.transform.localScale = scale;
                cell.transform.position = new Vector3(leftDownPoint.x + width * column , leftDownPoint.y + height * row);
                _cells[row, column] = cell;
            }    
        }

    }
}
