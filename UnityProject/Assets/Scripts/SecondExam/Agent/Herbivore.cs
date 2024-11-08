using System.Collections.Generic;
using System.Numerics;

namespace Miner.SecondExam.Agent
{
    public class HerbivoreMoveState : SporeMoveState
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

    public class HerbivoreEatState : SporeEatState
    {
        Brain brain;

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            float[] outputs = parameters[0] as float[];
            float posX = (float)(parameters[1]);
            float posY = (float)(parameters[2]);
            float nearFoodX = (float)(parameters[3]);
            float nearFoodY = (float)(parameters[4]);
            bool hasEatenFood = (bool)parameters[5];
            Brain brain = parameters[6] as Brain;

            if (outputs[0] >= 0f)
            {
                if (posX == nearFoodX && posY == nearFoodY && !hasEatenFood)
                {
                    //TODO: Eat++
                    //Fitness ++
                    //If comi 5
                    // fitness skyrocket
                    // hasEaten = true;
                }
                else if (hasEatenFood)
                {
                    //Todo: Fitness*-
                }
                else if (posX != nearFoodX || posY != nearFoodY)
                {
                    //TODO: Fitness--
                }
            }
            else
            {
                if (posX == nearFoodX && posY == nearFoodY && !hasEatenFood)
                {
                    //TODO: fitness--
                }
                else if (hasEatenFood)
                {
                    //Todo: Fitness++   
                }
            }

            return default;
        }

        public override BehaviourActions GetEnterBehaviours(params object[] parameters)
        {
            float posX = (float)(parameters[0]);
            float posY = (float)(parameters[1]);
            float nearFoodX = (float)(parameters[2]);
            float nearFoodY = (float)(parameters[3]);
            bool hasEatenFood = (bool)parameters[4];
            brain = parameters[5] as Brain;


            return default;
        }

        public override BehaviourActions GetExitBehaviours(params object[] parameters)
        {
            return default;
        }
    }

    public class HerbivoreEscapeState : SporeMoveState
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

    public class Herbivore : SporeAgent
    {
        private Vector2 pos;
        List<Vector2> nearEnemy = new List<Vector2>();
        List<Vector2> nearFood = new List<Vector2>();
        private int lives = 3;
        private int maxFood = 3;
        int currentFood = 3;
        bool hasEatenFood = false;
        private int maxMovementPerTurn = 3;


        public Herbivore()
        {
            fsm = new FSM<SporeAgentStates, SporeAgentActions>();

            fsm.AddBehaviour<HerbivoreMoveState>(SporeAgentStates.Move);
            fsm.AddBehaviour<HerbivoreEatState>(SporeAgentStates.Eat);
           // fsm.AddBehaviour<SporeDeadState>(SporeAgentStates.Dead);
           // fsm.AddBehaviour<SporeCorpseState>(SporeAgentStates.Corpse);
        }

        public override void Update(float deltaTime)
        {
            fsm.Tick();
        }
    }
}