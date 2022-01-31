using System;
using UnityEngine;
using UnityEngine.Events;

namespace ModularAbilityCraftingSystem.Abilities.StateMachines
{
    /// <summary>
    /// State Machine Config classes are used for
    /// </summary>
    [CreateAssetMenu(fileName = "New Ability State Machine Config", menuName = "Phantom Dragon Studio/Modular Ability Crafting System/FSM .config", order = 0)]
    public class AbilityStateMachineConfig : ScriptableObject
    {
        [SerializeField] private bool unlockedByDefault = true;
        [SerializeField] private bool ignoresCooldown = false;
    }
}