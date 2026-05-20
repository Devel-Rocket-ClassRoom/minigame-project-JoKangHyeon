using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RollManager : MonoBehaviour
{
    public List<DiceObject> dices;
    
    List<RollData> rollDatas = new();

    const int maxIterations = 1000;
    const int maxSimulations = 10;

    private void Start()
    {
        
        StartCoroutine(DeterministicRoll(new()
        {
            new(){"1","2","3","4","5","6"},
            new(){"1","2","3","4","5","6"},
            new(){"1","2","3","4","5","6"},
            new(){"1","2","3","4","5","6"},
            new(){"1","2","3","4","5","6"},
            new(){"1","2","3","4","5","6"},
        }, new() { 1,2,3,4,5,6 }));

        //StartCoroutine(PredictRoll());
    }

    public IEnumerator PredictRoll()
    {
        //물리처리는 보존되는 랜덤 요소가 아님
        //Random.State preservedState = Random.state;

        rollDatas.Clear();
        Physics.simulationMode = SimulationMode.Script;

        //Setup Dice
        foreach(var dice in dices)
        {
            dice.rb.isKinematic = false;
            dice.rb.linearVelocity = Random.insideUnitSphere * 3f;
            dice.rb.angularVelocity = Random.insideUnitSphere * 10f;

            rollDatas.Add(new RollData
            {
                dice = dice,
                pos = dice.transform.position,
                rot = dice.transform.rotation,
                linearVelicity = dice.rb.linearVelocity,
                angularVelicity = dice.rb.angularVelocity
            });
        }

        //Simulate
        for (int i = 0; i < maxIterations; i++)
        {
            Physics.Simulate(Time.fixedDeltaTime);
            if (rollDatas.All(data => data.dice.rb.IsSleeping()))
                break;
        }

        List<int> predicted = new();
        foreach (var dice in rollDatas)
        {
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
            predicted.Add(simulatedIndex);
        }

        //Reset
        foreach (var data in rollDatas)
        {
            data.dice.transform.position = data.pos;
            data.dice.transform.rotation = data.rot;
            data.dice.rb.linearVelocity = data.linearVelicity;
            data.dice.rb.angularVelocity = data.angularVelicity;
        }

        Physics.simulationMode = SimulationMode.FixedUpdate;
        
        //if ()
                
        yield return new WaitUntil(()=>rollDatas.All(data => data.dice.rb.IsSleeping()));

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

        Debug.Log($"Predicted: {string.Join(", ", predicted.Select(i => i + 1))}");
        Debug.Log($"Real: {string.Join(", ", real.Select(i => i + 1))}");
    }

    public IEnumerator DeterministicRoll(List<List<string>> faces, List<int> values)
    {
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
                linearVelicity = Random.insideUnitSphere * 3f,
                angularVelicity = Random.insideUnitSphere * 10f
            });
        }



        List<List<int>> predicted = new();

        for(int i=0; i < rollDatas.Count; i++)
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
            data.dice.TextSet(
                faces[i], 
                predicted[i].GroupBy(x => x)
                       .OrderByDescending(g => g.Count())
                       .Select(g => g.Key)
                       .FirstOrDefault(), 
                values[i]);
        }

        Physics.simulationMode = SimulationMode.FixedUpdate;

        //if ()

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

