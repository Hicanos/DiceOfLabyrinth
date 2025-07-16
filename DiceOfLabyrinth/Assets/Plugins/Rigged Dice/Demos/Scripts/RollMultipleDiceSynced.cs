using NUnit.Framework.Constraints;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
//using UnityEngine.InputSystem;

namespace PredictedDice.Demo
{
    public class RollMultipleDiceSynced : MonoBehaviour
    {
        //[SerializeField] int x;
        //[SerializeField] int y;
        //[SerializeField] int z;

        [Serializable]
        public struct DiceAndOutcome
        {
            public Dice dice;
            public bool random;
            [UnityEngine.Range(1, 6)] public int outcome;
        }
        float currentTime = 0;
        float lerpTime = 2.5f;
        [field: SerializeField] public DiceAndOutcome[] diceAndOutcomeArray { get; private set; }

        //private InputSystem_Actions _inputActions;

        //private void Awake()
        //{
        //    _inputActions = new InputSystem_Actions();
        //}

        public void SetDiceOutcome(int[] outcome)
        {
            for (int i = 0; i < diceAndOutcomeArray.Length; i++)
            {
                diceAndOutcomeArray[i].outcome = outcome[i];
            }
        }        

        public void RollAll()
        {
            foreach (var diceAndOutcome in diceAndOutcomeArray)
            {
                if (diceAndOutcome.dice == null) continue;

                diceAndOutcome.dice.RollDiceWithOutCome(
                    GetRandomForcedRollData(diceAndOutcome.random ? RollData.RandomFace : diceAndOutcome.outcome));
            }

            ProjectionSceneManager.Instance.Simulate();
            foreach (DiceAndOutcome diceAndOutcome in diceAndOutcomeArray)
            {
                if (diceAndOutcome.dice == null) continue;
                diceAndOutcome.dice.PlaySimulation();
            }
        }
        
        private RollData GetRandomForcedRollData(int outcome = RollData.RandomFace)
        {
            return new RollData
            {
                faceValue = outcome,
                force = GetRandomForce(),
                torque = GetRandomTorque()
            };
        }
        private Vector3 GetRandomForce()
        {
            return new Vector3(Random.Range(1, 3), Random.Range(4, 6), Random.Range(6, 9));
            //return new Vector3(x, y, z);
        }

        private Vector3 GetRandomTorque()
        {
            return new Vector3(Random.Range(2, 7), Random.Range(2, 7), Random.Range(2, 7));
            //return new Vector3(x, y, z);
        }

        //private void OnTapPerformed(InputAction.CallbackContext context)
        //{
        //    RollAll();
        //}

        //private void OnEnable()
        //{
        //    _inputActions.Enable();
        //    _inputActions.Player.GamePlay.performed += OnTapPerformed;
        //}

        //private void OnDisable()
        //{
        //    _inputActions.Player.GamePlay.performed -= OnTapPerformed;
        //    _inputActions.Disable();
        //}
    }
}