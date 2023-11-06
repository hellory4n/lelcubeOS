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
            Position = ResolutionManager.Resolution/2
        };
        string bullshit = LelfsManager.GenerateID();
        pinboard.Items.Add(bullshit, stickerdbgfdf);
        SavingManager.Save(SavingManager.CurrentUser, pinboard);

        // add it and stuff :)
        var ftgkvtfyu = ResourceLoader.Load<PackedScene>("res://OS/Lelsktop/Sticker.tscn");
        var sticker = ftgkvtfyu.Instance<Sticker>();
        sticker.Position = stickerdbgfdf.Position;
        sticker.Texture = texture;
        sticker.PinboardItem = bullshit;
        GetNode("/root/Lelsktop/Pinboard").AddChild(sticker);

        var notifications = GetNode<NotificationManager>("/root/NotificationManager");
        notifications.ShowNotification("A sticker has been added to your pinboard.");
    }
}