using System.Collections.Generic;
using RojoinSaveSystem;
using RojoinSaveSystem.Attributes;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Miner.SecondExam.Agent
{
    public class SporeManager : ISaveObject
    {
        SaveObjectData saveObject = new SaveObjectData();
        [SaveValue(0)] public int gridSizeX = 10;
        [SaveValue(1)] public int gridSizeY = 10;
        [SaveValue(3)] public int hervivoreCount = 30;
        [SaveValue(4)] public int carnivoreCount = 20;
        [SaveValue(5)] public int scavengerCount = 20;
        [SaveValue(7)] public int EliteCount = 4;
        [SaveValue(8)] public float MutationChance = 0.10f;
        [SaveValue(9)] public float MutationRate = 0.01f;
        public int turnCount = 100;
        private int currentTurn = 0;

        private List<Herbivore> herbis = new List<Herbivore>();
        private List<Plant> plants = new List<Plant>();
        private List<Carnivore> carnivores = new List<Carnivore>();
        private List<Scavenger> scavengers = new List<Scavenger>();

        private List<Brain> herbMainBrains = new List<Brain>();
        private List<Brain> herbEatBrains = new List<Brain>();
        private List<Brain> herbMoveBrains = new List<Brain>();
        private List<Brain> herbEscapeBrains = new List<Brain>();
        private List<Brain> carnMainBrains = new List<Brain>();
        private List<Brain> carnMoveBrains = new List<Brain>();
        private List<Brain> carnEatBrains = new List<Brain>();
        private List<Brain> scavMainBrains = new List<Brain>();
        private List<Brain> scavFlokingBrains = new List<Brain>();
        private bool isActive;
        private Dictionary<uint, Brain> entities;
        private Dictionary<List<Brain>, GeneticAlgorithmData> geneticInfo;

        public SporeManager()
        {
            CreateAgents();
            ECSManager.Init();
            entities = new Dictionary<uint, Brain>();
            InitEntities();
        }

        public void Tick(float deltaTime)
        {
            if (!isActive)
                return;
            if (currentTurn < turnCount)
            {
                PreUpdateAgents(deltaTime);
                UpdateInputs();
                ECSManager.Tick(deltaTime);
                AfterTick(deltaTime);
                currentTurn++;
            }
            else
            {
                EpochAllBrains();
                isActive = false;
                CreateNewGeneration();
            }
        }

        private void CreateNewGeneration()
        {
            throw new System.NotImplementedException();
        }

        private void InitEntities()
        {
            for (int i = 0; i < hervivoreCount; i++)
            {
                CreateEntity(herbis[i].mainBrain);
                CreateEntity(herbis[i].moveBrain);
                CreateEntity(herbis[i].eatBrain);
                CreateEntity(herbis[i].escapeBrain);
            }

            for (int i = 0; i < carnivoreCount; i++)
            {
                CreateEntity(carnivores[i].mainBrain);
                CreateEntity(carnivores[i].moveBrain);
                CreateEntity(carnivores[i].eatBrain);
            }

            for (int i = 0; i < scavengerCount; i++)
            {
                CreateEntity(scavengers[i].mainBrain);
                CreateEntity(scavengers[i].flockingBrain);
            }
        }

        private void CreateAgents()
        {
            for (int i = 0; i < hervivoreCount; i++)
            {
                herbis.Add(new Herbivore(this));
                herbMainBrains.Add(herbis[i].mainBrain);
                herbEatBrains.Add(herbis[i].eatBrain);
                herbEscapeBrains.Add(herbis[i].escapeBrain);
                herbMoveBrains.Add(herbis[i].moveBrain);
            }
            geneticInfo.Add(herbMainBrains,new GeneticAlgorithmData(EliteCount,MutationChance,MutationRate,herbMainBrains[0]));
            geneticInfo.Add(herbEatBrains,new GeneticAlgorithmData(EliteCount,MutationChance,MutationRate,herbEatBrains[0]));
            geneticInfo.Add(herbEscapeBrains,new GeneticAlgorithmData(EliteCount,MutationChance,MutationRate,herbEscapeBrains[0]));
            geneticInfo.Add(herbMoveBrains,new GeneticAlgorithmData(EliteCount,MutationChance,MutationRate,herbMoveBrains[0]));
            for (int i = 0; i < carnivoreCount; i++)
            {
                carnivores.Add(new Carnivore(this));
                carnMainBrains.Add(carnivores[i].mainBrain);
                carnEatBrains.Add(carnivores[i].eatBrain);
                carnMoveBrains.Add(carnivores[i].moveBrain);
            }
            geneticInfo.Add(carnMainBrains,new GeneticAlgorithmData(EliteCount,MutationChance,MutationRate,carnMainBrains[0]));
            geneticInfo.Add(carnEatBrains ,new GeneticAlgorithmData(EliteCount,MutationChance,MutationRate,carnEatBrains[0]));
            geneticInfo.Add(carnMoveBrains,new GeneticAlgorithmData(EliteCount,MutationChance,MutationRate,carnMoveBrains[0]));
            for (int i = 0; i < scavengerCount; i++)
            {
                scavengers.Add(new Scavenger(this));
                scavMainBrains.Add(scavengers[i].mainBrain);
                scavFlokingBrains.Add(scavengers[i].flockingBrain);
            }
            geneticInfo.Add(scavMainBrains,new GeneticAlgorithmData(EliteCount,MutationChance,MutationRate,scavMainBrains[0]));
            geneticInfo.Add(scavFlokingBrains ,new GeneticAlgorithmData(EliteCount,MutationChance,MutationRate,scavFlokingBrains[0]));
        }

        private void CreateEntity(Brain brain)
        {
            uint entityID = ECSManager.CreateEntity();
            ECSManager.AddComponent<BiasComponent>(entityID, new BiasComponent(brain.bias));
            ECSManager.AddComponent<SigmoidComponent>(entityID, new SigmoidComponent(brain.p));
            ECSManager.AddComponent<InputLayerComponent>(entityID, new InputLayerComponent(brain.GetInputLayer()));
            ECSManager.AddComponent<HiddenLayerComponent>(entityID, new HiddenLayerComponent(brain.GetHiddenLayers()));
            ECSManager.AddComponent<OutputLayerComponent>(entityID, new OutputLayerComponent(brain.GetOutputLayer()));
            ECSManager.AddComponent<OutputComponent>(entityID, new OutputComponent(brain.outputs));
            ECSManager.AddComponent<InputComponent>(entityID, new InputComponent(brain.inputs));
            entities.Add(entityID, brain);
        }

        #region Epoch

        private void EpochAllBrains()
        {
            EpochHerbivore();
            EpochCarnivore();
            EpochScavenger();

            foreach (KeyValuePair<uint, Brain> entity in entities)
            {
                HiddenLayerComponent inputComponent = ECSManager.GetComponent<HiddenLayerComponent>(entity.Key);
                inputComponent.hiddenLayers = entity.Value.GetHiddenLayers();
            }
        }

        private void EpochScavenger()
        {
            List<Brain> scavMainBrain = new List<Brain>();
            List<Brain> scavFlockingBrain = new List<Brain>();
            foreach (var scav in scavengers)
            {
                if (scav.hasEaten)
                {
                    scavMainBrain.Add(scav.mainBrain);
                    scavFlockingBrain.Add(scav.flockingBrain);
                }
            }
            bool isGenerationDead = scavMainBrain.Count <= 1;
            EpochLocal(scavMainBrain,isGenerationDead);
            EpochLocal(scavFlockingBrain,isGenerationDead);
        }

        void EpochCarnivore()
        {
            List<Brain> carnivoreMainBrain = new List<Brain>();
            List<Brain> carnivoreEatBrain = new List<Brain>();
            List<Brain> carnivoreMoveBrain = new List<Brain>();
            foreach (var carnivore in carnivores)
            {
                if (carnivore.hasEatenEnoughFood)
                {
                    carnivoreMainBrain.Add(carnivore.mainBrain);
                    carnivoreEatBrain.Add(carnivore.eatBrain);
                    carnivoreMoveBrain.Add(carnivore.moveBrain);
                }
            }

            bool isGenerationDead = carnivoreMainBrain.Count <= 1;
            EpochLocal(carnivoreMainBrain, isGenerationDead);
            EpochLocal(carnivoreEatBrain, isGenerationDead);
            EpochLocal(carnivoreMoveBrain, isGenerationDead);
        }

        private void EpochHerbivore()
        {
            List<Brain> herbivoresMainBrain = new List<Brain>();
            List<Brain> herbivoresEscapeBrain = new List<Brain>();
            List<Brain> herbivoresMoveBrain = new List<Brain>();
            List<Brain> herbivoresEatBrain = new List<Brain>();
            foreach (Herbivore herbivore in herbis)
            {
                if (herbivore.lives > 0 && herbivore.hasEatenFood)
                {
                    herbivoresMainBrain.Add(herbivore.mainBrain);
                    herbivoresEatBrain.Add(herbivore.eatBrain);
                    herbivoresMoveBrain.Add(herbivore.moveBrain);
                    herbivoresEscapeBrain.Add(herbivore.escapeBrain);
                }
            }

            bool isGenerationDead = herbivoresMainBrain.Count <= 1;

            EpochLocal(herbivoresMainBrain, isGenerationDead);
            EpochLocal(herbivoresMoveBrain, isGenerationDead);
            EpochLocal(herbivoresEatBrain, isGenerationDead);
            EpochLocal(herbivoresEscapeBrain, isGenerationDead);
        }

        private void EpochLocal(List<Brain> brains, bool force)
        {
            Genome[] newGenomes = GeneticAlgorithm.Epoch(GetGenomes(brains), geneticInfo[brains], force);

            for (int i = 0; i < brains.Count; i++)
            {
                Brain brain = brains[i];
                brain.SetWeights(newGenomes[i].genome);
            }
        }

        private static Genome[] GetGenomes(List<Brain> brains)
        {
            List<Genome> genomes = new List<Genome>();
            foreach (var brain in brains)
            {
                Genome genome = new Genome(brain.GetTotalWeightsCount());

                brain.SetWeights(genome.genome);
                brains.Add(brain);

                genomes.Add(genome);
            }

            return genomes.ToArray();
        }

        #endregion

        #region Updates

        private void PreUpdateAgents(float deltaTime)
        {
            foreach (Herbivore herbi in herbis)
            {
                herbi.PreUpdate(deltaTime);
            }

            foreach (Carnivore carn in carnivores)
            {
                carn.PreUpdate(deltaTime);
            }

            foreach (Scavenger scav in scavengers)
            {
                scav.PreUpdate(deltaTime);
            }
        }

        private void UpdateInputs()
        {
            foreach (KeyValuePair<uint, Brain> entity in entities)
            {
                InputComponent inputComponent = ECSManager.GetComponent<InputComponent>(entity.Key);
                inputComponent.inputs = entity.Value.inputs;
            }
        }

        public void AfterTick(float deltaTime = 0)
        {
            foreach (KeyValuePair<uint, Brain> entity in entities)
            {
                OutputComponent output = ECSManager.GetComponent<OutputComponent>(entity.Key);
                entity.Value.outputs = output.outputs;
            }

            foreach (Herbivore herbi in herbis)
            {
                herbi.Update(deltaTime);
            }

            foreach (Carnivore carn in carnivores)
            {
                carn.Update(deltaTime);
            }

            foreach (Scavenger scav in scavengers)
            {
                scav.Update(deltaTime);
            }
        }

        #endregion

        public int GetID()
        {
            return saveObject.id;
        }

        public ISaveObject GetObject()
        {
            return this;
        }

        public void Save()
        {
        }

        public Herbivore GetNearHerbivore(Vector2 position)
        {
            Herbivore nearest = herbis[0];
            float distance = (position.X * nearest.position.X) + (position.Y * nearest.position.Y);

            foreach (Herbivore go in herbis)
            {
                float newDist = (go.position.X * position.X) + (go.position.Y * position.Y);
                if (newDist < distance)
                {
                    nearest = go;
                    distance = newDist;
                }
            }

            return nearest;
        }

        public Plant GetNearPlant(Vector2 position)
        {
            Plant nearest = plants[0];
            float distance = (position.X * nearest.position.X) + (position.Y * nearest.position.Y);

            foreach (Plant go in plants)
            {
                if (go.isAvailable)
                {
                    float newDist = (go.position.X * position.X) + (go.position.Y * position.Y);
                    if (newDist < distance)
                    {
                        nearest = go;
                        distance = newDist;
                    }
                }
            }

            return nearest;
        }
    }
}