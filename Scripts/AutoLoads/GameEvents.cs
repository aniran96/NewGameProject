using Godot;
using GoblinGridPuzzle.Components;

namespace GoblinGridPuzzle.AutoLoads;

public partial class GameEvents : Node
{
    //singleton
    public static GameEvents Instance { get; private set; }

    //signals
    [Signal]
    public delegate void OnBuildingPlacedEventHandler(BuildingComponent buildingComponent);

    // initalisation
    public override void _Notification(int what)
    {
        base._Notification(what);
        if (what == NotificationSceneInstantiated) { Instance = this; }
    }

    public static void RaiseBuidingPlaced(BuildingComponent buildingComponent)
    {
        Instance.EmitSignal(SignalName.OnBuildingPlaced, buildingComponent);
    }

}
