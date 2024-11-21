using Miner.SecondExam.Agent;
using Unity.Mathematics;
using UnityEngine;

namespace Miner.SecondExam
{
    using System.Collections.Generic;
    using System.Numerics;
    using UnityEngine;
    using Vector2 = System.Numerics.Vector2; // Ensure you use the correct Vector2 for SporeManager

    public class SporeSimulation : MonoBehaviour
    {
        [Header("Meshes Settings")] public Mesh herbMesh;
        public Material entityMaterial;
        public Mesh plantMesh;
        public Mesh carnMesh;
        public Mesh scavMesh;
        [Header("Simulation Settings")] public int gridSizeX = 10;
        public int gridSizeY = 10;
        public int herbivoreCount = 30;
        public int carnivoreCount = 20;
        public int scavengerCount = 20;
        public int turnCount = 100;

        [Header("Mutation Settings")] public float mutationChance = 0.1f;
        public float mutationRate = 0.01f;
        public int eliteCount = 4;

        private SporeManager sporeManager;
        private float simulationDeltaTime = 0.1f; // Adjust for simulation speed
        private float timer = 0;
        [Header("BrainConfigs")] 
        public float herbBias = 0.5f;
        public float herbP = 0.5f;
        public BrainData mainHerb;
        public BrainData moveBrain;
        public BrainData eatBrain;
        public BrainData escapeBrain;
        public float carnBias = 0.5f;
        public float carnP = 0.5f;
        public BrainData mainCarn;
        public BrainData moveCarn;
        public BrainData eatCarn;
        public float scavBias = 0.5f;
        public float scavP = 0.5f;
        public BrainData mainScav;
        public BrainData flockScav;

        void Start()
        {
            mainHerb = new BrainData(11, new int[5], 3, herbBias, herbP);
            moveBrain = new BrainData(4, new int[5], 4, herbBias, herbP);
            eatBrain = new BrainData(5, new int[3], 1, herbBias, herbP);
            escapeBrain = new BrainData(8, new int[5], 4, herbBias, herbP);
            
            mainCarn = new BrainData(5, new int[3], 2, carnBias, carnP);
            moveCarn = new BrainData(4, new int[3], 2, carnBias, carnP);
            eatCarn =new BrainData(5, new int[2], 1, carnBias, carnP);
            
            mainScav =new BrainData(5, new int[3], 2, scavBias, scavP);
            flockScav = new BrainData(6, new int[5], 4, scavBias, scavP);;
            
            
            
            
            // Example brain data initialization (Replace with actual data)
            var herbBrainData = new List<BrainData>
                { mainHerb, moveBrain, eatBrain, escapeBrain };
            var carnBrainData = new List<BrainData> { mainCarn, moveCarn,eatCarn};
            var scavBrainData = new List<BrainData> { mainScav,flockScav};

            // Initialize SporeManager
            sporeManager = new SporeManager(
                herbBrainData,
                carnBrainData,
                scavBrainData,
                gridSizeX,
                gridSizeY,
                herbivoreCount,
                carnivoreCount,
                scavengerCount,
                turnCount
            );
        }

        void Update()
        {
            timer += Time.deltaTime;

            // Tick the simulation at a fixed interval
            if (timer >= simulationDeltaTime)
            {
                sporeManager.Tick(simulationDeltaTime);
                timer = 0;
            }


            DrawEntities();
        }

        private void DrawEntities()
        {
            foreach (var agent in sporeManager.herbis)
            {
                UnityEngine.Vector3 worldPosition = new UnityEngine.Vector3(agent.position.X, agent.position.Y, 0);
                entityMaterial.color = Color.yellow;
                if (agent.lives < 0)
                {
                    entityMaterial.color = Color.cyan;
                }

                Graphics.DrawMesh(herbMesh, worldPosition, UnityEngine.Quaternion.identity, entityMaterial, 0);
            }

            foreach (var agent in sporeManager.carnivores)
            {
                UnityEngine.Vector3 worldPosition = new UnityEngine.Vector3(agent.position.X, agent.position.Y, 0);
                entityMaterial.color = Color.red;
                Graphics.DrawMesh(herbMesh, worldPosition, UnityEngine.Quaternion.identity, entityMaterial, 0);
            }

            foreach (var agent in sporeManager.plants)
            {
                if (!agent.isAvailable)
                {
                    continue;
                }

                UnityEngine.Vector3 worldPosition = new UnityEngine.Vector3(agent.position.X, agent.position.Y, 0);
                entityMaterial.color = Color.green;
                Graphics.DrawMesh(herbMesh, worldPosition, UnityEngine.Quaternion.identity, entityMaterial, 0);
            }

            foreach (var agent in sporeManager.scavengers)
            {
                UnityEngine.Vector3 worldPosition = new UnityEngine.Vector3(agent.position.X, agent.position.Y, 0);
                entityMaterial.color = Color.black;
                Graphics.DrawMesh(herbMesh, worldPosition, UnityEngine.Quaternion.identity, entityMaterial, 0);
            }
        }

        private void OnDrawGizmos()
        {
            if (sporeManager == null) return;

            // Example: Draw grid or visualize entities
            Gizmos.color = Color.gray;
            for (int x = 0; x <= gridSizeX; x++)
                Gizmos.DrawLine(new UnityEngine.Vector3(x, 0, 0), new UnityEngine.Vector3(x, gridSizeY, 0));
            for (int y = 0; y <= gridSizeY; y++)
                Gizmos.DrawLine(new UnityEngine.Vector3(0, y, 0), new UnityEngine.Vector3(gridSizeX, y, 0));
        }
    }
}