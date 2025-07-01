using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PredictedDice
{
    [AddComponentMenu("")]
    public class DiceLocomotion : MonoBehaviour
    {
        private Pose _startPose;
        private Rigidbody _rb;
        public Rigidbody Rb => _rb;
        
        private Collider _collider;

        public Collider Collider
        {
            get
            {
                if (_collider == null)
                {
                    _collider = GetComponent<Collider>();
                }
                return _collider;
            }
        }
        public bool isEnd = false;
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
            var transformCache = transform;
            _startPose = new Pose(transformCache.position, transformCache.rotation);
        }
        public IEnumerator Play(List<Pose> projectory)
        {
            Pose[] poses = new Pose[projectory.Count];
            //Vector3[] testPosition = new Vector3[] { new Vector3(-2.3f, 54.5f, -17.5f), new Vector3(-3.58f, 54.5f, -16.86f), new Vector3(0.08f, 54.5f, -23.41f), new Vector3(-3, 54.5f, -24.73f), new Vector3(-4.14f, 54.5f, -19.8f) };
            projectory.CopyTo(poses);
            var wait = new WaitForFixedUpdate();
            foreach (var pose in poses)
            {
                _rb.MovePosition(pose.position);
                _rb.MoveRotation(pose.rotation);
                yield return wait;
            }
            isEnd = true;
        }
        public void ResetDice(Pose pose)
        {
            _rb.isKinematic = true;
            _rb.position = pose.position;
            _rb.rotation = pose.rotation;
        }
        public void Roll(Vector3 force, Vector3 torque)
        {
            _rb.isKinematic = false;
            _rb.AddForce(force, ForceMode.Impulse);
            _rb.AddTorque(torque, ForceMode.Impulse);
        }
        public Pose GetPose() => new(_rb.position, _rb.rotation);
    }
}