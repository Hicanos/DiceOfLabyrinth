using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PredictedDice.Demo
{
    public class RollMultipleDiceSynced : MonoBehaviour
    {
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

        public void SetDiceOutcome(int[] outcome)
        {
            for (int i = 0; i < diceAndOutcomeArray.Length; i++)
            {
                diceAndOutcomeArray[i].outcome = outcome[i];
            }
        }
        //public void SetWhiteDiceOutcome(float outcome)
        //{
        //    diceAndOutcomeArray[0].outcome = (int)outcome;
        //}

        //public void SetBlueDiceOutcome(float outcome)
        //{
        //    diceAndOutcomeArray[1].outcome = (int)outcome;
        //}

        //public void SetRedDiceOutcome(float outcome)
        //{
        //    diceAndOutcomeArray[2].outcome = (int)outcome;
        //}
        //public void SetGreenDiceOutcome(float outcome)
        //{
        //    diceAndOutcomeArray[3].outcome = (int)outcome;
        //}
        //public void SetPurpleDiceOutcome(float outcome)
        //{
        //    diceAndOutcomeArray[4].outcome = (int)outcome;
        //}

        public void RollAll()
        {
            foreach (var diceAndOutcome in diceAndOutcomeArray)
            {
                if (diceAndOutcome.dice == null) continue;

                diceAndOutcome.dice.RollDiceWithOutCome(
                    GetRandomForcedRollData(diceAndOutcome.random ? RollData.RandomFace : diceAndOutcome.outcome));
            }

            //StartCoroutine(SLerp());

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
                torque = GetRandomForce()
            };
        }
        private Vector3 GetRandomForce()
        {
            return new Vector3(Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10));
        }

        IEnumerator SLerp()
        {
            foreach (DiceAndOutcome diceAndOutcome in diceAndOutcomeArray)
            {
                if (diceAndOutcome.dice == null) continue;
                Vector3 vec = diceAndOutcome.dice.gameObject.transform.position;
                Vector3 vec2 = vec + new Vector3(10, 0, 10);
                while (currentTime < lerpTime)
                {
                    currentTime += Time.deltaTime;

                    if (currentTime >= lerpTime)
                    {
                        currentTime = lerpTime;
                    }
                    
                    diceAndOutcome.dice.gameObject.transform.position = Vector3.Slerp(vec, vec2, currentTime / lerpTime);
                    
                    yield return null;
                }
            }
            Debug.Log("코루틴 종료");
        }
    }
}