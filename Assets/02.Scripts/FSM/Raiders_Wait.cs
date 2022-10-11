using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raiders_Wait : MonoBehaviour, State
{
    private static Raiders_Wait instance;

    public static Raiders_Wait Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(Raiders_Wait)) as Raiders_Wait;
            }

            return instance;
        }
    }

    public virtual void Enter(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();

        Owner.State = RaidersState.Wait;
        //Debug.Log("대기 상태 실행");
    }

    public virtual void Execute(GameObject owner)
    {
        var Owner = owner.GetComponent<RaidersCtrl>();
        var OwnerRigidbody = Owner.GetRigidboy();

        // 대기 상태의 레이더는 멈춘 상태에서 업데이트 되는 플레이어 위치 방향 응시
        OwnerRigidbody.velocity = Vector3.zero;

        Vector3 Dir = Owner.TargetPos - Owner.transform.position;
        float Angle = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        Owner.transform.rotation = Quaternion.AngleAxis(Angle, Vector3.forward);
    }

    public virtual void Exit(GameObject owner)
    {
        //Debug.Log("대기 상태 종료");
    }
}
