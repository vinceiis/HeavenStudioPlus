using HeavenStudio.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; //don't ask

namespace HeavenStudio.Games
{
    // none yet
    using Scripts_FanClub;

    public class FanClub : Minigame
    {
        public enum IdolAnimations {
            Bop,
            PeaceVocal,
            Peace,
            Clap,
            Call,
            Response,
            Jump,
            Dab
        }

        // userdata here
        [Header("Animators")]

        [Header("Objects")]
        public GameObject Arisa;
        public ParticleSystem idolClapEffect;
        public GameObject spectator;
        public GameObject spectatorAnchor;

        // end userdata

        //arisa's animation controller
        private Animator idolAnimator;
        //spectators
        private List<GameObject> Spectators;
        public NtrIdolFan Player;

        //bop-type animations
        public GameEvent bop = new GameEvent();
        public GameEvent specBop = new GameEvent();
        public GameEvent handCrap = new GameEvent();
        public GameEvent response = new GameEvent();
        //game scene
        public static FanClub instance;

        private void Awake()
        {
            instance = this;
        }

        const int FAN_COUNT = 12;
        const float RADIUS = 1.3f;
        private void Start()
        {
            Spectators = new List<GameObject>();
            idolAnimator = Arisa.GetComponent<Animator>();

            // procedurally spawning the spectators
            // from middle of viewport:

            //  |========A========|
            //  f==f==f==0==p==f==f
            //  =f==f==0==0==f==f==

            //spawn 10, the 4th is our player (idx 3)
            Vector3 origin = spectatorAnchor.transform.localPosition;
            int sortOrigin = spectatorAnchor.GetComponent<SortingGroup>().sortingOrder;
            Vector3 spawnPos = new Vector3(origin.x, origin.y, origin.z);
            spawnPos.x -= RADIUS * 2 * 3;
            for (int i = 0; i < FAN_COUNT; i++)
            {
                GameObject mobj = Instantiate(spectator, spectatorAnchor.transform.parent);
                mobj.transform.localPosition = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z);
                mobj.GetComponent<SortingGroup>().sortingOrder = i + sortOrigin;
                if (i == 3)
                {
                    Player = mobj.GetComponent<NtrIdolFan>();
                    Player.player = true;
                }
                Spectators.Add(mobj);

                //prepare spawn point of next spectator
                spawnPos.x += RADIUS * 2;
                if (i == 2)
                    spawnPos.x += RADIUS * 2;
                if (i == 8)
                    spawnPos.x += RADIUS * 4;
                if (i == 5)
                {
                    spawnPos = new Vector3(origin.x, origin.y, origin.z);
                    spawnPos.x -= RADIUS * 2 * 4 - RADIUS;
                    spawnPos.y -= RADIUS;
                    // spawnPos.z -= RADIUS/4;
                }
            }
        }

        private void Update()
        {
            if (Conductor.instance.ReportBeat(ref bop.lastReportedBeat, bop.startBeat % 1))
            {
                if (Conductor.instance.songPositionInBeats >= specBop.startBeat && Conductor.instance.songPositionInBeats < specBop.startBeat + specBop.length)
                    BopAll();
                
                if (Conductor.instance.songPositionInBeats >= response.startBeat && Conductor.instance.songPositionInBeats < response.startBeat + response.length)
                {
                    idolAnimator.Play("IdolResponse", 0, 0);
                }
                else if (Conductor.instance.songPositionInBeats >= handCrap.startBeat && Conductor.instance.songPositionInBeats < handCrap.startBeat + handCrap.length)
                {
                    idolAnimator.Play("IdolCrap", 0, 0);
                    idolClapEffect.Play();
                }
                else if (Conductor.instance.songPositionInBeats >= bop.startBeat && Conductor.instance.songPositionInBeats < bop.startBeat + bop.length)
                {
                    idolAnimator.Play("IdolBeat", 0, 0);
                }
            }
        }

        public void Bop(float beat, float length)
        {
            bop.length = length;
            bop.startBeat = beat;
            SpecBop(beat, length);
        }

        public void SpecBop(float beat, float length)
        {
            specBop.length = length;
            specBop.startBeat = beat;
        }

        public void PlayAnim(float beat, int type)
        {
            switch (type)
            {
                case (int) IdolAnimations.Bop:
                    idolAnimator.Play("IdolBeat", 0, 0);
                    break;
                case (int) IdolAnimations.PeaceVocal:
                    idolAnimator.Play("IdolPeace", 0, 0);
                    break;
                case (int) IdolAnimations.Peace:
                    idolAnimator.Play("IdolPeaceNoSync", 0, 0);
                    break;
                case (int) IdolAnimations.Clap:
                    idolAnimator.Play("IdolCrap", 0, 0);
                    idolClapEffect.Play();
                    break;
                case (int) IdolAnimations.Call:
                    BeatAction.New(Arisa, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beat,         delegate { Arisa.GetComponent<Animator>().Play("IdolCall0", 0, 0); }),
                        new BeatAction.Action(beat + 0.75f, delegate { Arisa.GetComponent<Animator>().Play("IdolCall1", 0, 0); }),
                    });
                    break;
                case (int) IdolAnimations.Response:
                    idolAnimator.Play("IdolResponse", 0, 0);
                    break;
                case (int) IdolAnimations.Jump:
                    DoIdolJump(beat);
                    break;
                case (int) IdolAnimations.Dab:
                    idolAnimator.Play("IdolDab", 0, 0);
                    Jukebox.PlayOneShotGame("fanClub/arisa_dab");
                    break;
            }
        }

        private void DoIdolJump(float beat)
        {

        }

        const float HAIS_LENGTH = 4f;
        public void CallHai(float beat, int type = 0)
        {
            bool shouldSpecBop = false;
            // if spectators need to bop
            if (specBop.startBeat + specBop.length > beat && specBop.startBeat < beat)
            {
                //let bopping continue *after* this cue
                if (specBop.startBeat + specBop.length > beat + HAIS_LENGTH)
                {
                    shouldSpecBop = true;
                }
            }

            MultiSound.Play(new MultiSound.Sound[] { 
                new MultiSound.Sound("fanClub/arisa_hai_1_jp", beat), 
                new MultiSound.Sound("fanClub/arisa_hai_2_jp", beat + 1f),
                new MultiSound.Sound("fanClub/arisa_hai_3_jp", beat + 2f),
            });

            Prepare(beat + 3f); 
            Prepare(beat + 4f); 
            Prepare(beat + 5f); 
            Prepare(beat + 6f); 

            BeatAction.New(Arisa, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat,         delegate { Arisa.GetComponent<Animator>().Play("IdolPeace", 0, 0); if (shouldSpecBop) {BopAll();}}),
                new BeatAction.Action(beat + 1f,    delegate { Arisa.GetComponent<Animator>().Play("IdolPeace", 0, 0); if (shouldSpecBop) {BopAll();}}),
                new BeatAction.Action(beat + 2f,    delegate { Arisa.GetComponent<Animator>().Play("IdolPeace", 0, 0); if (shouldSpecBop) {BopAll();}}),
                new BeatAction.Action(beat + 3f,    delegate { Arisa.GetComponent<Animator>().Play("IdolPeaceNoSync", 0, 0); PlayAnimationAll("FanPrepare"); }),

                new BeatAction.Action(beat + 4f,    delegate { PlayOneClap(beat + 4f);}),
                new BeatAction.Action(beat + 5f,    delegate { PlayOneClap(beat + 5f);}),
                new BeatAction.Action(beat + 6f,    delegate { PlayOneClap(beat + 6f);}),
                new BeatAction.Action(beat + 7f,    delegate { PlayOneClap(beat + 7f);}),
            });

            MultiSound.Play(new MultiSound.Sound[] { 
                new MultiSound.Sound("fanClub/crowd_hai_jp", beat + 4f), 
                new MultiSound.Sound("fanClub/crowd_hai_jp", beat + 5f),
                new MultiSound.Sound("fanClub/crowd_hai_jp", beat + 6f),
                new MultiSound.Sound("fanClub/crowd_hai_jp", beat + 7f),
            });

            handCrap.length = 4f;
            handCrap.startBeat = beat + 4f;
        }

        public static void WarnHai(float beat, int type = 0)
        {
            MultiSound.Play(new MultiSound.Sound[] { 
                new MultiSound.Sound("fanClub/arisa_hai_1_jp", beat), 
                new MultiSound.Sound("fanClub/arisa_hai_2_jp", beat + 1f),
                new MultiSound.Sound("fanClub/arisa_hai_3_jp", beat + 2f),
            }, forcePlay:true);
        }

        const float CALL_LENGTH = 2f;
        public void CallKamone(float beat, int type = 0, bool doJump = false)
        {
            bool shouldSpecBop = false;
            // clip certain events to the start of this cue if needed
            if (bop.startBeat + bop.length > beat && bop.startBeat < beat)
            {
                //let bopping continue *after* this cue
                if (bop.startBeat + bop.length > beat + CALL_LENGTH)
                {
                    bop.length -= (beat + CALL_LENGTH - bop.startBeat);
                    bop.startBeat = beat + CALL_LENGTH;
                }
                else
                    bop.length = beat - bop.startBeat;
            }
            // interrupt clapping completely, no need to continue
            if (handCrap.startBeat + handCrap.length > beat && handCrap.startBeat < beat)
                handCrap.length = beat - handCrap.startBeat;
            // same with responses
            if (response.startBeat + response.length > beat && response.startBeat < beat)
                response.length = beat - response.startBeat;
            
            // if spectators need to bop
            if (specBop.startBeat + specBop.length > beat && specBop.startBeat < beat)
            {
                //let bopping continue *after* this cue
                if (specBop.startBeat + specBop.length > beat + CALL_LENGTH)
                {
                    shouldSpecBop = true;
                }
            }

            MultiSound.Play(new MultiSound.Sound[] { 
                new MultiSound.Sound("fanClub/arisa_ka_jp", beat), 
                new MultiSound.Sound("fanClub/arisa_mo_jp", beat + 0.5f),
                new MultiSound.Sound("fanClub/arisa_ne_jp", beat + 1f),
            });

            BeatAction.New(Arisa, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat,         delegate { Arisa.GetComponent<Animator>().Play("IdolCall0", 0, 0); if (shouldSpecBop) {BopAll();}}),
                new BeatAction.Action(beat + 0.75f, delegate { Arisa.GetComponent<Animator>().Play("IdolCall1", 0, 0); }),
                new BeatAction.Action(beat + 1f,    delegate { PlayAnimationAll("FanPrepare"); Prepare(beat + 1f);}),

                new BeatAction.Action(beat + 2f,    delegate { PlayLongClap(beat + 2f); Prepare(beat + 2.5f);}),
                new BeatAction.Action(beat + 3.5f,  delegate { PlayOneClap(beat + 3.5f); Prepare(beat + 3f, 2);}),
                new BeatAction.Action(beat + 4f,    delegate { PlayChargeClap(beat + 4f); Prepare(beat + 4f, 1);}),
                new BeatAction.Action(beat + 5f,    delegate { PlayJump(beat + 5f);
                    if (doJump)
                    {
                        DoIdolJump(beat + 5f);
                    }
                }),
            });

            MultiSound.Play(new MultiSound.Sound[] { 
                new MultiSound.Sound("fanClub/crowd_ka_jp", beat + 2f), 
                new MultiSound.Sound("fanClub/crowd_mo_jp", beat + 3.5f),
                new MultiSound.Sound("fanClub/crowd_ne_jp", beat + 4f),
                new MultiSound.Sound("fanClub/crowd_hey_jp", beat + 5f),
            });

            response.length = doJump ? 3f : 4f;
            response.startBeat = beat + CALL_LENGTH;
        }

        public static void WarnKamone(float beat, int type = 0)
        {
            MultiSound.Play(new MultiSound.Sound[] { 
                new MultiSound.Sound("fanClub/arisa_ka_jp", beat), 
                new MultiSound.Sound("fanClub/arisa_mo_jp", beat + 0.5f),
                new MultiSound.Sound("fanClub/arisa_ne_jp", beat + 1f),
            }, forcePlay:true);
        }

        const float BIGCALL_LENGTH = 2.5f;
        public void CallBigReady(float beat)
        {
            // clip certain events to the start of this cue if needed
            if (specBop.startBeat + specBop.length > beat && specBop.startBeat < beat)
            {
                //let bopping continue *after* this cue
                if (specBop.startBeat + specBop.length > beat + BIGCALL_LENGTH)
                {
                    specBop.length -= (beat + BIGCALL_LENGTH - specBop.startBeat);
                    specBop.startBeat = beat + BIGCALL_LENGTH;
                }
                else
                    specBop.length = beat - specBop.startBeat;
            }

            Jukebox.PlayOneShotGame("fanClub/crowd_big_ready");

            BeatAction.New(this.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat,         delegate { PlayAnimationAll("FanBigReady"); Prepare(beat + 1.5f);}),
                new BeatAction.Action(beat + 2f,    delegate { PlayAnimationAll("FanBigReady"); Prepare(beat + 2f);}),
                new BeatAction.Action(beat + 2.5f,  delegate { PlayOneClap(beat + 2.5f);}),
                new BeatAction.Action(beat + 3f,    delegate { PlayOneClap(beat + 3f);}),
            });
        }

        public static void WarnBigReady(float beat)
        {
            Jukebox.PlayOneShotGame("fanClub/crowd_big_ready");
        }

        public void Prepare(float beat, int type = 0)
        {
            Player.AddHit(beat, type);
        }

        private void PlayAnimationAll(string anim, bool noPlayer = false, bool doForced = false)
        {
            for (int i = 0; i < Spectators.Count; i++)
            {
                if (i == 3 && noPlayer)
                    continue;
                
                if (!Spectators[i].GetComponent<Animator>().IsAnimationNotPlaying() && !doForced)
                    continue;
                Spectators[i].GetComponent<Animator>().Play(anim);
            }
        }

        private void BopAll(bool noPlayer = false, bool doForced = false)
        {
            for (int i = 0; i < Spectators.Count; i++)
            {
                if (i == 3 && noPlayer)
                    continue;
                
                if (!Spectators[i].GetComponent<Animator>().IsAnimationNotPlaying() && !doForced)
                    continue;
                Spectators[i].GetComponent<NtrIdolFan>().Bop();
            }
        }

        private void PlayOneClap(float beat)
        {
            BeatAction.New(this.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { PlayAnimationAll("FanClap", true, true);}),
                new BeatAction.Action(beat + 0.25f, delegate { PlayAnimationAll("FanFree", true);}),
            });
        }

        private void PlayLongClap(float beat)
        {
            BeatAction.New(this.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { PlayAnimationAll("FanClap", true, true);}),
                new BeatAction.Action(beat + 1f, delegate { PlayAnimationAll("FanFree", true);}),
            });
        }

        private void PlayChargeClap(float beat)
        {
            BeatAction.New(this.gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { PlayAnimationAll("FanClap", true, true);}),
                new BeatAction.Action(beat + 0.1f, delegate { PlayAnimationAll("FanClapCharge", true, true);}),
            });
        }

        private void StartJump(int idx, float beat)
        {
            Spectators[idx].GetComponent<NtrIdolFan>().jumpStartTime = beat;
            BeatAction.New(Spectators[idx], new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { Spectators[idx].GetComponent<Animator>().Play("FanJump", -1, 0);}),
                new BeatAction.Action(beat + 1f, delegate { Spectators[idx].GetComponent<Animator>().Play("FanPrepare", -1, 0);}),
            });
        }

        private void PlayJump(float beat)
        {
            for (int i = 0; i < Spectators.Count; i++)
            {
                if (i == 3)
                    continue;
                
                StartJump(i, beat);
            }
        }

        public void AngerOnMiss()
        {
            for (int i = 0; i <= 5; i++)
            {
                if (i == 3)
                    continue;
                Spectators[i].GetComponent<NtrIdolFan>().MakeAngry(i > 3);
            }
        }
    }
}