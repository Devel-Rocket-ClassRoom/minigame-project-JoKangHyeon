using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using static Defines;

[TestFixture]
public class HandEvaluatorTests
{

    // int[] 입력을 List<Die>로 변환 — 테스트 데이터 작성 편의
    private static List<Dice> MakeNormalDice(params int[] values)
    {
        return values.Select(v => {
            Dice dice = new NormalDice();
            dice.ForceSetDice(v);
            return dice;
        }).ToList();
    }

    // Aces — 4 cases (A1~A4)
    [TestCase(new[] { 1, 1, 1, 1, 1 }, true, 5)]
    [TestCase(new[] { 1, 1, 1, 2, 3 }, true, 3)]
    [TestCase(new[] { 1, 2, 3, 4, 5 }, true, 1)]
    [TestCase(new[] { 2, 3, 4, 5, 6 }, false, 0)]   // 1이 0개 → isMatched=false
    public void Evaluate_Aces(int[] values, bool expectedMatched, int expectedSum)
    {
        Hand hand = new NumbersHand(1);
        List<Dice> dices = MakeNormalDice(values);
        int score = hand.GetDiceScore(dices);
        bool matched = hand.IsAchived(dices);
        Assert.AreEqual(expectedMatched, matched);
        Assert.AreEqual(expectedSum, score);
    }

    // 4 of a Kind — 안 2 룰 검증 포함
    [TestCase(new[] { 5, 5, 5, 5, 2 }, true, 20)]   // 정확히 4개
    [TestCase(new[] { 5, 5, 5, 5, 5 }, true, 20)]   // ★ 안 2 룰: 5번째 미포함
    [TestCase(new[] { 3, 3, 3, 3, 6 }, true, 12)]
    [TestCase(new[] { 3, 3, 3, 6, 6 }, false, 0)]   // FH 패턴, 4Kind 미충족
    [TestCase(new[] { 1, 2, 3, 4, 5 }, false, 0)]
    public void Evaluate_SmallAlignment(int[] values, bool expectedMatched, int expectedSum)
    {
        Hand hand = new SmallAlighmentHand();
        List<Dice> dices = MakeNormalDice(values);
        int score = hand.GetDiceScore(dices);
        bool matched = hand.IsAchived(dices);

        Assert.AreEqual(expectedMatched, matched);
        Assert.AreEqual(expectedSum, score);
    }

    [TestCase(new[] { 1, 2, 3, 4, 5 }, true,15)]
    [TestCase(new[] { 2, 3, 4, 5, 6 }, true,20)]
    [TestCase(new[] { 1, 2, 3, 4, 6 }, false,0)]
    [TestCase(new[] { 1, 1, 2, 3, 4 }, false,0)]
    public void Evaluate_BigStraight(int[] values, bool expectedMatched, int expectedSum)
    {
        Hand hand = new BigStraightHand();
        List<Dice> dices = MakeNormalDice(values);
        int score = hand.GetDiceScore(dices);
        bool matched = hand.IsAchived(dices);

        Assert.AreEqual(expectedMatched, matched);
        Assert.AreEqual(expectedSum, score);
    }
}