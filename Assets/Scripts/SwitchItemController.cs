using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class SwitchItemController : MonoBehaviour
{
    private Camera _camera;
    private bool _isSwitching;
    private Color _defaultCellColor;

    [SerializeField] private Cell _firstCell;
    [SerializeField] private Cell _secondCell;

    public delegate void SwitchDelegate();
    private event SwitchDelegate _onSwitched;
    public SwitchDelegate OnSwitched
    {
        get { return _onSwitched; }
        set { _onSwitched = value; }
    }

    private void Start()
    {
        _camera = FindObjectOfType<Camera>();
    }
    private void SelectCell(Collider2D coll)
    {
        if (coll == null) return;
        
        if (_firstCell != null)
        {
            _secondCell = coll.GetComponent<Cell>();
            _isSwitching = true;
            Switch();
        }
        else
        {
            _firstCell = coll.GetComponent<Cell>();
            _defaultCellColor = _firstCell.Renderer.color;
            _firstCell.Renderer.color = Color.green;
        }
    }
    private void Switch()
    {
        if(IsValidToSwich(_firstCell , _secondCell))
        {
            _secondCell.Item.transform.position = _firstCell.transform.position;
            _firstCell.Item.transform.position = _secondCell.transform.position;
            
            var temp = _firstCell.Item;
            _firstCell.Item = _secondCell.Item;
            _secondCell.Item = temp;
            _onSwitched?.Invoke();
            Debug.Log("valid");

        }
        else
        {
            Debug.Log("the cells are not adjacent together");
        }
        
        // at the end
        _isSwitching = false;
        _firstCell.Renderer.color = _defaultCellColor;
        _firstCell = null;
        _secondCell = null;

    }
    private bool IsValidToSwich(Cell cellA , Cell cellB)
    {
       
        if ((cellA.Row - cellB.Row == 0 &&   Mathf.Abs(cellA.Column - cellB.Column) == 1)
            || ( Mathf.Abs(cellA.Row - cellB.Row) == 1 && cellA.Column - cellB.Column == 0))
            return true;
        
        return false;
    }
    private void Update()
    {
        if (_isSwitching) return;
        
        if (Input.GetMouseButtonUp(0))
        {
            var ray = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 ray2D = new Vector2(ray.x , ray.y);
            RaycastHit2D hit = Physics2D.Raycast(ray2D, Vector2.zero);
            if (hit.collider != null)
            {
                SelectCell(hit.collider);
            }
        }
    }

}
