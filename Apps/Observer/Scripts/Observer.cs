using Godot;
using System;

public partial class Observer : Lelwindow {
    public enum Mode {
        Image,
        Audio,
        Video,
        Nothing
    }

    public Mode ObserverMode = Mode.Nothing;
    public string MediaId;

    public override void _Ready() {
        base._Ready();
        Load();
    }

    public void Load() {
        switch (ObserverMode) {
            case Mode.Image:
                PackedScene m = ResourceLoader.Load<PackedScene>("res://Apps/Observer/ImageView.tscn");
                Control coolImageThing = m.Instantiate<Control>();

                LelfsFile coolFile = LelfsManager.LoadById<LelfsFile>(MediaId);
                if (coolFile.Data.ContainsKey("Resource")) {
                    // one of the codes of all time
                    coolImageThing.GetNode<TextureRect>("Image").Texture2D = 
                        ResourceManager.LoadImage(coolFile.Data["Resource"].ToString());
                    
                    coolImageThing.GetNode<AddSticker>("AddSticker").TexturePath =
                        coolFile.Data["Resource"].ToString();
                }
                AddChild(coolImageThing);
                break;
            case Mode.Audio:
                PackedScene m2 = ResourceLoader.Load<PackedScene>("res://Apps/Observer/MusicPlayer.tscn");
                Control coolMusicThing = m2.Instantiate<Control>();

                LelfsFile epicFile = LelfsManager.LoadById<LelfsFile>(MediaId);
                if (epicFile.Data.ContainsKey("Resource")) {
                    // one of the codes of all time
                    coolMusicThing.GetNode<MusicPlayer>("M/N/O/Audio").Music = 
                        ResourceManager.LoadAudio(epicFile.Data["Resource"].ToString());
                }

                // figure out the epic cool name :)
                string coolName = "";
                if (epicFile.Metadata.ContainsKey("Artist")) 
                    coolName += $"{epicFile.Metadata["Artist"]} - ";
                if (epicFile.Metadata.ContainsKey("Album"))
                    coolName += $"{epicFile.Metadata["Album"]} - ";
                if (epicFile.Metadata.ContainsKey("TrackNumber"))
                    coolName += $"{epicFile.Metadata["TrackNumber"]} ";
                if (epicFile.Metadata.ContainsKey("Title"))
                    coolName += $"{epicFile.Metadata["Title"]}";
                if (!epicFile.Metadata.ContainsKey("Title"))
                    coolName = epicFile.Name;

                coolMusicThing.GetNode<Label>("M/N/Label").Text = coolName;

                AddChild(coolMusicThing);
                break;
            case Mode.Video:
                PackedScene m3 = ResourceLoader.Load<PackedScene>("res://Apps/Observer/VideoPlayer.tscn");
                Control coolVideoThing = m3.Instantiate<Control>();

                // one of the codes of all time
                LelfsFile majesticFile = LelfsManager.LoadById<LelfsFile>(MediaId);
                if (majesticFile.Data.ContainsKey("Resource") && majesticFile.Data.ContainsKey("Width") &&
                majesticFile.Data.ContainsKey("Height") && majesticFile.Data.ContainsKey("Duration")) {
                    coolVideoThing.GetNode<VideoStreamPlayer>("M/Video").Stream =
                        ResourceManager.LoadVideo(majesticFile.Data["Resource"].ToString());
                    coolVideoThing.GetNode<AspectRatioContainer>("M").Ratio = float.Parse(majesticFile.Data["Width"].ToString()) / float.Parse(majesticFile.Data["Height"].ToString());
                    coolVideoThing.GetNode<ProgressBar>("ProgressBar").MaxValue = float.Parse(
                        majesticFile.Data["Duration"].ToString()
                    );
                }
                AddChild(coolVideoThing);
                break;
            case Mode.Nothing:
                PackedScene m1 = ResourceLoader.Load<PackedScene>("res://Apps/Observer/NothingLoaded.tscn");
                CenterContainer coolNothingThing = m1.Instantiate<CenterContainer>();
                AddChild(coolNothingThing);
                break;
        }
    }
}