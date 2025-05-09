using Godot;

namespace GoblinGridPuzzle.Utilities.Constants;

public partial class GameConstants : Node
{
    // inputs
    public static readonly StringName INPUT_SELECT_BUILDING_PLACEMENT = "select_building_placement";
    public static readonly StringName INPUT_CANCEL_BUILDING_PLACEMENT = "cancel_building_placement";
    public static readonly StringName INPUT_DESTROY_BUILDING = "destroy_building";

    // layer names
    public const string IS_BUILDABLE_CUSTOM_DATA = "is_buildable";
    public const string IS_WOOD_CUSTOM_DATA = "is_wood";

    // nodes
    public const string CURSOR_PATH = "Cursor";
    public const string HIGHLIGHT_TILEMAP_LAYER_PATH = "%HighLightTileMapLayer";
    public const string GRIDMANAGER_PATH = "%GridManager";
    public const string YSORTROOT_PATH = "%YSortRoot";
    public const string GAMEUI_PATH = "GameUI";
    public const string BUTTON_HBOX_PATH = "ButtonHB";
    public const string GOLD_MINE_PATH = "%GoldMine";
    // scene paths
    public const string BUILDNG_SCENE_PATH = "res://Scenes/Structures/Buildings/Tower.tscn";
    public const string VILLAGE_SCENE_PATH = "res://Scenes/Structures/Buildings/Village.tscn";
    public const string TOWER_RESOURCE_PATH = "res://Resources/Files/Buildings/TowerResource/tower.tres";
    public const string VILLAGE_RESOURCE_PATH = "res://Resources/Files/Buildings/VillageResource/village.tres";
    // grid size
    public const int GRID_SIZE = 64;

    //highlight radius
    public const int HIGHLIGHT_RADIUS = 3;
}
