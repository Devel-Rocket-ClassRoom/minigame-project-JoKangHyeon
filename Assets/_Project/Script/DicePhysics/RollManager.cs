using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RollManager : MonoBehaviour
{    
    List<RollData> rollDatas = new();

    const int maxIterations = 1000;
    const int maxSimulations = 10;

    

    public IEnumerator DeterministicRoll(List<DiceObject> dices)
    {
        foreach (var dice in dices)
        {
            dice.gameObject.SetActive(true);
        }
        //물리처리는 보존되는 랜덤 요소가 아님
        Random.State preservedState = Random.state;

        rollDatas.Clear();
        Physics.simulationMode = SimulationMode.Script;

        //Setup Dice
        foreach (var dice in dices)
        {
            dice.rb.isKinematic = false;

            rollDatas.Add(new RollData
            {
                dice = dice,
                pos = dice.transform.position,
                rot = dice.transform.rotation,
                linearVelicity = Random.insideUnitSphere * 3f + Vector3.up * 5f,
                angularVelicity = Random.insideUnitSphere * 10f
            });
        }

        List<List<int>> predicted = new();

        for (int i = 0; i < rollDatas.Count; i++)
        {
            predicted.Add(new List<int>());
        }

        for (int simulation = 0; simulation < maxSimulations; simulation++)
        {
            for (int i = 0; i < rollDatas.Count; i++)
            {
                RollData data = rollDatas[i];
                data.dice.transform.position = data.pos;
                data.dice.transform.rotation = data.rot;
                data.dice.rb.linearVelocity = data.linearVelicity;
                data.dice.rb.angularVelocity = data.angularVelicity;
            }

            //Simulate
            for (int i = 0; i < maxIterations; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
                if (rollDatas.All(data => data.dice.rb.IsSleeping()))
                    break;
            }

            for (int i = 0; i < rollDatas.Count; i++)
            {
                RollData dice = rollDatas[i];
                int simulatedIndex = -1;
                float simulatedDot = -255f;
                for (int j = 0; j < dice.dice.faces.Count; j++)
                {
                    float dot = Vector3.Dot(dice.dice.faces[j].transform.up, Vector3.up);
                    if (dot > simulatedDot)
                    {
                        simulatedDot = dot;
                        simulatedIndex = j;
                    }
                }
                predicted[i].Add(simulatedIndex);
            }
        }

        List<int> simulationResult = new List<int>();

        //Reset
        for (int i = 0; i < rollDatas.Count; i++)
        {
            RollData data = rollDatas[i];
            data.dice.transform.position = data.pos;
            data.dice.transform.rotation = data.rot;
            data.dice.rb.linearVelocity = data.linearVelicity;
            data.dice.rb.angularVelocity = data.angularVelicity;
            data.dice.TextSetOffset(predicted[i].GroupBy(x => x)
                       .OrderByDescending(g => g.Count())
                       .Select(g => g.Key)
                       .FirstOrDefault());
        }

        Physics.simulationMode = SimulationMode.FixedUpdate;

        yield return new WaitUntil(() => rollDatas.All(data => data.dice.rb.IsSleeping()));

        List<int> real = new();
        foreach (var dice in rollDatas)
        {
            int maxIndex = -1;
            float maxDot = -255f;
            for (int j = 0; j < dice.dice.faces.Count; j++)
            {
                float dot = Vector3.Dot(dice.dice.faces[j].transform.up, Vector3.up);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    maxIndex = j;
                }
            }
            real.Add(maxIndex);
        }

        Debug.Log($"Predicted: {string.Join(", ", predicted.Select(list => list.GroupBy(x => x).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault() + 1))}");
        Debug.Log($"Real: {string.Join(", ", real.Select(i => i + 1))}");


    }

    struct RollData
    {
        public DiceObject dice;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 linearVelicity;
        public Vector3 angularVelicity;
    }
}

