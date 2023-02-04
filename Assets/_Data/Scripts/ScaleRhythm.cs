using DG.Tweening;
using SOA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleRhythm : MonoBehaviour
{
    [SerializeField]
    private FloatVariable timeLineBar;

    private float lastTimeLineBar;
    void Update()
    {
        if (timeLineBar % 2 == 0)
        {
            transform.DOScale(1.2f, 0.1f).SetEase(Ease.OutBounce).OnComplete(() =>
            transform.DOScale(1f, 0.1f).SetEase(Ease.OutBounce)
            );
            lastTimeLineBar = timeLineBar.value;
        }
    }
}
