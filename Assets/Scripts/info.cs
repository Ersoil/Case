using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class info : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI round;
    [SerializeField] TextMeshProUGUI queue;
    [SerializeField] GameObject button;
    [SerializeField] TextMeshProUGUI endGameStatus;
    [SerializeField] Slider enemyHitBar;
    [SerializeField] Slider allianHitBar;

    private void Awake()
    {
        UpdateQueue();
        UpdateRound();
        RoundSystem.onChangedQueue += UpdateQueue;
        RoundSystem.onChangedRound += UpdateRound;
        SuperHitBar.onChangedPoints += UpdateBars;
        RoundSystem.onEndGame += endGame;
    }

    private void UpdateQueue()
    {
        queue.text = $"Ход: {(RoundSystem.GetQueue ?"Противника":"Союзника")}";
    }

    private void UpdateRound()
    {
        round.text = $"Раунд: {RoundSystem.GetRound}";
    }

    private void endGame()
    {
        button.SetActive(false);
        endGameStatus.text = Convert.ToString(RoundSystem.GetWinner);
        endGameStatus.enabled = true;
    }

    private void UpdateBars()
    {
        if (enemyHitBar != null)
            enemyHitBar.value = SuperHitBar.GetPoints(true);

        if (allianHitBar != null)
            allianHitBar.value = SuperHitBar.GetPoints(false);
    }
}
