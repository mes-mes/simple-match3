using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject _framePrefab;
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private ItemSO[] _itemsData;


    [SerializeField] private float _xCrop;
    [SerializeField] private float _yCrop;

    [SerializeField] private int _row;
    [SerializeField] private int _column;

    private SpriteRenderer _frameSprite;
    private Dictionary<int, List<Cell>> _horizontalCells = new Dictionary<int, List<Cell>>();
    private Dictionary<int, List<Cell>> _verticalCells = new Dictionary<int, List<Cell>>();
    private SwitchItemController _switchItemController;


    private void Start()
    {
        _frameSprite = CreateFrame(_framePrefab);
        Crop(_xCrop, _yCrop, ref _frameSprite);
        CreateCells(_cellPrefab, _frameSprite);
        _switchItemController = FindObjectOfType<SwitchItemController>();
        _switchItemController.OnSwitched += OnSwitched;
    }

    #region Initial level

    private SpriteRenderer CreateFrame(GameObject framePrefab)
    {
        var cam = FindObjectOfType<Camera>();
        var frame = Instantiate(framePrefab, Vector3.zero, quaternion.identity).GetComponent<SpriteRenderer>();
        var width = cam.aspect * cam.orthographicSize * 2;
        var height = cam.orthographicSize * 2;

        frame.transform.localScale =
            new Vector2(width / frame.bounds.size.x, height / frame.bounds.size.y);
        return frame;
    }

    private void Crop(float x, float y, ref SpriteRenderer rnd)
    {
        var xScale = rnd.transform.localScale.x - x * rnd.transform.localScale.x / 100;
        var yScale = rnd.transform.localScale.y - y * rnd.transform.localScale.y / 100;
        rnd.transform.localScale = new Vector3(xScale, yScale);
    }

    private void CreateCells(GameObject cellPrefab, SpriteRenderer frame)
    {
        var width = frame.bounds.size.x / _column;
        var height = frame.bounds.size.y / _row;

        var cellSprite = cellPrefab.GetComponent<Cell>().Renderer;
        var scale = new Vector3(width / cellSprite.bounds.size.x, height / cellSprite.bounds.size.y);

        var leftDownPoint = new Vector2(-frame.bounds.size.x / 2 + width / 2
            , -frame.bounds.size.y / 2 + height / 2);

        // Initial horizontal list
        for (var row = 0; row < _row; row++)
        {
            _horizontalCells.Add(row, new List<Cell>());

            for (var column = 0; column < _column; column++)
            {
                var cell = Instantiate(cellPrefab, Vector3.zero, quaternion.identity).GetComponent<Cell>();
                cell.transform.localScale = scale;
                cell.transform.position = new Vector3(leftDownPoint.x + width * column, leftDownPoint.y + height * row);
                cell.Init(row, column);
                _horizontalCells[row].Add(cell);
                CreateItem(row, column);

            }
        }

        // Initial vertical list
        for (var col = 0; col < _column; col++)
        {
            _verticalCells.Add(col, new List<Cell>());
            for (var row = 0; row < _row; row++)
            {
                var cell = _horizontalCells[row][col];
                _verticalCells[col].Add(cell);
            }
        }

    }

    private void CreateItem(int row, int column)
    {
        var cell = _horizontalCells[row][column];
        var item = Instantiate(_itemPrefab, cell.transform.position, quaternion.identity).GetComponent<Item>();
        var index = Random.Range(0, _itemsData.Length);
        var data = _itemsData[index];
        item.Init(data);
        cell.Item = item;
    }

    #endregion

    #region Check rows and columns to find same shapes

    private void FindChange(Dictionary<int, List<Cell>> dictionary)
    {
        var len = 1;// dictionary.Count;

        for (var i = 0; i < len; i++)
        {
            var lst = dictionary[i];
            var dic = SplitList(lst);

            // Navigate in a row/column
            foreach (var d in dic)
            {
                if (d.Value.Count > 2)
                {
                    foreach (var cells in d.Value)
                    {
                        cells.Item.gameObject.SetActive(false);
                        cells.IsFree = true;
                    }
                }
            }
        }
    }

    private Dictionary<int, List<Cell>> SplitList(List<Cell> lst)
    {
        var dic = new Dictionary<int, List<Cell>>();
        var dicIndex = 0;
        dic.Add(dicIndex, new List<Cell>());

        for (var i = 0; i < lst.Count; i++)
        {
            if (dic[dicIndex].Count != 0)
            {
                if (lst[i].Item.Type == dic[dicIndex][0].Item.Type)
                {
                    dic[dicIndex].Add(lst[i]);
                }
                else
                {
                    dicIndex++;
                    dic.Add(dicIndex, new List<Cell>());
                    i--;
                }
            }
            else
            {
                dic[dicIndex].Add(lst[i]);
            }
        }

        return dic;
    }

    #endregion

    private void OnSwitched()
    {
        Debug.Log("Switch");
        //FindChange(_horizontalCells);
        FindChange(_verticalCells);
       
    }

    private void Replacement()
    {
        Cell firstFreeCell = null;

        for (var col = 0; col < 1; col++)
        {
            // Find the free cells and put items into the cells
            for (var row = 0; row < _row; row++)
            {
                if (_verticalCells[col][row].IsFree && firstFreeCell == null)
                {
                    firstFreeCell = _verticalCells[col][row];
                    continue;
                }

                if (_verticalCells[col][row].IsFree == false && firstFreeCell != null)
                {
                    Swap(firstFreeCell , _verticalCells[col][row]);
                    var rowIndex = firstFreeCell.Row + 1;

                    if(rowIndex < _row - 1)
                        firstFreeCell = _verticalCells[col][rowIndex];

                }
            }
            
            firstFreeCell = null;

            // Init destroyed items again
            for (var row = 0; row < _row; row++)
            {
                var cell = _verticalCells[col][row];
                
                if (cell.IsFree)
                {
                    cell.Item.transform.position = cell.transform.position;
                    cell.Item.gameObject.SetActive(true);
                    
                    var index = Random.Range(0, _itemsData.Length);
                    var data = _itemsData[index];
                    cell.Item.Init(data);
                    cell.IsFree = false;
                }
            }
        }
    }

    private void Swap(Cell cellA , Cell cellB)
    {
        var temp = cellA.Item;
        cellA.Item = cellB.Item;
        cellB.Item = temp;
        cellB.IsFree = true;
        cellA.Item.transform.position = cellA.transform.position;
        cellA.IsFree = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            Replacement();
    }

}
