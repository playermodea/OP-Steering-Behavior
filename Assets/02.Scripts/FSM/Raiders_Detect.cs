using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raiders_Detect : MonoBehaviour, State
{
    private static Raiders_Detect instance;

    public static Raiders_Detect Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(Raiders_Detect)) as Raiders_Detect;
            }

            return instance;
        }
    }

    public virtual void Enter(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();
        var DetectRange = owner.transform.GetChild(0).GetComponent<DetectionRange>();

        MsgInfo Msg;
        Msg.Sender = owner;
        Msg.Msg = RaidersMsg.DetectionStart;

        DetectRange.DetectCount++;
        Owner.State = RaidersState.Detect;
        EventManager.Instance.SendMessage("SendMsg", Msg);
        //Debug.Log("탐지 상태 실행");
    }

    public virtual void Execute(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();
        var OwnerRigidbody = Owner.GetRigidboy();
        var DetectRange = owner.transform.GetChild(0).GetComponent<DetectionRange>();

        OwnerRigidbody.velocity = Vector3.zero;

        Vector3 Dir = Owner.TargetPos - Owner.transform.position;
        float Angle = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        Owner.transform.rotation = Quaternion.AngleAxis(Angle, Vector3.forward);

        // BackToGuardTime 시간 내에 DetectCount가 DetectLimit 초과시 플레이어를 찾은 것으로 간주
        // 플레이어를 발견한 레이더가 보스에게 플레이어를 발견했다고 보고 후 레이더 무리 대형 갖춰서 플레이어 압박 시작
        if (DetectRange.DetectCount > DetectRange.DetectLimit)
        {
            MsgInfo Msg;
            Msg.Sender = owner;
            Msg.Msg = RaidersMsg.FindPlayer;

            Owner.GetSM().ChangeState(Raiders_Wait.Instance);
            EventManager.Instance.SendMessage("SendMsg", Msg);
        }

        // BackToGuardTime 시간 내에 DetectCount가 DetectLimit 미달시 Detect 상태인 레이더가 플레이어를 제대로 식별하지 못한 것으로 간주
        // Detect 상태인 레이더가 보스에게 보고 후 레이더 무리 주변 정찰 재개
        if (DetectRange.BackToGuardTime > DetectRange.BackToGuardTimeDelay)
        {
            MsgInfo Msg;
            Msg.Sender = owner;
            Msg.Msg = RaidersMsg.DetectionFinish;

            //Owner.GetSM().ChangeState(Raiders_Guard.Instance);
            EventManager.Instance.SendMessage("SendMsg", Msg);
        }
    }

    public virtual void Exit(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();
        var DetectRange = owner.transform.GetChild(0).GetComponent<DetectionRange>();

        DetectRange.DetectCount = 0;
        DetectRange.DetectTime = 0.0f;
        DetectRange.BackToGuardTime = 0.0f;
        //Debug.Log("탐지 상태 종료");
    }
}
