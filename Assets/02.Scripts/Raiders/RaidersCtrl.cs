using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RaidersState
{
    Guard,
    Detect,
    Protect,
    Hide,
    Chase,
    Wait,
    Evade
}

public enum RaidersRank
{
    Boss,
    Raiders
}

public class RaidersCtrl : MonoBehaviour
{
    private Transform Tr;
    private Rigidbody TrRigidbody;
    private StateMachine m_pStateMachine;

    public RaidersState State;
    public RaidersRank Rank;

    public Transform Target;
    public Transform OffsetTarget;

    public Vector3 CurrentSpeed;
    public Vector3 TargetPos;
    public Vector3 Offset;

    public bool IsDie;

    public List<Vector3> OffsetList = new List<Vector3>();

    private void Awake()
    {
        m_pStateMachine = gameObject.AddComponent<StateMachine>();
        m_pStateMachine.SetCurrentState(Raiders_Guard.Instance);
        m_pStateMachine.SetOwner(gameObject);

        Tr = GetComponent<Transform>();
        TrRigidbody = GetComponent<Rigidbody>();

        CurrentSpeed = Vector3.zero;
        TargetPos = Vector3.zero;

        IsDie = false;

        OffsetList.Add(new Vector3(-2, 4, 0));
        OffsetList.Add(new Vector3(2, -4, 0));

        Offset = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentSpeed = Vector3.zero;
        m_pStateMachine.CustomUpdate();
        TrRigidbody.AddForce(CurrentSpeed);
    }

    // 공유 받은 플레이어 객체 및 위치 업데이트
    public void UpdatePlayerPos(MsgInfo Msg)
    {
        Target = Msg.Sender.GetComponent<RaidersCtrl>().Target;
        TargetPos = Msg.Sender.GetComponent<RaidersCtrl>().TargetPos;
    }

    // 보스 -> 전체 레이더들에게 그 자리에서 대기하라고 명령
    public void BossWaitingCommand(Vector3 targetPos)
    {
        if (State != RaidersState.Wait && Rank == RaidersRank.Boss)
        {
            MsgInfo Msg;
            Msg.Sender = this.gameObject;
            Msg.Msg = RaidersMsg.BossWaitCommand;

            TargetPos = targetPos;
            if (State != RaidersState.Detect)
                m_pStateMachine.ChangeState(Raiders_Wait.Instance);
            EventManager.Instance.SendMessage("SendMsg", Msg);
        }
    }

    // 보스 -> 전체 레이더들에게 상황종료 알림 레이더 무리들 주변 정찰 재개 명령
    public void BossFinishCommand()
    {
        MsgInfo Msg;
        Msg.Sender = gameObject;
        Msg.Msg = RaidersMsg.BossFinishCommand;

        m_pStateMachine.ChangeState(Raiders_Guard.Instance);
        EventManager.Instance.SendMessage("SendMsg", Msg);
    }

    // 보스 -> 전체 레이더들에게 플레이어 발견 알림, 호위조, 대기조, 돌격조 편성 명령
    public void BossFindPlayerCommand(MsgInfo Message)
    {
        if (State != RaidersState.Hide && Rank == RaidersRank.Boss)
        {
            Target = Message.Sender.GetComponent<RaidersCtrl>().Target;
            TargetPos = Message.Sender.GetComponent<RaidersCtrl>().TargetPos;

            MsgInfo Msg;
            Msg.Sender = gameObject;
            Msg.Msg = RaidersMsg.BossFindPlayerCommand;

            EventManager.Instance.SendMessage("SendMsg", Msg);
            m_pStateMachine.ChangeState(Raiders_Hide.Instance);
        }
    }

    // 보스 -> 대기조 레이더들 중 한명을 돌격조로 재배치 명령, 대기조가 없다면 호위조에서 차출
    public void BossRaidersDieReportCommand(MsgInfo Message)
    {
        if (Rank == RaidersRank.Boss)
        {
            MsgInfo Msg;
            Msg.Sender = gameObject;
            Msg.Msg = RaidersMsg.AddAttackTeam;

            EventManager.Instance.SendMessage("SendMsg", Msg);
        }
    }

    // 대기 명령 받은 레이더들 그 자리에서 대기
    // 대기조 편성 받은 레이더들 그 자리에서 대기 (돌격조와 호위조를 제외한 레이더 최대 2명)
    public void RaidersWaitingCommand(Vector3 targetPos)
    {
        if (State != RaidersState.Detect && Rank == RaidersRank.Raiders)
        {
            TargetPos = targetPos;
            m_pStateMachine.ChangeState(Raiders_Wait.Instance);
        }
    }

    // 상황 종료 알림 받은 레이더들 그 자리에서 주변 정찰 재개
    public void RaidersFinishCommand()
    {
        m_pStateMachine.ChangeState(Raiders_Guard.Instance);
    }

    // 플레이어를 추격할 돌격조 편성받은 레이더들 돌격 상태 돌입 (플레이어에게서 가장 가까운 레이더 최대 2명)
    public void RaidersFindPlayerCommand(MsgInfo Msg)
    {
        if (State != RaidersState.Chase && Rank == RaidersRank.Raiders)
        {
            Target = Msg.Sender.GetComponent<RaidersCtrl>().Target;

            m_pStateMachine.ChangeState(Raiders_Chase.Instance);
        }
    }

    // 보스를 호위할 호위조 편성받은 레이더들 호위 상태 돌입 (보스에게서 가장 가까운 레이더 최대 2명)
    public void RaidersProtectBossCommand(MsgInfo Msg)
    {
        if (State != RaidersState.Protect && Rank == RaidersRank.Raiders)
        {
            OffsetTarget = Msg.Sender.transform;

            Offset = OffsetList[(int)Msg.Msg];

            m_pStateMachine.ChangeState(Raiders_Protect.Instance);
        }
    }

    // 레이더 돌격팀으로 재편성
    public void RaidersJoinAttackTeam()
    {
        m_pStateMachine.ChangeState(Raiders_Chase.Instance);
    }

    // 죽을 때 보스에게 보고 후 사망
    public void Die()
    {
        IsDie = true;

        if (State == RaidersState.Chase)
        {
            MsgInfo Msg;
            Msg.Sender = gameObject;
            Msg.Msg = RaidersMsg.ImDie;

            EventManager.Instance.SendMessage("SendMsg", Msg);
        }
        else if (State == RaidersState.Detect)
        {
            MsgInfo Msg;
            Msg.Sender = gameObject;
            Msg.Msg = RaidersMsg.FindPlayer;

            EventManager.Instance.SendMessage("SendMsg", Msg);
        }

        ObjectManager.Instance.RaidersList.Remove(ObjectManager.Instance.RaidersList.Find(Raiders => gameObject.name == Raiders.name));
        Destroy(gameObject);
    }

    public Transform GetTr()
    {
        return Tr;
    }

    public Rigidbody GetRigidboy()
    {
        return TrRigidbody;
    }

    public StateMachine GetSM()
    {
        return m_pStateMachine;
    }
}