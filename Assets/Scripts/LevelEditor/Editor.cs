using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using Newtonsoft.Json;
using TMPro;
using Starpelly;
using SFB;

using RhythmHeavenMania.Editor.Track;
using RhythmHeavenMania.Util;

using System.IO.Compression;
using System.Text;

namespace RhythmHeavenMania.Editor
{
    public class Editor : MonoBehaviour
    {
        private Initializer Initializer;

        [SerializeField] private Canvas MainCanvas;
        [SerializeField] public Camera EditorCamera;

        [Header("Rect")]
        [SerializeField] private RenderTexture ScreenRenderTexture;
        [SerializeField] private RawImage Screen;
        [SerializeField] private RectTransform GridGameSelector;

        [Header("Components")]
        [SerializeField] private Timeline Timeline;
        [SerializeField] private TMP_Text GameEventSelectorTitle;

        [Header("Toolbar")]
        [SerializeField] private Button NewBTN;
        [SerializeField] private Button OpenBTN;
        [SerializeField] private Button SaveBTN;
        [SerializeField] private Button UndoBTN;
        [SerializeField] private Button RedoBTN;
        [SerializeField] private Button MusicSelectBTN;
        [SerializeField] private Button EditorSettingsBTN;
        [SerializeField] private Button EditorThemeBTN;

        [Header("Properties")]
        private bool changedMusic = false;
        private string currentRemixPath = "";
        private int lastEditorObjectsCount = 0;

        public static Editor instance { get; private set; }

        private void Start()
        {
            instance = this;
            Initializer = GetComponent<Initializer>();
        }

        public void Init()
        {
            GameManager.instance.GameCamera.targetTexture = ScreenRenderTexture;
            GameManager.instance.CursorCam.targetTexture = ScreenRenderTexture;
            Screen.texture = ScreenRenderTexture;

            GameManager.instance.Init();
            Timeline.Init();

            for (int i = 0; i < EventCaller.instance.minigames.Count; i++)
            {
                GameObject GameIcon_ = Instantiate(GridGameSelector.GetChild(0).gameObject, GridGameSelector);
                GameIcon_.GetComponent<Image>().sprite = GameIcon(EventCaller.instance.minigames[i].name);
                GameIcon_.gameObject.SetActive(true);
                GameIcon_.name = EventCaller.instance.minigames[i].displayName;
            }

            Tooltip.AddTooltip(NewBTN.gameObject, "New <color=#adadad>[Ctrl+N]</color>");
            Tooltip.AddTooltip(OpenBTN.gameObject, "Open <color=#adadad>[Ctrl+O]</color>");
            Tooltip.AddTooltip(SaveBTN.gameObject, "Save Project <color=#adadad>[Ctrl+S]</color>\nSave Project As <color=#adadad>[Ctrl+Alt+S]</color>");
            Tooltip.AddTooltip(UndoBTN.gameObject, "Undo <color=#adadad>[Ctrl+Z]</color>");
            Tooltip.AddTooltip(RedoBTN.gameObject, "Redo <color=#adadad>[Ctrl+Y or Ctrl+Shift+Z]</color>");
            Tooltip.AddTooltip(MusicSelectBTN.gameObject, "Music Select");
            Tooltip.AddTooltip(EditorSettingsBTN.gameObject, "Editor Settings <color=#adadad>[Ctrl+Shift+O]</color>");
            Tooltip.AddTooltip(EditorThemeBTN.gameObject, "Editor Theme");

            UpdateEditorStatus(true);
        }

        public void Update()
        {
            // This is buggy
            /*if (Conductor.instance.isPlaying || Conductor.instance.isPaused)
            {
                GetComponent<Selections>().enabled = false;
                GetComponent<Selector>().enabled = false;
                GetComponent<BoxSelection>().enabled = false;
            }
            else
            {
                GetComponent<Selections>().enabled = true;
                GetComponent<Selector>().enabled = true;
                GetComponent<BoxSelection>().enabled = true;
            }*/

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                List<TimelineEventObj> ev = new List<TimelineEventObj>();
                for (int i = 0; i < Selections.instance.eventsSelected.Count; i++) ev.Add(Selections.instance.eventsSelected[i]);
                CommandManager.instance.Execute(new Commands.Deletion(ev));
            }

            if (CommandManager.instance.canUndo())
                UndoBTN.transform.GetChild(0).GetComponent<Image>().color = "BD8CFF".Hex2RGB();
            else
                UndoBTN.transform.GetChild(0).GetComponent<Image>().color = Color.gray;

            if (CommandManager.instance.canRedo())
                RedoBTN.transform.GetChild(0).GetComponent<Image>().color = "FFD800".Hex2RGB();
            else
                RedoBTN.transform.GetChild(0).GetComponent<Image>().color = Color.gray;

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                        CommandManager.instance.Redo();
                    else
                        CommandManager.instance.Undo();
                }
                else if (Input.GetKeyDown(KeyCode.Y))
                {
                    CommandManager.instance.Redo();
                }
            }

            if (Timeline.instance.timelineState.selected == true)
            if (Input.GetMouseButtonUp(0) && Timeline.instance.CheckIfMouseInTimeline())
            {
                List<TimelineEventObj> selectedEvents = Timeline.instance.eventObjs.FindAll(c => c.selected == true && c.eligibleToMove == true);

                if (selectedEvents.Count > 0)
                {
                    List<TimelineEventObj> result = new List<TimelineEventObj>();

                    for (int i = 0; i < selectedEvents.Count; i++)
                    {
                        if (selectedEvents[i].isCreating == false)
                        {
                            result.Add(selectedEvents[i]);
                        }
                        selectedEvents[i].OnUp();
                    }
                    CommandManager.instance.Execute(new Commands.Move(result));
                }
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.O))
                {
                    OpenRemix();
                }
                else if (Input.GetKey(KeyCode.LeftAlt))
                {
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        SaveRemix(true);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    SaveRemix(false);
                }
            }

            if (lastEditorObjectsCount != GameManager.instance.BeatmapEntities())
            {
                UpdateEditorStatus(false);
            }

            lastEditorObjectsCount = GameManager.instance.BeatmapEntities();
        }

        public static Sprite GameIcon(string name)
        {
            return Resources.Load<Sprite>($"Sprites/Editor/GameIcons/{name}");
        }

        #region Dialogs

        public void SelectMusic()
        {
            var extensions = new[]
            {
                new ExtensionFilter("Music Files", "mp3", "ogg", "wav")
            };

            StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, false, async (string[] paths) => 
            {
                if (paths.Length > 0)
                {
                    Conductor.instance.musicSource.clip = await LoadClip(Path.Combine(paths)); 
                    changedMusic = true;
                }
            } 
            );


            // byte[] bytes = OggVorbis.VorbisPlugin.GetOggVorbis(Conductor.instance.musicSource.clip, 1);
            // print(bytes.Length);
            // OggVorbis.VorbisPlugin.Save(@"C:/Users/Braedon/Downloads/test.ogg", Conductor.instance.musicSource.clip, 1);
        }

        private async Task<AudioClip> LoadClip(string path)
        {
            AudioClip clip = null;

            AudioType audioType = AudioType.OGGVORBIS;

            // this is a bad solution but i'm lazy
            if (path.Substring(path.Length - 3) == "ogg")
                audioType = AudioType.OGGVORBIS;
            else if (path.Substring(path.Length - 3) == "mp3")
                audioType = AudioType.MPEG;
            else if (path.Substring(path.Length - 3) == "wav")
                audioType = AudioType.WAV;

            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, audioType))
            {
                uwr.SendWebRequest();

                try
                {
                    while (!uwr.isDone) await Task.Delay(5);

                    if (uwr.result == UnityWebRequest.Result.ProtocolError) Debug.Log($"{uwr.error}");
                    else
                    {
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                    }
                }
                catch (Exception err)
                {
                    Debug.Log($"{err.Message}, {err.StackTrace}");
                }
            }

            return clip;
        }


        public void SaveRemix(bool saveAs = true)
        {
            if (saveAs == true)
            {
                SaveRemixFilePanel();
            }
            else
            {
                if (currentRemixPath == string.Empty)
                {
                    SaveRemixFilePanel();
                }
                else
                {
                    SaveRemixFile(currentRemixPath);
                }
            }
        }

        private void SaveRemixFilePanel()
        {
            var extensions = new[]
            {
                new ExtensionFilter("Rhythm Heaven Mania Remix File", "rhmania")
            };

            StandaloneFileBrowser.SaveFilePanelAsync("Save Remix As", "", "remix_level", extensions, (string path) =>
            {
                if (path != String.Empty)
                {
                    SaveRemixFile(path);
                }
            });
        }

        private void SaveRemixFile(string path)
        {
            using (FileStream zipFile = File.Open(path, FileMode.Create))
            {
                using (var archive = new ZipArchive(zipFile, ZipArchiveMode.Update, true))
                {
                    var levelFile = archive.CreateEntry("remix.json", System.IO.Compression.CompressionLevel.NoCompression);
                    using (var zipStream = levelFile.Open())
                        zipStream.Write(Encoding.ASCII.GetBytes(GetJson()), 0, Encoding.ASCII.GetBytes(GetJson()).Length);

                    if (changedMusic || currentRemixPath != path)
                    {
                        byte[] bytes = OggVorbis.VorbisPlugin.GetOggVorbis(Conductor.instance.musicSource.clip, 1);
                        var musicFile = archive.CreateEntry("song.ogg", System.IO.Compression.CompressionLevel.NoCompression);
                        using (var zipStream = musicFile.Open())
                            zipStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public void OpenRemix()
        {
            var extensions = new[]
            {
                new ExtensionFilter("Rhythm Heaven Mania Remix File", "rhmania")
            };

            StandaloneFileBrowser.OpenFilePanelAsync("Open Remix", "", extensions, false, (string[] paths) =>
            {
                var path = Path.Combine(paths);

                if (path != String.Empty)
                {
                    using (FileStream zipFile = File.Open(path, FileMode.Open))
                    {
                        using (var archive = new ZipArchive(zipFile, ZipArchiveMode.Read))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (entry.Name == "remix.json")
                                {
                                    using (var stream = entry.Open())
                                    {
                                        byte[] bytes;
                                        using (var ms = new MemoryStream())
                                        {
                                            stream.CopyTo(ms);
                                            bytes = ms.ToArray();
                                            string json = Encoding.Default.GetString(bytes);
                                            GameManager.instance.LoadRemix(json);
                                            Timeline.instance.LoadRemix();
                                        }
                                    }
                                }
                                else if (entry.Name == "song.ogg")
                                {
                                    using (var stream = entry.Open())
                                    {
                                        byte[] bytes;
                                        using (var ms = new MemoryStream())
                                        {
                                            stream.CopyTo(ms);
                                            bytes = ms.ToArray();
                                            Conductor.instance.musicSource.clip = OggVorbis.VorbisPlugin.ToAudioClip(bytes, "music");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    currentRemixPath = path;
                    CommandManager.instance.Clear();
                }
            });
        }

        #endregion


        private void UpdateEditorStatus(bool updateTime)
        {
            DiscordRPC.DiscordRPC.UpdateActivity("In Editor", $"Objects: {GameManager.instance.Beatmap.entities.Count + GameManager.instance.Beatmap.tempoChanges.Count}", updateTime);
        }

        public string GetJson()
        {
            string json = string.Empty;
            json = JsonConvert.SerializeObject(GameManager.instance.Beatmap);
            return json;
        }

        public void DebugSave()
        {
            // temp
#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(GameManager.instance.txt);
            path = Application.dataPath.Remove(Application.dataPath.Length - 6, 6) + path;
            System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(GameManager.instance.Beatmap));
            Debug.Log("Saved to " + path);
#endif
        }

        public void SetGameEventTitle(string txt)
        {
            GameEventSelectorTitle.text = txt;
        }
    }
}