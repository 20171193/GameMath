using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(SphereCollider))]
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
    [SerializeField]
    private float _detectDelay;
    private SphereCollider _detectCol;

    [Header("Debug")]
    [SerializeField]
    private int _segmentCount;
    [SerializeField]
    private int _slerpCount;
    [SerializeField]
    private Color _normalColor = Color.green;
    [SerializeField]
    private Color _detectColor = Color.red;

    [SerializeField]
    private bool isDetectEnter = false;
    [SerializeField]
    private bool isDetect = false;

    private Coroutine _detectRoutine = null;

    private void Awake()
    {
        DetectInit();
    }

    private void DetectInit()
    {
        _detectCol = GetComponent<SphereCollider>();
        _detectCol.isTrigger = true;
        _detectCol.includeLayers = _playerLM;
        _detectCol.excludeLayers = ~_playerLM;
        _detectCol.radius = _detectRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_playerLM.Contain(other.gameObject.layer) && !isDetectEnter)
        {
            isDetectEnter = true;
            if(_detectRoutine == null)
            {
                _detectRoutine = StartCoroutine(DetectRoutine(other.transform));
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (_playerLM.Contain(other.gameObject.layer) && isDetectEnter)
        { 
            if(_detectRoutine != null)
            {
                StopCoroutine(_detectRoutine);
                _detectRoutine = null;
            }

            isDetectEnter = false;
        }
    }

    private bool DetectPlayer(Transform playerTr)
    {
        Vector3 detectNormal = transform.forward;
        Vector3 toPlayerVec = playerTr.position - transform.position;
        float sqrDistane = toPlayerVec.sqrMagnitude;
        // 거리 확인
        if (_detectRange * _detectRange < sqrDistane)
            return false;

        Vector3 toPlayerNormal = toPlayerVec.normalized;
        // 단위 벡터 내적 : dot은 스칼라 값임과 동시에 코사인 값
        float dot = Vector3.Dot(detectNormal, toPlayerNormal);

        // 아크코사인으로 실제 각도 추출
        float cosAngle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (cosAngle > _detectAngle * 0.5f)
            return false;

        return true;
    }

    private IEnumerator DetectRoutine(Transform playerTr)
    {
        while(isDetectEnter)
        {
            isDetect = DetectPlayer(playerTr);
            yield return new WaitForSeconds(_detectDelay);
        }

        _detectRoutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectRange);

        if (!isDetectEnter) return;

        Gizmos.color = isDetect ? _detectColor : _normalColor;

        Vector3 center = transform.position;
        Vector3 forward = transform.forward;
        Vector3 up = transform.up;
        Vector3 right = transform.right;

        float halfAngle = _detectAngle * 0.5f;

        for (int y = 0; y <= _segmentCount; y++)
        {
            float tYaw = (float)y / _segmentCount;
            float yawAngle = Mathf.Lerp(-halfAngle, halfAngle, tYaw);
            Quaternion yawRot = Quaternion.AngleAxis(yawAngle, up);

            for (int p = 0; p <= _slerpCount; p++)
            {
                float tPitch = (float)p / _slerpCount;
                float pitchAngle = Mathf.Lerp(-halfAngle, halfAngle, tPitch);
                Quaternion pitchRot = Quaternion.AngleAxis(pitchAngle, right);

                Vector3 dir = yawRot * pitchRot * forward;
                Gizmos.DrawLine(center, center + dir.normalized * _detectRange);
            }
        }
    }
}
