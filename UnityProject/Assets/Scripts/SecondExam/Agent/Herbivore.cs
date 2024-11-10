using System;
using System.Collections.Generic;
using System.Numerics;

namespace Miner.SecondExam.Agent
{
    public enum HeribovoreStates
    {
        Move,
        Eat,
        Escape,
        Dead,
        Corpse
    }

    public enum HerbivoreFlags
    {
        ToMove,
        ToEat,
        ToEscape,
        ToDead,
        ToCorpse
    }

    public class HerbivoreMoveState : SporeMoveState
    {
        List<Vector2> nearEnemyPositions = new List<Vector2>();
        private float previousDistance;

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            BehaviourActions behaviour = new BehaviourActions();

            float[] outputs = parameters[0] as float[];
            position = (Vector2)parameters[1];
            Vector2 nearFoodPos = (Vector2)parameters[2];
            var onMove = parameters[3] as Action<Vector2[]>;
            Plant plant = parameters[4] as Plant;
            behaviour.AddMultiThreadBehaviour(0, () =>
            {
                //Outputs:
                //
                //0 cuanto se mueve
                //1 a 3 es direction

                positiveHalf = Neuron.Sigmoid(0.5f, brain.p);
                negativeHalf = Neuron.Sigmoid(-0.5f, brain.p);

                int movementPerTurn = 0;

                if (outputs[0] > positiveHalf)
                {
                    movementPerTurn = 3;
                }
                else if (outputs[0] < positiveHalf && outputs[0] > 0)
                {
                    movementPerTurn = 2;
                }
                else if (outputs[0] < 0 && outputs[0] < negativeHalf)
                {
                    movementPerTurn = 1;
                }
                else if (outputs[0] < negativeHalf)
                {
                    movementPerTurn = 0;
                }

                Vector2[] direction = new Vector2[movementPerTurn];
                for (int i = 0; i < 3; i++)
                {
                    direction[i] = GetDir(outputs[i + 1]);
                }

                if (movementPerTurn > 0)
                {
                    foreach (Vector2 dir in direction)
                    {
                        onMove.Invoke(direction);
                        position += dir;
                        //Todo: Make a way to check the limit of the grid
                    }

                    List<Vector2> newPositions = new List<Vector2>();
                    newPositions.Add(nearFoodPos);
                    float distanceFromFood = GetDistanceFrom(newPositions);
                    if (distanceFromFood >= previousDistance)
                    {
                        brain.FitnessReward += 20;
                        brain.FitnessMultiplier += 0.05f;
                    }
                    else
                    {
                        brain.FitnessMultiplier -= 0.05f;
                    }

                    previousDistance = distanceFromFood;
                }
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

    public class HerbivoreEatState : SporeEatState
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
            Plant plant = parameters[8] as Plant;
            behaviour.AddMultiThreadBehaviour(0, () =>
            {
                if (plant == null)
                {
                    return;
                }

                if (outputs[0] >= 0f)
                {
                    if (position == nearFoodPos && !hasEatenEnoughFood)
                    {
                        if (plant.CanBeEaten())
                        {
                            //TODO: Eat++
                            //Fitness ++
                            plant.Eat();
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

    public class HerbivoreEscapeState : SporeMoveState
    {
        List<Vector2> nearEnemyPositions = new List<Vector2>();
        float positiveHalf;
        float negativeHalf;
        private float previousDistance;

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            BehaviourActions behaviour = new BehaviourActions();

            float[] outputs = parameters[0] as float[];
            position = (Vector2)parameters[1];
            nearEnemyPositions = parameters[2] as List<Vector2>;
            var onMove = parameters[3] as Action<Vector2[]>;
            behaviour.AddMultiThreadBehaviour(0, () =>
            {
                //Outputs:
                //
                //0 cuanto se mueve
                //1 a 3 es direction

                positiveHalf = Neuron.Sigmoid(0.5f, brain.p);
                negativeHalf = Neuron.Sigmoid(-0.5f, brain.p);

                int movementPerTurn = 0;

                if (outputs[0] > positiveHalf)
                {
                    movementPerTurn = 3;
                }
                else if (outputs[0] < positiveHalf && outputs[0] > 0)
                {
                    movementPerTurn = 2;
                }
                else if (outputs[0] < 0 && outputs[0] < negativeHalf)
                {
                    movementPerTurn = 1;
                }
                else if (outputs[0] < negativeHalf)
                {
                    movementPerTurn = 0;
                }

                Vector2[] direction = new Vector2[movementPerTurn];
                for (int i = 0; i < 3; i++)
                {
                    direction[i] = GetDir(outputs[i + 1]);
                }

                if (movementPerTurn > 0)
                {
                    foreach (Vector2 dir in direction)
                    {
                        onMove.Invoke(direction);
                        position += dir;
                        //Todo: Make a way to check the limit of the grid
                    }

                    float distanceFromEnemies = GetDistanceFrom(nearEnemyPositions);
                    if (distanceFromEnemies <= previousDistance)
                    {
                        brain.FitnessReward += 20;
                        brain.FitnessMultiplier += 0.05f;
                    }
                    else
                    {
                        brain.FitnessMultiplier -= 0.05f;
                    }

                    previousDistance = distanceFromEnemies;
                }
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

    public class HerbivoreDeadState : SporeDeadState
    {
        private int lives;

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            BehaviourActions behaviour = new BehaviourActions();

            lives = (int)parameters[0];

            behaviour.SetTransitionBehavior(() =>
            {
                if (lives <= 0)
                {
                    OnFlag.Invoke(HerbivoreFlags.ToCorpse);
                }
            });

            return behaviour;
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

    public class HerbivoreCorpseState : SporeCorpseState
    {
        private int lives;

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

    public sealed class Herbivore : SporeAgent<HeribovoreStates, HerbivoreFlags>
    {
        List<Vector2> nearEnemy = new List<Vector2>();
        List<Vector2> nearFood = new List<Vector2>();
        private int lives = 3;
        private int maxFood = 5;
        int currentFood = 3;
        bool hasEatenFood = false;
        private int maxMovementPerTurn = 3;
        private Brain moveBrain;
        private Brain escapeBrain;
        private Brain eatBrain;

        public Herbivore()
        {
            Action<Vector2> onMove;
            fsm.AddBehaviour<HerbivoreMoveState>(HeribovoreStates.Move,
                onEnterParametes: () => { return new object[] {moveBrain };},
                onTickParametes: () => { return new object[] { moveBrain.outputs, position,GetNearestFoodPosition(),onMove = MoveTo ,GetNearestFood() }; });
            fsm.AddBehaviour<HerbivoreEatState>(HeribovoreStates.Eat);
            fsm.AddBehaviour<HerbivoreEscapeState>(HeribovoreStates.Escape);
            fsm.AddBehaviour<HerbivoreDeadState>(HeribovoreStates.Dead);
            fsm.AddBehaviour<HerbivoreCorpseState>(HeribovoreStates.Corpse);

            //TODO: Add transitions
        }

        public override void DecideState(float[] outputs)
        {
            if (outputs[0] > 0.0f)
            {
                fsm.ForceState(HeribovoreStates.Escape);
            }
            else if (outputs[1] > 0.0f)
            {
                fsm.ForceState(HeribovoreStates.Move);
            }
            else if (outputs[2] > 0.0f)
            {
                fsm.ForceState(HeribovoreStates.Eat);
            }
        }

        public override void Update(float deltaTime)
        {
            fsm.Tick();
        }

        public override void MoveTo(Vector2 dir)
        {
            position += dir;
        }

        public void ReceiveDamage()
        {
            lives--;
            if (lives <= 0)
            {
                fsm.ForceState(HeribovoreStates.Dead);
            }
        }

        public bool CanBeEaten()
        {
            if (fsm.currentState == (int)HeribovoreStates.Dead)
            {
                fsm.ForceState(HeribovoreStates.Corpse);
                return true;
            }

            return false;
        }

        public Plant GetNearestFood()
        {
            throw new NotImplementedException();
        } 
        public Vector2 GetNearestFoodPosition()
        {
            throw new NotImplementedException();
        }
    }

    public class Plant : SporeAgent
    {
        private int lives = 5;
        private bool isAvailable = true;

        public void Eat()
        {
            if (isAvailable)
            {
                lives--;

                if (lives <= 0)
                {
                    isAvailable = false;
                }
            }
        }

        public bool CanBeEaten()
        {
            return lives > 0;
        }
    }
}