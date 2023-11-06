using Godot;
using System;
using System.Linq;

public class MinesGameGenerator9000 : Node {
    public readonly Texture Mine = ResourceLoader.Load<Texture>("res://Apps/Mines/Assets/OhNoes.png");
    public readonly Texture[] NumberStuff = new Texture[]{
        ResourceLoader.Load<Texture>("res://Apps/Mines/Assets/One.png"),
        ResourceLoader.Load<Texture>("res://Apps/Mines/Assets/Two.png"),
        ResourceLoader.Load<Texture>("res://Apps/Mines/Assets/Three.png"),
        ResourceLoader.Load<Texture>("res://Apps/Mines/Assets/Four.png"),
        ResourceLoader.Load<Texture>("res://Apps/Mines/Assets/Five.png"),
        ResourceLoader.Load<Texture>("res://Apps/Mines/Assets/Six.png"),
        ResourceLoader.Load<Texture>("res://Apps/Mines/Assets/Seven.png"),
        ResourceLoader.Load<Texture>("res://Apps/Mines/Assets/Eight.png"),
    };
    public readonly Texture Nothingness = ResourceLoader.Load<Texture>("res://Apps/Mines/Assets/Nothingness.png");
    public readonly Texture EmptySquare = ResourceLoader.Load<Texture>("res://Apps/Mines/Assets/Thingy2.png");
    Random random = new Random();
    [Export]
    Vector2 Grid = new Vector2(9, 9);
    [Export]
    int Mines = 10;
    public int ShownStuff = 0;
    public int NonExplosiveSquares = 0;

    public override void _Ready() {
        base._Ready();
        // first put the mines in places
        Vector2[] mines = new Vector2[Mines];
        for (int i = 0; i < Mines; i++) {
            Vector2 fun = new Vector2(random.Next(1, (int)Grid.x), random.Next(1, (int)Grid.x));

            // if that mine was already there then try again
            if (mines.Contains(fun)) {
                i--;
                continue;
            }

            mines[i] = fun;
            GetNode<TextureRect>($"../Why/A/Stuff/{fun.x}x{fun.y}").Texture = Mine;
        }

        // then we put the number stuff :)
        foreach (TextureRect square in GetNode("../Why/A/Stuff").GetChildren()) {
            // get the position thingy :)
            string[] yes = square.Name.Split("x");
            var position = new Vector2(int.Parse(yes[0]), int.Parse(yes[1]));

            if (mines.Contains(position))
                continue;

            int nearbyMines = 0;
            // we need to check the thing in all 8 sides of the squares
            Vector2[] epicPlaces = new Vector2[] {
                new Vector2(position.x+1, position.y),
                new Vector2(position.x+1, position.y+1),
                new Vector2(position.x, position.y+1),
                new Vector2(position.x-1, position.y+1),
                new Vector2(position.x-1, position.y),
                new Vector2(position.x-1, position.y-1),
                new Vector2(position.x, position.y-1),
                new Vector2(position.x+1, position.y-1)
            };

            foreach (var place in epicPlaces) {
                TextureRect thingy1 = GetNodeOrNull<TextureRect>($"../Why/A/Stuff/{place.x}x{place.y}");

                if (thingy1 == null)
                    continue;
                
                if (thingy1.Texture == Mine)
                    nearbyMines++;
            }

            if (nearbyMines > 0)
                square.Texture = NumberStuff[nearbyMines-1];
        }

        // yes.
        int totalSquares = (int)(Grid.x * Grid.y);
        NonExplosiveSquares = totalSquares - Mines;
    }

    public override void _Process(float delta) {
        base._Process(delta);
        // win the game :))))
        if (ShownStuff == NonExplosiveSquares) {
            GetNode<GridContainer>($"../Why/A/ClickingStuff").Visible = false;
            var notificationManager = GetNode<NotificationManager>("/root/NotificationManager");
            notificationManager.ShowNotification("You won!");
            ShownStuff = 894028902;
        }
    }
}