using UnityEngine;

namespace Game.Scripts
{
    public class CameraController : MonoBehaviour, ISceneLoadHandler
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private float dragSensitivity = 1f;

        private bool _isDragging;
        private Camera _camera;

        private float _moveDirection;
        private NavigationInput _navigationInput;
        private Vector3 _initialPosition;

        private float _minX;
        private float _maxX;

        private void Awake()
        {
            _initialPosition = transform.position;
            _camera = GetComponent<Camera>();
            if (_camera == null)
                _camera = Camera.main;
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

        public void Initialize(NavigationInput navigationInput)
        {
            if (_navigationInput == navigationInput)
                return;

            UnsubscribeInput();
            _navigationInput = navigationInput;

            if (_navigationInput == null)
                return;

            _navigationInput.MoveLeftStarted += OnMoveLeftStarted;
            _navigationInput.MoveLeftStopped += OnMoveLeftStopped;
            _navigationInput.MoveRightStarted += OnMoveRightStarted;
            _navigationInput.MoveRightStopped += OnMoveRightStopped;
            _navigationInput.DragStarted += OnDragStarted;
            _navigationInput.DragDelta += OnDragDelta;
            _navigationInput.DragEnded += OnDragEnded;
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

        private void OnDragStarted()
        {
            _isDragging = true;
            _moveDirection = 0f; // stop keyboard scrolling while dragging
        }

        private void OnDragEnded()
        {
            _isDragging = false;
        }

        private void OnDragDelta(float deltaPixels)
        {
            if (!_isDragging || _camera == null)
                return;

            float pixelsPerWorldUnit = Screen.width / (_camera.orthographicSize * 2f * _camera.aspect);

            float deltaWorld = deltaPixels / pixelsPerWorldUnit;

            Vector3 pos = transform.position;

            // Invert if you want "grab and drag" behavior.
            pos.x -= deltaWorld * dragSensitivity;

            pos.x = Mathf.Clamp(pos.x, _minX, _maxX);

            transform.position = pos;
        }
        
        private void Update()
        {
            if (_isDragging)
                return;
            
            if (_moveDirection == 0f)
                return;

            Vector3 newPosition = transform.position;
            newPosition += Vector3.right * (_moveDirection * moveSpeed * Time.deltaTime);
            newPosition.x = Mathf.Clamp(newPosition.x, _minX, _maxX);

            transform.position = newPosition;
        }

        private void OnDestroy()
        {
            UnsubscribeInput();
        }

        private void UnsubscribeInput()
        {
            if (_navigationInput == null)
                return;

            _navigationInput.MoveLeftStarted -= OnMoveLeftStarted;
            _navigationInput.MoveLeftStopped -= OnMoveLeftStopped;
            _navigationInput.MoveRightStarted -= OnMoveRightStarted;
            _navigationInput.MoveRightStopped -= OnMoveRightStopped;
            _navigationInput.DragStarted -= OnDragStarted;
            _navigationInput.DragDelta -= OnDragDelta;
            _navigationInput.DragEnded -= OnDragEnded;
            _navigationInput = null;
        }

        public void OnBeforeSceneLoad()
        {
            ResetCamera();
        }
    }
}
