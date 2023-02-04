using DG.Tweening;
using SOA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RotationRhythm : MonoBehaviour
{
    [SerializeField]
    private float velocityRotation;
    [SerializeField]
    private FloatVariable timeLineBar;

    private bool rotated;

    void Update()
    {
        if (timeLineBar % 2 == 0 && !rotated)
        {
            rotated = true;
            transform.DORotateQuaternion(Quaternion.AngleAxis(velocityRotation, Vector3.forward), 0.2f).SetRelative(true).SetEase(Ease.Linear);
            //transform.Rotate(new Vector3(0, 0, 30)); //= Quaternion.AngleAxis(transform.rotation.z + 30, Vector3.forward);
        }

        if (timeLineBar % 2 != 0)
        {
            rotated = false;
        }
    }
}
