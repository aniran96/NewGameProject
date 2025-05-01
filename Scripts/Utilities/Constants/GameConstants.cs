using Godot;

namespace GoblinGridPuzzle.Utilities.Constants;

public partial class GameConstants : Node
{
    // inputs
    public static readonly StringName LEFT_CLICK = "left_click";

    // layer names
    public const string BUILDABLE_CUSTOM_DATA = "buildable";

    // nodes
    public const string CURSOR_PATH = "Cursor";
    public const string PLACE_BUILDING_BUTON_PATH = "%PlaceBuildingButton";
    public const string HIGHLIGHT_TILEMAP_LAYER_PATH = "%HighLightTileMapLayer";
    public const string GRIDMANAGER_PATH = "%GridManager";

    // scene paths
    public const string BUILDNG_PATH = "res://Scenes/Structures/Buildings/Building.tscn";

    // grid size
    public const int GRID_SIZE = 64;

    //highlight radius
    public const int HIGHLIGHT_RADIUS = 3;
}
