using UnityEngine;

namespace Game.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
       
        private float _moveDirection;
        private InputMapping _input;
        private Camera _camera;

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        public void Initialize(InputMapping inputMapping)
        {
            _input = inputMapping;
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
            _input.MoveLeftStarted -= OnMoveLeftStarted;
            _input.MoveLeftStopped -= OnMoveLeftStopped;
            _input.MoveRightStarted -= OnMoveRightStarted;
            _input.MoveRightStopped -= OnMoveRightStopped;
        }
    }
}