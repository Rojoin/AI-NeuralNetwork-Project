using System;
using System.Collections.Generic;
using System.Numerics;
using Miner.SecondExam.Agent;

public enum ScavengerStates
{
    Move
}

public enum ScavengerFlags
{
    ToMove
}

public sealed class ScavengerMoveState : SporeMoveState
{
    private float MinEatRadius;
    private int counter;


    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {
        BehaviourActions behaviour = new BehaviourActions();

        float[] outputs = parameters[0] as float[];
        position = (Vector2)(parameters[1]);
        Vector2 nearFoodPos = (Vector2)parameters[2];
        MinEatRadius = (float)(parameters[3]);
        bool hasEatenFood = (bool)parameters[4];
        Herbivore herbivore = parameters[5] as Herbivore;
        var onMove = parameters[6] as Action<Vector2>;
        counter = (int)parameters[7];
        var onEat = parameters[8] as Action<int>;
        behaviour.AddMultiThreadBehaviour(0, () =>
        {
            List<Vector2> newPositions = new List<Vector2> { nearFoodPos };
            float distanceFromFood = GetDistanceFrom(newPositions);
//TODO: Hacer flocking
            if (distanceFromFood < MinEatRadius && !hasEatenFood)
            {
                counter++;
                onEat.Invoke(counter);
                brain.FitnessReward += 1;

                if (counter >= 20)
                {
                    brain.FitnessReward += 20;
                    brain.FitnessMultiplier += 0.10f;
                    hasEatenFood = true;
                }
            }
            else if (distanceFromFood > MinEatRadius)
            {
                brain.FitnessMultiplier -= 0.05f;
            }

            // Vector2[] direction = new Vector2[movesPerTurn];
            // for (int i = 0; i < direction.Length; i++)
            // {
            //     direction[i] = GetDir(outputs[i]);
            // }
            //
            // foreach (Vector2 dir in direction)
            // {
            //     onMove.Invoke(direction);
            // }
        });


        return behaviour;
    }

    public override BehaviourActions GetEnterBehaviours(params object[] parameters)
    {
        brain = parameters[0] as Brain;
        position = (Vector2)(parameters[1]);
        MinEatRadius = (float)(parameters[2]);
        positiveHalf = Neuron.Sigmoid(0.5f, brain.p);
        negativeHalf = Neuron.Sigmoid(-0.5f, brain.p);

        return default;
    }

    public override BehaviourActions GetExitBehaviours(params object[] parameters)
    {
        return default;
    }
}


public class Scavenger : SporeAgent<ScavengerStates, ScavengerFlags>
{
    public Brain flockingBrain;
    float minEatRadius;
    protected Vector2 dir;
    public bool hasEaten = false;
    public int counterEating = 0;
    protected float speed = 5;

    public void Reset(Vector2 position)
    {
        hasEaten = false;
        this.position = position;
        counterEating = 0;
        fsm.ForceState(ScavengerStates.Move);
    }

    public Scavenger(SporeManager populationManager, Brain main, Brain flockBrain) : base(populationManager, main)
    {
        flockingBrain = flockBrain;
        minEatRadius = 4f;

        Action<Vector2> setDir;
        Action<int> setEatingCounter;
        fsm.AddBehaviour<ScavengerMoveState>(ScavengerStates.Move,
            onEnterParametes: () => { return new object[] { mainBrain, position, minEatRadius }; },
            onTickParametes: () =>
            {
                return new object[]
                {
                    mainBrain.outputs, position,  GetNearFoodPos(),minEatRadius, hasEaten, GetNearHerbivore(),
                    setDir = MoveTo, counterEating, setEatingCounter = b => counterEating = b
                };
            });

        fsm.ForceState(ScavengerStates.Move);
    }

    public override void DecideState(float[] outputs)
    {
    }

    public override void PreUpdate(float deltaTime)
    {
        var nearFoodPos = GetNearFoodPos();
        mainBrain.inputs = new[] { position.X, position.Y, minEatRadius, nearFoodPos.X, nearFoodPos.Y };
        //Todo: Agregar los otros parametros de flocking
        var ner = GetNearScavs();
        flockingBrain.inputs = new[] { position.X, position.Y, ner[0].X, ner[0].Y, ner[1].X, ner[1].Y };
    }

    public override void Update(float deltaTime)
    {
        fsm.Tick();
        Move(deltaTime);
    }

    private void Move(float deltaTime)
    {
        position += dir * speed * deltaTime;
        if (position.X > populationManager.gridSizeX)
        {
            position.X = populationManager.gridSizeX;
        }
        else if (position.X < 0)
        {
            position.X = 0;
        }

        if (position.Y > populationManager.gridSizeY)
        {
            position.Y = populationManager.gridSizeY;
        }
        else if (position.Y < 0)
        {
            position.Y = 0;
        }
    }

    public Vector2 GetNearFoodPos()
    {
        return GetNearHerbivore().position;
    }

    public Herbivore GetNearHerbivore()
    {
        return populationManager.GetNearHerbivore(position);
    }

    public List<Vector2> GetNearScavs()
    {
        return populationManager.GetNearScavs(position);
    }

    public override void MoveTo(Vector2 dir)
    {
        this.dir = dir;
    }

    public override void GiveFitnessToMain()
    {
        mainBrain.FitnessMultiplier = 1.0f;
        mainBrain.FitnessReward = 0f;
        mainBrain.FitnessReward += flockingBrain.FitnessReward + (hasEaten ? flockingBrain.FitnessReward : 0);
        mainBrain.FitnessMultiplier += flockingBrain.FitnessMultiplier + (hasEaten ? 1 : 0);

        mainBrain.ApplyFitness();
    }
}