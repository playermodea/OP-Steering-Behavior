using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehavior : MonoBehaviour
{
    private Transform Tr;
    private Rigidbody TrRigidbody;
    public Vector3 WanderTarget;

    public float MaxSpeed;
    public float Deceleration;
    public float FleeDistance;
    public float WanderRadius;
    public float WanderJitter;
    public float WanderDistance;
    public float HideDistance;
    public float DistanceFromBoundary;

    private void Awake()
    {
        Tr = GetComponent<Transform>();
        TrRigidbody = GetComponent<Rigidbody>();
        WanderTarget = Vector3.zero;

        MaxSpeed = 10.0f;
        Deceleration = 1.2f;
        FleeDistance = 15.0f;
        WanderRadius = 1.2f;
        WanderJitter = 40.0f;
        WanderDistance = 2.0f;
        HideDistance = 15.0f;
        DistanceFromBoundary = 8.0f;
}

    public Vector3 Seek(Vector3 TargetPos)
    {
        Vector3 DesiredVelocity = TargetPos - Tr.position;

        DesiredVelocity.Normalize();

        DesiredVelocity *= MaxSpeed;

        return (DesiredVelocity - TrRigidbody.velocity);
    }

    public Vector3 Arrive(Vector3 TargetPos)
    {
        Vector3 ToTarget = TargetPos - Tr.position;

        float Dist = ToTarget.magnitude;

        if (Dist > 0.3)
        {
            float Speed = Dist / Deceleration;

            Speed = Mathf.Min(Speed, MaxSpeed);

            Vector3 DesiredVelocity = ToTarget / Dist * Speed;

            return (DesiredVelocity - TrRigidbody.velocity);
        }

        return TrRigidbody.velocity = Vector3.zero;
    }

    public Vector3 Flee(Vector3 Target)
    {
        // 혼자 남았을 때 플레이어와 일정거리 이상 멀어지면 주변 배회 시작
        if ((Tr.position - Target).sqrMagnitude > FleeDistance * FleeDistance && ObjectManager.Instance.RaidersList.Count == 1)
        {
            return Wander();
        }

        Vector3 DesiredVelocity = Tr.position - Target;

        DesiredVelocity.Normalize();

        DesiredVelocity *= MaxSpeed;

        return (DesiredVelocity - TrRigidbody.velocity);
    }

    public Vector3 Pursuit(Transform Target)
    {
        Vector3 ToEvader = Target.position - Tr.position;

        float RelativeHeading = Dot(Target.position);

        if (RelativeHeading > 0)
        {
            return Seek(Target.position);
        }

        Vector3 EvaderSpeed = Target.GetComponent<Rigidbody>().velocity;
        float LookAheadTime = ToEvader.magnitude / (MaxSpeed + EvaderSpeed.magnitude);
        Vector3 Evader = Target.position + EvaderSpeed * LookAheadTime;

        return Seek(Evader);
    }

    public Vector3 Evade(Transform Target)
    {
        Vector3 ToPursuer = Target.position - Tr.position;

        Vector3 PursuerSpeed = Target.GetComponent<Rigidbody>().velocity;
        float LookAheadTime = ToPursuer.magnitude / (MaxSpeed + PursuerSpeed.magnitude);
        Vector3 Pursuer = Target.position + PursuerSpeed * LookAheadTime;

        return Flee(Pursuer);
    }

    public Vector3 Wander()
    {
        float JitterThisTimeSlice = WanderJitter * Time.deltaTime;

        WanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * JitterThisTimeSlice, Random.Range(-1.0f, 1.0f) * JitterThisTimeSlice, 0);

        WanderTarget.Normalize();

        WanderTarget *= WanderRadius;

        Vector3 target = WanderTarget + Tr.localRotation * Vector3.right * WanderDistance;

        return (target - TrRigidbody.velocity);
    }

    public Vector3 Hide(Transform Target, List<GameObject> ObstacleList)
    {
        if (ObstacleList.Count != 0)
        {
            Vector3 BestSpot = ObstacleList[0].GetComponent<Transform>().position;
            for (int i = 1; i < ObstacleList.Count; i++)
            {
                Vector3 CurrentSpot = ObstacleList[i].GetComponent<Transform>().position;
                float BestDistance = (BestSpot - Tr.position).sqrMagnitude;
                float CurrentDistance = (CurrentSpot - Tr.position).sqrMagnitude;
                if (BestDistance > CurrentDistance)
                {
                    BestSpot = CurrentSpot;
                }
            }
            
            float Dist = (BestSpot - Tr.position).sqrMagnitude;

            if (Dist < HideDistance * HideDistance)
            {
                Vector3 HidingSpot = GetHidingPos(Target.position, BestSpot);

                return Arrive(HidingSpot);
            }
        }

        return Evade(Target);
    }

    public Vector3 OffsetPursuit(Transform Target, Vector3 Offset)
    {
        Vector3 WorldOffsetPos = Target.position + Target.localRotation * Offset;

        Vector3 ToOffset = WorldOffsetPos - Tr.position;

        Vector3 TargetSpeed = Target.GetComponent<Rigidbody>().velocity;

        float LookAheadTime = ToOffset.magnitude / (MaxSpeed + TargetSpeed.magnitude);

        return Arrive(WorldOffsetPos + TargetSpeed * LookAheadTime);
    }

    public float Dot(Vector3 TargetPos)
    {
        float Angle = Tr.position.x * TargetPos.x + Tr.position.y * TargetPos.y + Tr.position.z * TargetPos.z;

        return Angle;
    }

    public Vector3 GetHidingPos(Vector3 TargetPos, Vector3 Obstacle)
    {
        Vector3 ToObstacleDir = Obstacle - TargetPos;

        ToObstacleDir.Normalize();

        return (ToObstacleDir * DistanceFromBoundary) + Obstacle;
    }
}
