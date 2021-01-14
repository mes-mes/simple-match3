using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class SwitchItemController : MonoBehaviour
{
    private Camera _camera;

    private bool _isSwitching;

    private void Start()
    {
        _camera = FindObjectOfType<Camera>();
    }

    void Update()
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

    private Cell _firstCell;
    private Cell _secondCell;
    
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
            _firstCell.Renderer.color = Color.green;
        }
    }


    
    private void Switch()
    {
        if(IsValidToSwich(_firstCell , _secondCell))
        {
            /*
            Vector2 temp = _firstCell.transform.position;
            _firstCell.transform.position = _secondCell.transform.position;
            _secondCell.transform.position = temp;
            */
            
            // for debugging  ******************************

            var rowTemp = _firstCell.Row;
            var colTemp = _firstCell.Column;
            _firstCell.Init(_secondCell.Row , _secondCell.Column); 
            _secondCell.Init(rowTemp , colTemp); 
            
        }
        else
        {
            Debug.Log("the cells are not adjacent together");
        }
        
        // at the end
        _isSwitching = false;
        _firstCell.Renderer.color = Color.red;
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
}
