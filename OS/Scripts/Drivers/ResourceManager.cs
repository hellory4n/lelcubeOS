using Godot;
using System;

public class ResourceManager : Node {
    /// <summary>
    /// Loads an image from an specified path (not in lelfs).
    /// </summary>
    /// <param name="path">The path of the image (not in lelfs).</param>
    /// <returns>The image loaded.</returns>
    public static ImageTexture LoadImage(string path) {
        Image image = new Image();
        image.Load(path);
        ImageTexture texture = new ImageTexture();
        texture.CreateFromImage(image);
        return texture;
    }

    /// <summary>
    /// Loads audio from an specified path (not in lelfs).
    /// </summary>
    /// <param name="path">The path of the audio (not in lelfs).</param>
    /// <returns>The audio loaded.</returns>
    public static AudioStream LoadAudio(string path) {
        switch (StringExtensions.Extension(path)) {
            case "mp3":
                var emPeeThree = new AudioStreamMP3();
                File file = new File();
                file.Open(path, File.ModeFlags.Read);
                emPeeThree.Data = file.GetBuffer((long)file.GetLen());
                file.Close();
                return emPeeThree;
            case "ogg":
                var ohGeeGee = new AudioStreamOGGVorbis();
                File file2 = new File();
                file2.Open(path, File.ModeFlags.Read);
                ohGeeGee.Data = file2.GetBuffer((long)file2.GetLen());
                file2.Close();
                return ohGeeGee;
            case "wav":
                var wave = new AudioStreamSample();
                File file3 = new File();
                file3.Open(path, File.ModeFlags.Read);
                wave.Data = file3.GetBuffer((long)file3.GetLen());
                file3.Close();
                return wave;
            default:
                GD.PushError("Invalid file!");
                return default;
        }
    }

    /// <summary>
    /// Loads a video from an specified path (not in lelfs).
    /// </summary>
    /// <param name="path">The path of the video (not in lelfs).</param>
    /// <returns>The video loaded.</returns>
    public static VideoStream LoadVideo(string path) {
        return ResourceLoader.Load<VideoStream>(path);
    }
}
