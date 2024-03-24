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

        // reset the movement tiles layer (1)
        TileMap.ClearLayer(1);

        // MoveRange

        GD.Print("Move Range Position List: ");
        List<Vector2I> move_positions = MoveRange();
        for (int i = 0; i < move_positions.Count; i++)
        {
            GD.Print("  " + move_positions[i]);
            TileMap.SetCell(1, move_positions[i], atlus_source_id, atlus_coord, alternative_tile);
        }

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

    public List<T> RemoveDuplicatesFromList<T>(List<T> list)
    {
        // Use HashSet to store unique items
        HashSet<T> set = new HashSet<T>();

        // List to store unique items
        List<T> uniqueList = new List<T>();

        // Iterate through the list
        foreach (T item in list)
        {
            // If item is not already in the set, add it to the set and unique list
            if (!set.Contains(item))
            {
                set.Add(item);
                uniqueList.Add(item);
            }
        }

        return uniqueList;
    }

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

    public List<Vector2I> GetNeighbors(Vector2I orig_map_position)
    {
        List<Vector2I> map_pos_list = new List<Vector2I>();
        Vector2I map_position;

        // add the neighboring tiles to the list
        map_position = TileMap.GetNeighborCell(orig_map_position, TileSet.CellNeighbor.TopLeftSide);
        map_pos_list.Add(map_position);
        map_position = TileMap.GetNeighborCell(orig_map_position, TileSet.CellNeighbor.TopSide);
        map_pos_list.Add(map_position);
        map_position = TileMap.GetNeighborCell(orig_map_position, TileSet.CellNeighbor.TopRightSide);
        map_pos_list.Add(map_position);
        map_position = TileMap.GetNeighborCell(orig_map_position, TileSet.CellNeighbor.BottomRightSide);
        map_pos_list.Add(map_position);
        map_position = TileMap.GetNeighborCell(orig_map_position, TileSet.CellNeighbor.BottomSide);
        map_pos_list.Add(map_position);
        map_position = TileMap.GetNeighborCell(orig_map_position, TileSet.CellNeighbor.BottomLeftSide);
        map_pos_list.Add(map_position);

        // GD.Print(map_pos_list);

        return map_pos_list;
    }

    public List<Vector2I> MoveRange()
    {
        GD.Print("In MoveRange");

        List<Vector2I> anchor_map_pos_list = new List<Vector2I>();
        List<Vector2I> temp_map_pos_list;
        List<Vector2I> ret_map_pos_list = new List<Vector2I>();

        var orig_map_position = TileMap.LocalToMap(current_player.Position);
        anchor_map_pos_list.Add(orig_map_position);

        GD.Print("Start For Loop");
        for (int i = 0; i < current_player.ActionPoints; i++)
        {
            GD.Print("anchor_map_pos_list:");
            for (int j = 0; j < anchor_map_pos_list.Count; j++)
            {
                GD.Print(anchor_map_pos_list[j]);
            }

            temp_map_pos_list = new List<Vector2I>();

            GD.Print("Start For Loop 2");
            foreach (Vector2I anchor_position in anchor_map_pos_list)
            {
                GD.Print(anchor_position);
                temp_map_pos_list.AddRange(GetNeighbors(anchor_position));
                ret_map_pos_list.AddRange(GetNeighbors(anchor_position));
            }
            GD.Print("End For Loop 2");

            anchor_map_pos_list = temp_map_pos_list; // assign the new anchors
        }
        GD.Print("End For Loop");

        GD.Print("HERE1");

        ret_map_pos_list = RemoveDuplicatesFromList(ret_map_pos_list); // get rid of duplicates

        return ret_map_pos_list;
    }

    //public List<Vector2I> MoveRange()
    //{
    //    List<Vector2I> map_pos_list = new List<Vector2I>();

    //    var orig_map_position = TileMap.LocalToMap(current_player.Position);
    //    Vector2I map_position = orig_map_position;
    //    // in each direction, mark tiles that can be reached based on remaining action points
    //    // TopLeftSide
    //    for (int i = 0; i < current_player.ActionPoints; i++)
    //    {
    //        map_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.TopLeftSide);
    //        map_pos_list.Add(map_position);
    //    }

    //    // TopSide
    //    map_position = orig_map_position;
    //    for (int i = 0; i < current_player.ActionPoints; i++)
    //    {
    //        map_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.TopSide);
    //        map_pos_list.Add(map_position);
    //    }

    //    // TopRightSide
    //    map_position = orig_map_position;
    //    for (int i = 0; i < current_player.ActionPoints; i++)
    //    {
    //        map_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.TopRightSide);
    //        map_pos_list.Add(map_position);
    //    }

    //    // BottomRightSide
    //    map_position = orig_map_position;
    //    for (int i = 0; i < current_player.ActionPoints; i++)
    //    {
    //        map_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.BottomRightSide);
    //        map_pos_list.Add(map_position);
    //    }

    //    // BottomSide
    //    map_position = orig_map_position;
    //    for (int i = 0; i < current_player.ActionPoints; i++)
    //    {
    //        map_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.BottomSide);
    //        map_pos_list.Add(map_position);
    //    }

    //    // BottomLeftSide
    //    map_position = orig_map_position;
    //    for (int i = 0; i < current_player.ActionPoints; i++)
    //    {
    //        map_position = TileMap.GetNeighborCell(map_position, TileSet.CellNeighbor.BottomLeftSide);
    //        map_pos_list.Add(map_position);
    //    }

    //    return map_pos_list;
    //}
}
