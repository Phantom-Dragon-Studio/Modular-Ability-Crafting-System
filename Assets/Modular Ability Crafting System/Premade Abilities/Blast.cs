using System.Globalization;
using ModularAbilityCraftingSystem.Abilities.Utilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    /// <summary>
    /// Area of effect damage.
    /// </summary>
    [CreateAssetMenu(fileName ="New Blast Ability", menuName = "Phantom Dragon Studio/Modular Ability Crafting System/Abilities/Blast")]
    public class Blast : BaseAbility
    {
        [SerializeField] private float _baseBlastRadius;
        [SerializeField] private string abilityName;
        
        public Blast(ActivationType activationType) : base(activationType)
        {
            _baseBlastRadius = 5f;
            Debug.Log("Blast Ability: Constructor", this);
        }
        
        public override void Activate()
        {
            Debug.Log($"Activating Blast Ability: {abilityName}!");
            Debug.Log($"Base Blast Radius: {_baseBlastRadius.ToString(CultureInfo.CurrentCulture)}!");
        }

        public override void Deactivate()
        {
            Debug.Log($"Deactivating Blast Ability: {abilityName}!");
        }
    }
}