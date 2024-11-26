using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RojoinNeuralNetwork;
using UnityEngine;
using Logger = RojoinNeuralNetwork.Utils.Logger;

public class SporeSimulation : MonoBehaviour
{
    public string fileToLoad;
    public string filePath = "/Saves/Genomes";
    public string fileExtension = ".spore";
    public bool isSimulationActive = true;
    public bool showGizmos = true;
    public bool loadOnActive = true;
    [SerializeField] private bool logSimulations;
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

    [NonSerialized] public SporeManagerLib sporeManager;
    [SerializeField] public VoronoiDiagram voronoi;
    public float simulationDeltaTime = 0.1f; // Adjust for simulation speed
    private float timer = 0;
    [Header("BrainConfigs")] public float herbBias = 0.5f;
    public float herbP = 0.5f;
    public float herbMoveBias = 0.5f;
    public float herbMoveP = 0.5f;
    public float herbEatBias = 0.5f;
    public float herbEatP = 0.5f;
    public float herbEscapeBias = 0.5f;
    public float herbEscapeP = 0.5f;
    private BrainData mainHerb;
    private BrainData moveBrain;
    private BrainData eatBrain;
    private BrainData escapeBrain;
    public float carnBias = 0.5f;
    public float carnP = 0.1f;
    public float carnEatBias = 0.5f;
    public float carnMoveBias = 0.5f;
    public float carnEatP = 0.1f;
    public float carnMoveP = 0.1f;
    private BrainData mainCarn;
    private BrainData moveCarn;
    private BrainData eatCarn;
    public float scavBias = 0.5f;
    public float scavFlockingBias = 0.5f;
    public float scavP = 0.5f;
    public float scavFlokingP = 0.5f;
    private BrainData mainScav;
    private BrainData flockScav;
    ParallelOptions parallel = new ParallelOptions();
    SaveSystemUnity _saveSystem;


    private const int MAX_OBJS_PER_DRAWCALL = 1000;

    void OnEnable()
    {
        _saveSystem = GetComponent<SaveSystemUnity>();
        Logger.LogAction += LogSimulations;
        mainHerb = new BrainData(11, new int[] { 7, 5, 3 }, 3, herbBias, herbP);
        moveBrain = new BrainData(4, new int[] { 5, 4 }, 4, herbMoveBias, herbMoveP);
        eatBrain = new BrainData(5, new int[] { 3, 2, 2 }, 1, herbEatBias, herbEatP);
        escapeBrain = new BrainData(8, new int[] { 5, 3 }, 4, herbEscapeBias, herbEscapeP);

        mainCarn = new BrainData(5, new int[] { 3, 2 }, 2, carnBias, carnP);
        moveCarn = new BrainData(4, new int[] { 3, 2 }, 2, carnMoveBias, carnMoveP);
        eatCarn = new BrainData(5, new int[] { 2, 2 }, 1, carnEatBias, carnEatP);

        mainScav = new BrainData(5, new int[] { 4, 3 }, 2, scavBias, scavP);
        flockScav = new BrainData(8, new int[] { 7, 6, 5 }, 6, scavFlockingBias, scavFlokingP);
        ;
        parallel.MaxDegreeOfParallelism = 32;


        var herbBrainData = new List<BrainData>
            { mainHerb, moveBrain, eatBrain, escapeBrain };
        var carnBrainData = new List<BrainData> { mainCarn, moveCarn, eatCarn };
        var scavBrainData = new List<BrainData> { mainScav, flockScav };


        sporeManager = new SporeManagerLib(
            herbBrainData,
            carnBrainData,
            scavBrainData,
            gridSizeX,
            gridSizeY,
            herbivoreCount,
            carnivoreCount,
            scavengerCount,
            turnCount
        )
        {
            filepath = Application.dataPath + filePath,
            fileType = fileExtension
        };
        if (loadOnActive)
        {
            Load();
        }
        // _saveSystem.AddObjectToSave(sporeManager);
    }

    private void OnDisable()
    {
        Logger.LogAction -= LogSimulations;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Tick the simulation at a fixed interval
        if (timer >= simulationDeltaTime)
        {
            currentGeneration = sporeManager.generation;
            sporeManager.isActive = isSimulationActive;
            sporeManager.Tick(simulationDeltaTime);
            timer = 0;
        }


        DrawEntities();
    }

    private void LogSimulations(string message)
    {
        if (logSimulations)
        {
            Debug.Log(message);
        }
    }

    private void DrawEntities()
    {
        var entityGroups = new Dictionary<string, (Mesh mesh, Material material, Color color)>
        {
            { "herbis", (herbMesh, herbMaterial, Color.yellow) },
            { "carnivores", (carnMesh, carnMaterial, Color.red) },
            { "plants", (plantMesh, plantMaterial, Color.green) },
            { "scavengers", (scavMesh, scavMaterial, Color.black) }
        };

        foreach (var group in entityGroups)
        {
            List<SporeAgent> entities = group.Key switch
            {
                "herbis" => new List<SporeAgent>(sporeManager.herbis),
                "carnivores" => new List<SporeAgent>(sporeManager.carnivores),
                "plants" => new List<SporeAgent>(sporeManager.plants) // Filter unavailable plants
                ,
                "scavengers" => new List<SporeAgent>(sporeManager.scavengers),
                _ => throw new ArgumentOutOfRangeException(nameof(group.Key))
            };

            (Mesh mesh, Material material, Color defaultColor) = group.Value;

            List<Matrix4x4[]> drawMatrix = new List<Matrix4x4[]>();
            int meshes = entities.Count();

            for (int i = 0; i < meshes; i += MAX_OBJS_PER_DRAWCALL)
            {
                drawMatrix.Add(new Matrix4x4[Math.Min(MAX_OBJS_PER_DRAWCALL, meshes - i)]);
            }

            int index = 0;
            foreach (var agent in entities)
            {
                material.color = defaultColor;

                if (agent.lives < 0)
                {
                    continue;
                }

                // Calculate the transformation matrix
                Vector3 position = new Vector3(agent.position.X, agent.position.Y, 0);
                drawMatrix[index / MAX_OBJS_PER_DRAWCALL][index % MAX_OBJS_PER_DRAWCALL]
                    = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);

                index++;
            }


            for (int i = 0; i < drawMatrix.Count; i++)
            {
                for (int j = 0; j < mesh.subMeshCount; j++)
                {
                    Graphics.DrawMeshInstanced(mesh, j, material, drawMatrix[i]);
                }
            }
        }
    }

    [ContextMenu("Load Save")]
    private void Load()
    {
        sporeManager.fileToLoad = $"{Application.dataPath}{filePath}{fileToLoad}{fileExtension}";
        sporeManager.Load();
    }

    // [ContextMenu("Save")]
    // private void Save()
    // {
    //     sporeManager.Save();
    // }

    private void OnDrawGizmosSelected()
    {
        if (sporeManager == null || !showGizmos) return;


        Gizmos.color = Color.gray;
        for (int x = 0; x <= gridSizeX; x++)
            Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, gridSizeY, 0));
        for (int y = 0; y <= gridSizeY; y++)
            Gizmos.DrawLine(new Vector3(0, y, 0), new Vector3(gridSizeX, y, 0));
    }
}