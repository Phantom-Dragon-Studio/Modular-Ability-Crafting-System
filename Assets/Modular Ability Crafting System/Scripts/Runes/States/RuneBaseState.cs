namespace ModularAbilityCraftingSystem.Runes.States
{
    public abstract class RuneBaseState
    {
        private bool _isRootState = false;
        private readonly RuneStateMachine _context;
        private readonly RuneStateFactory _factory;
        private RuneBaseState _currentSubState;
        private RuneBaseState _currentSuperState;

        protected RuneBaseState(RuneStateMachine context, RuneStateFactory factory) {
            _context = context;
            _factory = factory;
        }

        protected bool IsRootState {
            get => _isRootState;
            set => _isRootState = value;
        }

        protected RuneStateMachine Context => _context;
        protected RuneStateFactory Factory => _factory;

        protected RuneBaseState CurrentSubState {
            get => _currentSubState;
            set => _currentSubState = value;
        }

        protected RuneBaseState CurrentSuperState {
            get => _currentSuperState;
            set => _currentSuperState = value;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract void InitializeSubState();
        public abstract void CheckSwitchStates();

        public void UpdateStates() {
            UpdateState();
            CurrentSubState?.UpdateStates();
        }

        public void ExitStates() {
            ExitState();
            CurrentSubState?.ExitStates();
        }

        protected void SwitchState(RuneBaseState newState) {
            ExitState();

            newState.EnterState();
            if (IsRootState)
                Context.CurrentState = newState;
            else if (CurrentSuperState != null)
                SetSubState(newState);
        }

        protected void SetSuperState(RuneBaseState newSuperState) {
            CurrentSuperState = newSuperState;
        }

        protected void SetSubState(RuneBaseState newSubState) {
            CurrentSubState = newSubState;
            newSubState.SetSuperState(this);
        }
    }
}