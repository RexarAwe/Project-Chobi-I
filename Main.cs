using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node
{
    private List<Player> players = new List<Player>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{ 
        CollectPlayers();
        CheckPlayers();
        ShufflePlayersList();
        CheckPlayers();
        PlayRound();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void PlayRound()
    {
        // iterate through and assign action points to each player according to the shuffled order, and allow the player to play their turn
        foreach (Player player in players)
        {
            GD.Print(player.ID + "'s Turn");

            // play turn 
            // action buttons to use action points
        }
    }

    public void CollectPlayers()
	{
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
}
