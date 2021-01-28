using System.Collections;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SwitchItemController : MonoBehaviour
{
    public delegate void SwitchDelegate();

    private Camera _camera;
    private bool _isSwitching;
    private Color _defaultCellColor;
    private Cell _firstCell;
    private Cell _secondCell;
    private event SwitchDelegate _onSwitched;
    
    public SwitchDelegate OnSwitched
    {
        get { return _onSwitched; }
        set { _onSwitched = value; }
    }
    public bool IsActive { get; set; }
    

    private void Start()
    {
        _camera = FindObjectOfType<Camera>();
        IsActive = true;
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
        if(IsValidToSwitch(_firstCell , _secondCell))
        {

            StartCoroutine(Movement(_firstCell.Item.transform, _secondCell.Item.transform , _onSwitched));
            var temp = _firstCell.Item;
            _firstCell.Item = _secondCell.Item;
            _secondCell.Item = temp;
            IsActive = false;

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
    private bool IsValidToSwitch(Cell cellA , Cell cellB)
    {
       
        if ((cellA.Row - cellB.Row == 0 &&   Mathf.Abs(cellA.Column - cellB.Column) == 1)
            || ( Mathf.Abs(cellA.Row - cellB.Row) == 1 && cellA.Column - cellB.Column == 0))
            return true;
        
        return false;
    }

    private IEnumerator Movement(Transform firstItem ,Transform secondItem , SwitchDelegate onSwitched = null)
    {
        var pos1 = firstItem.position;
        var pos2 = secondItem.position;
        var speed = 5f * Time.deltaTime;
        var delay = new WaitForSeconds(0.3f);
        onSwitched = _onSwitched;
        
        while (true)
        {
            secondItem.position = Vector3.MoveTowards(secondItem.position , pos1 , speed);
            firstItem.position = Vector3.MoveTowards(firstItem.position , pos2 , speed);

            // at the end of movement
            if (Vector3.Distance(firstItem.position, pos2) < 0.1f)
            {
                firstItem.position = pos2;
                secondItem.position = pos1;
                yield return delay;
                onSwitched?.Invoke();
                Debug.Log("valid");
                yield break;
            }

            yield return null;
        }
        

        
    }

    public void RevertSwitching()
    {
        /*
        // movement items to the first pos
        StartCoroutine(Movement(_firstItem, _secondItem ));
        IsActive = true;
        var temp = _firstCell.Item;
        _firstCell.Item = _secondCell.Item;
        _secondCell.Item = temp;
        */
    }

    private void Update()
    {
        if (!IsActive) return;
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
