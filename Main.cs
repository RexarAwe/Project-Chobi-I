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
    private List<Vector2I> allowed_move_positions = new List<Vector2I>();
    private Player current_player;
    private int current_player_idx;
    private bool start_round = false;
    //private bool round_ongoing = false;

    [Signal]
    public delegate void DoneActionEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        TileMap = GetNode<TileMap>("TileMap");        

        var a = new Class1(); // test custom nuget package
        GD.Print(a.one());

        start_round = true;
    }

    private bool allowed_move()
    {
        //GD.Print("checking if legitimate move...");
        if (allowed_move_positions.Contains(TileMap.LocalToMap(GetViewport().GetMousePosition())))
        {
            return true;
        }

        return false;
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
            //if (current_player.Playing && round_ongoing && allowed_move())
            if (current_player.Playing && allowed_move())
            {
                current_player.MovePlayer();
                current_player.SetActionPoints(current_player.ActionPoints - 1);
                GD.Print("current_player.ActionPoints: " + current_player.ActionPoints);

                if (current_player.ActionPoints == 0)
                {
                    current_player.SetPlaying(false);

                    if (current_player_idx < players.Count)
                    {
                        PlayTurn(); // play next player
                    }
                    else
                    {
                        GD.Print("All players have played a turn.");
                        TileMap.ClearLayer(1);
                        //round_ongoing = false;
                        StartRound();
                    }
                }
                else
                {
                    // do i need this?
                    PerformAction();
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
        GD.Print("Starting Round");
        CollectPlayers();
        CheckPlayers();
        ShufflePlayersList();
        CheckPlayers();
        //round_ongoing = true;
        current_player_idx = 0;
        ResetActionPoints();
        PlayTurn(); // play first turn
    }

    public void PlayTurn()
    {
        // setup next player index
        current_player = players[current_player_idx];
        GD.Print("Playing " + current_player.ID + "'s Turn");
        current_player.SetPlaying(true);
        current_player_idx++;

        GD.Print("remaining action points: " + current_player.ActionPoints);
        PerformAction();

        
    }

    public void PerformAction()
    {
        // deduct action point based on action performed

        Move();
    }

    public void Move()
    {
        // define the movement range tile
        int atlus_source_id = 2;
        Vector2I atlus_coord = new Vector2I(0, 0);
        int alternative_tile = 0;

        // reset the movement tiles layer (1)
        TileMap.ClearLayer(1);

        // MoveRange
        allowed_move_positions = MoveRange();
        for (int i = 0; i < allowed_move_positions.Count; i++)
        {
            //GD.Print("  " + allowed_move_positions[i]);
            TileMap.SetCell(1, allowed_move_positions[i], atlus_source_id, atlus_coord, alternative_tile);
        }
    }

    public void EndTurn()
    {

    }

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

    private void ResetActionPoints()
    {
        foreach (Player player in players)
        {
            player.SetActionPoints(2);
        }
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
        List<Vector2I> anchor_map_pos_list = new List<Vector2I>();
        List<Vector2I> temp_map_pos_list;
        List<Vector2I> ret_map_pos_list = new List<Vector2I>();

        var orig_map_position = TileMap.LocalToMap(current_player.Position);
        anchor_map_pos_list.Add(orig_map_position);

        for (int i = 0; i < current_player.Speed; i++)
        {
            temp_map_pos_list = new List<Vector2I>();

            foreach (Vector2I anchor_position in anchor_map_pos_list)
            {
                temp_map_pos_list.AddRange(GetNeighbors(anchor_position));
                ret_map_pos_list.AddRange(GetNeighbors(anchor_position));
            }

            anchor_map_pos_list = temp_map_pos_list; // assign the new anchors
        }

        ret_map_pos_list = RemoveDuplicatesFromList(ret_map_pos_list); // get rid of duplicates

        return ret_map_pos_list;
    }

}
