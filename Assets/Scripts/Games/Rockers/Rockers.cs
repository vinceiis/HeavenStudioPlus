using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Util;

using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;

    public static class NtrRockersLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("rockers", "Rockers", "EB4C94", false, false, new List<GameAction>()
            {
                new GameAction("intervalStart", "Start Interval")
                {
                    defaultLength = 8f,
                    resizable = true,
                    preFunction = delegate { var e = eventCaller.currentEntity; Rockers.PreInterval(e.beat, e.length, e["auto"], e["moveCamera"], e["movePass"]); },
                    parameters = new List<Param>()
                    {
                        new Param("moveCamera", true, "Move Camera", "Should the camera move?"),
                        new Param("movePass", true, "Move Camera (Pass Turn)", "Should the camera move at the auto pass turn?"),
                        new Param("auto", true, "Auto Pass Turn")
                    },
                },
                new GameAction("riff", "Riff")
                {
                    defaultLength = 1f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("respond", true, "Respond", "Will this guitar riff have to be responded to?"),
                        new Param("1JJ", new EntityTypes.Integer(-1, 24, 0), "E2 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("2JJ", new EntityTypes.Integer(-1, 24, 0), "A2 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("3JJ", new EntityTypes.Integer(-1, 24, 0), "D3 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("4JJ", new EntityTypes.Integer(-1, 24, 0), "G3 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("5JJ", new EntityTypes.Integer(-1, 24, 0), "B3 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("6JJ", new EntityTypes.Integer(-1, 24, 0), "E4 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("sampleJJ", Rockers.PremadeSamples.None, "Premade Sample (JJ)", "Use a premade sample?"),
                        new Param("pitchSampleJJ", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones (JJ)", "Pitch up the sample by X amount of semitones?"),
                        new Param("gcJJ", false, "Glee Club Guitar (JJ)", "Will JJ use the same guitar as in the glee club lessons?"),
                        new Param("1S", new EntityTypes.Integer(-1, 24, 0), "E2 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("2S", new EntityTypes.Integer(-1, 24, 0), "A2 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("3S", new EntityTypes.Integer(-1, 24, 0), "D3 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("4S", new EntityTypes.Integer(-1, 24, 0), "G3 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("5S", new EntityTypes.Integer(-1, 24, 0), "B3 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("6S", new EntityTypes.Integer(-1, 24, 0), "E4 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("sampleS", Rockers.PremadeSamples.None, "Premade Sample (Soshi)", "Use a premade sample?"),
                        new Param("pitchSampleS", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones (Soshi)", "Pitch up the sample by X amount of semitones?"),
                        new Param("gcS", false, "Glee Club Guitar (Soshi)", "Will Soshi use the same guitar as in the glee club lessons?")
                    },
                },
                new GameAction("bend", "Bend")
                {
                    defaultLength = 1f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("respond", true, "Respond", "Will this guitar bend have to be responded to?"),
                        new Param("1JJ", new EntityTypes.Integer(-24, 24, 1), "Pitch Bend (JJ)", "How many semitones up is the current riff gonna be pitchbended?"),
                        new Param("1S", new EntityTypes.Integer(-24, 24, 1), "Pitch Bend (Soshi)", "How many semitones up is the current riff gonna be pitchbended?"),
                    },
                },
                new GameAction("prepare", "Prepare")
                {
                    function = delegate { Rockers.instance.Mute(eventCaller.currentEntity["who"]); },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("who", Rockers.WhoMutes.JJ, "Who?", "Who will prepare? (Soshi is only affected by this event in auto-play.)")
                    }
                },
                new GameAction("unPrepare", "Unprepare")
                {
                    function = delegate { Rockers.instance.UnMute(eventCaller.currentEntity["who"]); },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("who", Rockers.WhoMutes.JJ, "Who?", "Who will unprepare? (Soshi is only affected by this event in auto-play.)")
                    }
                },
                new GameAction("passTurn", "Pass Turn")
                {
                    preFunction = delegate { var e = eventCaller.currentEntity; Rockers.PrePassTurn(e.beat, e["moveCamera"]); },
                    parameters = new List<Param>
                    {
                        new Param("moveCamera", true, "Move Camera", "Should the camera move?")
                    },
                    preFunctionLength = 1
                },
                new GameAction("cmon", "C'mon!")
                {
                    function = delegate { var e = eventCaller.currentEntity; Rockers.instance.DefaultCmon(e.beat, new int[4]
                    {
                        e["JJ1"],
                        e["JJ2"],
                        e["JJ3"],
                        e["JJ4"],
                    }, new int[4]
                    {
                        e["pJJ1"],
                        e["pJJ2"],
                        e["pJJ3"],
                        e["pJJ4"],
                    }, new int[4]
                    {
                        e["S1"],
                        e["S2"],
                        e["S3"],
                        e["S4"],
                    }, new int[4]
                    {
                        e["pS1"],
                        e["pS2"],
                        e["pS3"],
                        e["pS4"], 
                    }, e["moveCamera"]
                    ); },
                    defaultLength = 11,
                    parameters = new List<Param>()
                    {
                        new Param("moveCamera", true, "Move Camera", "Should the camera move?"),
                        new Param("JJ1", Rockers.PremadeSamples.ChordG5, "Premade Sample 1 (JJ)", "What sample to use for the 1st riff?"),
                        new Param("pJJ1", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 1 (JJ)", "Pitch up the sample by X amount of semitones?"),
                        new Param("JJ2", Rockers.PremadeSamples.ChordG5, "Premade Sample 2 (JJ)", "What sample to use for the 2nd riff?"),
                        new Param("pJJ2", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 2 (JJ)", "Pitch up the sample by X amount of semitones?"),
                        new Param("JJ3", Rockers.PremadeSamples.ChordG5, "Premade Sample 3 (JJ)", "What sample to use for the 3rd riff?"),
                        new Param("pJJ3", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 3 (JJ)", "Pitch up the sample by X amount of semitones?"),
                        new Param("JJ4", Rockers.PremadeSamples.ChordA, "Premade Sample 4 (JJ)", "What sample to use for the final riff?"),
                        new Param("pJJ4", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 4 (JJ)", "Pitch up the sample by X amount of semitones?"),
                        new Param("S1", Rockers.PremadeSamples.ChordG, "Premade Sample 1 (Soshi)", "What sample to use for the 1st riff?"),
                        new Param("pS1", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 1 (Soshi)", "Pitch up the sample by X amount of semitones?"),
                        new Param("S2", Rockers.PremadeSamples.ChordG, "Premade Sample 2 (Soshi)", "What sample to use for the 2nd riff?"),
                        new Param("pS2", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 2 (Soshi)", "Pitch up the sample by X amount of semitones?"),
                        new Param("S3", Rockers.PremadeSamples.ChordG, "Premade Sample 3 (Soshi)", "What sample to use for the 3rd riff?"),
                        new Param("pS3", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 3 (Soshi)", "Pitch up the sample by X amount of semitones?"),
                        new Param("S4", Rockers.PremadeSamples.ChordA, "Premade Sample 4 (Soshi)", "What sample to use for the final riff?"),
                        new Param("pS4", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 4 (Soshi)", "Pitch up the sample by X amount of semitones?"),
                    }
                },
                new GameAction("lastOne", "Last One!")
                {
                    function = delegate { var e = eventCaller.currentEntity; Rockers.instance.DefaultLastOne(e.beat, new int[3]
                    {
                        e["JJ1"],
                        e["JJ2"],
                        e["JJ3"],
                    }, new int[3]
                    {
                        e["pJJ1"],
                        e["pJJ2"],
                        e["pJJ3"],
                    }, new int[3]
                    {
                        e["S1"],
                        e["S2"],
                        e["S3"],
                    }, new int[3]
                    {
                        e["pS1"],
                        e["pS2"],
                        e["pS3"],
                    }, e["moveCamera"]
                    ); },
                    defaultLength = 7,
                    parameters = new List<Param>()
                    {
                        new Param("moveCamera", true, "Move Camera", "Should the camera move?"),
                        new Param("JJ1", Rockers.PremadeSamples.ChordAsus4, "Premade Sample 1 (JJ)", "What sample to use for the 1st riff?"),
                        new Param("pJJ1", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 1 (JJ)", "Pitch up the sample by X amount of semitones?"),
                        new Param("JJ2", Rockers.PremadeSamples.ChordAsus4, "Premade Sample 2 (JJ)", "What sample to use for the 2nd riff?"),
                        new Param("pJJ2", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 2 (JJ)", "Pitch up the sample by X amount of semitones?"),
                        new Param("JJ3", Rockers.PremadeSamples.ChordAsus4, "Premade Sample 3 (JJ)", "What sample to use for the final riff?"),
                        new Param("pJJ3", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 3 (JJ)", "Pitch up the sample by X amount of semitones?"),
                        new Param("S1", Rockers.PremadeSamples.ChordDmaj9, "Premade Sample 1 (Soshi)", "What sample to use for the 1st riff?"),
                        new Param("pS1", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 1 (Soshi)", "Pitch up the sample by X amount of semitones?"),
                        new Param("S2", Rockers.PremadeSamples.ChordDmaj9, "Premade Sample 2 (Soshi)", "What sample to use for the 2nd riff?"),
                        new Param("pS2", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 2 (Soshi)", "Pitch up the sample by X amount of semitones?"),
                        new Param("S3", Rockers.PremadeSamples.ChordDmaj9, "Premade Sample 3 (Soshi)", "What sample to use for the final riff?"),
                        new Param("pS3", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones 3 (Soshi)", "Pitch up the sample by X amount of semitones?"),
                    }
                },
                new GameAction("count", "Count In")
                {
                    parameters = new List<Param>()
                    {
                        new Param("count", Rockers.CountIn.One, "Count", "Which voiceline?")
                    },
                    preFunction = delegate 
                    { 
                        var e = eventCaller.currentEntity;
                        float offSet = 0;
                        switch (e["count"])
                        {
                            case 1:
                                offSet = 0.028f;
                                break;
                            case 2:
                            case 3:
                                offSet = 0.033f;
                                break;
                            case 4:
                                offSet = 0.034f;
                                break;
                        }
                        SoundByte.PlayOneShot($"games/rockers/count/{e["count"]}", e.beat, 1, 1, false, null, offSet); 
                    }
                },
                new GameAction("voiceLine", "Together Voice Line")
                {
                    parameters = new List<Param>()
                    {
                        new Param("cmon", true, "C'mon!", "Use the C'mon voiceline? If unchecked it uses the Last One voiceline."),
                    },
                    preFunction = delegate
                    {
                        SoundByte.PlayOneShot(eventCaller.currentEntity["cmon"] ? "games/rockers/Cmon" : "games/rockers/LastOne", eventCaller.currentEntity.beat);
                    }
                },
                new GameAction("prepareTogether", "Custom Together Prepare")
                {
                    function = delegate { var e = eventCaller.currentEntity; Rockers.instance.TogetherPrepare(e.beat, e["cmon"] == (int)Rockers.VoiceLineSelection.Cmon, e["cmon"] == (int)Rockers.VoiceLineSelection.None, 
                        e["muteBeat"], e["middleBeat"], e["moveCamera"]); },
                    defaultLength = 3f,
                    parameters = new List<Param>()
                    {
                        new Param("cmon", Rockers.VoiceLineSelection.Cmon, "Voiceline", "Which voiceline should be used?"),
                        new Param("muteBeat", new EntityTypes.Integer(0, 30, 2), "Mute Beat", "How many beats from the start of this event will they prepare mute?"),
                        new Param("middleBeat", new EntityTypes.Integer(0, 30, 2), "Go-to-middle Beat", "How many beats from the start of this event will they go to the middle?"),
                        new Param("moveCamera", true, "Move Camera", "Should the camera move?")
                    }
                },
                new GameAction("riffTogether", "Custom Together Riff")
                {
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("1JJ", new EntityTypes.Integer(-1, 24, 0), "E2 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("2JJ", new EntityTypes.Integer(-1, 24, 0), "A2 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("3JJ", new EntityTypes.Integer(-1, 24, 0), "D3 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("4JJ", new EntityTypes.Integer(-1, 24, 0), "G3 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("5JJ", new EntityTypes.Integer(-1, 24, 0), "B3 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("6JJ", new EntityTypes.Integer(-1, 24, 0), "E4 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("sampleJJ", Rockers.PremadeSamples.None, "Premade Sample (JJ)", "Use a premade sample?"),
                        new Param("pitchSampleJJ", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones (JJ)", "Pitch up the sample by X amount of semitones?"),
                        new Param("gcJJ", false, "Glee Club Guitar (JJ)", "Will JJ use the same guitar as in the glee club lessons?"),
                        new Param("1S", new EntityTypes.Integer(-1, 24, 0), "E2 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("2S", new EntityTypes.Integer(-1, 24, 0), "A2 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("3S", new EntityTypes.Integer(-1, 24, 0), "D3 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("4S", new EntityTypes.Integer(-1, 24, 0), "G3 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("5S", new EntityTypes.Integer(-1, 24, 0), "B3 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("6S", new EntityTypes.Integer(-1, 24, 0), "E4 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("sampleS", Rockers.PremadeSamples.None, "Premade Sample (Soshi)", "Use a premade sample?"),
                        new Param("pitchSampleS", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones (Soshi)", "Pitch up the sample by X amount of semitones?"),
                        new Param("gcS", false, "Glee Club Guitar (Soshi)", "Will Soshi use the same guitar as in the glee club lessons?")
                    }
                },
                new GameAction("riffTogetherEnd", "Custom Together End Riff")
                {
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("1JJ", new EntityTypes.Integer(-1, 24, 0), "E2 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("2JJ", new EntityTypes.Integer(-1, 24, 0), "A2 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("3JJ", new EntityTypes.Integer(-1, 24, 0), "D3 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("4JJ", new EntityTypes.Integer(-1, 24, 0), "G3 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("5JJ", new EntityTypes.Integer(-1, 24, 0), "B3 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("6JJ", new EntityTypes.Integer(-1, 24, 0), "E4 String (JJ)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("sampleJJ", Rockers.PremadeSamples.None, "Premade Sample (JJ)", "Use a premade sample?"),
                        new Param("pitchSampleJJ", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones (JJ)", "Pitch up the sample by X amount of semitones?"),
                        new Param("gcJJ", false, "Glee Club Guitar (JJ)", "Will JJ use the same guitar as in the glee club lessons?"),
                        new Param("1S", new EntityTypes.Integer(-1, 24, 0), "E2 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("2S", new EntityTypes.Integer(-1, 24, 0), "A2 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("3S", new EntityTypes.Integer(-1, 24, 0), "D3 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("4S", new EntityTypes.Integer(-1, 24, 0), "G3 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("5S", new EntityTypes.Integer(-1, 24, 0), "B3 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("6S", new EntityTypes.Integer(-1, 24, 0), "E4 String (Soshi)", "How many semitones up is this string pitched? If left at -1, this string will not play."),
                        new Param("sampleS", Rockers.PremadeSamples.None, "Premade Sample (Soshi)", "Use a premade sample?"),
                        new Param("pitchSampleS", new EntityTypes.Integer(-24, 24, 0), "Sample Semtiones (Soshi)", "Pitch up the sample by X amount of semitones?"),
                        new Param("gcS", false, "Glee Club Guitar (Soshi)", "Will Soshi use the same guitar as in the glee club lessons?")
                    }
                },
            });
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_Rockers;

    public class Rockers : Minigame
    {
        public enum PremadeSamples
        {
            None,
            BendG5,
            BendC6,
            ChordA,
            ChordAsus4,
            ChordBm,
            ChordCSharpm7,
            ChordDmaj7,
            ChordDmaj9,
            ChordFSharp5,
            ChordG,
            ChordG5,
            ChordGdim7,
            ChordGm,
            NoteASharp4,
            NoteA5,
            PracticeChordD,
            Remix6ChordA,
            Remix10ChordD,
            Remix10ChordFSharpm,
            DoremiChordA7,
            DoremiChordAm7,
            DoremiChordC,
            DoremiChordC7,
            DoremiChordCadd9,
            DoremiChordDm,
            DoremiChordDm7,
            DoremiChordEm,
            DoremiChordF,
            DoremiChordFadd9,
            DoremiChordFm,
            DoremiChordG,
            DoremiChordG7,
            DoremiChordGm,
            DoremiChordGsus4,
            DoremiNoteA2,
            DoremiNoteE2
        }
        public enum WhoMutes
        {
            JJ,
            Soshi,
            Both
        }
        public enum CountIn
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4
        }
        public enum VoiceLineSelection
        {
            Cmon,
            LastOne,
            None
        }
        public static Rockers instance;

        public static CallAndResponseHandler crHandlerInstance;

        [Header("Rockers")]
        public RockersRocker JJ;
        public RockersRocker Soshi;

        [Header("Input")]
        [SerializeField] RockersInput rockerInputRef;
        [SerializeField] RockerBendInput rockerBendInputRef;

        private float lastTargetCameraX = 0;
        private float targetCameraX = 0;
        private double cameraMoveBeat = -1;
        private double endBeat = double.MaxValue;
        private static List<double> queuedCameraEvents = new();

        private List<RiqEntity> riffEvents = new List<RiqEntity>();

        private List<RiqEntity> bendEvents = new List<RiqEntity>();

        private List<double> prepareBeatsJJ = new();

        private struct QueuedInterval
        {
            public double beat;
            public float length;
            public bool moveCamera;
            public bool moveCameraPass;
            public bool autoPassTurn;
        }

        private static List<QueuedInterval> queuedIntervals = new();

        private void Awake()
        {
            instance = this;
            if (crHandlerInstance == null)
            {
                crHandlerInstance = new CallAndResponseHandler();
            }
            var tempEvents = EventCaller.GetAllInGameManagerList("rockers", new string[] { "prepare" });
            foreach (var tempEvent in tempEvents)
            {
                if (tempEvent["who"] != (int)WhoMutes.Soshi) prepareBeatsJJ.Add(tempEvent.beat);
            }
            riffEvents = GrabAllRiffEvents();
            bendEvents = GrabAllBendEvents();
        }

        private static List<RiqEntity> GrabAllRiffEvents()
        {
            var tempEvents = EventCaller.GetAllInGameManagerList("rockers", new string[] { "riff" });
            if (tempEvents.Count > 1)
            {
                tempEvents.Sort((s1, s2) => s1.beat.CompareTo(s2.beat));
                double forbiddenLength = tempEvents[0].beat + tempEvents[0].length;
                List<RiqEntity> tempEvents2 = new List<RiqEntity>();
                for (int i = 1; i < tempEvents.Count; i++)
                {
                    if (tempEvents[i].beat > forbiddenLength)
                    {
                        tempEvents2.Add(tempEvents[i]);
                        forbiddenLength = tempEvents[i].beat + tempEvents[i].length;
                    }
                }
                tempEvents2.Add(tempEvents[0]);
                return tempEvents2;
            }
            else
            {
                return tempEvents;
            }
        }

        private static List<RiqEntity> GrabAllBendEvents()
        {
            var tempEvents = EventCaller.GetAllInGameManagerList("rockers", new string[] { "bend" });
            if (tempEvents.Count > 1)
            {
                tempEvents.Sort((s1, s2) => s1.beat.CompareTo(s2.beat));
                double forbiddenLength = tempEvents[0].beat + tempEvents[0].length;
                List<RiqEntity> tempEvents2 = new List<RiqEntity>();
                for (int i = 1; i < tempEvents.Count; i++)
                {
                    if (tempEvents[i].beat > forbiddenLength)
                    {
                        tempEvents2.Add(tempEvents[i]);
                        forbiddenLength = tempEvents[i].beat + tempEvents[i].length;
                    }
                }
                tempEvents2.Add(tempEvents[0]);
                return tempEvents2;
            }
            else
            {
                return tempEvents;
            }
        }

        private static List<RiqEntity> GrabAllInputsBetween(double beat, double endBeat)
        {
            List<RiqEntity> hairEvents = EventCaller.GetAllInGameManagerList("rockers", new string[] { "riff", "bend" });
            List<RiqEntity> tempEvents = new();

            foreach (var entity in hairEvents)
            {
                if (entity.beat >= beat && entity.beat < endBeat)
                {
                    tempEvents.Add(entity);
                }
            }
            return tempEvents;
        }

        private List<RiqEntity> GrabAllTogetherEvents(double beat)
        {
            var tempEvents = EventCaller.GetAllInGameManagerList("rockers", new string[] { "riffTogether", "riffTogetherEnd" });
            var allEnds = EventCaller.GetAllInGameManagerList("gameManager", new string[] { "switchGame" });
            allEnds.Sort((x, y) => x.beat.CompareTo(y.beat));

            //get the beat of the closest end event
            foreach (var end in allEnds)
            {
                if (end.datamodel.Split(2) == "rockers") continue;
                if (end.beat > beat)
                {
                    endBeat = end.beat;
                    break;
                }
            }
            if (tempEvents.Count > 0)
            {
                tempEvents.Sort((s1, s2) => s1.beat.CompareTo(s2.beat));
                List<RiqEntity> tempEvents2 = new List<RiqEntity>();
                for (int i = 0; i < tempEvents.Count; i++)
                {
                    if (tempEvents[i].beat > beat)
                    {
                        tempEvents2.Add(tempEvents[i]);
                    }
                }
                List<RiqEntity> tempEvents3 = new List<RiqEntity>();
                double forbiddenLength = tempEvents2[0].beat + tempEvents2[0].length;
                tempEvents3.Add(tempEvents2[0]);
                for (int i = 1; i < tempEvents2.Count; i++)
                {
                    if (tempEvents2[i].beat > forbiddenLength)
                    {
                        tempEvents3.Add(tempEvents2[i]);
                    }
                }
                List<RiqEntity> tempEvents4 = new List<RiqEntity>();
                for (int i = 0; i < tempEvents3.Count; i++)
                {
                    if (tempEvents3[i].beat < endBeat)
                    {
                        tempEvents4.Add(tempEvents3[i]);
                    }
                }
                return tempEvents4;
            }
            return tempEvents;
        }

        struct QueuedPassTurn
        {
            public double beat;
            public bool moveCamera;
        }

        private static List<QueuedPassTurn> passedTurns = new List<QueuedPassTurn>();

        private void Start()
        {
            if (PlayerInput.Pressing())
            {
                Soshi.Mute();
            }
        }

        public override void OnPlay(double beat)
        {
            crHandlerInstance = null;
        }

        private void OnDestroy()
        {
            if (queuedCameraEvents.Count > 0) queuedCameraEvents.Clear();
            foreach (var evt in scheduledInputs)
            {
                evt.Disable();
            }
        }

        public override void OnGameSwitch(double beat)
        {
            if (queuedIntervals.Count > 0)
            {
                foreach (var interval in queuedIntervals)
                {
                    StartInterval(interval.beat, interval.length, beat, interval.autoPassTurn, interval.moveCameraPass);
                }
                queuedIntervals.Clear();
            }
        }

        private void Update()
        {
            var cond = Conductor.instance;

            if (cond.isPlaying && !cond.isPaused) 
            { 
                if (PlayerInput.Pressed())
                {
                    Soshi.Mute();
                }
                if (PlayerInput.PressedUp() && !IsExpectingInputNow(InputType.STANDARD_UP))
                {
                    Soshi.UnHold();
                }
                if (PlayerInput.GetAnyDirectionDown() && !IsExpectingInputNow(InputType.DIRECTION_DOWN))
                {
                    Soshi.BendUp(Soshi.lastBendPitch);
                }
                if (PlayerInput.GetAnyDirectionUp() && !IsExpectingInputNow(InputType.DIRECTION_UP))
                {
                    Soshi.BendDown();
                }

                if (queuedCameraEvents.Count > 0)
                {
                    foreach (var cameraEvent in queuedCameraEvents)
                    {
                        MoveCamera(cameraEvent);
                    }
                    queuedCameraEvents.Clear();
                }

                if (passedTurns.Count > 0)
                {
                    foreach (var turn in passedTurns)
                    {
                        StandalonePassTurn(turn.beat, turn.moveCamera);
                    }
                    passedTurns.Clear();
                }

                float normalizedBeat = cond.GetPositionFromBeat(cameraMoveBeat, 1f);

                if (normalizedBeat >= 0f && normalizedBeat <= 1f)
                {
                    Util.EasingFunction.Function func = Util.EasingFunction.GetEasingFunction(Util.EasingFunction.Ease.EaseInOutQuad);

                    float newX = func(lastTargetCameraX, targetCameraX, normalizedBeat);
                    GameCamera.additionalPosition = new Vector3(newX, 0, 0);
                }
            }
        }

        public void DefaultLastOne(double beat, int[] JJSamples, int[] JJPitches, int[] SoshiSamples, int[] SoshiPitches, bool moveCamera)
        {
            SoundByte.PlayOneShotGame("rockers/lastOne");
            if (moveCamera)
            {
                lastTargetCameraX = GameCamera.additionalPosition.x;
                targetCameraX = 0;
                cameraMoveBeat = beat + 2;
            }

            BeatAction.New(instance.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 2, delegate
                {
                    if (JJ.together || Soshi.together) return;
                    JJ.PrepareTogether(true);
                    Soshi.PrepareTogether(GameManager.instance.autoplay);
                }),
                new BeatAction.Action(beat + 3, delegate
                {
                    JJ.StrumStrings(false, new int[6], (PremadeSamples)JJSamples[0], JJPitches[0]);
                }),
                new BeatAction.Action(beat + 3.5f, delegate
                {
                    JJ.Mute();
                }),
                new BeatAction.Action(beat + 4.5f, delegate
                {
                    JJ.StrumStrings(false, new int[6], (PremadeSamples)JJSamples[1], JJPitches[1]);
                }),
                new BeatAction.Action(beat + 5f, delegate
                {
                    JJ.Mute();
                }),
                new BeatAction.Action(beat + 6, delegate
                {
                    JJ.StrumStrings(false, new int[6], (PremadeSamples)JJSamples[2], JJPitches[2]);
                }),
                new BeatAction.Action(beat + 6.5f, delegate
                {
                    JJ.Mute();
                }),
            });
            RockersInput riffComp = Instantiate(rockerInputRef, transform);
            riffComp.Init(false, new int[6], beat, 3, (PremadeSamples)SoshiSamples[0], SoshiPitches[0]);
            ScheduleInput(beat, 3.5f, InputType.STANDARD_DOWN, JustMute, MuteMiss, Empty);

            RockersInput riffComp2 = Instantiate(rockerInputRef, transform);
            riffComp2.Init(false, new int[6], beat, 4.5f, (PremadeSamples)SoshiSamples[1], SoshiPitches[1]);
            ScheduleInput(beat, 5f, InputType.STANDARD_DOWN, JustMute, MuteMiss, Empty);

            RockersInput riffComp3 = Instantiate(rockerInputRef, transform);
            riffComp3.Init(false, new int[6], beat, 6, (PremadeSamples)SoshiSamples[2], SoshiPitches[2]);
            ScheduleInput(beat, 6.5f, InputType.STANDARD_DOWN, JustMute, MuteMiss, Empty);
        }

        public void DefaultCmon(double beat, int[] JJSamples, int[] JJPitches, int[] SoshiSamples, int[] SoshiPitches, bool moveCamera)
        {
            SoundByte.PlayOneShotGame("rockers/cmon");
            if (moveCamera)
            {
                lastTargetCameraX = GameCamera.additionalPosition.x;
                targetCameraX = 0;
                cameraMoveBeat = beat + 2;
            }
            BeatAction.New(instance.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 2, delegate
                {
                    if (JJ.together || Soshi.together) return;
                    JJ.PrepareTogether(true);
                    Soshi.PrepareTogether(GameManager.instance.autoplay);
                }),
                new BeatAction.Action(beat + 3, delegate
                {
                    JJ.StrumStrings(false, new int[6], (PremadeSamples)JJSamples[0], JJPitches[0]);
                }),
                new BeatAction.Action(beat + 4, delegate
                {
                    JJ.Mute();
                }),
                new BeatAction.Action(beat + 4.5f, delegate
                {
                    JJ.StrumStrings(false, new int[6], (PremadeSamples)JJSamples[1], JJPitches[1]);
                }),
                new BeatAction.Action(beat + 5.5f, delegate
                {
                    JJ.Mute();
                }),
                new BeatAction.Action(beat + 6, delegate
                {
                    JJ.StrumStrings(false, new int[6], (PremadeSamples)JJSamples[2], JJPitches[2]);
                }),
                new BeatAction.Action(beat + 6.5, delegate
                {
                    JJ.Mute();
                }),
                new BeatAction.Action(beat + 7, delegate
                {
                    JJ.StrumStrings(false, new int[6], (PremadeSamples)JJSamples[3], JJPitches[3], false, true);
                }),
                new BeatAction.Action(beat + 10, delegate
                {
                    JJ.Mute();
                }),
            });
            RockersInput riffComp = Instantiate(rockerInputRef, transform);
            riffComp.Init(false, new int[6], beat, 3, (PremadeSamples)SoshiSamples[0], SoshiPitches[0]);
            ScheduleInput(beat, 4, InputType.STANDARD_DOWN, JustMute, MuteMiss, Empty);

            RockersInput riffComp2 = Instantiate(rockerInputRef, transform);
            riffComp2.Init(false, new int[6], beat, 4.5f, (PremadeSamples)SoshiSamples[1], SoshiPitches[1]);
            ScheduleInput(beat, 5.5f, InputType.STANDARD_DOWN, JustMute, MuteMiss, Empty);

            RockersInput riffComp3 = Instantiate(rockerInputRef, transform);
            riffComp3.Init(false, new int[6], beat, 6, (PremadeSamples)SoshiSamples[2], SoshiPitches[2]);
            ScheduleInput(beat, 6.5f, InputType.STANDARD_DOWN, JustMute, MuteMiss, Empty);

            RockersInput riffComp4 = Instantiate(rockerInputRef, transform);
            riffComp4.Init(false, new int[6], beat, 7, (PremadeSamples)SoshiSamples[3], SoshiPitches[3], true);
            ScheduleInput(beat, 10, InputType.STANDARD_DOWN, JustMute, MuteMiss, Empty);
        }

        public void TogetherPrepare(double beat, bool cmon, bool muteSound, float muteBeat, float goToMiddleBeat, bool moveCamera)
        {
            List<RiqEntity> togetherEvents = GrabAllTogetherEvents(beat);
            if (togetherEvents.Count == 0 || crHandlerInstance.IntervalIsActive()) return;
            if (!muteSound) SoundByte.PlayOneShotGame(cmon ? "rockers/Cmon" : "rockers/LastOne");
            List<BeatAction.Action> actions = new List<BeatAction.Action>();
            if (moveCamera)
            {
                lastTargetCameraX = GameCamera.additionalPosition.x;
                targetCameraX = 0;
                cameraMoveBeat = beat + goToMiddleBeat;
            }
            BeatAction.New(instance.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + goToMiddleBeat, delegate
                {
                    if (JJ.together || Soshi.together) return;
                    JJ.PrepareTogether(goToMiddleBeat == muteBeat);
                    Soshi.PrepareTogether(goToMiddleBeat == muteBeat && GameManager.instance.autoplay);
                }),
                new BeatAction.Action(beat + muteBeat, delegate
                {
                    if (goToMiddleBeat == muteBeat) return;
                    if (JJ.together || Soshi.together) return;
                    Mute((int)WhoMutes.Both);
                }),
            });
            for (int i = 0; i < togetherEvents.Count; i++)
            {
                var e = togetherEvents[i];
                if (togetherEvents[i].datamodel == "rockers/riffTogether")
                {
                    actions.Add(new BeatAction.Action(e.beat, delegate 
                    {
                        JJ.StrumStrings(e["gcJJ"], new int[6]
                        {
                            e["1JJ"],
                            e["2JJ"],
                            e["3JJ"],
                            e["4JJ"],
                            e["5JJ"],
                            e["6JJ"],
                        }, (PremadeSamples)e["sampleJJ"], e["pitchSampleJJ"]); 
                    }));
                    actions.Add(new BeatAction.Action(e.beat + e.length, delegate { JJ.Mute(); }));
                    RockersInput riffComp = Instantiate(rockerInputRef, transform);
                    riffComp.Init(e["gcS"], new int[6] { e["1S"], e["2S"], e["3S"], e["4S"], e["5S"], e["6S"] }, beat, e.beat - beat,
                        (PremadeSamples)e["sampleS"], e["pitchSampleS"]);
                    ScheduleInput(beat, e.beat - beat + e.length, InputType.STANDARD_DOWN, JustMute, MuteMiss, Empty);
                }
                else
                {
                    actions.Add(new BeatAction.Action(e.beat, delegate
                    {
                        JJ.StrumStrings(e["gcJJ"], new int[6]
                        {
                            e["1JJ"],
                            e["2JJ"],
                            e["3JJ"],
                            e["4JJ"],
                            e["5JJ"],
                            e["6JJ"],
                        }, (PremadeSamples)e["sampleJJ"], e["pitchSampleJJ"], false, true);
                    }));
                    actions.Add(new BeatAction.Action(e.beat + e.length, delegate { JJ.Mute(); }));
                    RockersInput riffComp = Instantiate(rockerInputRef, transform);
                    riffComp.Init(e["gcS"], new int[6] { e["1S"], e["2S"], e["3S"], e["4S"], e["5S"], e["6S"] }, beat, e.beat - beat,
                        (PremadeSamples)e["sampleS"], e["pitchSampleS"], true);
                    ScheduleInput(beat, e.beat - beat + e.length, InputType.STANDARD_DOWN, JustMute, MuteMiss, Empty);
                    break;
                }
            }
            BeatAction.New(instance.gameObject, actions);
        }

        public static void PreInterval(double beat, float length, bool autoPassTurn, bool moveCamera, bool movePass)
        {
            if (GameManager.instance.currentGame == "rockers")
            {
                if (moveCamera) instance.MoveCamera(beat - 1);
                instance.StartInterval(beat, length, beat, autoPassTurn, movePass);
            }
            if (moveCamera) queuedCameraEvents.Add(beat - 1);
            queuedIntervals.Add(new QueuedInterval()
            {
                beat = beat,
                length = length,
                autoPassTurn = autoPassTurn,
                moveCamera = moveCamera,
                moveCameraPass = movePass
            });
        }

        private void MoveCamera(double beat)
        {
            lastTargetCameraX = GameCamera.additionalPosition.x;
            targetCameraX = JJ.transform.localPosition.x;
            cameraMoveBeat = beat;

        }

        public void StartInterval(double beat, float length, double gameSwitchBeat, bool autoPassTurn, bool moveCamera)
        {
            CallAndResponseHandler newHandler = new();
            crHandlerInstance = newHandler;
            crHandlerInstance.StartInterval(beat, length);
            List<RiqEntity> relevantInputs = GrabAllInputsBetween(beat, beat + length);
            List<double> riffUsedBeats = new List<double>();
            List<double> bendUsedBeats = new();
            foreach (var input in relevantInputs) 
            { 
                if (input.datamodel == "rockers/riff")
                {
                    RiqEntity foundEvent = riffEvents.Find(x => x.beat == input.beat);
                    if ((foundEvent == null || (riffUsedBeats.Count > 0 && riffUsedBeats.Contains((float)foundEvent.beat))) && riffEvents.Count > 1) continue;
                    riffUsedBeats.Add(input.beat);
                    if (input["respond"])
                    {
                        crHandlerInstance.AddEvent(input.beat, input.length, "riff", new List<CallAndResponseHandler.CallAndResponseEventParam>()
                        {
                            new CallAndResponseHandler.CallAndResponseEventParam("gleeClub", input["gcS"]),
                            new CallAndResponseHandler.CallAndResponseEventParam("1", input["1S"]),
                            new CallAndResponseHandler.CallAndResponseEventParam("2", input["2S"]),
                            new CallAndResponseHandler.CallAndResponseEventParam("3", input["3S"]),
                            new CallAndResponseHandler.CallAndResponseEventParam("4", input["4S"]),
                            new CallAndResponseHandler.CallAndResponseEventParam("5", input["5S"]),
                            new CallAndResponseHandler.CallAndResponseEventParam("6", input["6S"]),
                            new CallAndResponseHandler.CallAndResponseEventParam("sample", input["sampleS"]),
                            new CallAndResponseHandler.CallAndResponseEventParam("sampleTones", input["pitchSampleS"])
                        });
                    }
                    if (input.beat >= gameSwitchBeat)
                    {
                        BeatAction.New(instance.gameObject, new List<BeatAction.Action>()
                        {
                            new BeatAction.Action(input.beat, delegate { Riff(input.beat, input.length, new int[]
                            {
                                input["1JJ"],
                                input["2JJ"],
                                input["3JJ"],
                                input["4JJ"],
                                input["5JJ"],
                                input["6JJ"],
                            }, input["gcJJ"], input["sampleJJ"], input["pitchSampleJJ"], !input["respond"]);
                            })
                        });
                    }
                }
                else
                {
                    if (riffEvents.Count == 0) continue;
                    RiqEntity foundEvent = bendEvents.Find(x => x.beat == beat);
                    if ((foundEvent == null || (bendUsedBeats.Count > 0 && bendUsedBeats.Contains((float)foundEvent.beat))) && bendEvents.Count > 1) continue;
                    RiqEntity riffEventToCheck = riffEvents.Find(x => beat >= x.beat && beat < x.beat + x.length);
                    if (riffEventToCheck == null) continue;
                    bendUsedBeats.Add(beat);
                    if (input["respond"])
                    {
                        crHandlerInstance.AddEvent(input.beat, input.length, "bend", new List<CallAndResponseHandler.CallAndResponseEventParam>()
                        {
                            new CallAndResponseHandler.CallAndResponseEventParam("Pitch", input["1S"]),
                        });
                    }
                    if (input.beat >= gameSwitchBeat)
                    {
                        BeatAction.New(instance.gameObject, new List<BeatAction.Action>()
                        {
                            new BeatAction.Action(input.beat, delegate
                            {
                                Bend(input.beat, input.length, input["1JJ"]);
                            })
                        });
                    }
                }
            }
            BeatAction.New(instance.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate
                {
                    if (GameManager.instance.autoplay) Soshi.UnHold();
                    if (JJ.together || Soshi.together)
                    {
                        JJ.ReturnBack();
                        if (prepareBeatsJJ.Count > 0 && prepareBeatsJJ.Contains(beat)) JJ.Mute(false);
                        Soshi.ReturnBack();
                    }
                }),
            });
            if (autoPassTurn) PassTurn(beat + length, moveCamera, newHandler);
        }

        public void Riff(double beat, float length, int[] pitches, bool gleeClubJJ, int sampleJJ, int sampleTonesJJ, bool noRespond)
        {
            JJ.StrumStrings(gleeClubJJ, pitches, (PremadeSamples)sampleJJ, sampleTonesJJ, noRespond);
            BeatAction.New(instance.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + length, delegate { JJ.Mute(); })
            });
            /*
            if (noRespond) return;
            crHandlerInstance.AddEvent(beat, length, "riff", new List<CallAndResponseHandler.CallAndResponseEventParam>()
            {
                new CallAndResponseHandler.CallAndResponseEventParam("gleeClub", gleeClubPlayer),
                new CallAndResponseHandler.CallAndResponseEventParam("1", pitchesPlayer[0]),
                new CallAndResponseHandler.CallAndResponseEventParam("2", pitchesPlayer[1]),
                new CallAndResponseHandler.CallAndResponseEventParam("3", pitchesPlayer[2]),
                new CallAndResponseHandler.CallAndResponseEventParam("4", pitchesPlayer[3]),
                new CallAndResponseHandler.CallAndResponseEventParam("5", pitchesPlayer[4]),
                new CallAndResponseHandler.CallAndResponseEventParam("6", pitchesPlayer[5]),
                new CallAndResponseHandler.CallAndResponseEventParam("sample", sampleSoshi),
                new CallAndResponseHandler.CallAndResponseEventParam("sampleTones", sampleTonesSoshi)
            });
            */
        }

        public void Bend(double beat, float length, int pitchJJ)
        {
            JJ.BendUp(pitchJJ);
            BeatAction.New(instance.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + length, delegate { JJ.BendDown(); })
            });
        }

        public void Mute(int whoMutes)
        {
            if (whoMutes is (int)WhoMutes.JJ or (int)WhoMutes.Both)
            {
                JJ.Mute();
            }
            if (whoMutes is (int)WhoMutes.Soshi or (int)WhoMutes.Both)
            {
                if (GameManager.instance.autoplay) Soshi.Mute();
            }
        }

        public void UnMute(int whoMutes)
        {
            if (whoMutes is (int)WhoMutes.JJ or (int)WhoMutes.Both)
            {
                JJ.UnHold(true);
            }
            if (whoMutes is (int)WhoMutes.Soshi or (int)WhoMutes.Both)
            {
                if (GameManager.instance.autoplay) Soshi.UnHold(true);
            }
        }

        public static void PrePassTurn(double beat, bool moveCamera)
        {
            if (GameManager.instance.currentGame == "rockers")
            {
                instance.StandalonePassTurn(beat, moveCamera);
            }
            else
            {
                passedTurns.Add(new QueuedPassTurn
                {
                    beat = beat,
                    moveCamera = moveCamera
                });
            }
        }

        private void StandalonePassTurn(double beat, bool moveCamera)
        {
            if (crHandlerInstance != null) PassTurn(beat, moveCamera, crHandlerInstance);
        }

        private void PassTurn(double beat, bool moveCamera, CallAndResponseHandler handler)
        {
            if (crHandlerInstance.queuedEvents.Count > 0)
            {
                BeatAction.New(instance.gameObject, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat - 1, delegate
                    {
                        if (moveCamera)
                        {
                            lastTargetCameraX = GameCamera.additionalPosition.x;
                            targetCameraX = Soshi.transform.localPosition.x;
                            cameraMoveBeat = beat - 1;
                        }
                    }),
                    new BeatAction.Action(beat -0.25, delegate
                    {
                        List<CallAndResponseHandler.CallAndResponseEvent> crEvents = handler.queuedEvents;

                        foreach (var crEvent in crEvents)
                        {
                            if (crEvent.tag == "riff")
                            {
                                RockersInput riffComp = Instantiate(rockerInputRef, transform);
                                riffComp.Init(crEvent["gleeClub"], new int[6] { crEvent["1"], crEvent["2"], crEvent["3"], crEvent["4"], crEvent["5"], crEvent["6"] }, beat, crEvent.relativeBeat,
                                    (PremadeSamples)crEvent["sample"], crEvent["sampleTones"]);
                                ScheduleInput(beat, crEvent.relativeBeat + crEvent.length, InputType.STANDARD_DOWN, JustMute, MuteMiss, Empty);
                            }
                            else if (crEvent.tag == "bend")
                            {
                                RockerBendInput bendComp = Instantiate(rockerBendInputRef, transform);
                                bendComp.Init(crEvent["Pitch"], beat, crEvent.relativeBeat);
                                ScheduleInput(beat, crEvent.relativeBeat + crEvent.length, InputType.DIRECTION_UP, JustUnBend, UnBendMiss, Empty);
                            }
                        }
                        handler.queuedEvents.Clear();
                    }),
                    new BeatAction.Action(beat, delegate 
                    { 
                        JJ.UnHold();
                    })
                });
            }
        }

        private void JustMute(PlayerActionEvent caller, float state)
        {
            Soshi.Mute();
        } 

        private void MuteMiss(PlayerActionEvent caller)
        {
            JJ.Miss();
        }

        private void JustUnBend(PlayerActionEvent caller, float state)
        {
            Soshi.BendDown();
        }

        private void UnBendMiss(PlayerActionEvent caller)
        {
            JJ.Miss();
        }

        private void Empty(PlayerActionEvent caller)
        {

        }
    }
}
