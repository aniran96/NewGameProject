using System.Collections.Generic;
using GoblinGridPuzzle.Components;
using GoblinGridPuzzle.Utilities.Constants;
using Godot;
using GoblinGridPuzzle.AutoLoads;

namespace GoblinGridPuzzle.Managers;

public partial class GridManager : Node
{
    //exported
    // tile nodes
    [ExportGroup("TileMaps")]
    [Export]
    private TileMapLayer _highLightTileMapLayerNode;
    [Export]
    private TileMapLayer _baseTerrainTileMapLayerNode;

    // variables
    private HashSet<Vector2I> _validBuildableTiles = new HashSet<Vector2I>();

    public override void _Ready()
    {
        GameEvents.Instance.OnBuildingPlaced += HandleBuildingPlaced;
    }

    public void HighLightBuildableTiles()
    {
        foreach (var tilePosition in _validBuildableTiles)
        {
            _highLightTileMapLayerNode.SetCell(tilePosition, 1, Vector2I.Zero);
        }
    }

    private bool IsTilePositionValid(Vector2I tilePosition)
    {
        TileData customData = _baseTerrainTileMapLayerNode.GetCellTileData(tilePosition);
        if (customData == null) { return false; }
        return (bool)customData.GetCustomData(GameConstants.BUILDABLE_CUSTOM_DATA);
    }

    public bool IsTilePositionBuildable(Vector2I tilePosition)
    {
        return _validBuildableTiles.Contains(tilePosition);
    }

    private void UpdateValidBuildableTiles(BuildingComponent buildingComponent)
    {
        Vector2I rootCell = buildingComponent.GetGridCellPosition();
        var validTiles = HighLightValidTilesInRadius(rootCell, buildingComponent.BuildableRadius);
        _validBuildableTiles.UnionWith(validTiles);
        _validBuildableTiles.Remove(buildingComponent.GetGridCellPosition());
    }

    public void HighlightExpandedBuildableTiles(Vector2I rootCell, int radius)
    {

    }

    private List<Vector2I> HighLightValidTilesInRadius(Vector2I rootCell, int radius)
    {
        var result = new List<Vector2I>();
        for (var x = rootCell.X - radius; x <= rootCell.X + radius; x++)
        {
            for (var y = rootCell.Y - radius; y <= rootCell.Y + radius; y++)
            {
                Vector2I tilePosition = new Vector2I(x, y);
                if (!IsTilePositionValid(tilePosition)) { continue; }
                result.Add(tilePosition);
            }
        }
        return result;
    }
    public void ClearHighLightedTiles() { _highLightTileMapLayerNode.Clear(); }

    public Vector2I GetMouseGridCellPosition()
    {
        var mousePosition = _highLightTileMapLayerNode.GetGlobalMousePosition();
        var gridPosition = mousePosition / 64;
        gridPosition = gridPosition.Floor();
        return new Vector2I((int)gridPosition.X, (int)gridPosition.Y);
    }


    private void HandleBuildingPlaced(BuildingComponent buildingComponent)
    {
        UpdateValidBuildableTiles(buildingComponent);
    }
}