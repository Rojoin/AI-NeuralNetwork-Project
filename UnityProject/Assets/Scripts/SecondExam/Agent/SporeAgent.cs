namespace Miner.SecondExam.Agent
{

    public enum SporeAgentStates
    {
        Move,
        Eat,
        Dead,
        Corpse
    } 
    public enum SporeAgentActions
    {
        ToMove,
        ToEat,
        ToEscape
    }

    public class SporeMoveState : State
    {
        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetEnterBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetExitBehaviours(params object[] parameters)
        {
            return default;
        }
    }
    public class SporeEatState : State
    {
        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetEnterBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetExitBehaviours(params object[] parameters)
        {
            return default;
        }
    }
    public class SporeDeadState : State
    {
        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetEnterBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetExitBehaviours(params object[] parameters)
        {
            return default;
        }
    }
    public class SporeCorpseState : State
    {
        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetEnterBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetExitBehaviours(params object[] parameters)
        {
            return default;
        }
    }
    public class SporeAgent
    {
        public FSM<SporeAgentStates, SporeAgentActions> fsm;
        public SporeAgent()
        {
            fsm = new FSM<SporeAgentStates, SporeAgentActions>();
            
            fsm.AddBehaviour<SporeMoveState>(SporeAgentStates.Move);
            fsm.AddBehaviour<SporeEatState>(SporeAgentStates.Eat); 
            fsm.AddBehaviour<SporeDeadState>(SporeAgentStates.Dead);
            fsm.AddBehaviour<SporeCorpseState>(SporeAgentStates.Corpse);
        }
    }
}