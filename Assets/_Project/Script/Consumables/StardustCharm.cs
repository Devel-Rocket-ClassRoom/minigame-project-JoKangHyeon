using System;

[Serializable]
public class StardustCharm : Consumable
{
    // 일회성 구독용 핸들러 참조 — Unsubscribe 시 동일 인스턴스 필요
    Action<HandSlot> _handler;

    protected override Consumable CloneInstance() => new StardustCharm();

    public override bool OnUse(GameManager gameManager)
    {
        // 가산 유물(c_priorityDefault=100)보다 나중에 곱하도록 우선순위 50
        _handler = slot =>
        {
            slot.currentScore *= 2;
            EventBus.Unsubscribe<HandSlot>(EventType.OnSlotScored, _handler);

            // 게임오버/라운드클리어 안전 해제 구독도 함께 정리
            EventBus.Unsubscribe<object>(EventType.OnRoundClear, CancelOnRoundEnd);
            EventBus.Unsubscribe<object>(EventType.OnGameOver, CancelOnRoundEnd);
        };

        EventBus.Subscribe<HandSlot>(EventType.OnSlotScored, _handler, Relic.c_priorityGoldenMirror);

        // 점수화 없이 Round 종료/게임오버 시 구독 누수 방지
        EventBus.Subscribe<object>(EventType.OnRoundClear, CancelOnRoundEnd, Relic.c_priorityDefault);
        EventBus.Subscribe<object>(EventType.OnGameOver, CancelOnRoundEnd, Relic.c_priorityDefault);

        return true;
    }

    void CancelOnRoundEnd(object _)
    {
        EventBus.Unsubscribe<HandSlot>(EventType.OnSlotScored, _handler);
        EventBus.Unsubscribe<object>(EventType.OnRoundClear, CancelOnRoundEnd);
        EventBus.Unsubscribe<object>(EventType.OnGameOver, CancelOnRoundEnd);
    }
}
