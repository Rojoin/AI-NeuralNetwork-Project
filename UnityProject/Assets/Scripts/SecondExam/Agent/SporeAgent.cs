using System.Numerics;

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

    public abstract class SporeMoveState : State
    {
        protected Vector2 position;
        
    }

    public abstract class SporeEatState : State
    {
       
    }

    public abstract class SporeDeadState : State
    {
      
    }

    public abstract class SporeCorpseState : State
    {
    
    }

    public abstract class SporeAgent
    {
        public FSM<SporeAgentStates, SporeAgentActions> fsm;

        public SporeAgent()
        {
        }

        public abstract void Update(float deltaTime);
    }
}