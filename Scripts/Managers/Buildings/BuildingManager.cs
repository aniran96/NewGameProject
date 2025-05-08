using System.Linq;
using GoblinGridPuzzle.Components;
using GoblinGridPuzzle.Managers.Grid;
using GoblinGridPuzzle.Resources.Buildings;
using GoblinGridPuzzle.Structures.Buildings.Ghosts;
using GoblinGridPuzzle.UI;
using GoblinGridPuzzle.Utilities.Constants;
using GoblinGridPuzzle.Utilities.States;
using Godot;

namespace GoblinGridPuzzle.Managers.Buildings;

public partial class BuildingManager : Node
{
    //class references
    private State _currentState = State.NORMAL;
    // exported Scene References
    [Export]
    private PackedScene _buildingGhostScene;

    // resource references
    private BuildingResource _toPlaceBuildingResource;

    //exported node references
    [ExportGroup("required nodes")]
    [Export]
    private GridManager _gridManagerNode;
    [Export]
    private GameUI _gameUINode;
    [Export]
    private Node2D _ySortRootNode;

    //node references
    private BuildingGhost _buildingGhost;



    //variables
    private int _currentResourceCount;
    private int _startingResourcecount = 4;
    private int _currentlyUsedResourceCount; // used in building
    private int AvailableResourceCount => (_startingResourcecount + _currentResourceCount) - _currentlyUsedResourceCount; // used to calculate remaining resources

    private Vector2I _hoveregGridCellPosition;

    public override void _Ready()
    {
        ConnectSignals();
        GD.Print(AvailableResourceCount);
    }

    public override void _Process(double delta)
    {
        Vector2I gridPosition = _gridManagerNode.GetMouseGridCellPosition();
        if (_hoveregGridCellPosition != gridPosition)
        {
            _hoveregGridCellPosition = gridPosition;
            UpdateHoveredCellDisplay();
        }

        switch (_currentState)
        {
            case State.NORMAL:
                {
                    break;
                }
            case State.PLACING_BUILDING:
                {
                    _buildingGhost.GlobalPosition = gridPosition * GameConstants.GRID_SIZE;
                    break;
                }
        }
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        switch (_currentState)
        {
            case State.NORMAL:
                {
                    if (evt.IsActionPressed(GameConstants.INPUT_DESTROY_BUILDING))
                    {
                        DestroyBuildingAtHoveredCellPosition();

                    }
                    break;
                }
            case State.PLACING_BUILDING:
                {
                    if (evt.IsActionPressed(GameConstants.INPUT_CANCEL_BUILDING_PLACEMENT))
                    {
                        ChangeState(State.NORMAL);
                    }
                    else if (
                        _toPlaceBuildingResource != null &&
                        evt.IsActionPressed(GameConstants.INPUT_SELECT_BUILDING_PLACEMENT) &&
                        IsTilePositionBuildable(_hoveregGridCellPosition)
                    )
                    {

                        PlaceBuildingAtHoveredCellPosition();
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void ConnectSignals()
    {
        _gridManagerNode.ResourceTilesUpdated += HandleResourceTilesUpdated;
        _gameUINode.BuildingResourceSelected += HandleBuildingResourceSelected;
    }

    private void UpdateGridDisplay()
    {
        _gridManagerNode.ClearHighLightedTiles();
        _gridManagerNode.HighLightBuildableTiles();
        if (IsTilePositionBuildable(_hoveregGridCellPosition))
        {
            _gridManagerNode.HighlightExpandedBuildableTiles
            (_hoveregGridCellPosition, _toPlaceBuildingResource.BuildableRadius);
            _gridManagerNode.HighlightResourceTiles
            (_hoveregGridCellPosition, _toPlaceBuildingResource.ResourceRadius);
            _buildingGhost.SetValid();
        }
        else
        {
            _buildingGhost.SetInvalid();
        }

    }

    private void PlaceBuildingAtHoveredCellPosition()
    {
        var building = _toPlaceBuildingResource.BuildingScene.Instantiate<Node2D>();
        building.GlobalPosition = _hoveregGridCellPosition * GameConstants.GRID_SIZE;
        _ySortRootNode.AddChild(building);
        _currentlyUsedResourceCount += _toPlaceBuildingResource.ResourceCost;
        ChangeState(State.NORMAL);
    }

    private void DestroyBuildingAtHoveredCellPosition()
    {
        var buildingComponents = GetTree()
                                .GetNodesInGroup(nameof(BuildingComponent))
                                .Cast<BuildingComponent>()
                                .FirstOrDefault(
                                    (buildingComponent) =>
                                    buildingComponent.GetGridCellPosition() == _hoveregGridCellPosition
                                );
        if (buildingComponents == null)
        {
            return;
        }

        _currentlyUsedResourceCount -= buildingComponents.BuildingResource.ResourceCost;
        buildingComponents.Destroy();
        GD.Print(_currentResourceCount);
    }

    private void ClearBuildingGhost()
    {
        _gridManagerNode.ClearHighLightedTiles();
        if (IsInstanceValid(_buildingGhost))
        {
            _buildingGhost.QueueFree();
        }
        _buildingGhost = null;

    }

    private void ChangeState(State toState)
    {
        switch (_currentState)
        {
            case State.NORMAL:
                {
                    break;
                }
            case State.PLACING_BUILDING:
                {
                    ClearBuildingGhost();
                    _toPlaceBuildingResource = null;
                    break;
                }
            default:
                {
                    break;
                }

        }
        _currentState = toState;

        switch (_currentState)
        {
            case State.NORMAL:
                {
                    break;
                }
            case State.PLACING_BUILDING:
                {
                    _buildingGhost = _buildingGhostScene.Instantiate<BuildingGhost>();
                    _ySortRootNode.AddChild(_buildingGhost);
                    break;
                }
        }
    }

    private bool IsTilePositionBuildable(Vector2I tilePosition)
    {
        return _gridManagerNode.IsTilePositionBuildable(tilePosition) &&
            AvailableResourceCount >= _toPlaceBuildingResource.ResourceCost;
    }

    private void UpdateHoveredCellDisplay()
    {
        switch (_currentState)
        {
            case State.NORMAL:
                {
                    break;
                }
            case State.PLACING_BUILDING:
                {
                    UpdateGridDisplay();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void HandleResourceTilesUpdated(int resourceCount)
    {
        _currentResourceCount = resourceCount;
    }


    private void HandleBuildingResourceSelected(BuildingResource buildingResource)
    {
        ChangeState(State.PLACING_BUILDING);
        var buildingSprite = buildingResource.SpriteScene.Instantiate<Sprite2D>();
        _buildingGhost.AddChild(buildingSprite);
        _toPlaceBuildingResource = buildingResource;
        UpdateGridDisplay();
    }
}
