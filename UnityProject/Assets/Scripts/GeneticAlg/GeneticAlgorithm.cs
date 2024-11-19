using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Genome
{
    public float[] genome;
    public float fitness = 0;

    public Genome(float[] genes)
    {
        this.genome = genes;
        fitness = 0;
    }

    public Genome(int genesCount)
    {
        genome = new float[genesCount];

        for (int j = 0; j < genesCount; j++)
            genome[j] = Random.Range(-1.0f, 1.0f);

        fitness = 0;
    }

    public Genome()
    {
        fitness = 0;
    }
}

[System.Serializable]
public class GeneticAlgorithm
{
    public List<Genome> population = new List<Genome>();
    List<Genome> newPopulation = new List<Genome>();
    Brain brain;

    float totalFitness;

    int eliteCount = 0;
    float mutationChance = 0.0f;
    float mutationRate = 0.0f;

    public GeneticAlgorithm(int eliteCount, float mutationChance, float mutationRate, Brain brain)
    {
        this.eliteCount = eliteCount;
        this.mutationChance = mutationChance;
        this.mutationRate = mutationRate;
        this.brain = brain;
    }

    public Genome[] GetRandomGenomes(int count, int genesCount)
    {
        Genome[] genomes = new Genome[count];

        for (int i = 0; i < count; i++)
        {
            genomes[i] = new Genome(genesCount);
        }

        return genomes;
    }


    public Genome[] Epoch(Genome[] oldGenomes)
    {
        totalFitness = 0;

        population.Clear();
        newPopulation.Clear();

        population.AddRange(oldGenomes);
        population.Sort(HandleComparison);

        foreach (Genome g in population)
        {
            totalFitness += g.fitness;
        }

        SelectElite();

        while (newPopulation.Count < population.Count)
        {
            Crossover(brain);
        }

        return newPopulation.ToArray();
    }

    void SelectElite()
    {
        for (int i = 0; i < eliteCount && newPopulation.Count < population.Count; i++)
        {
            newPopulation.Add(population[i]);
        }
    }

    void Crossover(Brain brain)
    {
        Genome mom = RouletteSelection();
        Genome dad = RouletteSelection();

        Genome child1;
        Genome child2;

        Crossover(brain,mom, dad, out child1, out child2);

        newPopulation.Add(child1);
        newPopulation.Add(child2);
    }

    void Crossover(Brain brainStructure, Genome mom, Genome dad, out Genome child1, out Genome child2)
    {
        child1 = new Genome();
        child2 = new Genome();

        child1.genome = new float[mom.genome.Length];
        child2.genome = new float[mom.genome.Length];

        int pivot = Random.Range(0, mom.genome.Length);

        for (int i = 0; i < pivot; i++)
        {
            child1.genome[i] = mom.genome[i];

            if (ShouldMutate())
                child1.genome[i] += Random.Range(-mutationRate, mutationRate);

            child2.genome[i] = dad.genome[i];

            if (ShouldMutate())
                child2.genome[i] += Random.Range(-mutationRate, mutationRate);
        }

        EvolveChild(child1, brainStructure);

        for (int i = pivot; i < mom.genome.Length; i++)
        {
            child2.genome[i] = mom.genome[i];

            if (ShouldMutate())
                child2.genome[i] += Random.Range(-mutationRate, mutationRate);

            child1.genome[i] = dad.genome[i];

            if (ShouldMutate())
                child1.genome[i] += Random.Range(-mutationRate, mutationRate);
        }

        EvolveChild(child2, brainStructure);
    }

    bool ShouldMutate()
    {
        return Random.Range(0.0f, 1.0f) < mutationChance;
    }

    int HandleComparison(Genome x, Genome y)
    {
        return x.fitness > y.fitness ? 1 : x.fitness < y.fitness ? -1 : 0;
    }

    void EvolveChild(Genome child, Brain brain)
    {
        int newNeuronToAddQuantity = Random.Range(0, 3);

        List<NeuronLayer> neuronLayers = brain.layers;
        int randomLayer = Random.Range(1, neuronLayers.Count - 1);

        int neuronPositionToAdd = Random.Range(0, neuronLayers[randomLayer].NeuronsCount);


        int newNeuronCount = child.genome.Length + newNeuronToAddQuantity * neuronLayers[randomLayer].InputsCount +
                             neuronLayers[randomLayer + 1].InputsCount * newNeuronToAddQuantity;
        float[] newWeight = new float[newNeuronCount];

        int neuronPos = 0;

        for (int i = 0; i < neuronLayers.Count; i++)
        {
            if (i < randomLayer)
            {
                neuronPos += neuronLayers[i].NeuronsCount;
            }
            else if (i == randomLayer)
            {
                int neuronCount = 0;
                while (neuronCount < neuronPositionToAdd)
                {
                    neuronCount++;
                }

                neuronPos += neuronCount;
            }
        }

        //Neurona

        int count = 0;
        int originalNeuronsCount = 0;

        int previousLayerInputs = neuronLayers[randomLayer].inputsCount;
        int afterLayerInputs = neuronLayers[randomLayer + 1].inputsCount;
        int afterLayerCounter = 0;
        bool hasCreatedNewConections = false;

        while (count < newNeuronCount)
        {
            if (count < neuronPos)
            {
                newWeight[count] = child.genome[count];
            }
            else if (count >= neuronPos && count < neuronPos + newNeuronToAddQuantity)
            {
                newWeight[count] = Random.Range(-1.0f, 1.0f);
            }
            else if (!hasCreatedNewConections)
            {
                if (afterLayerCounter < afterLayerInputs)
                {
                    if (originalNeuronsCount < previousLayerInputs)
                    {
                        newWeight[count] = child.genome[count - newNeuronToAddQuantity];
                        originalNeuronsCount++;
                    }
                    else if (originalNeuronsCount < previousLayerInputs + newNeuronToAddQuantity)
                    {
                        newWeight[count] = Random.Range(-1.0f, 1.0f);
                        originalNeuronsCount = 0;
                        afterLayerCounter++;
                    }
                }
                else
                {
                    hasCreatedNewConections = true;
                }
            }
            else
            {
                newWeight[count] = child.genome[count - newNeuronToAddQuantity -
                                                ((newNeuronToAddQuantity) * afterLayerCounter)];
            }

            count++;
        }


        //float[]weight = new float[weightCount+ neuronq * neurons[random].InputsCount + neurons[random+1].InputsCount ];


        //Layer
    }


    public Genome RouletteSelection()
    {
        float rnd = Random.Range(0, Mathf.Max(totalFitness, 0));

        float fitness = 0;

        for (int i = 0; i < population.Count; i++)
        {
            fitness += Mathf.Max(population[i].fitness, 0);
            if (fitness >= rnd)
                return population[i];
        }

        return null;
    }
}