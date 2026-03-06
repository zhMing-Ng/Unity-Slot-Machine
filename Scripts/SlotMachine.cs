using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    public enum MachineState { Idle, Spinning, Stopping, Payout }

    [Header("Dependencies")]
    [SerializeField] private RewardCalculator rewardCalculator;
    [SerializeField] private ReelBehaviour[] reels;

    [Header("Settings")]
    [SerializeField] private float spinDuration = 3.0f;
    [SerializeField] private float stopDelay = 0.5f;

    private MachineState _currentState = MachineState.Idle;
    public MachineState CurrentState => _currentState;

    public static event Action OnSpinStarted;
    public static event Action<int> OnSpinEnded;

    public void StartSpin()
    {
        if (_currentState != MachineState.Idle) return;
        
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        ChangeState(MachineState.Spinning);
        OnSpinStarted?.Invoke();
        var results = rewardCalculator.RollResult(reels.Length);
        int reward = rewardCalculator.CalculateReward(results);

        // 2. Start all reels spinning
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].SetResult(rewardCalculator.GetOffset(results[i]));
            reels[i].SetState(ReelBehaviour.ReelState.Roll);
        }

        yield return new WaitForSeconds(spinDuration);

        ChangeState(MachineState.Stopping);
        foreach (var reel in reels)
        {
            reel.SetState(ReelBehaviour.ReelState.Stop);
            yield return new WaitForSeconds(stopDelay);
        }

        ChangeState(MachineState.Payout);
        Debug.Log($"Results: {string.Join(", ", results)} | Reward: {reward}");
        OnSpinEnded?.Invoke(reward);

        ChangeState(MachineState.Idle);
    }

    private void ChangeState(MachineState newState)
    {
        _currentState = newState;
    }
}