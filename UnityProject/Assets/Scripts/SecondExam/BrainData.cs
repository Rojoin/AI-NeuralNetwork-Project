using System;
using System.Collections.Generic;


[System.Serializable]
public class BrainData<AgentType> where AgentType : Enum
{
    public AgentType GenomeType { get; set; }

    public int IterationCount = 1;

    public int InputsCount = 4;
    public int HiddenLayers = 1;
    public int OutputsCount = 2;
    public int NeuronsCountPerHL = 7;
    public float Bias = 1f;
    public float P = 0.5f;
    public List<Genome> genomeCollection { get; set; }

    public BrainData(AgentType genomeType)
    {
        GenomeType = genomeType;
    }
}