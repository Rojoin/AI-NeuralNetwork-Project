using System.Collections.Generic;
using RojoinSaveSystem;
using RojoinSaveSystem.Attributes;

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

        private List<Herbivore> herbis = new List<Herbivore>();
        private List<Carnivore> carnivores = new List<Carnivore>();
        private List<Scavenger> scavengers = new List<Scavenger>();
        private bool isActive;
        private Dictionary<uint, Brain> entities;

        public SporeManager()
        {
            for (int i = 0; i < hervivoreCount; i++)
            {
                herbis.Add(new Herbivore());
            }

            for (int i = 0; i < carnivoreCount; i++)
            {
                carnivores.Add(new Carnivore());
            }

            for (int i = 0; i < scavengerCount; i++)
            {
                scavengers.Add(new Scavenger());
            }

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

        private static void CreateEntity(Brain brain)
        {
            uint entityID = ECSManager.CreateEntity();
            ECSManager.AddComponent<BiasComponent>(entityID, new BiasComponent(brain.bias));
            ECSManager.AddComponent<SigmoidComponent>(entityID, new SigmoidComponent(brain.p));
            ECSManager.AddComponent<InputLayerComponent>(entityID, new InputLayerComponent(brain.GetInputLayer()));
            ECSManager.AddComponent<HiddenLayerComponent>(entityID, new HiddenLayerComponent(brain.GetHiddenLayer()));
            ECSManager.AddComponent<OutputLayerComponent>(entityID, new OutputLayerComponent(brain.GetOutputLayer()));
            ECSManager.AddComponent<OutputComponent>(entityID, new OutputComponent(brain.outputs));
            ECSManager.AddComponent<InputComponent>(entityID, new InputComponent(brain.inputs));
        }

        public void Tick(float deltaTime)
        {
            ECSManager.Tick(deltaTime);
        }

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
    }
}