using Godot;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

public partial class Player : Area2D
{
    //[Signal]
    //public delegate void MovedPlayerEventHandler();
    [Signal]
    public delegate void EndTurnEventHandler();

    public int ID { get; set; }

    //private int ActionPoint { get; set; } = 2;
    private int action_point = 0;

    private bool hovered = false;
    public bool Playing { get; set; } = false;
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        
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

    //public void PlayTurn()
    //{
    //    SetPlaying(true);
    //}
    
    public void SetPlaying(bool val)
    {
        var SelectionBorderIndicator = GetNode<Sprite2D>("SelectionBorderIndicator");
        SelectionBorderIndicator.Visible = val;

        Playing = val;
    }

    public void SetActionPoints(int val)
    {
        Debug.Assert(val >= 0, "Assertion failed: The value should be larger than or equal to 0 when assigning action points to a player");
        action_point = val;
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

    public void MovePlayer()
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        Position = mousePosition;
    }

    public void OnMouseEntered()
    {
        GD.Print("Player ID: " + ID);
        hovered = true;
    }

    public void OnMouseExited()
    {
        hovered = false;
    }
}
