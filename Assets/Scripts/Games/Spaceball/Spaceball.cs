using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.Spaceball
{
    public class Spaceball : Minigame
    {
        public GameObject Ball;
        public GameObject BallsHolder;

        public GameObject Dispenser;
        public GameObject Dust;

        private float lastCamDistance;
        private float currentZoomCamBeat;
        private float currentZoomCamLength;
        private float currentZoomCamDistance;

        private int currentZoomIndex;

        public Sprite[] Balls;

        private List<Beatmap.Entity> allCameraEvents = new List<Beatmap.Entity>();

        public static Spaceball instance { get; set; }

        public override void OnGameSwitch()
        {
            for (int i = 1; i < BallsHolder.transform.childCount; i++)
                Destroy(BallsHolder.transform.GetChild(i).gameObject);
            GameManager.instance.GameCamera.orthographic = false;
            SpaceballPlayer.instance.EligibleHits.RemoveRange(0, SpaceballPlayer.instance.EligibleHits.Count);
        }

        public override void OnTimeChange()
        {
            UpdateCameraZoom();
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            UpdateCameraZoom();
        }

        private void Update()
        {
            var allPlayerActions = EventCaller.GetAllPlayerEntities("spaceball");
            int currentPlayerEvent = GameManager.instance.currentPlayerEvent - EventCaller.GetAllPlayerEntitiesExceptBeforeBeat("spaceball", Conductor.instance.songPositionInBeats).Count;

            if (currentPlayerEvent < allPlayerActions.Count)
            {
                if (Conductor.instance.songPositionInBeats > allPlayerActions[currentPlayerEvent].beat - 1)
                {
                    Dispenser.GetComponent<Animator>().Play("DispenserPrepare", 0, 0);
                }
            }

            if (currentZoomIndex < allCameraEvents.Count && currentZoomIndex >= 0)
            {
                if (Conductor.instance.songPositionInBeats >= allCameraEvents[currentZoomIndex].beat)
                {
                    UpdateCameraZoom();
                    currentZoomIndex++;
                }
            }

            float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(currentZoomCamBeat, currentZoomCamLength);

            if (normalizedBeat > Minigame.EndTime())
            {
                lastCamDistance = GameManager.instance.GameCamera.transform.localPosition.z;
            }
            else
            {
                if (currentZoomCamLength <= 0)
                    GameManager.instance.GameCamera.transform.localPosition = new Vector3(0, 0, currentZoomCamDistance);
                else
                {
                    float newPosZ = Mathf.Lerp(lastCamDistance, currentZoomCamDistance, normalizedBeat);
                    GameManager.instance.GameCamera.transform.localPosition = new Vector3(0, 0, newPosZ);
                }
            }
        }

        private void UpdateCameraZoom()
        {
            allCameraEvents = EventCaller.GetAllInGameManagerList("spaceball", new string[] { "cameraZoom" });

            if (currentZoomIndex < allCameraEvents.Count && currentZoomIndex >= 0)
            {
                if (currentZoomIndex - 1 >= 0)
                    lastCamDistance = allCameraEvents[currentZoomIndex - 1].valA * -1;
                else
                    lastCamDistance = allCameraEvents[0].valA * -1;

                currentZoomCamBeat = allCameraEvents[currentZoomIndex].beat;
                currentZoomCamLength = allCameraEvents[currentZoomIndex].length;

                float dist = allCameraEvents[currentZoomIndex].valA * -1;

                print(dist);

                if (dist > 0)
                    currentZoomCamDistance = 0;
                else
                    currentZoomCamDistance = dist;
            }
        }

        public void Shoot(float beat, bool high, string type)
        {
            GameObject ball = Instantiate(Ball);
            ball.transform.parent = Ball.transform.parent;
            ball.SetActive(true);
            ball.GetComponent<SpaceballBall>().startBeat = beat;
            if (high)
            {
                ball.GetComponent<SpaceballBall>().high = true;
                Jukebox.PlayOneShotGame("spaceball/longShoot");
            }
            else
            {
                Jukebox.PlayOneShotGame("spaceball/shoot");
            }

            if (type == "riceball")
            {
                ball.GetComponent<SpaceballBall>().Sprite.sprite = Balls[1];
            }

            Dispenser.GetComponent<Animator>().Play("DispenserShoot", 0, 0);
        }
    }
}