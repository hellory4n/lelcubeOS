using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

/// <summary>
/// A lelfs folder.
/// </summary>
public class Folder : LelfsFile {
    /// <summary>
    /// Copies this folder and all of its items.
    /// </summary>
    /// <param name="name">The name of the new folder.</param>
    /// <param name="parent">The ID of the parent of the new folder.</param>
    /// <returns>The ID of the copied folder.</returns>
    public override string Copy(string name, string parent = null) {
        // MemberwiseClone() is no worky xd
        var gaming = JsonConvert.DeserializeObject<Folder>(
            JsonConvert.SerializeObject(this)
        );

        gaming.Name = name;
        gaming.Parent = parent;
        gaming.Id = LelfsManager.GenerateID();

        if (parent != "root") {
            LelfsFile m = LelfsManager.LoadById<LelfsFile>(parent);
            gaming.Path = $"{m.Path}/{name}";
        } else {
            gaming.Path = $"/{name}";
        }

        gaming.Save();

        foreach (string item in LelfsManager.GetFolderItems(Path)) {
            LelfsFile m = LelfsManager.LoadById<LelfsFile>(item);
            if (m.Type == "Folder") {
                Folder ha = LelfsManager.LoadById<Folder>(m.Id);
                // haha recursion
                ha.Copy(ha.Name, gaming.Id);
            } else {
                m.Copy(m.Name, gaming.Id);
            }
        }

        return gaming.Id;
    }

    /// <summary>
    /// Renames this folder and updates the paths of its items.
    /// </summary>
    /// <param name="name">The new name of the folder.</param>
    public override void Rename(string name) {
        Name = name;

        LelfsManager.Paths.Remove(Path);

        if (Parent != "root") {
            LelfsFile m = LelfsManager.LoadById<LelfsFile>(Parent);
            Path = $"{m.Path}/{name}";
        } else {
            Path = $"/{name}";
        }

        Save();

        foreach (string item in LelfsManager.GetFolderItems(Path)) {
            LelfsFile m = LelfsManager.LoadById<LelfsFile>(item);
            if (m.Type == "Folder") {
                Folder ha = LelfsManager.LoadById<Folder>(m.Id);
                // haha recursion
                ha.Rename(ha.Name);
            } else {
                m.Parent = Id;
                LelfsManager.Paths.Remove(m.Path);
                m.Path = $"{Path}/{m.Name}";
                LelfsManager.Paths.Add(m.Path, m.Id);
                m.Save();
            }
        }

        LelfsManager.SavePaths();
    }

    /// <summary>
    /// Deletes this folder and all of its items.
    /// </summary>
    public override void Delete() {
        Directory directory = new Directory();
        if (directory.FileExists($"user://Users/{SavingManager.CurrentUser}/Files/{Id}.json")) {
            foreach (string item in LelfsManager.GetFolderItems(Path)) {
                LelfsFile m = LelfsManager.LoadById<LelfsFile>(item);
                if (m.Type == "Folder") {
                    Folder ha = LelfsManager.LoadById<Folder>(m.Id);
                    // haha recursion
                    ha.Delete();
                } else {
                    m.Delete();
                }
            }

            directory.Remove($"user://Users/{SavingManager.CurrentUser}/Files/{Id}.json");
            LelfsManager.Paths.Remove(Path);
            LelfsManager.SavePaths();            
        } else {
            GD.PushError("File not saved yet!");
        }
    }

    /// <summary>
    /// Loads a file inside this folder.
    /// </summary>
    /// <typeparam name="T">The type of the file.</typeparam>
    /// <param name="path">The path of the file, example: <c>FolderInsideThis/File</c></param>
    /// <returns>The file loaded.</returns>
    public T LoadLocal<T>(string path) where T : LelfsFile {
        string actualPath = $"{Path}/{path}";
        return LelfsManager.Load<T>(actualPath);
    }

    /// <summary>
    /// Changes the parent of this folder and all of its items.
    /// </summary>
    /// <param name="parent">The ID of the new parent.</param>
    public override void Move(string parent) {
        Parent = parent;

        LelfsManager.Paths.Remove(Path);

        if (Parent != "root") {
            LelfsFile m = LelfsManager.LoadById<LelfsFile>(Parent);
            Path = $"{m.Path}/{Name}";
        } else {
            Path = $"/{Name}";
        }

        Save();

        foreach (string item in LelfsManager.GetFolderItems(Path)) {
            LelfsFile m = LelfsManager.LoadById<LelfsFile>(item);
            if (m.Type == "Folder") {
                Folder ha = LelfsManager.LoadById<Folder>(m.Id);
                // haha recursion
                ha.Move(Id);
            } else {
                m.Parent = Id;
                LelfsManager.Paths.Remove(m.Path);
                m.Path = $"{Path}/{m.Name}";
                LelfsManager.Paths.Add(m.Path, m.Id);
                m.Save();
            }
        }

        LelfsManager.SavePaths();
    }
}
