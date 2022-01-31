using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.Abilities.StateMachines;
using ModularAbilityCraftingSystem.Abilities.States;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Sockets
{
    public class AbilitySocket : MonoBehaviour
    {
        [SerializeField] private AbilityStateMachine stateMachine;
        [SerializeField] private AbilityBase ability;

        public void Activate()
        {
            ability.Activate();
        }
    }
}
