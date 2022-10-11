using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct MsgInfo
{
    public GameObject Sender;
    public RaidersMsg Msg;
}

public enum RaidersMsg
{
    None,
    UpdatePlayerPos,

    // 레이더 -> 보스에게 보고
    DetectionStart,
    DetectionFinish,
    FindPlayer,
    ImDie,

    // 보스 -> 레이더에게 명령
    BossWaitCommand,
    BossFinishCommand,
    BossFindPlayerCommand,
    AddAttackTeam
}

public class EventManager : MonoBehaviour
{
    private static EventManager instance;

    public static EventManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(EventManager)) as EventManager;
            }

            return instance;
        }
    }
    public void SendMsg(MsgInfo Message)
    {

        GameObject Boss = ObjectManager.Instance.RaidersList.Find(Raiders => Raiders.name == "Boss");
        Vector3 TargetPos = Message.Sender.GetComponent<RaidersCtrl>().TargetPos;

        switch (Message.Msg)
        {
            // 레이더 -> 보스 보고 : "플레이어를 본 것 같다"
            case RaidersMsg.DetectionStart:
                Boss.SendMessage("BossWaitingCommand", TargetPos);
                break;
            // 레이더 -> 보스 보고 : "플레이어가 아니었다"
            case RaidersMsg.DetectionFinish:
                Boss.SendMessage("BossFinishCommand", SendMessageOptions.DontRequireReceiver);
                break;
            // 레이더 -> 보스 보고 : "플레이어를 발견했다"
            case RaidersMsg.FindPlayer:
                Boss.SendMessage("BossFindPlayerCommand", Message);
                break;
            // 보스 -> 전체 레이더 명령 : "플레이어를 본 것 같으니 그 자리에서 대기하라"
            case RaidersMsg.BossWaitCommand:
                for (int i = 0; i < ObjectManager.Instance.RaidersList.Count; i++)
                {
                    if (ObjectManager.Instance.RaidersList[i].name != "Boss")
                    {
                        ObjectManager.Instance.RaidersList[i].SendMessage("RaidersWaitingCommand", TargetPos);
                    }
                }
                break;
            // 보스 -> 전체 레이더 명령 : "플레이어가 아니었으니 다시 주변 정찰을 재개하라"
            case RaidersMsg.BossFinishCommand:
                for (int i = 0; i < ObjectManager.Instance.RaidersList.Count; i++)
                {
                    if (ObjectManager.Instance.RaidersList[i].name != "Boss")
                    {
                        ObjectManager.Instance.RaidersList[i].SendMessage("RaidersFinishCommand", SendMessageOptions.DontRequireReceiver);
                    }
                }
                break;
            // 보스 -> 전체 레이더 명령 : "플레이어를 발견했다. 대형을 갖춰라" (호위조 2명, 대기조 2명, 돌격조 2명)
            case RaidersMsg.BossFindPlayerCommand:
                int ChaseTeamCount = 0;
                int ProtectTeamCount = 0;
                int WaitingTeamCount = 0;
                // 플레이어 근처 2명 돌격조 편성
                ObjectManager.Instance.AsendingSortingSortingList(Boss.GetComponent<RaidersCtrl>().TargetPos, ObjectManager.Instance.RaidersList);
                for (int i = 0; i < ObjectManager.Instance.RaidersList.Count; i++)
                {
                    if(ObjectManager.Instance.RaidersList[i].name != "Boss")
                    {
                        RaidersCtrl Element = ObjectManager.Instance.RaidersList[i].GetComponent<RaidersCtrl>();

                        if (Element.State != RaidersState.Chase && Element.IsDie != true)
                        {
                            MsgInfo Msg_1;
                            Msg_1.Sender = Message.Sender;
                            Msg_1.Msg = RaidersMsg.None;

                            ChaseTeamCount++;

                            ObjectManager.Instance.RaidersList[i].SendMessage("RaidersFindPlayerCommand", Msg_1);
                        }
                    }
                    if (ChaseTeamCount == 2 )
                        break;
                }
                // 보스 근처 2명 호위조 편성, 나머지 대기조 편성
                ObjectManager.Instance.AsendingSortingSortingList(Boss.transform.position, ObjectManager.Instance.RaidersList);
                for (int i = 1; i < ObjectManager.Instance.RaidersList.Count; i++)
                {
                    if (ObjectManager.Instance.RaidersList[i].name != "Boss")
                    {
                        RaidersCtrl Element = ObjectManager.Instance.RaidersList[i].GetComponent<RaidersCtrl>();

                        if (Element.State != RaidersState.Chase && ProtectTeamCount < 2)
                        {
                            MsgInfo Msg_2;
                            Msg_2.Sender = Message.Sender;
                            Msg_2.Msg = (RaidersMsg)ProtectTeamCount;

                            ProtectTeamCount++;

                            ObjectManager.Instance.RaidersList[i].SendMessage("RaidersProtectBossCommand", Msg_2);
                        }
                        else if (Element.State != RaidersState.Chase && WaitingTeamCount < 2)
                        {
                            WaitingTeamCount++;

                            ObjectManager.Instance.RaidersList[i].SendMessage("RaidersWaitingCommand", TargetPos);
                        }
                        else if (ProtectTeamCount >= 2 && WaitingTeamCount >= 2)
                        {
                            break;
                        }
                    }
                }
                break;
                // 보스 -> 레이더 명령 : 돌격조 추가
            case RaidersMsg.AddAttackTeam:
                int AddCount = 0;
                ObjectManager.Instance.AsendingSortingSortingList(Boss.transform.position, ObjectManager.Instance.RaidersList);
                // 대기조에서 돌격조로 재편성
                for (int i = ObjectManager.Instance.RaidersList.Count - 1; i >= 0; i--)
                {
                    if (ObjectManager.Instance.RaidersList[i].name != "Boss")
                    {
                        RaidersCtrl Element = ObjectManager.Instance.RaidersList[i].GetComponent<RaidersCtrl>();

                        if (Element.State == RaidersState.Wait)
                        {
                            AddCount++;
                            Element.SendMessage("RaidersJoinAttackTeam", SendMessageOptions.DontRequireReceiver);
                        }
                    }
                    if (AddCount >= 1)
                        break;
                }
                // 대기조가 없다면 호위조에서 돌격조로 재편성
                if (AddCount == 0)
                {
                    for (int i = ObjectManager.Instance.RaidersList.Count - 1; i >= 0; i--)
                    {
                        if (ObjectManager.Instance.RaidersList[i].name != "Boss")
                        {
                            RaidersCtrl Element = ObjectManager.Instance.RaidersList[i].GetComponent<RaidersCtrl>();

                            if (Element.State == RaidersState.Protect)
                            {
                                AddCount++;
                                Element.SendMessage("RaidersJoinAttackTeam", SendMessageOptions.DontRequireReceiver);
                            }
                        }
                        if (AddCount >= 1)
                            break;
                    }
                }
                break;
                // 모든 레이더들에게 플레이어 객체 및 위치 업데이트
            case RaidersMsg.UpdatePlayerPos:
                for (int i = 0; i < ObjectManager.Instance.RaidersList.Count; i++)
                {
                    RaidersCtrl Element = ObjectManager.Instance.RaidersList[i].GetComponent<RaidersCtrl>();

                    Element.SendMessage("UpdatePlayerPos", Message);
                }
                break;
                // 레이더가 죽을 때 보스에게 죽었다고 알림
            case RaidersMsg.ImDie:
                Boss.SendMessage("BossRaidersDieReportCommand", Message);
                break;
        }
    }
}
