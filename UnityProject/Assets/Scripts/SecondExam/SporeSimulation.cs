using System.Collections.Generic;
using RojoinNeuralNetwork;
using UnityEngine;

public class SporeSimulation : MonoBehaviour
    {
        public string fileToLoad;
        public string filePath = "/Saves/Genomes";
        public bool isSimulationActive = true;
        [Header("Meshes Settings")] public Mesh herbMesh;
        public Material herbMaterial;
        public Material plantMaterial;
        public Material carnMaterial;
        public Material scavMaterial;
        public Mesh plantMesh;
        public Mesh carnMesh;
        public Mesh scavMesh;
        [Header("Simulation Settings")] public int gridSizeX = 10;
        public int gridSizeY = 10;
        public int herbivoreCount = 30;
        public int carnivoreCount = 20;
        public int scavengerCount = 20;
        public int turnCount = 100;

        public int currentGeneration;
        [Header("Mutation Settings")] public float mutationChance = 0.1f;
        public float mutationRate = 0.01f;
        public int eliteCount = 4;

       [SerializeField] private SporeManager sporeManager;
        public float simulationDeltaTime = 0.1f; // Adjust for simulation speed
        private float timer = 0;
        [Header("BrainConfigs")] 
        public float herbBias = 0.5f;
        public float herbP = 0.5f;
        private BrainData mainHerb;
        private BrainData moveBrain;
        private BrainData eatBrain;
        private BrainData escapeBrain;
        public float carnBias = 0.5f;
        public float carnP = 0.5f;
         private BrainData mainCarn;
         private BrainData moveCarn;
         private BrainData eatCarn;
        public float scavBias = 0.5f;
        public float scavP = 0.5f;
         private BrainData mainScav;
         private BrainData flockScav;


        void Start()
        {
            mainHerb = new BrainData(11, new int[] { 7, 5,3 }, 3, herbBias, herbP);
            moveBrain = new BrainData(4, new int[] { 5 ,4}, 4, herbBias, herbP);
            eatBrain = new BrainData(5, new int[] { 3 ,}, 1, herbBias, herbP);
            escapeBrain = new BrainData(8, new int[] { 5,3 }, 4, herbBias, herbP);

            mainCarn = new BrainData(5, new int[] { 3,2 }, 2, carnBias, carnP);
            moveCarn = new BrainData(4, new int[] { 3 ,2}, 2, carnBias, carnP);
            eatCarn = new BrainData(5, new int[] { 2 ,2}, 1, carnBias, carnP);

            mainScav = new BrainData(5, new int[] { 3 ,5}, 2, scavBias, scavP);
            flockScav = new BrainData(8, new int[] { 5 ,5,5}, 4, scavBias, scavP);
            ;


            // Example brain data initialization (Replace with actual data)
            var herbBrainData = new List<BrainData>
                { mainHerb, moveBrain, eatBrain, escapeBrain };
            var carnBrainData = new List<BrainData> { mainCarn, moveCarn, eatCarn };
            var scavBrainData = new List<BrainData> { mainScav, flockScav };

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
            sporeManager.filepath = Application.dataPath + filePath;
        }

        void Update()
        {
            timer += Time.deltaTime;

            // Tick the simulation at a fixed interval
            // if (timer >= simulationDeltaTime)
            // {
            currentGeneration = sporeManager.generation;
            sporeManager.isActive = isSimulationActive;
            sporeManager.Tick(Time.deltaTime);
            timer = 0;
            // }


            DrawEntities();
        }

        private void DrawEntities()
        {
            foreach (var agent in sporeManager.herbis)
            {
                Vector3 worldPosition = new Vector3(agent.position.X, agent.position.Y, 0);
                herbMaterial.color = Color.yellow;
                if (agent.lives < 0)
                {
                    herbMaterial.color = Color.cyan;
                }

                Graphics.DrawMesh(herbMesh, worldPosition, Quaternion.identity, herbMaterial, 0);
            }

            foreach (var agent in sporeManager.carnivores)
            {
                Vector3 worldPosition = new Vector3(agent.position.X, agent.position.Y, 0);
                carnMaterial.color = Color.red;
                Graphics.DrawMesh(carnMesh, worldPosition, Quaternion.identity, carnMaterial, 0);
            }

            foreach (var agent in sporeManager.plants)
            {
                if (!agent.isAvailable)
                {
                    continue;
                }

                Vector3 worldPosition = new Vector3(agent.position.X, agent.position.Y, 0);
                plantMaterial.color = Color.green;
                Graphics.DrawMesh(plantMesh, worldPosition, Quaternion.identity, plantMaterial, 0);
            }

            foreach (var agent in sporeManager.scavengers)
            {
                Vector3 worldPosition = new Vector3(agent.position.X, agent.position.Y, 0);
                scavMaterial.color = Color.black;
                Graphics.DrawMesh(scavMesh, worldPosition, Quaternion.identity, scavMaterial, 0);
            }
        }
[ContextMenu("Load Save")]
        private void Load()
        {
            sporeManager.fileToLoad = Application.dataPath +filePath+ fileToLoad;
            sporeManager.Load();
        }
        private void OnDrawGizmos()
        {
            if (sporeManager == null) return;

            // Example: Draw grid or visualize entities
            Gizmos.color = Color.gray;
            for (int x = 0; x <= gridSizeX; x++)
                Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, gridSizeY, 0));
            for (int y = 0; y <= gridSizeY; y++)
                Gizmos.DrawLine(new Vector3(0, y, 0), new Vector3(gridSizeX, y, 0));
        }
    }
