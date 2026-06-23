using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DiceObjectPool
{
    static DiceObjectPool _instance;
    public static DiceObjectPool Instance
    {
        get
        {
            if( _instance == null )
                _instance = new DiceObjectPool();

            if (_instance.gameManager==null)
                _instance.gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();

            return _instance;
        }
    }

    Dictionary<Type, IObjectPool<DiceObject>> pool;
    GameManager gameManager;

    private DiceObjectPool()
    {
        pool = new();
    }

    public DiceObject GetDiceObject(Dice dice)
    {
        Type type = dice.GetType();
        if (!pool.ContainsKey(type))
        {
            pool.Add(dice.GetType(), new ObjectPool<DiceObject>(
                createFunc: () => CreatePooledObject(dice.nameStringKey),
                actionOnGet: (d)=>ActionOnGet(d, type),
                actionOnRelease: ActionOnRelease
                ));
        }

        return pool[dice.GetType()].Get();
    }

    private DiceObject CreatePooledObject(string key)
    {
        return UnityEngine.Object.Instantiate(gameManager.diceDefine.Find(key).prefab,gameManager.DiceSpawnPoint.transform);
    }

    private void ActionOnGet(DiceObject diceObject, Type key) {
        diceObject.pool = pool[key];
    }

    private void ActionOnRelease(DiceObject diceObject)
    {
        diceObject.Dice = null;
        diceObject.pool = null;
        diceObject.rb.linearVelocity = Vector3.zero;
        diceObject.rb.angularVelocity = Vector3.zero;
        diceObject.gameObject.SetActive(false);
    }
}
