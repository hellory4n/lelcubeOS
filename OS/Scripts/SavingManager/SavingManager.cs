using Godot;
using System;
using Newtonsoft.Json;
using System.Linq;

/// <summary>
/// Manages settings and progress both for users, and settings that apply for all users.
/// </summary>
public partial class SavingManager : Node {
    public static string CurrentUser = "";

    // setups settings files, in _EnterTree() since it needs to be ready before the things that use the settings
    public override void _EnterTree() {
        base._EnterTree();
        DirAccess dir = new();
        dir.MakeDirRecursive("user://Settings/");

        File displaySettings = new File();

        if (!displaySettings.FileExists("user://Settings/DisplaySettings.json")) {
            displaySettings.Open("user://Settings/DisplaySettings.json", File.ModeFlags.Write);
            DisplaySettings thing = new()
            {
                Resolution = OS.GetScreenSize()
            };

            // you won't do the the mobile setup thing on a pc
            if (OS.GetName() != "Android")
                thing.AlreadySetup = true;

            displaySettings.StoreString(
                JsonConvert.SerializeObject(thing)
            );
            displaySettings.Close();
        }

        // yes
        File installerInfo = new File();
        if (!installerInfo.FileExists("user://Settings/InstallerInfo.json")) {
            installerInfo.Open("user://Settings/InstallerInfo.json", File.ModeFlags.Write);
            installerInfo.StoreString(
                JsonConvert.SerializeObject(new InstallerInfo())
            );
            installerInfo.Close();
        }
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The name of the new user.</param>
    /// <param name="info">The photo and picture of the new user.</param>
    public static void NewUser(string user, UserInfo info) {
        CurrentUser = user;
        File file = new File();
        DirAccess dir = new();
        dir.MakeDirRecursive($"user://Users/{user}/");
        file.Open($"user://Users/{user}/BasicInfo.json", File.ModeFlags.Write);
        BasicUser m = new();
        file.StoreString(
            JsonConvert.SerializeObject(m)
        );
        file.Close();

        File pain = new File();
        pain.Open($"user://Users/{user}/UserInfo.json", File.ModeFlags.Write);
        pain.StoreString(
            JsonConvert.SerializeObject(info)
        );
        pain.Close();

        File bruh = new File();
        bruh.Open($"user://Users/{user}/UserLelsktop.json", File.ModeFlags.Write);
        bruh.StoreString(
            JsonConvert.SerializeObject(new UserLelsktop())
        );
        bruh.Close();

        File j = new File();
        j.Open($"user://Users/{user}/InstalledApps.json", File.ModeFlags.Write);
        j.StoreString(
            JsonConvert.SerializeObject(new InstalledApps())
        );
        j.Close();

        File suffer = new File();
        suffer.Open($"user://Users/{user}/QuickLaunch.json", File.ModeFlags.Write);
        suffer.StoreString(
            JsonConvert.SerializeObject(new QuickLaunch())
        );
        suffer.Close();

        File sdfhjjhgf = new File();
        sdfhjjhgf.Open($"user://Users/{user}/SocialStuff.json", File.ModeFlags.Write);
        sdfhjjhgf.StoreString(
            JsonConvert.SerializeObject(new SocialStuff())
        );
        sdfhjjhgf.Close();

        File fgbfg = new File();
        fgbfg.Open($"user://Users/{user}/LelsktopPinboard.json", File.ModeFlags.Write);
        fgbfg.StoreString(
            JsonConvert.SerializeObject(new LelsktopPinboard())
        );
        fgbfg.Close();

        // setup the filesystem
        dir.MakeDirRecursive($"user://Users/{user}/Files/");

        File haha = new File();
        haha.Open($"user://Users/{user}/Files/root.json", File.ModeFlags.Write);
        haha.StoreString("{\"$type\":\"LelfsRoot, lelcubeOS\",\"Id\":\"root\",\"Parent\":null,\"Name\":\"\",\"Metadata\":{\"$type\":\"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Object, mscorlib]], mscorlib\"},\"Path3D\":\"/\",\"Type\":\"Root\",\"Data\":{\"$type\":\"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Object, mscorlib]], mscorlib\"}}");

        haha.Close();
        LelfsManager.UpdatePaths();
        LelfsRoot.CreateRoot();
        LelfsManager.NewFileStructure();
    }

    /// <summary>
    /// Loads data from a user.
    /// </summary>
    /// <typeparam name="T">The type of data to load.</typeparam>
    /// <param name="user">The user to load data from.</param>
    /// <returns>The data loaded.</returns>
    public static T Load<T>(string user) {
        string filename = "";
        switch (typeof(T).Name) {
            case nameof(BasicUser):
                filename = $"user://Users/{user}/BasicInfo.json";
                break;
            case nameof(UserInfo):
                filename = $"user://Users/{user}/UserInfo.json";
                break;
            case nameof(UserLelsktop):
                filename = $"user://Users/{user}/UserLelsktop.json";
                break;
            case nameof(InstalledApps):
                filename = $"user://Users/{user}/InstalledApps.json";
                break;
            case nameof(QuickLaunch):
                filename = $"user://Users/{user}/QuickLaunch.json";
                break;
            case nameof(SocialStuff):
                filename = $"user://Users/{user}/SocialStuff.json";
                break;
            case nameof(LelsktopPinboard):
                filename = $"user://Users/{user}/LelsktopPinboard.json";
                break;
            default:
                GD.PushError("Invalid user info type!");
                return default;
        }

        File file = new File();
        if (file.FileExists(filename)) {
            file.Open(filename, File.ModeFlags.Read);
            T m = JsonConvert.DeserializeObject<T>(file.GetAsText(), new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.All
            });
            file.Close();
            return m;
        } else {
            file.Open(filename, File.ModeFlags.Write);
            file.StoreString(
                JsonConvert.SerializeObject(Activator.CreateInstance<T>(), new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.All
            }));
            return Activator.CreateInstance<T>();
        }
    }

    /// <summary>
    /// Saves new data from a user.
    /// </summary>
    /// <typeparam name="T">The type of data to save.</typeparam>
    /// <param name="user">The user to save data from.</param>
    /// <param name="data">The new data to save.</param>
    public static void Save<T>(string user, T data) {
        string filename = "";
        switch (typeof(T).Name) {
            case nameof(BasicUser):
                filename = $"user://Users/{user}/BasicInfo.json";
                break;
            case nameof(UserInfo):
                filename = $"user://Users/{user}/UserInfo.json";
                break;
            case nameof(UserLelsktop):
                filename = $"user://Users/{user}/UserLelsktop.json";
                break;
            case nameof(InstalledApps):
                filename = $"user://Users/{user}/InstalledApps.json";
                break;
            case nameof(QuickLaunch):
                filename = $"user://Users/{user}/QuickLaunch.json";
                break;
            case nameof(SocialStuff):
                filename = $"user://Users/{user}/SocialStuff.json";
                break;
            case nameof(LelsktopPinboard):
                filename = $"user://Users/{user}/LelsktopPinboard.json";
                break;
            default:
                GD.PushError("Invalid user info type!");
                return;
        }

        File file = new File();
        if (file.Open(filename, File.ModeFlags.Write) == Error.Ok) {
            file.StoreString(JsonConvert.SerializeObject(data, new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.All
            }));
            file.Close();
        } else {
            GD.PushError($"Failed to save data from user \"{user}\", are you sure it exists?");
        }
    }

    /// <summary>
    /// Loads settings that apply across all users.
    /// </summary>
    /// <typeparam name="T">The type of settings data to load.</typeparam>
    /// <returns>The loaded settings data.</returns>
    public static T LoadSettings<T>() {
        string filename = "";
        switch (typeof(T).Name) {
            case nameof(DisplaySettings):
                filename = $"user://Settings/DisplaySettings.json";
                break;
            case nameof(InstallerInfo):
                filename = $"user://Settings/InstallerInfo.json";
                break;
            default:
                GD.PushError("Invalid settings type!");
                return default;
        }

        File file = new File();
        if (file.Open(filename, File.ModeFlags.Read) == Error.Ok) {
            T m = JsonConvert.DeserializeObject<T>(file.GetAsText(), new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.All
            });
            file.Close();
            return m;
        } else {
            GD.PushError($"Failed to load settings.");
            return default;
        }
    }

    /// <summary>
    /// Saves new settings that apply across all users.
    /// </summary>
    /// <typeparam name="T">The type of settings data to save.</typeparam>
    /// <param name="data">The new settings data to save.</param>
    public static void SaveSettings<T>(T data) {
        string filename = "";
        switch (typeof(T).Name) {
            case nameof(DisplaySettings):
                filename = $"user://Settings/DisplaySettings.json";
                break;
            case nameof(InstallerInfo):
                filename = $"user://Settings/InstallerInfo.json";
                break;
            default:
                GD.PushError("Invalid settings type!");
                return;
        }

        File file = new File();
        if (file.Open(filename, File.ModeFlags.Write) == Error.Ok) {
            file.StoreString(JsonConvert.SerializeObject(data, new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.All
            }));
            file.Close();
        } else {
            GD.PushError($"Failed to save settings.");
        }
    }

    /// <summary>
    /// Converts a save to the latest version.
    /// </summary>
    /// <param name="user">The user to convert.</param>
    public static void ConvertOldUser(string user) {
        var version = Load<BasicUser>(user);

        // versions before this code existed
        if (version.MajorVersion == 0 && version.MinorVersion < 7) {
            // create the installed apps and quick settings thing
            File j = new File();
            j.Open($"user://Users/{user}/InstalledApps.json", File.ModeFlags.Write);
            j.StoreString(
                JsonConvert.SerializeObject(new InstalledApps(), new JsonSerializerSettings {
                    TypeNameHandling = TypeNameHandling.All
            }));
            j.Close();

            File suffer = new File();
            suffer.Open($"user://Users/{user}/QuickLaunch.json", File.ModeFlags.Write);
            suffer.StoreString(
                JsonConvert.SerializeObject(new QuickLaunch(), new JsonSerializerSettings {
                    TypeNameHandling = TypeNameHandling.All
            }));
            suffer.Close();

            // setup the filesystem
            DirAccess dir = new();
            dir.MakeDirRecursive($"user://Users/{user}/Files/");

            File haha = new File();
            haha.Open($"user://Users/{user}/Files/root.json", File.ModeFlags.Write);
            haha.StoreString("{\"$type\":\"LelfsRoot, lelcubeOS\",\"Id\":\"root\",\"Parent\":null,\"Name\":\"\",\"Metadata\":{\"$type\":\"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Object, mscorlib]], mscorlib\"},\"Path3D\":\"/\",\"Type\":\"Root\",\"Data\":{\"$type\":\"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Object, mscorlib]], mscorlib\"}}");

            haha.Close();
            LelfsManager.UpdatePaths();
            LelfsRoot.CreateRoot();
            LelfsManager.NewFileStructure();

            version.MinorVersion = 7;
            Save(user, version);
        }

        // v0.8 added epic multimedia stuff and very cool utilities :)
        if (version.MajorVersion == 0 && version.MinorVersion == 7) {
            var coolApps = Load<InstalledApps>(user);
            // fun
            var fuckAll = coolApps.All.ToList();
            fuckAll.Add(new Lelapp("Observer", "res://Apps/Observer/Assets/IconSmall.png", "res://Apps/Observer/Observer.tscn"));
            fuckAll.Add(new Lelapp("Notebook", "res://Apps/Notebook/Assets/IconSmall.png", "res://Apps/Notebook/Notebook.tscn"));
            fuckAll.Add(new Lelapp("Calculator", "res://Apps/Calculator/Assets/IconSmall.png", "res://Apps/Calculator/Calculator.tscn"));
            var fuckUtilities = coolApps.Utilities.ToList();
            fuckUtilities.Add(new Lelapp("Observer", "res://Apps/Observer/Assets/IconSmall.png", "res://Apps/Observer/Observer.tscn"));
            fuckUtilities.Add(new Lelapp("Notebook", "res://Apps/Notebook/Assets/IconSmall.png", "res://Apps/Notebook/Notebook.tscn"));
            fuckUtilities.Add(new Lelapp("Calculator", "res://Apps/Calculator/Assets/IconSmall.png", "res://Apps/Calculator/Calculator.tscn"));
            coolApps.All = fuckAll.ToArray();
            coolApps.Utilities = fuckUtilities.ToArray();
            var fuckGraphics = coolApps.Graphics.ToList();
            fuckGraphics.Add(new Lelapp("Observer", "res://Apps/Observer/Assets/IconSmall.png", "res://Apps/Observer/Observer.tscn"));
            coolApps.Graphics = fuckGraphics.ToArray();
            var fuckMultimedia = coolApps.Multimedia.ToList();
            fuckMultimedia.Add(new Lelapp("Observer", "res://Apps/Observer/Assets/IconSmall.png", "res://Apps/Observer/Observer.tscn"));
            coolApps.Multimedia = fuckMultimedia.ToArray();
            Save(user, coolApps);

            version.MinorVersion = 8;
            Save(user, version);
        }

        // v0.9 added the web browser and messaging app and stuff :)
        if (version.MajorVersion == 0 && version.MinorVersion == 8) {
            var coolApps = Load<InstalledApps>(user);
            // fun
            var fuckAll = coolApps.All.ToList();
            fuckAll.Add(new Lelapp("Websites", "res://Apps/Websites/Assets/IconSmall.png", "res://Apps/Websites/Websites.tscn"));
            fuckAll.Add(new Lelapp("Messages", "res://Apps/Messages/Assets/IconSmall.png", "res://Apps/Messages/Messages.tscn"));
            coolApps.All = fuckAll.ToArray();
            var fuckInternet = coolApps.Internet.ToList();
            fuckInternet.Add(new Lelapp("Websites", "res://Apps/Websites/Assets/IconSmall.png", "res://Apps/Websites/Websites.tscn"));
            fuckInternet.Add(new Lelapp("Messages", "res://Apps/Messages/Assets/IconSmall.png", "res://Apps/Messages/Messages.tscn"));
            coolApps.Internet = fuckInternet.ToArray();
            Save(user, coolApps);

            File suffer = new File();
            suffer.Open($"user://Users/{user}/SocialStuff.json", File.ModeFlags.Write);
            suffer.StoreString(
                JsonConvert.SerializeObject(new SocialStuff(), new JsonSerializerSettings {
                    TypeNameHandling = TypeNameHandling.All
            }));
            suffer.Close();

            version.MinorVersion = 9;
            Save(user, version);
        }

        // v0.10 added the games of all time and epic lelsktop updates :)
        if (version.MajorVersion == 0 && version.MinorVersion == 9) {
            var coolApps = Load<InstalledApps>(user);
            // fun
            coolApps.All = coolApps.All.Append(new Lelapp("Mines", "res://Apps/Mines/Assets/IconSmall.png", "res://Apps/Mines/Mines.tscn")).ToArray();
            coolApps.All = coolApps.All.Append(new Lelapp("Sausage Clicker", "res://Apps/SausageClicker/Assets/IconSmall.png", "res://Apps/SausageClicker/SausageClicker.tscn")).ToArray();
            coolApps.All = coolApps.All.Append(new Lelapp("Tour", "res://Apps/Tour/Assets/IconSmall.png", "res://Apps/Tour/Tour.tscn")).ToArray();
            coolApps.Games = coolApps.Games.Append(new Lelapp("Mines", "res://Apps/Mines/Assets/IconSmall.png", "res://Apps/Mines/Mines.tscn")).ToArray();
            coolApps.Games = coolApps.Games.Append(new Lelapp("Sausage Clicker", "res://Apps/SausageClicker/Assets/IconSmall.png", "res://Apps/SausageClicker/SausageClicker.tscn")).ToArray();
            Save(user, coolApps);
            coolApps.Utilities = coolApps.Utilities.Append(new Lelapp("Tour", "res://Apps/Tour/Assets/IconSmall.png", "res://Apps/Tour/Tour.tscn")).ToArray();

            File fgbfg = new File();
            fgbfg.Open($"user://Users/{user}/LelsktopPinboard.json", File.ModeFlags.Write);
            fgbfg.StoreString(
                JsonConvert.SerializeObject(new LelsktopPinboard())
            );
            fgbfg.Close();

            // new files and stuff :)
            Folder samplePictures = LelfsManager.NewFolder("Sample Pictures", LelfsManager.PermanentPath("/Home/Pictures"));
            samplePictures.Metadata.Add("CreationDate", DateTime.Now);
            samplePictures.Save();

            LelfsFile highPeaks = LelfsManager.NewFile("High Peaks", samplePictures.Id);
            highPeaks.Type = "Picture";
            highPeaks.Data.Add("Resource", "res://Assets/Wallpapers/HighPeaks.jpg");
            highPeaks.Metadata.Add("CreationDate", DateTime.Now);
            highPeaks.Save();

            LelfsFile flowers = LelfsManager.NewFile("Flowers", samplePictures.Id);
            flowers.Type = "Picture";
            flowers.Data.Add("Resource", "res://Assets/Wallpapers/Flowers.png");
            flowers.Metadata.Add("CreationDate", DateTime.Now);
            flowers.Save();

            LelfsFile beaches = LelfsManager.NewFile("Beaches", samplePictures.Id);
            beaches.Type = "Picture";
            beaches.Data.Add("Resource", "res://Assets/Wallpapers/Beaches.png");
            beaches.Metadata.Add("CreationDate", DateTime.Now);
            beaches.Save();

            LelfsFile aurora = LelfsManager.NewFile("Aurora", samplePictures.Id);
            aurora.Type = "Picture";
            aurora.Data.Add("Resource", "res://Assets/Wallpapers/Aurora.png");
            aurora.Metadata.Add("CreationDate", DateTime.Now);
            aurora.Save();

            LelfsFile mountains = LelfsManager.NewFile("Mountains", samplePictures.Id);
            mountains.Type = "Picture";
            mountains.Data.Add("Resource", "res://Assets/Wallpapers/Mountains.png");
            mountains.Metadata.Add("CreationDate", DateTime.Now);
            mountains.Save();

            LelfsFile space = LelfsManager.NewFile("Space", samplePictures.Id);
            space.Type = "Picture";
            space.Data.Add("Resource", "res://Assets/Wallpapers/Space.png");
            space.Metadata.Add("CreationDate", DateTime.Now);
            space.Save();

            LelfsFile logo = LelfsManager.NewFile("lelcubeOS", LelfsManager.PermanentPath("/Home/Pictures"));
            logo.Type = "Picture";
            logo.Data.Add("Resource", "res://Assets/Boot/Logo2.png");
            logo.Metadata.Add("CreationDate", DateTime.Now);
            logo.Save();

            version.MinorVersion = 10;
            Save(user, version);
        }
    }
}