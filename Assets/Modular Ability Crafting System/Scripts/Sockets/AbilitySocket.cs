using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.Abilities.States;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Sockets
{
    public class AbilitySocket : MonoBehaviour
    {
        private IAbility Ability;
        private CooldownState Cooldown;

        public void Activate()
        {
            Ability.Activate();
            //TODO: Cooldown.StartTimer();
        }
    }
}
