using Godot;

namespace Bravestone.scenes
{
    public partial class Enemy : CharacterBody3D
    {
        [Export]
        public float Speed = 7.0f;

        [Export]
        public float Gravity = -30.0f;

        [Export]
        public float DetectionRange = 10.0f;

        [Export]
        public float RotationSpeed = 2.0f;

        // Bullet throwing variables
        [Export]
        public PackedScene BulletScene;
        [Export]
        public float FireRate = 1.0f;
        [Export]
        public float BulletSpeed = 20.0f;

        private RayCast3D _leftWallRaycast;
        private RayCast3D _rightWallRaycast;
        private RayCast3D _playerRaycast;

        private Vector3 _velocity;
        private bool _playerDetected = false;
        private float _currentRotationAngle = 0f;
        private Timer _fireTimer;

        public override void _Ready()
        {
            // Setup wall detection raycasts
            _leftWallRaycast = CreateRaycast(new Vector3(0, 0, -1.0f), "LeftWall");
            _rightWallRaycast = CreateRaycast(new Vector3(0, 0, 1.0f), "RightWall");

            // Setup player detection raycast
            _playerRaycast = CreateRaycast(new Vector3(0, 0, DetectionRange), "Player");

            // Setup fire timer
            _fireTimer = new Timer();
            _fireTimer.WaitTime = 1.0 / FireRate;
            _fireTimer.OneShot = true;
            _fireTimer.Timeout += OnFireTimerTimeout;
            AddChild(_fireTimer);
        }

        private RayCast3D CreateRaycast(Vector3 targetPosition, string name)
        {
            var raycast = new RayCast3D
            {
                Name = name,
                TargetPosition = targetPosition,
                CollisionMask = 1, // Adjust as needed
                Enabled = true
            };
            AddChild(raycast);
            return raycast;
        }

        public override void _PhysicsProcess(double delta)
        {
            // Apply gravity
            if (!IsOnFloor())
                _velocity.Y += Gravity * (float)delta;
            else
                _velocity.Y = 0;

            // Rotate player detection raycast
            RotatePlayerDetectionRaycast(delta);

            // Check player detection
            UpdatePlayerDetection();

            // Only move when player is detected
            if (_playerDetected)
            {
                MoveTowardsPlayer();
                TryShootPlayer();
            }
            else
            {
                // Stop moving when player is not detected
                _velocity.Z = 0;
            }

            Velocity = _velocity;
            MoveAndSlide();
        }

        private void RotatePlayerDetectionRaycast(double delta)
        {
            // Increment rotation angle
            _currentRotationAngle += RotationSpeed * (float)delta;

            // Keep angle between 0 and 2PI
            if (_currentRotationAngle > Mathf.Tau)
                _currentRotationAngle -= Mathf.Tau;

            // Calculate new target position based on rotation
            Vector3 rotatedTarget = new Vector3(
                Mathf.Sin(_currentRotationAngle) * DetectionRange,
                0,
                Mathf.Cos(_currentRotationAngle) * DetectionRange
            );

            _playerRaycast.TargetPosition = rotatedTarget;
        }

        private void UpdatePlayerDetection()
        {
            _playerRaycast.ForceRaycastUpdate();
            _playerDetected = _playerRaycast.IsColliding();
        }

        private void MoveTowardsPlayer()
        {
            // If player is detected, move in the raycast's direction
            Vector3 rayDirection = _playerRaycast.TargetPosition.Normalized();
            _velocity.Z = rayDirection.Z * Speed;
        }

        private void TryShootPlayer()
        {
            // Only shoot if can fire and player is detected
            if (_fireTimer.IsStopped() && _playerDetected && BulletScene != null)
            {
                ShootBullet();
                _fireTimer.Start();
            }
        }

        private void ShootBullet()
        {
            // Instantiate bullet
            var bullet = BulletScene.Instantiate<RigidBody3D>();
            
            // Set bullet position (you might want to add a specific weapon position node)
            bullet.GlobalPosition = GlobalPosition + new Vector3(0, 1, 0);

            // Calculate direction to player
            Vector3 shootDirection = _playerRaycast.TargetPosition.Normalized();
            
            // Set bullet velocity
            bullet.LinearVelocity = shootDirection * BulletSpeed;

            // Add bullet to scene
            GetParent().AddChild(bullet);
        }

        private void OnFireTimerTimeout()
        {
            // Timer will restart when TryShootPlayer is called
        }
    }
}