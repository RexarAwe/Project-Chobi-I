using Godot;
using System;
using System.Collections.Generic;
using statemachine; 

public partial class Main : Node
{
    private TileMap TileMap;

    [Export]
    public PackedScene PlayerScene { get; set; }
    private List<Player> players = new List<Player>();
    private Player current_player;
    private int current_player_idx;
    private bool start_round = false;
    private bool round_ongoing = false;

    [Signal]
    public delegate void DoneActionEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        TileMap = GetNode<TileMap>("TileMap");        

        var a = new Class1(); // test custom nuget package
        GD.Print(a.one());

        start_round = true;

        // PlayTurn(); // needs a redesign, this should probably move to process, let every "signal" stuff be within this script, with actions and stuff coming from the player scripts
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (start_round)
        {
            start_round = false; // play just one round
            StartRound();
        }

        if (Input.IsActionJustReleased("left_mouse_click"))
        {
            if (current_player.Playing && round_ongoing)
            {
                current_player.MovePlayer();
                current_player.SetPlaying(false);

                if (current_player_idx < players.Count)
                {
                    PlayTurn(); // play next player
                }
                else
                {
                    GD.Print("All players have played a turn.");
                    round_ongoing = false;
                }
            }
        }

        if (Input.IsActionJustReleased("end_turn"))
        {
            //GD.Print("End Turn Button Pressed");
            //EmitSignal(SignalName.TurnEnd);
            //GD.Print("Emitted TurnEnd Signal");
        }
    }

    public void StartRound()
    {
        GD.Print("StartRound");
        CollectPlayers();
        CheckPlayers();
        ShufflePlayersList();
        CheckPlayers();
        round_ongoing = true;
        current_player_idx = 0;
        PlayTurn(); // play first turn
    }

    public void PlayTurn()
    {
        // define the movement range tile
        int atlus_source_id = 2;
        Vector2I atlus_coord = new Vector2I(0, 0);
        int alternative_tile = 0;

        current_player = players[current_player_idx];
        GD.Print("Playing " + current_player.ID + "'s Turn");
        // player.PlayTurn();
        current_player.SetPlaying(true);
        current_player_idx++;
        //GD.Print("current_player_idx: " + current_player_idx);
        //if (current_player_idx == players.Count + 1)
        //{
        //    GD.Print("All players have played a turn.");
        //    round_ongoing = false;
        //}

        //GD.Print("TileMap.GetUsedCells(0): " + TileMap.GetUsedCells(0));
        //GD.Print("TileMap.GetUsedCells(1): " + TileMap.GetUsedCells(1));

        // determine movement range

        //var map_position = TileMap.LocalToMap(current_player.Position);
        //GD.Print("Player at position: " + map_position);

        ////Vector2I neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.LeftSide);
        ////GD.Print("LeftSide: " + neighbor_position);
        //Vector2I neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.TopLeftSide);
        ////GD.Print("TopLeftSide: " + neighbor_position);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);

        //neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.TopSide);
        ////GD.Print("TopSide: " + neighbor_position);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);

        //neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.TopRightSide);
        ////GD.Print("TopRightSide: " + neighbor_position);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);

        ////neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.RightSide);
        ////GD.Print("RightSide: " + neighbor_position);
        //neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.BottomRightSide);
        ////GD.Print("BottomRightSide: " + neighbor_position);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);

        //neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.BottomSide);
        ////GD.Print("BottomSide: " + neighbor_position);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);

        //neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.BottomLeftSide);
        ////GD.Print("BottomLeftSide: " + neighbor_position);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);


    }

    public void EndTurn()
    {

    }

    //async public void PlayTurn()
    //{
    //    // grab the first player
    //    Player player = players[0];
    //    GD.Print(player.ID + "'s Turn");
    //    player.PlayTurn(); 
    //    await ToSignal(player, Player.SignalName.EndTurn); // need to wait for signal, otherwise it will just go to the next player
    //    GD.Print(player.ID + "'s Turn Ended");

    //    //player = players[1];
    //    //GD.Print(player.ID + "'s Turn");
    //    //player.PlayTurn();
    //    //await ToSignal(player, Player.SignalName.EndTurn); // need to wait for signal, otherwise it will just go to the next player
    //    //GD.Print(player.ID + "'s Turn Ended");

    //    //player = players[2];
    //    //GD.Print(player.ID + "'s Turn");
    //    //player.PlayTurn();
    //    //await ToSignal(player, Player.SignalName.EndTurn); // need to wait for signal, otherwise it will just go to the next player
    //    //GD.Print(player.ID + "'s Turn Ended");

    //    //// iterate through and assign action points to each player according to the shuffled order, and allow the player to play their turn
    //    //foreach (Player player in players)
    //    //{
    //    //    GD.Print(player.ID + "'s Turn");

    //    //    // deselect all existing players
    //    //    GetTree().CallGroup("players", "SetPlaying", false);

    //    //    // automatically select the current turn's player
    //    //    player.PlayTurn();
    //    //    await ToSignal(player, Player.SignalName.EndTurn);
    //    //    GD.Print(player.ID + "'s Turn Ended");
    //    //}
    //}

    public void CollectPlayers()
	{
        players = new List<Player>();
        int id = 0;

        // get the players within the scene
        foreach (Node child in GetChildren())
        {
            // Check if the child has the PlayerScript attached
            if (child is Player player)
            {
                player.ID = id;
                id++;
                // Add the player to the list
                players.Add(player);
            }
        }

        GD.Print("players.Count: " + players.Count);
    }

    private void ShufflePlayersList()
    {
        Random random = new Random();

        // Perform Fisher-Yates shuffle
        int n = players.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            Player value = players[k];
            players[k] = players[n];
            players[n] = value;
        }
    }

    public void CheckPlayers()
    {
        GD.Print("Listing player IDs...");
        foreach (Player player in players) 
        { 
            GD.Print(player.ID);
        }
    }

    public void MoveRange(int action_points)
    {
        List<Vector2I> tile_coord_list = new List<Vector2I>();

        var map_position = TileMap.LocalToMap(current_player.Position);

        // in each direction, mark tiles that can be reached based on remaining action points
        for (int i = 0; i < current_player.ActionPoints; i++)
        {

        }

        //Vector2I neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.TopLeftSide);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);

        //neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.TopSide);
        ////GD.Print("TopSide: " + neighbor_position);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);

        //neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.TopRightSide);
        ////GD.Print("TopRightSide: " + neighbor_position);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);

        ////neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.RightSide);
        ////GD.Print("RightSide: " + neighbor_position);
        //neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.BottomRightSide);
        ////GD.Print("BottomRightSide: " + neighbor_position);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);

        //neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.BottomSide);
        ////GD.Print("BottomSide: " + neighbor_position);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);

        //neighbor_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.BottomLeftSide);
        ////GD.Print("BottomLeftSide: " + neighbor_position);
        //TileMap.SetCell(1, neighbor_position, atlus_source_id, atlus_coord, alternative_tile);
    }
}
