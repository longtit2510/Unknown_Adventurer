using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_DieState : DieState
{
    private Enemy1 enemy;
    public E1_DieState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DieState stateData, Enemy1 enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
