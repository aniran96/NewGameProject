using System.Collections.Generic;
using GoblinGridPuzzle.Components;
using GoblinGridPuzzle.Utilities.Constants;
using Godot;
using GoblinGridPuzzle.AutoLoads;
using System.Linq;
using System;

namespace GoblinGridPuzzle.Managers.Grid;

public partial class GridManager : Node
{
    //signals
    [Signal]
    public delegate void ResourceTilesUpdatedEventHandler(int collectedResourceTiles);
    [Signal]
    public delegate void GridStateUpdatedEventHandler();

    //exported
    // tile nodes
    [ExportGroup("TileMaps")]
    [Export]
    private TileMapLayer _highLightTileMapLayerNode;
    [Export]
    private TileMapLayer _baseTerrainTileMapLayerNode;

    // variables
    private HashSet<Vector2I> _validBuildableTiles = new HashSet<Vector2I>();
    private HashSet<Vector2I> _collectedResourceTiles = new HashSet<Vector2I>();
    private HashSet<Vector2I> _occupiedTiles = new();
    private List<TileMapLayer> _allTileMapLayers;

    public override void _Ready()
    {
        InitalizeVariables();
        ConnectSignals();
    }

    private void InitalizeVariables()
    {
        _allTileMapLayers = GetAllTileMapLayers(_baseTerrainTileMapLayerNode);
    }

    private void ConnectSignals()
    {
        GameEvents.Instance.OnBuildingPlaced += HandleBuildingPlaced;
        GameEvents.Instance.OnBuildingDestroyed += HandleBuildingDestroyed;
    }

    public void HighLightBuildableTiles()
    {
        foreach (var tilePosition in _validBuildableTiles)
        {
            _highLightTileMapLayerNode.SetCell(tilePosition, 1, Vector2I.Zero);
        }
    }

    private bool TileHasCustomData(Vector2I tilePosition, string dataName)
    {
        foreach (var tileLayer in _allTileMapLayers)
        {
            TileData customData = tileLayer.GetCellTileData(tilePosition);
            if (customData == null) { continue; }
            return (bool)customData.GetCustomData(dataName);
        }
        return false;
    }

    public bool IsTilePositionBuildable(Vector2I tilePosition)
    {
        return _validBuildableTiles.Contains(tilePosition);
    }

    private void UpdateValidBuildableTiles(BuildingComponent buildingComponent)
    {
        _occupiedTiles.Add(buildingComponent.GetGridCellPosition());
        Vector2I rootCell = buildingComponent.GetGridCellPosition();
        var validTiles = GetValidTilesInRadius(rootCell, buildingComponent.BuildingResource.BuildableRadius);
        _validBuildableTiles.UnionWith(validTiles);
        _validBuildableTiles.ExceptWith(_occupiedTiles);

        EmitSignal(SignalName.GridStateUpdated);
    }

    private void RecalculateGrid(BuildingComponent excludeBuildingComponent)
    {
        _occupiedTiles.Clear();
        _validBuildableTiles.Clear();
        _collectedResourceTiles.Clear();

        var buildingComponents = GetTree()
                                .GetNodesInGroup(nameof(BuildingComponent))
                                .Cast<BuildingComponent>()
                                .Where(
                                (buildingComponent) =>
                                buildingComponent != excludeBuildingComponent
                                );

        foreach (var buildingComponent in buildingComponents)
        {
            UpdateValidBuildableTiles(buildingComponent);
            UpdateCollectedResourceTiles(buildingComponent);
        }

        EmitSignal(SignalName.ResourceTilesUpdated, _collectedResourceTiles.Count);
        EmitSignal(SignalName.GridStateUpdated);
    }

    private void UpdateCollectedResourceTiles(BuildingComponent buildingComponent)
    {
        Vector2I rootCell = buildingComponent.GetGridCellPosition();
        var resourceTiles = GetResourceTilesInRadius(rootCell, buildingComponent.BuildingResource.ResourceRadius);
        var oldResourceTileCount = _collectedResourceTiles.Count;
        _collectedResourceTiles.UnionWith(resourceTiles);
        if (oldResourceTileCount != _collectedResourceTiles.Count)
        {
            EmitSignal(SignalName.ResourceTilesUpdated, _collectedResourceTiles.Count);
        }
        EmitSignal(SignalName.GridStateUpdated);
    }

    public void HighlightExpandedBuildableTiles(Vector2I rootCell, int radius)
    {
        var validTiles = GetValidTilesInRadius(rootCell, radius).ToHashSet();
        var expandedTiles = validTiles.Except(_validBuildableTiles).Except(_occupiedTiles);
        var atlasCoords = new Vector2I(1, 0);
        foreach (var tilePosition in expandedTiles)
        {
            _highLightTileMapLayerNode.SetCell(tilePosition, 1, atlasCoords);
        }
    }

    public void HighlightResourceTiles(Vector2I rootCell, int radius)
    {
        var resourceTiles = GetResourceTilesInRadius(rootCell, radius);
        var atlasCoords = new Vector2I(1, 0);
        foreach (var tilePosition in resourceTiles)
        {
            _highLightTileMapLayerNode.SetCell(tilePosition, 1, atlasCoords);
        }
    }

    public List<Vector2I> GetTilesInRadius(Vector2I rootCell, int radius, Func<Vector2I, bool> filterfn)
    {
        var result = new List<Vector2I>();
        for (var x = rootCell.X - radius; x <= rootCell.X + radius; x++)
        {
            for (var y = rootCell.Y - radius; y <= rootCell.Y + radius; y++)
            {
                Vector2I tilePosition = new Vector2I(x, y);
                if (!filterfn(tilePosition)) { continue; }
                result.Add(tilePosition);
            }
        }
        return result;
    }

    private List<Vector2I> GetValidTilesInRadius(Vector2I rootCell, int radius)
    {
        return GetTilesInRadius(
                            rootCell,
                            radius,
                            (tilePosition) => TileHasCustomData(tilePosition, GameConstants.IS_BUILDABLE_CUSTOM_DATA));
    }

    private List<Vector2I> GetResourceTilesInRadius(Vector2I rootCell, int radius)
    {
        return GetTilesInRadius(
                           rootCell,
                           radius,
                           (tilePosition) => TileHasCustomData(tilePosition, GameConstants.IS_WOOD_CUSTOM_DATA));
    }

    public void ClearHighLightedTiles()
    {
        _highLightTileMapLayerNode.Clear();
    }

    public Vector2I GetMouseGridCellPosition()
    {
        var mousePosition = _highLightTileMapLayerNode.GetGlobalMousePosition();
        return ConvertWorldPositionToTilePosition(mousePosition);
    }

    public Vector2I ConvertWorldPositionToTilePosition(Vector2 worldPosition)
    {
        var tilePosition = worldPosition / GameConstants.GRID_SIZE;
        tilePosition = tilePosition.Floor();
        return new Vector2I((int)tilePosition.X, (int)tilePosition.Y);
    }

    private List<TileMapLayer> GetAllTileMapLayers(TileMapLayer rootTileMapLayer)
    {
        var result = new List<TileMapLayer>();
        var children = rootTileMapLayer.GetChildren();
        children.Reverse();

        foreach (var child in children)
        {
            if (child is TileMapLayer childLayer)
            {
                result.AddRange(GetAllTileMapLayers(childLayer));
            }
        }
        result.Add(rootTileMapLayer);
        return result;
    }

    private void HandleBuildingPlaced(BuildingComponent buildingComponent)
    {
        UpdateValidBuildableTiles(buildingComponent);
        UpdateCollectedResourceTiles(buildingComponent);
    }

    private void HandleBuildingDestroyed(BuildingComponent buildingComponent)
    {
        RecalculateGrid(buildingComponent);
    }
}