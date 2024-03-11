using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class Player : Area2D
{
    public int ID { get; set; }
    private bool selected = false;

    //private int ActionPoint { get; set; } = 2;
    private int action_point;
    public bool Turn { get; set; } = false;

    private bool hovered = false;
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        var SelectionBorderIndicator = GetNode<Sprite2D>("SelectionBorderIndicator");
        SelectionBorderIndicator.Visible = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Check for mouse click
        //if (Input.IsActionPressed("mouse_click") && hovered)
        if (Input.IsActionJustReleased("left_mouse_click"))
        {
            if (hovered)
            {
                //GD.Print("Mouse is clicked within the CollisionShape2D!");
                var SelectionBorderIndicator = GetNode<Sprite2D>("SelectionBorderIndicator");
                SelectionBorderIndicator.Visible = !SelectionBorderIndicator.Visible;
                selected = !selected;
                if (selected)
                {
                    GD.Print("Unit Selected");
                }
                else
                {
                    GD.Print("Unit Deselected");
                }
            }
            else
            {
                if (selected)
                {
                    Vector2 mousePosition = GetGlobalMousePosition();
                    Position = mousePosition;

                    //Vector2 mousePosition = GetViewport().GetMousePosition();
                    //Position = mousePosition;
                }
            }
        }
    }

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

    public void OnMouseEntered()
    {
        GD.Print("Mouse is within the CollisionShape2D!");
        hovered = true;
    }

    public void OnMouseExited()
    {
        GD.Print("Mouse is within the CollisionShape2D!");
        hovered = false;
    }
}
