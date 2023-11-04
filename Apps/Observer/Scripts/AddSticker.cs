using Godot;
using System;
using System.Linq;

public class AddSticker : TextureButton {
    public string TexturePath;

    public override void _Ready() {
        base._Ready();
        Connect("pressed", this, nameof(Click));
    }

    public void Click() {
        Texture texture = GetNode<TextureRect>("../Image").Texture;

        // save the sticker
        var pinboard = SavingManager.Load<LelsktopPinboard>(SavingManager.CurrentUser);
        var stickerdbgfdf = new PinboardItem {
            TexturePath = TexturePath,
            Position = new Vector2(15, 55)
        };
        pinboard.Items = pinboard.Items.Append(stickerdbgfdf).ToArray();
        SavingManager.Save(SavingManager.CurrentUser, pinboard);

        // add it and stuff :)
        var ftgkvtfyu = ResourceLoader.Load<PackedScene>("res://OS/Lelsktop/Sticker.tscn");
        var sticker = ftgkvtfyu.Instance<KinematicBody2D>();
        sticker.Position = stickerdbgfdf.Position;
        sticker.GetNode<Sprite>("Sprite").Texture = texture;
        sticker.GetNode<CollisionShape2D>("Collision").Shape = new RectangleShape2D {
            Extents = texture.GetSize()/2
        };
        GetNode("/root/Lelsktop/Pinboard").AddChild(sticker);

        var notifications = GetNode<NotificationManager>("/root/NotificationManager");
        notifications.ShowNotification("A sticker has been added to your pinboard.");
    }
}
