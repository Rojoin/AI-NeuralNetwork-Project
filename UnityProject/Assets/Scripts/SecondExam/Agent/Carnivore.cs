using System;
using System.Collections.Generic;
using System.Numerics;
using Miner.SecondExam.Agent;


public class CarnivoreMoveState : SporeMoveState
{
    private int movesPerTurn = 2;
    private float previousDistance;

    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {
        BehaviourActions behaviour = new BehaviourActions();

        float[] outputs = parameters[0] as float[];
        position = (Vector2)parameters[1];
        Vector2 nearFoodPos = (Vector2)parameters[2];
        var onMove = parameters[3] as Action<Vector2[]>;
        Herbivore herbivore = parameters[4] as Herbivore;
        behaviour.AddMultiThreadBehaviour(0, () =>
        {
            if (position == nearFoodPos)
            {
                herbivore.ReceiveDamage();
            }
            
            Vector2[] direction = new Vector2[movesPerTurn];
            for (int i = 0; i < direction.Length; i++)
            {
                direction[i] = GetDir(outputs[i]);
            }

            foreach (Vector2 dir in direction)
            {
                onMove.Invoke(direction);
                position += dir;
                //Todo: Make a way to check the limit of the grid
            }

            List<Vector2> newPositions = new List<Vector2> { nearFoodPos };
            float distanceFromFood = GetDistanceFrom(newPositions);
            if (distanceFromFood <= previousDistance)
            {
                brain.FitnessReward += 20;
                brain.FitnessMultiplier += 0.05f;
            }
            else
            {
                brain.FitnessMultiplier -= 0.05f;
            }

            previousDistance = distanceFromFood;
        });
        return behaviour;
    }

    public override BehaviourActions GetEnterBehaviours(params object[] parameters)
    {
        brain = parameters[0] as Brain;
        positiveHalf = Neuron.Sigmoid(0.5f, brain.p);
        negativeHalf = Neuron.Sigmoid(-0.5f, brain.p);
        return default;
    }

    public override BehaviourActions GetExitBehaviours(params object[] parameters)
    {
        return default;
    }
}
public class CarnivoreEatState : SporeEatState
    {
        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            BehaviourActions behaviour = new BehaviourActions();
            
            float[] outputs = parameters[0] as float[];
            position = (Vector2)parameters[1];
            Vector2 nearFoodPos = (Vector2)parameters[2];
            bool hasEatenEnoughFood = (bool)parameters[3];
            int counterEating = (int)parameters[4];
            int maxEating = (int)parameters[5];
            var onHasEatenEnoughFood = parameters[6] as Action<bool>;
            var onEaten = parameters[7] as Action<int>;
            Herbivore herbivore = parameters[8] as Herbivore;
            behaviour.AddMultiThreadBehaviour(0, () =>
            {
                if (herbivore == null)
                {
                    return;
                }

                if (outputs[0] >= 0f)
                {
                    if (position == nearFoodPos && !hasEatenEnoughFood)
                    {
                        if (herbivore.CanBeEaten())
                        {
                            //TODO: Eat++
                            //Fitness ++
                            onEaten(++counterEating);
                            brain.FitnessReward += 20;
                            if (counterEating == maxEating)
                            {
                                brain.FitnessReward += 30;
                                onHasEatenEnoughFood.Invoke(true);
                            }
                            //If comi 5
                            // fitness skyrocket
                        }
                    }
                    else if (hasEatenEnoughFood || position != nearFoodPos)
                    {
                        brain.FitnessMultiplier -= 0.05f;
                    }
                }
                else
                {
                    if (position == nearFoodPos && !hasEatenEnoughFood)
                    {
                        brain.FitnessMultiplier -= 0.05f;
                    }
                    else if (hasEatenEnoughFood)
                    {
                        brain.FitnessMultiplier += 0.10f;
                    }
                }
            });
            return behaviour;
        }

        public override BehaviourActions GetEnterBehaviours(params object[] parameters)
        {
            brain = parameters[0] as Brain;
            return default;
        }

        public override BehaviourActions GetExitBehaviours(params object[] parameters)
        {
            brain.ApplyFitness();
            return default;
        }
    }
public class Carnivore : SporeAgent
{
    FSM<SporeAgentStates, SporeAgentFlags> fsm = new FSM<SporeAgentStates, SporeAgentFlags>();

    public Carnivore()
    {
        fsm = new FSM<SporeAgentStates, SporeAgentFlags>();

        fsm.AddBehaviour<HerbivoreMoveState>(SporeAgentStates.Move);
        fsm.AddBehaviour<HerbivoreEatState>(SporeAgentStates.Eat);
        fsm.AddBehaviour<HerbivoreEscapeState>(SporeAgentStates.Escape);
        fsm.AddBehaviour<HerbivoreDeadState>(SporeAgentStates.Dead);
        fsm.AddBehaviour<HerbivoreCorpseState>(SporeAgentStates.Corpse);

        //TODO: Add transitions
    }

    public override void Update(float deltaTime)
    {
        fsm.Tick();
    }
}