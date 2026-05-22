using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

public class RollManager : MonoBehaviour
{    
    List<RollData> rollDatas = new();

    const int maxIterations = 1000;
    const int maxSimulations = 10;

    public bool rolling = false;

    private WaitForSeconds wait = new WaitForSeconds(0.2f);

    public IEnumerator DeterministicRoll(List<DiceObject> dices)
    {
        rolling = true;

        foreach (var dice in dices)
        {
            dice.gameObject.SetActive(true);
        }
        //물리처리는 보존되는 랜덤 요소가 아님
        Random.State preservedState = Random.state;

        rollDatas.Clear();
        //다이스 던지기
        foreach (var dice in dices)
        {
            dice.rb.isKinematic = false;
            dice.rb.linearVelocity = Random.insideUnitSphere * 3f + Vector3.up * 5f;
            dice.rb.angularVelocity = Random.insideUnitSphere * 10f;
        }

        //전부 시뮬레이션 하면 오차가 너무 크니,
        //약간 진행한 후에 시뮬레이션 진행
        yield return wait;

        Physics.simulationMode = SimulationMode.Script;
        foreach(var dice in dices)
        {
            rollDatas.Add(new RollData()
            {
                dice = dice,
                pos = dice.transform.position,
                rot = dice.transform.rotation,
                linearVelicity = dice.rb.linearVelocity,
                angularVelicity = dice.rb.angularVelocity
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

        //Debug.Log($"Real: {string.Join(", ", real.Select(i => i + 1))}");

        var predict = predicted.Select(list => list.GroupBy(x => x).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault()).ToList();
        var realResult = real.Select(i => i).ToList();
        Debug.Log($"Predicted: {string.Join(',', predict)}");
        Debug.Log($"real: {string.Join(',', realResult)}");


        if (predict.SequenceEqual(realResult))
        {
            Debug.Log("Predict success");
        }
        else
        {
            Debug.Log("Predict Failed, Realign");
            for (int i = 0; i < rollDatas.Count; i++)
            {
                RollData data = rollDatas[i];
                data.dice.TextSetOffset(real[i]);
            }
        }

        rolling = false;
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

