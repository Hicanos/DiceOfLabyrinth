using System.Collections.Generic;
using UnityEngine;

namespace PredictedDice
{
    [AddComponentMenu("")]
    public class DiceSimulation : MonoBehaviour
    {
        private FaceMap _faceMap;
        private DiceLocomotion _locomotion;
        
        private List<Pose> _trajectory = new List<Pose>();
        public List<Pose> Trajectory => _trajectory;
        public Collider Collider => _locomotion.Collider;
        public Pose GetPose() => _locomotion.GetPose();
        public void Setup(FaceMap faceMap)
        {
            _faceMap = faceMap;
            _locomotion = GetComponent<DiceLocomotion>();
        }
        public void Roll(Vector3 force, Vector3 torque)
        {
            _locomotion.Roll(force, torque);
        }
        public void GetPoseOnStep()
        {
            _trajectory.Add(_locomotion.GetPose());
        }

        public void ResetSimulationDice(Dice dice)
        {
            _trajectory.Clear();
            _locomotion.ResetDice(dice.GetPose());
            _faceMap = dice.faceMap;
        }

        public void Destroy()
        {
            if(!gameObject) return;
            Destroy(gameObject);
        }
        public int OutCome()
        {
            var face = _faceMap.FaceLookingUp(transform.localToWorldMatrix);
            return face;
        }
    }
}