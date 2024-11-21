using System.Collections.Generic;
using System.Linq;
using RojoinNeuralNetwork;
using UnityEngine;

public class VoronoiSporeManager : SporeManager
{
    private VoronoiDiagram voronoi;

    public VoronoiSporeManager(VoronoiDiagram voronoi, List<BrainData> herbBrainData,
        List<BrainData> carnivoreBrainData,
        List<BrainData> scavBrainData, int gridSizeX, int gridSizeY, int hervivoreCount, int carnivoreCount,
        int scavengerCount, int turnCount) : base(herbBrainData, carnivoreBrainData, scavBrainData, gridSizeX,
        gridSizeY, hervivoreCount, carnivoreCount, scavengerCount, turnCount)
    {
        this.voronoi = voronoi;
    }


    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        foreach (Plant plant in plants)
        {
            if (!plant.isAvailable)
            {
                voronoi.RemoveItem(new Vector2(plant.position.X, plant.position.Y));
            }
        }
    }

    public override Plant GetNearPlant(System.Numerics.Vector2 position)
    {
        foreach (var poly in voronoi.GetPoly)
        {
            if (poly.IsInside(new Vector2(position.X, position.Y)))
            {
                System.Numerics.Vector2 polyItem = new System.Numerics.Vector2(poly.itemSector.x, poly.itemSector.y);
                var a = plants.Where(p => p.position.Equals(polyItem));

                if (a.Count() > 0)
                {
                    return a.First();
                }
                else
                {
                    return base.GetNearPlant(position);
                }
            }
        }

        return base.GetNearPlant(position);
    }
}