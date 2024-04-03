using Godot;
using System;

public partial class HUD : CanvasLayer
{
    [Signal]
    public delegate void MoveActionSelectedEventHandler();
    [Signal]
    public delegate void EndTurnActionSelectedEventHandler();

    public bool hovered_over_ui;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void OnMoveButtonPressed()
    {
        //GetNode<Button>("StartButton").Hide();
        EmitSignal(SignalName.MoveActionSelected);
        GD.Print("Move Action Selected");
    }

    private void OnEndTurnButtonPressed()
    {
        //GetNode<Button>("StartButton").Hide();
        EmitSignal(SignalName.EndTurnActionSelected);
        GD.Print("End Turn Action Selected");
    }

    public void OnMouseEnteredUI()
    {
        hovered_over_ui = true;
        GD.Print("hovered_over_ui: " + hovered_over_ui);
    }

    public void OnMouseExitedUI()
    {
        hovered_over_ui = false;
        GD.Print("hovered_over_ui: " + hovered_over_ui);
    }
}
