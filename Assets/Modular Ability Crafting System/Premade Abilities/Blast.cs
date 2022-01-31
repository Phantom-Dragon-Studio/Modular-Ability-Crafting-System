using System.Globalization;
using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.Abilities.Utilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Premade_Abilities
{
    /// <summary>
    /// Area of effect damage.
    /// </summary>
    [CreateAssetMenu(fileName ="New Blast Ability", menuName = "Phantom Dragon Studio/Modular Ability Crafting System/Abilities/Blast")]
    public class Blast : AbilityBase
    {
        [SerializeField] private float baseBlastRadius;
        [SerializeField] private string abilityName;
        
        
        public Blast() : base() {
            activationType = ActivationType.Castable;
            baseBlastRadius = 5f;
            Debug.Log("Blast Ability: Constructor", this);
        }
        
        public override void Activate()
        {
            Debug.Log($"Activating Blast Ability: {abilityName}!");
            Debug.Log($"Base Blast Radius: {baseBlastRadius.ToString(CultureInfo.CurrentCulture)}!");
        }

        public override void Deactivate()
        {
            Debug.Log($"Deactivating Blast Ability: {abilityName}!");
        }
    }
}