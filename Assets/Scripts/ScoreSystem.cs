using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _scoreTxt;
    private Animator _animator;
    private int _totalScore;

    public int Score
    {
        set
        {
            _totalScore += value;
            _scoreTxt.text = $"{_totalScore} ";
            _animator.SetTrigger("EarnScore");
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt("TotalScore" , _totalScore);
        Debug.Log(_totalScore);
    }

    private void OnEnable()
    {
        _totalScore = PlayerPrefs.GetInt("TotalScore" , 0);
        if(_animator == null) _animator =  _scoreTxt.GetComponent<Animator>();

        Score = 0;
    }
}
