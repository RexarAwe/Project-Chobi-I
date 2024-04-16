using Godot;
using System;
using System.Collections.Generic;
using statemachine;
using GodotPlugins.Game;
public partial class Main : Node
{
    private TileMap TileMap;

    [Export]
    public PackedScene PlayerScene { get; set; }
    [Export]
    public int CameraSpeed { get; set; } = 400;

    private List<Player> players = new List<Player>();
    private List<Vector2I> allowed_move_positions = new List<Vector2I>();
    private List<Vector2I> allowed_attack_positions = new List<Vector2I>();
    private Player current_player;
    private int current_player_idx;
    private bool start_round = false;
    private HUD hud;
    private Camera2D camera;

    private bool moving = false;
    private bool melee_attacking = false;
    private bool ranged_attacking = false;
    private bool praying = false;
    private bool ritualing = false;

    public Vector2 TileMapSize; // Size of the game window.
    private Random random = new Random();

    //private bool round_ongoing = false;

    [Signal]
    public delegate void DoneActionEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        TileMap = GetNode<TileMap>("TileMap");
        TileMapSize = TileMap.GetUsedRect().Size;
        //var UsedRect = TileMap.GetUsedRect();
        //GD.Print("UsedRect: " + UsedRect);

        var a = new Class1(); // test custom nuget package
        GD.Print(a.one());

        hud = GetNode<HUD>("HUD");
        camera = GetNode<Camera2D>("camera");

        //GD.Print(camera.GetViewportRect().Size.X); // 1200
        //GD.Print(camera.GetViewportRect().Size.Y); // 800

        //GD.Print(TileMapSize.X * TileMap.TileSet.TileSize.X); // 2688
        //GD.Print(TileMapSize.Y * TileMap.TileSet.TileSize.Y); // 1280

        start_round = true;
    }

    // this should be in move rather than a separate function
    private bool allowed_move()
    {
        GD.Print("checking if legitimate move...");

        // get locations of all other players
        List<Vector2I> player_positions = new List<Vector2I>();
        //GD.Print("players cnt: " + players.Count);
        //GD.Print("player positions:");
        foreach (Player player in players)
        {
            //GD.Print("HERE");
            //GD.Print(player.ID);
            //GD.Print(current_player.ID);

            if (player.ID != current_player.ID && player.dead == false)
            {
                player_positions.Add(TileMap.LocalToMap(player.Position));
                GD.Print(TileMap.LocalToMap(player.Position));
            }
        } 

        var target_position = TileMap.LocalToMap(TileMap.GetLocalMousePosition());
        //GD.Print("target_position: " + target_position);
        if (allowed_move_positions.Contains(target_position) && !player_positions.Contains(target_position))
        {
            //GD.Print("true");
            return true;
        }

        //GD.Print("allowed_move_positions");
        //for (int i = 0; i < allowed_move_positions.Count; i++)
        //{
        //    GD.Print(allowed_move_positions[i]);
        //}
        
        //GD.Print("mouse position");
        //GD.Print(TileMap.LocalToMap(TileMap.GetLocalMousePosition()));
        //GD.Print("false");
        return false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        //GD.Print(camera.Position.X + ", " + camera.Position.Y);

        Vector2 velocity = Vector2.Zero;

        // camera movement through wasd
        if (Input.IsActionPressed("camera_up"))
        {
            velocity.Y -= 1;
        }

        if (Input.IsActionPressed("camera_down"))
        {
            velocity.Y += 1;
        }

        if (Input.IsActionPressed("camera_left"))
        {
            velocity.X -= 1;
        }

        if (Input.IsActionPressed("camera_right"))
        {
            velocity.X += 1;
        }

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * CameraSpeed;
        }

        camera.Position += velocity * (float)delta;
        camera.Position = new Vector2(
            x: Mathf.Clamp(camera.Position.X, (camera.GetViewportRect().Size.X / 2), 1900 - (camera.GetViewportRect().Size.X / 2)),
            y: Mathf.Clamp(camera.Position.Y, (camera.GetViewportRect().Size.Y / 2), 1154 - (camera.GetViewportRect().Size.Y / 2))
        );

        if (start_round)
        {
            start_round = false; // play just one round
            StartRound();
        }

        if (current_player.ActionPoints == 0)
        {
            current_player.SetPlaying(false);

            if (current_player_idx < players.Count)
            {
                PlayTurn(); // next player plays turn
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

        // for move only right now, better way to do this? move button -> set flag to prepare for this, right now it doesn't check? TODO
        if (Input.IsActionJustReleased("left_mouse_click"))
        {
            // for debug
            GD.Print("mouse position from viewport: " + TileMap.LocalToMap(GetViewport().GetMousePosition()));
            GD.Print("mouse position from local: " + TileMap.LocalToMap(TileMap.GetLocalMousePosition()));

            //if (current_player.Playing && round_ongoing && allowed_move())
            if (current_player.Playing && moving && allowed_move() && !hud.hovered_over_ui)
            {
                moving = false;

                current_player.MovePlayer();
                current_player.SetActionPoints(current_player.ActionPoints - 1);

                // reset move range
                allowed_move_positions.Clear();
                TileMap.ClearLayer(1);

                GD.Print("current_player.ActionPoints: " + current_player.ActionPoints);
            }

            if (current_player.Playing && melee_attacking && !hud.hovered_over_ui)
            {
                // do the melee attack stuff here

                // determine the target
                Player target = null;
                foreach (Player player in players)
                {
                    if (player.ID != current_player.ID)
                    {
                        if (player.TilePosition == TileMap.LocalToMap(TileMap.GetLocalMousePosition()))
                        {
                            // found target
                            GD.Print("player " + player.ID + " targeted");
                            target = player;
                        }
                    }
                }

                // roll for the attack and defense
                if (AttackRoll(current_player.Strength, target.Defense))
                {
                    target.Health -= current_player.MeleeDamage;

                    if (target.Health <= 0) 
                    {
                        target.Destroy();
                        // adjust player list // TODO
                        // or just when going to next player turn, check if already dead, if so, go to the next and so on
                    }
                }

                melee_attacking = false;
                current_player.SetActionPoints(current_player.ActionPoints - 1);

                allowed_attack_positions.Clear();
                TileMap.ClearLayer(1);
            }

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

    private int RollDie(int sides)
    {
        return random.Next(sides + 1);
    }

    private bool AttackRoll(int stat_str, int stat_def)
    {
        GD.Print("ATTACKING");

        int max_val_stat_str = 0;
        int max_val_stat_def = 0;
        int val = -1;

        for (int i = 0; i < stat_str; i++)
        {
            val = RollDie(10);
            if (val > max_val_stat_str) // played with d10s
            {
                max_val_stat_str = val;
            }
        }

        for (int i = 0; i < stat_def; i++)
        {
            val = RollDie(10);
            if (val > max_val_stat_def) // played with d10s
            {
                max_val_stat_def = val;
            }
        }

        GD.Print("max_val_stat_str: " + max_val_stat_str);
        GD.Print("max_val_stat_def: " + max_val_stat_def);

        if (max_val_stat_str >= max_val_stat_def)
        {
            return true; // attacker hits
        }
        else
        {
            return false; // nothing happens
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

    // let the next player play their turn
    //public void PlayTurn()
    //{
    //    // setup next player index
    //    current_player = players[current_player_idx];

    //    GD.Print("Playing " + current_player.ID + "'s Turn");
    //    current_player.SetPlaying(true);
    //    current_player_idx++;

    //    GD.Print("remaining action points: " + current_player.ActionPoints);
    //    //PerformAction();
    //}

    public void PlayTurn()
    {
        GD.Print("current_player_idx: " + current_player_idx);
        GD.Print("players.Count: " + players.Count);

        // setup next player index
        if (current_player_idx < players.Count)
        {
            current_player = players[current_player_idx];

            // skip players that were killed before their turns
            while (current_player.dead)
            {
                GD.Print("dead loop: current_player.ID: " + current_player.ID);
                current_player_idx++;
                if (current_player_idx < players.Count)
                {
                    current_player = players[current_player_idx];
                }
                else
                {
                    break;
                }
            }

            if (current_player_idx < players.Count)
            {
                
                GD.Print("Playing " + current_player.ID + "'s Turn");
                current_player.SetPlaying(true);
                current_player_idx++;

                GD.Print("remaining action points: " + current_player.ActionPoints);
                ////PerformAction();
            }
            else
            {
                GD.Print("All players have played a turn. Starting new round.");
                TileMap.ClearLayer(1);
                //round_ongoing = false;
                StartRound();
            }
        }
        else
        {
            GD.Print("All players have played a turn. Starting new round.");
            TileMap.ClearLayer(1);
            //round_ongoing = false;
            StartRound();
        }
    }

    //public void PlayTurn()
    //{
    //    if (current_player_idx >= players.Count)
    //    {
    //        GD.Print("All players have played a turn. Starting new round.");
    //        TileMap.ClearLayer(1);
    //        //round_ongoing = false;
    //        StartRound();
    //    }
    //    else
    //    {
    //        // setup next player index
    //        current_player = players[current_player_idx];

    //        // skip players that were killed before their turns
    //        while (current_player.dead && current_player_idx < players.Count)
    //        {
    //            current_player_idx++;
    //            current_player = players[current_player_idx];
    //        }

    //        if (current_player_idx < players.Count)
    //        {
    //            GD.Print("Playing " + current_player.ID + "'s Turn");
    //            current_player.SetPlaying(true);
    //            current_player_idx++;

    //            GD.Print("remaining action points: " + current_player.ActionPoints);
    //            ////PerformAction();
    //        }
    //        else
    //        {
    //            GD.Print("All players have played a turn. Starting new round.");
    //            TileMap.ClearLayer(1);
    //            //round_ongoing = false;
    //            StartRound();
    //        }
    //    }
    //}

    public void PerformAction()
    {
        // enable the action buttons
        hud.Visible = true;

        // deduct action point based on action performed

        //Move();
    }

    public void Move()
    {
        GD.Print("Move");

        moving = true;

        //GD.Print("MOVE!");

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

    public void MeleeAttack()
    {
        GD.Print("MeleeAttack");
        // check for any valid targets
        allowed_attack_positions = MeleeAttackRange();

        GD.Print("allowed_attack_positions count: " + allowed_attack_positions.Count);

        if (allowed_attack_positions.Count > 0) 
        {
            melee_attacking = true;

            int atlus_source_id = 6;
            Vector2I atlus_coord = new Vector2I(0, 0);
            int alternative_tile = 0;

            for (int i = 0; i < allowed_attack_positions.Count; i++)
            {
                //GD.Print("  " + allowed_move_positions[i]);
                TileMap.SetCell(1, allowed_attack_positions[i], atlus_source_id, atlus_coord, alternative_tile);
            }
        }
        else
        {
            GD.Print("No Valid Melee Attack Targets");
        }
    }

    public void RangedAttack()
    {
        ranged_attacking = true;
    }

    public void Pray()
    {
        praying = true;
    }

    public void Ritual()
    {
        ritualing = true;
    }

    public void EndTurn()
    {
        current_player.ActionPoints = 0;
        GetNode<HUD>("HUD").Visible = false;
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
                if (player.dead == false)
                {
                    player.ID = id;
                    id++;
                    // Add the player to the list
                    players.Add(player);
                }
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
        GD.Print("MoveRange");

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

    public List<Vector2I> MeleeAttackRange()
    {
        GD.Print("MeleeAttackRange");
        //current_player.MeleeAttackRange;

        List<Vector2I> anchor_map_pos_list = new List<Vector2I>();
        List<Vector2I> temp_map_pos_list;
        List<Vector2I> within_range_list = new List<Vector2I>();

        var orig_map_position = TileMap.LocalToMap(current_player.Position);
        anchor_map_pos_list.Add(orig_map_position);

        // check if any other player within melee attack range
        for (int i = 0; i < current_player.MeleeAttackRange; i++)
        {
            temp_map_pos_list = new List<Vector2I>();

            foreach (Vector2I anchor_position in anchor_map_pos_list)
            {
                temp_map_pos_list.AddRange(GetNeighbors(anchor_position));
                within_range_list.AddRange(GetNeighbors(anchor_position));
            }

            anchor_map_pos_list = temp_map_pos_list; // assign the new anchors
        }

        within_range_list = RemoveDuplicatesFromList(within_range_list); // get rid of duplicates

        GD.Print("within_range_list: ");
        for (int i = 0; i < within_range_list.Count; i++)
        {
            GD.Print(within_range_list[i]);
        }

        // check if any player positions are within this range, if not, remove it from the list
        List<Vector2I> target_tile_list = new List<Vector2I>();
        foreach (Player player in players)
        {
            if (player.ID != current_player.ID && player.dead == false)
            {
                GD.Print(player.ID);
                GD.Print(player.TilePosition);
                if (within_range_list.Contains((Vector2I)player.TilePosition))
                {
                    target_tile_list.Add((Vector2I)player.TilePosition);
                }
            }
        }

        return target_tile_list;
    }
}
