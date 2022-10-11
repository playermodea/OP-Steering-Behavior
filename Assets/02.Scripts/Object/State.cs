using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface State
{
    void Enter(GameObject owner);
    void Execute(GameObject owner);
    void Exit(GameObject owner);
}
