using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Agent : MonoBehaviour
{
    [SerializeField]
    private LayerMask _playerLM;

    [Header("Detect")]
    [Range(0f, 360f)]
    [SerializeField]
    private float _detectAngle;
    [SerializeField]
    private float _detectRange;

    [Header("Debug")]
    [SerializeField]
    private int _segmentCount;
    [SerializeField]
    private Color _gizmosColor = Color.green;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _gizmosColor;

        Vector3 center = transform.position;
        Vector3 forward = transform.forward;
        Vector3 up = transform.up;

        Quaternion leftRot = Quaternion.AngleAxis(-_detectAngle, up);
        Vector3 leftDir = leftRot * forward;

        Quaternion rightRot = Quaternion.AngleAxis(_detectAngle, up);
        Vector3 rightDir = rightRot * forward;  

        for(int step = 1; step <= _segmentCount; step++)
        {
            Vector3 dir = Vector3.Slerp(leftDir, rightDir, (float)step / _segmentCount);
            Vector3 endPoint = center + dir * _detectRange;

            Gizmos.DrawLine(center, endPoint);
        }
        

    }
}
