using UnityEngine;

/// <summary> Makes an object move back and forth between two points relative to its start position. </summary>
public class MovingTarget : MonoBehaviour {
    [Header("Movement settings")]
    [Tooltip("Local offset from the starting position for point A")]
    [SerializeField] private Vector3 localOffsetA = new(0, 0, -2);
    [Tooltip("Local offset from the starting position for point B")]
    [SerializeField] private Vector3 localOffsetB = new(0, 0, 2);
    [Tooltip("Speed of back-and-forth movement")]
    [SerializeField] private float moveSpeed = 2f;
    [Tooltip("Pause duration at each end (seconds)")]
    [SerializeField] private float pauseDuration;

    private Vector3 _startPos;
    private Vector3 _pointA;
    private Vector3 _pointB;
    private Vector3 _target;
    private bool _movingToB = true;
    private float _pauseTimer;

    void Start() {
        _startPos = transform.position;
        _pointA = _startPos + localOffsetA;
        _pointB = _startPos + localOffsetB;
        _target = _pointB;
    }

    void Update() {
        if (_pauseTimer > 0f) { _pauseTimer -= Time.deltaTime; return; }

        transform.position = Vector3.MoveTowards(transform.position, _target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _target) < 0.01f) {
            _target = _movingToB ? _pointA : _pointB;
            _movingToB = !_movingToB;
            if (pauseDuration > 0f) _pauseTimer = pauseDuration;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Vector3 startPos = Application.isPlaying ? _startPos : transform.position;
        Vector3 a = startPos + localOffsetA;
        Vector3 b = startPos + localOffsetB;
        Gizmos.DrawSphere(a, 0.1f);
        Gizmos.DrawSphere(b, 0.1f);
        Gizmos.DrawLine(a, b);
    }
}
