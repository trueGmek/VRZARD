using System.Collections.Generic;
using UnityEngine;

namespace Telekinesis {
    public class TelekinesisObject : MonoBehaviour {
        private const float FRAME_RATE = 0.0166f;
        public DrawnToPoint drawnToPoint;
        public ForceMode forceMode;
        [Header("PID coefficients")] public float proportionalCoefficient = 100;
        public float integralCoefficient = 2;
        public float derivativeCoefficient = 10;

        [Header("\n")] public int integrationSampleSize = 12;
        [SerializeField] private bool isActive = true;
        private readonly List<Vector3> _pastErrors = new List<Vector3>();

        private Rigidbody _rigidbody;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            var position = transform.position;
            var drawnToPointGameObject = new GameObject("DrawnToPoint");
            drawnToPointGameObject.transform.position = new Vector3(position.x, position.y, position.z);
            drawnToPoint = drawnToPointGameObject.AddComponent<DrawnToPoint>();
            drawnToPoint.MovePositionBy(new Vector3(0, 0.5f, 0));
        }

        private void FixedUpdate() {
            if (isActive) _rigidbody.AddForce(GetForceFromPID(), forceMode);
        }

        public void Activate() {
            isActive = true;
        }

        public void Deactivate() {
            isActive = false;
        }

        public bool IsActive() {
            return isActive;
        }

        // ReSharper disable once InconsistentNaming PID that's the name
        private Vector3 GetForceFromPID() {
            var error = drawnToPoint.transform.position - transform.position;
            _pastErrors.Add(error);
            var proportional = error * proportionalCoefficient;
            var integral = GetRiemannSum(integrationSampleSize) * integralCoefficient;
            var differential = GetDerivative() * derivativeCoefficient;
            return proportional + integral + differential;
        }

        private Vector3 GetDerivative() {
            if (_pastErrors.Count < 2) return Vector3.zero;
            var derivative = _pastErrors[_pastErrors.Count - 1] - _pastErrors[_pastErrors.Count - 2];
            derivative /= FRAME_RATE;
            return derivative;
        }

        private Vector3 GetRiemannSum(int sampleLimit = 20) {
            var riemannSum = new Vector3(0, 0, 0);
            var startIndex = _pastErrors.Count < sampleLimit
                ? _pastErrors.Count - 1
                : _pastErrors.Count - sampleLimit;
            for (var i = startIndex; i < _pastErrors.Count; i++) riemannSum += _pastErrors[i] * FRAME_RATE;

            return riemannSum;
        }
    }
}