using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static Godot.Projection;

public partial class Player : Area2D
{
    [Export]
    public int team { get; set; }

    [Export]
    public Cult Cult { get; set; }

    //[Signal]
    //public delegate void MovedPlayerEventHandler();
    [Signal]
    public delegate void EndTurnEventHandler();

    public int ID { get; set; }

    //private int ActionPoint { get; set; } = 2;


    private TileMap TileMap;
    private Sprite2D SelectionBorderIndicator;
    private TileMap MovementRangeIndicator;

    private bool hovered = false;
    public bool Playing { get; set; } = false;
    public bool dead {  get; set; } = false;
    public int ActionPoints { get; set; } = 2;
    public int Speed { get; set; } = 2;

    public int Strength {  get; set; } = 1;
    public int Dexterity { get; set; } = 1;
    public int Defense { get; set; } = 1;
    public int Health { get; set; } = 1;
    public int MeleeAttackRange {  get; set; } = 1;
    public int MeleeDamage {  get; set; } = 1;
    public int RangedAttackRange { get; set; } = 3;
    public int RangedDamage { get; set; } = 1;
    public int team_no { get; set; } = -1;

    public Vector2I TilePosition { get; set; }
    public TileData TileData { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //GD.Print("Position: " + Position);

        TileMap = GetNode<TileMap>("../TileMap");
        SelectionBorderIndicator = GetNode<Sprite2D>("SelectionBorderIndicator");
        MovementRangeIndicator = GetNode<TileMap>("MoveRange");

        var map_position = TileMap.LocalToMap(Position);
        //GD.Print("Map Position: " + map_position);
        var centered_position = TileMap.MapToLocal(map_position);
        //GD.Print("Centered Position: " + centered_position);

        // center the position to the tilemap
        Position = centered_position;
        TilePosition = TileMap.LocalToMap(Position);
        TileData = TileMap.GetCellTileData(0, TilePosition);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Check for mouse click
        //if (Input.IsActionPressed("mouse_click") && hovered)
        //if (Input.IsActionJustReleased("left_mouse_click"))
        //{
        //    if (Playing)
        //    {
        //        Vector2 mousePosition = GetGlobalMousePosition();
        //        Position = mousePosition;

        //        SetPlaying(false);
        //        GD.Print(ID + " moved to" + Position);

        //        EmitSignal(SignalName.EndTurn);
        //        GD.Print("Emitted TurnEnd Signal");
        //    }

        //    //if (hovered)
        //    //{
        //    //    //GD.Print("Mouse is clicked within the CollisionShape2D!");
        //    //    SetSelect(!selected);
        //    //}
        //    //else
        //    //{
        //    //    if (selected)
        //    //    {
        //    //        Vector2 mousePosition = GetGlobalMousePosition();
        //    //        Position = mousePosition;

        //    //        //Vector2 mousePosition = GetViewport().GetMousePosition();
        //    //        //Position = mousePosition;
        //    //    }
        //    //}
        //}

        //if (Input.IsActionJustReleased("end_turn"))
        //{
        //    //GD.Print("End Turn Button Pressed");
        //    //EmitSignal(SignalName.TurnEnd);
        //    //GD.Print("Emitted TurnEnd Signal");
        //}
    }

    public void SetPlaying(bool val)
    {
        // MovementRangeIndicator.Visible = val;
        SelectionBorderIndicator.Visible = val;
        Playing = val;

        // try coloring tiles here
        // MovementRangeIndicator.Clear(); // clear tiles for some reason
        // GD.Print("MovementRangeIndicator.GetUsedCells(1): " + MovementRangeIndicator.GetUsedCells(-1));

        //MovementRangeIndicator.SetCell()
    }

    public void SetActionPoints(int val)
    {
        Debug.Assert(val >= 0, "Assertion failed: The value should be larger than or equal to 0 when assigning action points to a player");
        ActionPoints = val;
    }

    //public void PlayTurn()
    //{
    //    GD.Print("Playing Turn...");

    //    // players can only move during their turn for now, so always default to move
    //    GD.Print("Awaiting Move Action");
    //    // move action
    //    PerformMoveAction();
    //    //await ToSignal(player, Player.SignalName.EndTurn);

    //    EmitSignal(SignalName.TurnEnd);
    //    GD.Print("Emitted TurnEnd Signal");
    //}

    //async public void PerformMoveAction()
    //{
    //    player_moving = true;

    //    // await user input to move to location
    //    await ToSignal(this, Player.SignalName.MovedPlayer);
    //    player_moving = false;
    //    GD.Print("Move Action Performed");
    //}

    //public override void _Input(InputEvent @event)
    //{
    //    //// Mouse in viewport coordinates.
    //    //if (@event is InputEventMouseButton && @event.is_action_released("left_mouse_click"))
    //    //{

    //    //}
    //    //if (@event is InputEventMouseButton eventMouseButton)
    //    //{
    //    //    GD.Print("Mouse Click/Unclick at: ", eventMouseButton.Position);

    //    //    if (hovered)
    //    //    {
    //    //        var SelectionBorderIndicator = GetNode<Sprite2D>("SelectionBorderIndicator");
    //    //        SelectionBorderIndicator.Visible = !SelectionBorderIndicator.Visible;
    //    //    }
    //    //}

    //    //else if (@event is InputEventMouseMotion eventMouseMotion)
    //    //    GD.Print("Mouse Motion at: ", eventMouseMotion.Position);

    //    //// Print the size of the viewport.
    //    //GD.Print("Viewport Resolution is: ", GetViewport().GetVisibleRect().Size);
    //}

    //public void MovePlayer()
    //{
    //    Vector2 mousePosition = GetGlobalMousePosition();
    //    Position = mousePosition;
    //}

    public void MovePlayer()
    {
        Vector2 mousePosition = GetGlobalMousePosition();

        var map_position = TileMap.LocalToMap(mousePosition);
        var centered_position = TileMap.MapToLocal(map_position);

        // center the position to the tilemap
        Position = centered_position;
        TilePosition = TileMap.LocalToMap(Position);
        TileData = TileMap.GetCellTileData(0, TilePosition);
        GD.Print("on " + TileData.GetCustomData("terrain_type"));
    }

    public void OnMouseEntered()
    {
        GD.Print("Player ID: " + ID + "; Health: " + Health);
        GD.Print();
        hovered = true;
    }

    public void OnMouseExited()
    {
        hovered = false;
    }

    public void Destroy()
    {
        dead = true;
        Hide(); // maybe just change to grave sprite instead of hiding
    }
}
