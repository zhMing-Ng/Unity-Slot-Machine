using System.Collections;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] private RewardCalculator rewardCalculator;
    [SerializeField] private ReelBehaviour[] wheels;
    public enum MachineState
    {
        Idle,
        Spin,
        Payout
    }
    private MachineState _state;
    [SerializeField] private float rollDuration = 3;
    private float rollTimer;

    public void SetState(MachineState newState)
    {
        if (_state == newState) return;
        _state = newState;
        OnStateEnter(_state);
    }

    private void OnStateEnter(MachineState newState)
    {
        switch (newState)
        {
            case MachineState.Spin:
                rollTimer = rollDuration;
                DesignateResult();
                SpinWheels();
                break;

            case MachineState.Idle:
                StopWheels();
                break;
        }
    }

    void Start()
    {
        SetState(MachineState.Spin);
    }

    void Update()
    {
        switch (_state)
        {
            case MachineState.Spin:
                rollTimer -= Time.deltaTime;
                if (rollTimer <= 0f)
                {
                    SetState(MachineState.Idle);
                }
                break;
        }
    }

    private void SpinWheels()
    {
        foreach (ReelBehaviour wheel in wheels)
        {
            wheel.SetState(ReelBehaviour.ReelState.Roll);
        }
    }
    private void StopWheels()
    {
        StartCoroutine(StopWheelsSequentially());
    }

    private IEnumerator StopWheelsSequentially()
    {
        foreach (ReelBehaviour wheel in wheels)
        {
            wheel.SetState(ReelBehaviour.ReelState.Stop);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void DesignateResult()
    {
        var results = rewardCalculator.RollResult(wheels.Length);
        int reward = rewardCalculator.CalculateReward(results);
        for (int i = 0; i < wheels.Length; i++)
        {
            int offset = rewardCalculator.GetOffset(results[i]);
            wheels[i].SetResult(offset);
        }
        Debug.Log($"Results = [{string.Join(", ", results)}], Reward = {reward}");
    }
}
