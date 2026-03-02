using UnityEngine;

public class ReelBehaviour : MonoBehaviour
{
    [Header("Reel References")]
    [SerializeField] private Transform reel_1;
    [SerializeField] private Transform reel_2;

    private const int ReelHeight = 5;

    [Header("Roll Parameters")]
    [SerializeField] private float rollSpeed = 15f;
    private float _finalOffset;

    [Header("Stop Parameters")]
    [SerializeField] private float stopSpeed = 10f;
    [SerializeField] private float overshootAmount = 0.5f;
    [SerializeField] private float settleSpeed = 5f;

    private bool settling = false;
    private float overshootTarget;
    private float _startStopY;

    private Transform finalReel;
    private Transform otherReel;

    public enum ReelState
    {
        Roll,
        Stop,
        Idle
    }

    private ReelState _state = ReelState.Idle;

    public void SetState(ReelState newState)
    {
        if (_state == newState) return;

        _state = newState;
        OnStateEnter(_state);
    }

    private void OnStateEnter(ReelState newState)
    {
        switch (newState)
        {
            case ReelState.Stop:

                DesignateReels();

                overshootTarget = _finalOffset - overshootAmount;
                settling = false;

                _startStopY = finalReel.position.y;
                float dist = Mathf.Abs(_startStopY - _finalOffset);

                if (_finalOffset > _startStopY)
                {
                    dist = (_startStopY + ReelHeight) + (ReelHeight - _finalOffset);
                }

                break;
        }
    }

    private void Update()
    {
        switch (_state)
        {
            case ReelState.Roll:
                Roll(reel_1);
                Roll(reel_2);
                break;

            case ReelState.Stop:
                LerpToOvershoot();
                SyncOtherReel();
                break;
        }
    }

    private void Roll(Transform reel)
    {
        float movement = rollSpeed * Time.deltaTime;
        float newY = reel.position.y - movement;

        if (newY <= -ReelHeight)
        {
            float overflow = -ReelHeight - newY;
            newY = ReelHeight - overflow;
        }

        reel.position = new Vector3(reel.position.x, newY, reel.position.z);
    }

    public void SetResult(int offset)
    {
        _finalOffset = offset;
    }

    private void DesignateReels()
    {
        if (reel_1.position.y > reel_2.position.y)
        {
            finalReel = reel_1;
            otherReel = reel_2;
        }
        else
        {
            finalReel = reel_2;
            otherReel = reel_1;
        }
    }

    private void LerpToOvershoot()
    {
        if (!settling)
        {
            finalReel.position = Vector3.MoveTowards(
                finalReel.position,
                new Vector3(finalReel.position.x, overshootTarget, finalReel.position.z),
                stopSpeed * Time.deltaTime
            );

            if (Mathf.Abs(finalReel.position.y - overshootTarget) < 0.01f)
                settling = true;
        }
        else
        {
            finalReel.position = Vector3.MoveTowards(
                finalReel.position,
                new Vector3(finalReel.position.x, _finalOffset, finalReel.position.z),
                settleSpeed * Time.deltaTime
            );

            if (Mathf.Abs(finalReel.position.y - _finalOffset) < 0.01f)
                SetState(ReelState.Idle);
        }
    }

    private void SyncOtherReel()
    {
        float y = finalReel.position.y - ReelHeight;

        if (y < -ReelHeight)
            y += ReelHeight * 2f;
        else if (y > ReelHeight)
            y -= ReelHeight * 2f;

        otherReel.position = new Vector3(otherReel.position.x, y, otherReel.position.z);
    }
}