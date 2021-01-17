using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _speedMove = 10f;

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

    public void Movement(Vector2 pos)
    {
        StartCoroutine(Move(pos));
    }
    
    public void Movement(Vector2 pos , float delay)
    {
        StartCoroutine(Move(pos , delay));
    }
    
    public bool IsMove { get; private set; }
    private IEnumerator Move(Vector2 pos , float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        var wait = new WaitForEndOfFrame();
        IsMove = true;

        
        while (IsMove)
        {
            transform.position = Vector3.MoveTowards(transform.position , pos , _speedMove * Time.deltaTime);
            if (Vector2.Distance(transform.position, pos) < 0.1f)
            {
                transform.position = pos;
                IsMove = false;
            }
                
            yield return wait;
        }
        
    }
    
}
