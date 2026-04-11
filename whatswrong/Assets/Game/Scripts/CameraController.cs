using UnityEngine;

namespace Game.Scripts
{
    public class CameraController : MonoBehaviour, ISceneLoadHandler
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private SpriteRenderer backgroundRenderer;

        private float _moveDirection;
        private InputMapping _input;
        private Vector3 _initialPosition;

        private float _minX;
        private float _maxX;

        private void Awake()
        {
            _initialPosition = transform.position;
            CalculateBounds();
        }

        private void CalculateBounds()
        {
            if (backgroundRenderer == null)
            {
                _minX = _initialPosition.x;
                _maxX = _initialPosition.x;
                return;
            }

            Camera cam = GetComponent<Camera>();
            if (cam == null)
                cam = Camera.main;

            if (cam == null || !cam.orthographic)
            {
                _minX = _initialPosition.x;
                _maxX = _initialPosition.x;
                return;
            }

            Bounds bgBounds = backgroundRenderer.bounds;
            float cameraHalfWidth = cam.orthographicSize * cam.aspect;

            _minX = bgBounds.min.x + cameraHalfWidth;
            _maxX = bgBounds.max.x - cameraHalfWidth;

            if (_minX > _maxX)
            {
                float centerX = bgBounds.center.x;
                _minX = centerX;
                _maxX = centerX;
            }
        }

        private void ResetCamera()
        {
            Vector3 pos = _initialPosition;
            pos.x = Mathf.Clamp(pos.x, _minX, _maxX);

            transform.position = pos;
            _moveDirection = 0f;
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

            Vector3 newPosition = transform.position;
            newPosition += Vector3.right * (_moveDirection * moveSpeed * Time.deltaTime);
            newPosition.x = Mathf.Clamp(newPosition.x, _minX, _maxX);

            transform.position = newPosition;
        }

        private void OnDestroy()
        {
            if (_input == null)
                return;

            _input.MoveLeftStarted -= OnMoveLeftStarted;
            _input.MoveLeftStopped -= OnMoveLeftStopped;
            _input.MoveRightStarted -= OnMoveRightStarted;
            _input.MoveRightStopped -= OnMoveRightStopped;
        }

        public void OnBeforeSceneLoad()
        {
            ResetCamera();
        }
    }
}