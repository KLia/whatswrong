using UnityEngine;

namespace Game.Scripts
{
    public class CameraController : MonoBehaviour, ISceneLoadHandler
    {
        [SerializeField] private float moveSpeed = 5f;
       
        private float _moveDirection;
        private InputMapping _input;
        private Vector3 _initialPosition;

        private void Awake()
        {
            _initialPosition = transform.position;
        }

        private void ResetCamera()
        {
            transform.position = _initialPosition;
            _moveDirection = 0f; // optional: stop movement
        }

        public void Initialize(InputMapping inputMapping)
        {
            if (_input == inputMapping)
                return;

            UnsubscribeInput();
            _input = inputMapping;

            if (_input == null)
                return;

            _input.MoveLeftStarted += OnMoveLeftStarted;
            _input.MoveLeftStopped += OnMoveLeftStopped;
            _input.MoveRightStarted += OnMoveRightStarted;
            _input.MoveRightStopped += OnMoveRightStopped;
        }

        public void OnMoveLeftStarted()
        {
            _moveDirection = -1f;
        }

        public void OnMoveLeftStopped()
        {
            if (_moveDirection < 0f)
                _moveDirection = 0f;
        }

        public void OnMoveRightStarted()
        {
            _moveDirection = 1f;
        }

        public void OnMoveRightStopped()
        {
            if (_moveDirection > 0f)
                _moveDirection = 0f;
        }

        private void Update()
        {
            if (_moveDirection == 0f)
                return;

            transform.position += Vector3.right * (_moveDirection * moveSpeed * Time.deltaTime);
        }

        private void OnDestroy()
        {
            UnsubscribeInput();
        }

        private void UnsubscribeInput()
        {
            if (_input == null)
                return;

            _input.MoveLeftStarted -= OnMoveLeftStarted;
            _input.MoveLeftStopped -= OnMoveLeftStopped;
            _input.MoveRightStarted -= OnMoveRightStarted;
            _input.MoveRightStopped -= OnMoveRightStopped;
            _input = null;
        }

        public void OnBeforeSceneLoad()
        {
            ResetCamera();
        }
    }
}
