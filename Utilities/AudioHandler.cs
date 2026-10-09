using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using UnityEngine;

namespace Undefined.Utilities;

public static class AudioHandler
{
    private static readonly Dictionary<string, AudioClip> sounds = new();
    private static readonly HttpClient client = new HttpClient();

    private static readonly string[] Folders =
    {
        "Open-Close",
        "Click-Sounds",
        "Notification-Sounds"
    };

    private static string BasePath =>
        Path.Combine(GetGorillaTagPath(), Constants.PluginName, "Sounds");

    private static string GitBase =>
        Constants.UndefinedResourcesRawUrl;

    public static void LoadSounds()
    {
        Directory.CreateDirectory(BasePath);
        sounds.Clear();

        foreach (var folder in Folders)
            LoadFolder(folder);
    }

    private static void LoadFolder(string folder)
    {
        string localFolder = Path.Combine(BasePath, folder);
        Directory.CreateDirectory(localFolder);

        string manifestUrl = GitBase + folder + "/manifest.txt";

        string manifest;

        try
        {
            manifest = client.GetStringAsync(manifestUrl).Result;
        }
        catch
        {
            return;
        }

        string[] files = manifest.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var file in files)
        {
            if (!file.EndsWith(".wav")) continue;
            ProcessFile(folder, file.Trim(), localFolder);
        }
    }

    private static void ProcessFile(string folder, string file, string localFolder)
    {
        string name = Path.GetFileNameWithoutExtension(file);
        string localPath = Path.Combine(localFolder, file);
        string url = GitBase + folder + "/" + file;

        try
        {
            if (!File.Exists(localPath))
            {
                byte[] data = client.GetByteArrayAsync(url).Result;
                File.WriteAllBytes(localPath, data);
            }

            byte[] audio = File.ReadAllBytes(localPath);
            AudioClip clip = WavUtility.ToAudioClip(audio, name);

            if (clip != null)
                sounds[name] = clip;
        }
        catch { }
    }

    public static void Play(string soundName, float volume = 1f, float pitch = 1f)
    {
        if (!sounds.TryGetValue(soundName, out AudioClip clip))
            return;

        GameObject obj = new GameObject("Sound_" + soundName);
        AudioSource src = obj.AddComponent<AudioSource>();

        src.clip = clip;
        src.volume = volume;
        src.pitch = pitch;
        src.spatialBlend = 0f;
        src.Play();

        UnityEngine.Object.Destroy(obj, clip.length / Mathf.Max(pitch, 0.1f));
    }

    public static string GetGorillaTagPath()
    {
        return BepInEx.Paths.GameRootPath;
    }
}