using BezierSolution;
using DG.Tweening;
using UnityEngine;

namespace TestLocation
{
    public class PlayerController : MonoBehaviour
    {
        private const float MINIMAL_SPEED = 3.0f;

        private BezierWalkerWithSpeed _bezier;

        private float _acceleration;
        private float _maxSpeed;
        private float _baseMobility;
        private float _mobility;

        private Vector3 _droneTargetPosition = Vector3.zero;
        private bool _isGameRun;
        private Sequence _sequence;

        private bool _firstGestureDone = false;

        private void OnEnable()
        {
            InputManager.OnGesture += OnGesture;
        }

        private void OnDisable()
        {
            InputManager.OnGesture -= OnGesture;
        }

        private void Awake()
        {
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _sequence = DOTween.Sequence();
        }

        private void OnGesture(Vector2 vector)
        {
            if (!_firstGestureDone) {
                _firstGestureDone = true;
                OnStartGame();
                return;
            }
            Vector3 swipe = new Vector3(vector.x, vector.y, 0f);
            Vector3 newPosition = NewPosition(_droneTargetPosition, swipe);
            if (_droneTargetPosition.Equals(newPosition)) {
                return;
            }
            DotWeenMove(newPosition);
        }

        private Vector3 NewPosition(Vector3 dronPos, Vector3 swipe)
        {
            Vector3 newPos = dronPos + swipe;
            if (newPos.x > 1.0f) {
                swipe.x = 0.0f;
            }
            if (newPos.x < -1.0f) {
                swipe.x = 0.0f;
            }
            if (newPos.y > 1.0f) {
                swipe.y = 0.0f;
            }
            if (newPos.y < -1.0f) {
                swipe.y = 0.0f;
            }
            Vector3 newPosition = dronPos + swipe;
            return newPosition;
        }

        private void DotWeenMove(Vector3 newPos)
        {
            _mobility = _baseMobility * (MINIMAL_SPEED / _bezier.speed);
            Vector3 rotation = new Vector3(_droneTargetPosition.y - newPos.y, transform.localRotation.y, _droneTargetPosition.x - newPos.x) * 45;
            _droneTargetPosition = newPos;
            _sequence.Append(transform.DOLocalMove(newPos, _mobility))
                     .Join(transform.DOLocalRotate(rotation, _mobility).OnComplete(() => { transform.DOLocalRotate(Vector3.zero, _mobility); }));
        }

        private void Start()
        {
            _bezier.enabled = false;
            _bezier.speed = MINIMAL_SPEED;
            _maxSpeed = 10f;
            _acceleration = 1.8f;
            _baseMobility = 0.9f;
        }

        private void OnStartGame()
        {
            _isGameRun = true;
            _bezier.enabled = true;
        }

        public void Update()
        {
            if (!_isGameRun) {
                return;
            }
            SetBezierSpeed();
        }

        private void SetBezierSpeed()
        {
            if (_bezier.speed >= _maxSpeed) {
                return;
            }
            float newSpeed = _bezier.speed + _acceleration * Time.deltaTime;
            _bezier.speed = newSpeed > _maxSpeed ? _maxSpeed : newSpeed;
        }
    }
}