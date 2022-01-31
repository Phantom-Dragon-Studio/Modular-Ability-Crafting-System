
namespace ModularAbilityCraftingSystem.Runes.States
{
    public class RuneStateFactory
    {
        private readonly RuneStateMachine _context;

        public RuneStateFactory(RuneStateMachine currentContext)
        {
            _context = currentContext;
        }
        
        public RuneBaseState Locked()
        {
            return new LockedState(_context, this);
        }
        
        public RuneBaseState Unlocked()
        {
            return new UnlockedState(_context, this);
        }
        
        public RuneBaseState Equipped()
        {
            return new EquippedState(_context, this);
        }
        
        public RuneBaseState Unequipped()
        {
            return new UnequippedState(_context, this);
        }
    }
}