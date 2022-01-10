using System;
using System.Collections;
using System.Collections.Generic;
using ModularAbilityCraftingSystem.Abilities.StateMachines;
using UnityEngine;

[RequireComponent(typeof(AbilityStateMachine))]
public class AbilityActivator : MonoBehaviour
{
    private AbilityStateMachine _stateMachine;
    private void Awake()
    {
        _stateMachine = gameObject.GetComponent<AbilityStateMachine>();
    }

    // public void ActivateAbility(int index)
    // {
    //     _stateMachine.UsableAbilities[index].Activate();
    // }
}
