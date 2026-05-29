using System;
using System.Linq;
using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
// F1  CardP1 — 펜타클 에이스 (1면 → 6면 자동 변환)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardP1 : Card
{
    public GameManager gameManager;

    protected override Card CloneInstance() => new CardP1
    {
    };

    // §3.1 — 1면 가진 주사위가 1개 이상이어야 구매 가능
    public override bool CanBuy(RunState run)
        => run.dices.Any(d => d.faces.Any(f => f.Value == 1));

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        gameManager.StartDiceSelect(OnDiceSelect);
    }

    // §3.1 — 면 선택 모달 없이 자동으로 1면을 6면으로 변환
    private void OnDiceSelect(Dice dice)
    {
        int faceIndex = dice.faces.FindIndex(f => f.Value == 1);
        if (faceIndex >= 0)
        {
            dice.SetFace(faceIndex, 6);
        }
        // 1면이 없는 주사위는 CanBuy에서 차단되므로 여기까지 도달하지 않음
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// F2  CardP6 — 펜타클 6 (+2 코인, 라운드 5회 한정)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardP6 : Card
{
    public GameManager gameManager;
    public int coinAmount = 2;
    int currentRoundLimit;
    public int roundLimit = 5;

    protected override Card CloneInstance() => new CardP6
    {
        coinAmount = this.coinAmount,
        roundLimit = this.roundLimit,
    };

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        gameManager.StartDiceSelect(OnDiceSelect);
    }

    private void OnDiceSelect(Dice dice)
    {
        gameManager.StartDiceFaceSelect(dice, OnFaceSelect);
    }

    public void OnFaceSelect(Dice dice, int face)
    {
        dice.faces[face].OnRolled += (d, f) =>
        {
            if (currentRoundLimit > 0)
            {
                currentRoundLimit--;
                gameManager.currentRun.GetCoin(coinAmount);
            }
        };
    }

    // §3.2 — RunState.RoundStart에서 cards.ForEach(c => c.OnRoundStart()) 호출됨
    public override void OnRoundStart()
    {
        currentRoundLimit = roundLimit;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// F3  CardC7 — 컵 7 (+1 리롤, 라운드 1회 한정)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardC7 : Card
{
    public GameManager gameManager;
    public int rerollAmount = 1;
    int currentRoundLimit;
    public int roundLimit = 1;

    protected override Card CloneInstance() => new CardC7
    {
        rerollAmount = this.rerollAmount,
        roundLimit = this.roundLimit,
    };

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        gameManager.StartDiceSelect(OnDiceSelect);
    }

    private void OnDiceSelect(Dice dice)
    {
        gameManager.StartDiceFaceSelect(dice, OnFaceSelect);
    }

    public void OnFaceSelect(Dice dice, int face)
    {
        dice.faces[face].OnRolled += (d, f) =>
        {
            if (currentRoundLimit > 0)
            {
                currentRoundLimit--;
                gameManager.currentRun.GetReroll(rerollAmount);
            }
        };
    }

    // §3.3 — 버그 수정: OnRoundStart가 없어 첫 라운드부터 0이었음
    public override void OnRoundStart()
    {
        currentRoundLimit = roundLimit;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// F4  CardM10 — X 운명의 수레바퀴 (선택 면값 +5)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardM10 : Card
{
    public GameManager gameManager;
    public int increaseAmount = 5;

    protected override Card CloneInstance() => new CardM10
    {
        increaseAmount = this.increaseAmount,
    };

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        gameManager.StartDiceSelect(OnDiceSelect);
    }

    private void OnDiceSelect(Dice dice)
    {
        gameManager.StartDiceFaceSelect(dice, OnFaceSelect);
    }

    public void OnFaceSelect(Dice dice, int face)
    {
        dice.SetFaceValue(face, dice.faces[face].Value + increaseAmount);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// F5  CardM21 — XXI 세계 (선택 면이 굴려지면 그 Cycle의 dicesRemain 전체 +1)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardM21 : Card
{
    public GameManager gameManager;
    public int increaseAmount = 1;

    // §3.5 버그 수정 — 원본에서 new CardM10() 반환하던 것을 new CardM21()로 수정
    protected override Card CloneInstance() => new CardM21
    {
        increaseAmount = this.increaseAmount,
    };

    public override void OnObtain(GameManager gameManager)
    {
        this.gameManager = gameManager;
        gameManager.StartDiceSelect(OnDiceSelect);
    }

    private void OnDiceSelect(Dice dice)
    {
        gameManager.StartDiceFaceSelect(dice, OnFaceSelect);
    }

    public void OnFaceSelect(Dice dice, int face)
    {
        dice.faces[face].OnRolled += (d, f) =>
        {
            // §3.5 버그 수정 — 변수 섀도잉 해소: 파라미터 dice와 구분하기 위해 d2 사용
            foreach (var d2 in gameManager.currentRun.currentRound.currentCycle.dicesRemain)
            {
                d2.GetFace().OverrideValue(d2.GetFace().Value + increaseAmount);
            }
            // §6.6 보고: dicesRemain만 대상 — dicesSetted(점수 확정 주사위)는 변경 안 함.
            // 기획서 §2.8.3.A "그 굴림의 모든 주사위"가 dicesRemain 범위와 일치하는지 확인 필요.
        };
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// F6  CardM0 — 0 광대 (주사위 +1, 최대 7개)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardM0 : Card
{
    public DiceDefinitionSO diceSO;

    protected override Card CloneInstance() => new CardM0
    {
        diceSO = this.diceSO,
    };

    // §3.6 — 보유 주사위 7개 미만이어야 구매 가능
    public override bool CanBuy(RunState run) => run.dices.Count < 7;

    public override void OnObtain(GameManager gameManager)
    {
        // §3.6 — 방어적 중복 체크 (UI 외부 경로 차단)
        if (gameManager.currentRun.dices.Count >= 7) return;

        // §3.6 — diceSO.dices[0]은 NormalDice 가정. 실제 SO 인스펙터에서 확인 필요.
        gameManager.currentRun.dices.Add(diceSO.dices[0].Clone());
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// F7  CardS0 — 소드 1 (슬롯 족보 교체, 진열 시 타겟 무작위 결정)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardS0 : Card
{
    public string targetHandName;
    [NonSerialized] public HandDefinitionSO handSO;

    protected override Card CloneInstance() => new CardS0
    {
        targetHandName = this.targetHandName,   // 진열 시 결정된 타겟 보존
    };

    public override string Description {
        get
        {
            return string.Format(StringTable.GetString(descriptionStringKey), StringTable.GetString(targetHandName));
        }
    }

    // §2.1 — 진열 시 12종 중 무작위로 타겟 족보 결정
    public override void OnDisplay(RunState run)
    {
        handSO = run.gameManager.handDefine;
        var pool = handSO.hands;
        if (pool == null || pool.Count == 0) return;

        targetHandName = pool[UnityEngine.Random.Range(0, pool.Count)].name;
    }

    // §3.7 — 모든 슬롯이 이미 타겟 족보면 의미 없음 → 구매 불가
    public override bool CanBuy(RunState run)
    {
        if (run.hands.Count == 0) return false;
        if (string.IsNullOrEmpty(targetHandName)) return false;
        return run.hands.Any(h => h.hand != null && h.hand.name != targetHandName);
    }

    public override void OnObtain(GameManager gameManager)
    {
        if (handSO == null) handSO = gameManager.handDefine;

        // §3.7 — 이미 타겟 족보인 슬롯은 선택 불가 (filter)
        gameManager.StartHandSelect(
            OnHandSelected,
            h => h.hand != null && h.hand.name != targetHandName
        );
    }

    public void OnHandSelected(HandSlot slot)
    {
        // §3.7 — hand만 교체, slotLevel 유지
        int savedLevel = slot.slotLevel;
        slot.hand = handSO.Find(targetHandName);
        slot.slotLevel = savedLevel;
        slot.ResetSlot();
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// W4  CardW4 — 완드 4 (특정 족보 슬롯 Lv 1 → Lv 2, 지정, 25코인, Common)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardW4 : Card
{
    public string targetHandName;
    [NonSerialized] public HandDefinitionSO handSO;

    protected override Card CloneInstance() => new CardW4
    {
        targetHandName = this.targetHandName,
    };

    public override string Description
    {
        get
        {
            return string.Format(StringTable.GetString(descriptionStringKey), StringTable.GetString(targetHandName));
        }
    }

    // §4.1 — 진열 시 강화 대상 족보 무작위 결정
    public override void OnDisplay(RunState run)
    {
        handSO = run.gameManager.handDefine;
        var pool = handSO.hands;
        if (pool == null || pool.Count == 0) return;

        targetHandName = pool[UnityEngine.Random.Range(0, pool.Count)].name;
    }

    // §4.1 — 보드에 해당 족보 Lv 1 슬롯이 1개 이상이어야 구매 가능
    public override bool CanBuy(RunState run)
        => run.hands.Any(h => h.hand != null
                              && h.hand.name == targetHandName
                              && h.slotLevel == 1);

    public override void OnObtain(GameManager gameManager)
    {
        if (handSO == null) handSO = gameManager.handDefine;

        gameManager.StartHandSelect(
            OnHandSelected,
            h => h.hand != null && h.hand.name == targetHandName && h.slotLevel == 1
        );
    }

    void OnHandSelected(HandSlot slot)
    {
        slot.slotLevel = 2;
        // §2.5 — slotLevel이 scoreMultiplier에 반영되는 경로 확인 필요 (§6.4 보고)
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// W6  CardW6 — 완드 6 (임의 슬롯 Lv 1 → Lv 2, 자유, 50코인, Rare)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardW6 : Card
{
    protected override Card CloneInstance() => new CardW6
    {
    };

    // §4.2 — Lv 1 슬롯이 1개 이상이어야 구매 가능
    public override bool CanBuy(RunState run)
        => run.hands.Any(h => h.slotLevel == 1);

    public override void OnObtain(GameManager gameManager)
    {
        gameManager.StartHandSelect(
            OnHandSelected,
            h => h.slotLevel == 1
        );
    }

    void OnHandSelected(HandSlot slot)
    {
        slot.slotLevel = 2;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// M8  CardM8 — VIII 힘 (특정 족보 슬롯 Lv 2 → Lv 3, 지정, 50코인, Rare)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardM8 : Card
{
    public string targetHandName;
    [NonSerialized] public HandDefinitionSO handSO;

    protected override Card CloneInstance() => new CardM8
    {
        targetHandName = this.targetHandName,
    };

    public override string Description
    {
        get
        {
            return string.Format(StringTable.GetString(descriptionStringKey), StringTable.GetString(targetHandName));
        }
    }

    // §4.3 — 진열 시 강화 대상 족보 무작위 결정
    public override void OnDisplay(RunState run)
    {
        handSO = run.gameManager.handDefine;
        var pool = handSO.hands;
        if (pool == null || pool.Count == 0) return;

        targetHandName = pool[UnityEngine.Random.Range(0, pool.Count)].name;
    }

    // §4.3 — 보드에 해당 족보 Lv 2 슬롯이 1개 이상이어야 구매 가능
    public override bool CanBuy(RunState run)
        => run.hands.Any(h => h.hand != null
                              && h.hand.name == targetHandName
                              && h.slotLevel == 2);

    public override void OnObtain(GameManager gameManager)
    {
        if (handSO == null) handSO = gameManager.handDefine;

        gameManager.StartHandSelect(
            OnHandSelected,
            h => h.hand != null && h.hand.name == targetHandName && h.slotLevel == 2
        );
    }

    void OnHandSelected(HandSlot slot)
    {
        slot.slotLevel = 3;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// M1  CardM1 — I 마법사 (임의 슬롯 Lv 2 → Lv 3, 자유, 100코인, Epic)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardM1 : Card
{
    protected override Card CloneInstance() => new CardM1
    {
    };

    // §4.4 — Lv 2 슬롯이 1개 이상이어야 구매 가능
    public override bool CanBuy(RunState run)
        => run.hands.Any(h => h.slotLevel == 2);

    public override void OnObtain(GameManager gameManager)
    {
        gameManager.StartHandSelect(
            OnHandSelected,
            h => h.slotLevel == 2
        );
    }

    void OnHandSelected(HandSlot slot)
    {
        slot.slotLevel = 3;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// M4  CardM4 — IV 황제 (슬롯 추가, 진열 시 족보 무작위, 45코인, Epic)
// ─────────────────────────────────────────────────────────────────────────────
[Serializable]
public class CardM4 : Card
{
    public string targetHandName;
    [NonSerialized] public HandDefinitionSO handSO;

    protected override Card CloneInstance() => new CardM4
    {
        targetHandName = this.targetHandName,
    };

    public override string Description
    {
        get
        {
            return string.Format(StringTable.GetString(descriptionStringKey), StringTable.GetString(targetHandName));
        }
    }

    // §4.5 — 진열 시 추가될 족보 무작위 결정
    public override void OnDisplay(RunState run)
    {
        handSO = run.gameManager.handDefine;
        var pool = handSO.hands;
        if (pool == null || pool.Count == 0) return;

        targetHandName = pool[UnityEngine.Random.Range(0, pool.Count)].name;
    }

    // §4.5 — CanBuy override 없음 (항상 구매 가능, 동일 족보 복수 슬롯 허용)

    public override void OnObtain(GameManager gameManager)
    {
        if (handSO == null) handSO = gameManager.handDefine;

        var newSlot = new HandSlot
        {
            hand = handSO.Find(targetHandName),
            slotLevel = 1,
        };
        newSlot.hand.slot = newSlot;
        gameManager.currentRun.hands.Add(newSlot);

        // §6.7 보고: RunState.hands와 RoundState.hands 관계 확인 필요.
        // 상점 페이즈에서 추가되므로 다음 RoundStart 시 RoundState.Init에서 Clone됨 → 정상.
        // 단, 같은 Round 중 실시간 추가는 현재 currentRound.hands에 반영되지 않음.
    }
}
