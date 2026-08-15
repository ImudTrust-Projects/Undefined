using BepInEx;
using Photon.Pun;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Undefined.Menu;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using Undefined.Mods.Categories;

namespace Undefined.Mods.Categories;

public class SoundBoard : MonoBehaviour
{
    private static SoundBoard instance;
    private static GameObject audioObject;
    private static AudioSource audioSource;
    private static readonly Dictionary<string, AudioClip> audioClipCache = new();
    private static Coroutine currentCoroutine;
    private static bool initialized;
    private static readonly string[] supportedExtensions = { ".wav", ".ogg", ".mp3" };
    private static readonly Dictionary<string, AudioType> extensionToAudioType = new()
    {
        { ".ogg", AudioType.OGGVORBIS },
        { ".mp3", AudioType.MPEG },
        { ".wav", AudioType.WAV }
    };

    public static bool IsPlaying = false;
    public static float RecoverTimer = -1f;
    public static bool HearSelf = true;
    public static bool LoopAudio = false;

    public static string SoundFolder
    {
        get
        {
            string path = Path.Combine(BepInEx.Paths.GameRootPath, Constants.PluginName, "Soundboard");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Initialize()
    {
        if (initialized) return;
        initialized = true;
        LoadSounds();
    }

    public static void LoadSounds()
    {
        try
        {
            List<ButtonInfo> buttons = new List<ButtonInfo>
            {
                new ButtonInfo
                {
                    buttonText = "Return to Main",
                    method = () => Main.activeCategory = Category.Main,
                    isTogglable = false
                },
                
                new ButtonInfo
                {
                    buttonText = "Stop All Sounds",
                    method = StopAll,
                    isTogglable = false
                },
                new ButtonInfo
                {
                    buttonText = "Only Play Audio In Mic",
                    enableMethod = () => { HearSelf = false; },
                    disableMethod = () => { HearSelf = true; },
                    isTogglable = true,
                    enabled = !HearSelf
                },
                new ButtonInfo
                {
                    buttonText = "Loop Audio",
                    enableMethod = () => { LoopAudio = true; },
                    disableMethod = () => { LoopAudio = false; },
                    isTogglable = true,
                    enabled = LoopAudio
                },
                new ButtonInfo
                {
                    buttonText = "Open Sound Folder",
                    method = OpenFolder,
                    isTogglable = false
                },
                new ButtonInfo
                {
                    buttonText = "Reload Sounds",
                    method = ReloadSounds,
                    isTogglable = false
                },
                new ButtonInfo
                {
                    buttonText = "Stop Current Sound",
                    method = Stop,
                    isTogglable = false
                }
            };

            List<string> soundFiles = GetSoundFiles().ToList();

            if (soundFiles.Any())
            {
                buttons.Add(new ButtonInfo
                {
                    buttonText = "↓ Sounds ↓",
                    method = null,
                    isTogglable = false
                });
            }

            List<ButtonInfo> soundButtons = soundFiles
                .Select(file => new ButtonInfo
                {
                    buttonText = Path.GetFileNameWithoutExtension(file).Replace("_", " "),
                    method = () => PlayFile(file),
                    isTogglable = false
                })
                .ToList();

            buttons.AddRange(soundButtons);
            ModButtons.Buttons[Category.SoundBoard] = buttons.ToArray();

            Debug.Log($"Undefined Soundboard: Loaded {soundButtons.Count} sounds from {SoundFolder}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Undefined Soundboard: Failed to load sounds - {ex.Message}");
        }
    }

    private static List<string> GetSoundFiles()
    {
        List<string> files = new List<string>();
        try
        {
            if (!Directory.Exists(SoundFolder))
                return files;

            foreach (string file in Directory.GetFiles(SoundFolder))
            {
                if (supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                    files.Add(file);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Undefined Soundboard: Failed to get sound files - {ex.Message}");
        }
        return files;
    }

    private static void EnsureAudioSource()
    {
        if (audioObject != null) return;

        try
        {
            audioObject = new GameObject("Undefined Soundboard");
            DontDestroyOnLoad(audioObject);
            audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Undefined Soundboard: Failed to create audio source - {ex.Message}");
        }
    }

    public static void PlayFile(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.LogError($"Undefined Soundboard: File not found - {path}");
            return;
        }

        Stop();

        if (currentCoroutine != null && CoroutineManager.instance != null)
        {
            CoroutineManager.instance.StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        EnsureAudioSource();

        if (audioSource == null)
        {
            Debug.LogError("Undefined Soundboard: AudioSource is null");
            return;
        }

        string extension = Path.GetExtension(path).ToLower();
        AudioType audioType = extensionToAudioType.TryGetValue(extension, out var type) ? type : AudioType.WAV;

        if (instance != null)
            currentCoroutine = CoroutineManager.instance.StartCoroutine(instance.LoadAndPlay(path, audioType));
    }

    private IEnumerator LoadAndPlay(string path, AudioType audioType)
    {
        AudioClip clip = null;

        if (audioClipCache.TryGetValue(path, out AudioClip cachedClip) && cachedClip != null)
        {
            clip = cachedClip;
        }
        else
        {
            string uri = "file:///" + path.Replace("\\", "/");
            using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(uri, audioType);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Undefined Soundboard: Failed to load {path} - {request.error}");
                currentCoroutine = null;
                yield break;
            }

            clip = DownloadHandlerAudioClip.GetContent(request);
            if (clip == null)
            {
                Debug.LogError($"Undefined Soundboard: Failed to get audio clip from {path}");
                currentCoroutine = null;
                yield break;
            }

            audioClipCache[path] = clip;
        }

        PushToMic(clip);
        currentCoroutine = null;
    }

    private static void PushToMic(AudioClip clip)
    {
        if (clip == null) return;

        if (PhotonNetwork.InRoom)
        {
            if (HearSelf)
                PlayLocal(clip);

            try
            {
                var recorder = GorillaTagger.Instance.myRecorder;
                if (recorder != null)
                {
                    recorder.SourceType = Recorder.InputSourceType.AudioClip;
                    recorder.AudioClip = clip;
                    recorder.LoopAudioClip = LoopAudio;
                    recorder.IsRecording = false;
                    recorder.RestartRecording(true);
                }
            }
            catch
            {
                PlayLocal(clip);
            }
        }
        else
        {
            PlayLocal(clip);
        }

        IsPlaying = true;
        RecoverTimer = Time.time + clip.length + 0.5f;
    }

    private static void PlayLocal(AudioClip clip)
    {
        EnsureAudioSource();
        audioSource.clip = clip;
        audioSource.loop = LoopAudio;
        audioSource.volume = 1f;
        audioSource.Play();
    }

    private IEnumerator AutoStopCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay + 0.1f);
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (!LoopAudio)
        {
            IsPlaying = false;
            RecoverTimer = -1f;
        }
    }

    public static void Stop()
    {
        if (audioSource != null)
            audioSource.Stop();

        if (currentCoroutine != null && CoroutineManager.instance != null)
        {
            CoroutineManager.instance.StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (PhotonNetwork.InRoom)
        {
            try
            {
                var recorder = GorillaTagger.Instance.myRecorder;
                if (recorder != null)
                {
                    recorder.SourceType = Recorder.InputSourceType.Microphone;
                    recorder.AudioClip = null;
                    recorder.IsRecording = false;
                    recorder.RestartRecording(true);
                }
            }
            catch { }
        }

        IsPlaying = false;
        RecoverTimer = -1f;
    }

    public static void StopAll()
    {
        Stop();

        foreach (AudioClip clip in audioClipCache.Values)
        {
            if (clip != null)
                Destroy(clip);
        }
        audioClipCache.Clear();

        Debug.Log("Undefined Soundboard: All sounds stopped and cache cleared");
    }

    public static void ReloadSounds()
    {
        foreach (AudioClip clip in audioClipCache.Values)
        {
            if (clip != null)
                Destroy(clip);
        }
        audioClipCache.Clear();

        Stop();
        LoadSounds();

        Debug.Log("Undefined Soundboard: Sounds reloaded successfully");
    }

    public static void ClearCache()
    {
        foreach (AudioClip clip in audioClipCache.Values)
        {
            if (clip != null)
                Destroy(clip);
        }
        audioClipCache.Clear();
        Debug.Log("Undefined Soundboard: Audio cache cleared");
    }

    private static void OpenFolder()
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = SoundFolder,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"Undefined Soundboard: Failed to open folder - {ex.Message}");
        }
    }

    public static void Cleanup()
    {
        Stop();
        ClearCache();

        if (audioObject != null)
        {
            Destroy(audioObject);
            audioObject = null;
            audioSource = null;
        }

        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }

    private void Update()
    {
        if (!LoopAudio && IsPlaying && RecoverTimer > 0f && Time.time >= RecoverTimer)
        {
            Stop();
        }
    }
}