using System;
using System.Collections.Generic;

public enum EventType
{
    /// <summary>
    /// 사이클 시작 시
    /// return : int HandNumber
    /// </summary>
    OnCycleStart,
    /// <summary>
    /// 이번 사이클의 첫번째 주사위 굴림 후
    /// return : List Dice
    /// </summary>
    OnFirstRollComplete,
    /// <summary>
    /// 주사위 굴림 완료 후
    /// return : List Dice
    /// </summary>
    OnRollComplete,
    /// <summary>
    /// 족보 슬롯에 점수가 입력되었을 때
    /// return : HandSlot
    /// </summary>
    OnSlotScored,
    /// <summary>
    /// 족보 슬롯에 점수가 최종 결정되었을 때,
    /// return : HandSlot
    /// </summary>
    OnSlotScoreFixed,
    /// <summary>
    /// 이번 라운드의 첫 점수 활성화시
    /// return : HandSlot
    /// </summary>
    OnFirstScoreOfRound,
    /// <summary>
    /// 라운드 시작 시
    /// return : null
    /// </summary>
    OnRoundStart,
    /// <summary>
    /// 라운드 성공 시
    /// retrun : null
    /// </summary>
    OnRoundClear,
    /// <summary>
    /// 사이클 종료 시
    /// return : null
    /// </summary>
    OnCycleEnd,
    /// <summary>
    /// 게임오버 시
    /// return : null
    /// </summary>
    OnGameOver,
}


public static class EventBus
{
    private static readonly Dictionary<EventType, List<PriorityCallback>> eventTable = new();
    private static readonly Dictionary<EventType, Dictionary<Delegate, PriorityCallback>> delegateLookup = new();

    public static void Subscribe<T>(EventType eventType, Action<T> callback, int priority)
    {
        if (callback == null) return;

        if (!delegateLookup.TryGetValue(eventType, out var map))
        {
            map = new();
            delegateLookup[eventType] = map;
        }

        if (map.ContainsKey(callback)) return;

        Action<object> wrapper = (obj) => callback((T)obj);
        map[callback] = new PriorityCallback { Callback = wrapper, Priority = priority };

        if (!eventTable.TryGetValue(eventType, out var existing) || existing == null)
        {
            eventTable[eventType] = new List<PriorityCallback> { map[callback] };
        }
        else
        {
            eventTable[eventType].Add(map[callback]);
        }

        eventTable[eventType].Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    public static void Unsubscribe<T>(EventType eventType, Action<T> callback)
    {
        if (callback == null) return;

        if (!delegateLookup.TryGetValue(eventType, out var map)) return;
        if (!map.TryGetValue(callback, out var wrapper)) return;

        map.Remove(callback);

        if (eventTable.TryGetValue(eventType, out var existing))
        {
            if (existing == null)
                return;
            existing.Remove(wrapper);
        }
    }

    public static void Publish(EventType eventType, object eventData)
    {
        if (eventTable.TryGetValue(eventType, out var action))
        {
            action?.ForEach(cb => cb.Callback(eventData));
        }
    }

    private class PriorityCallback
    {
        public int Priority { get; set; }
        public Action<object> Callback { get; set; }
    }
}