using System.Collections.Generic;
using UnityEngine;

namespace Utils.SmartUpdate
{

    public enum UpdateGroup
    {
        Always,
        EvenFrames,
        OddFrames,
        Timed
    }

    public class SmartUpdateController : MonoBehaviour
    {
        #region VARIABLES

        public static SmartUpdateController Instance;

#if UNITY_ANDROID || UNITY_IOS
        [SerializeField] private int targetFrameRate = 60;
#endif

        [SerializeField, Tooltip("Only used for timed updates")] private float timedUpdateRate = .1f;

        private HashSet<ISmartUpdate> alwaysUpdate = new HashSet<ISmartUpdate>();
        private HashSet<ISmartUpdate> evenFramesUpdate = new HashSet<ISmartUpdate>();
        private HashSet<ISmartUpdate> oddFramesUpdate = new HashSet<ISmartUpdate>();
        private HashSet<ISmartUpdate> timedUpdate = new HashSet<ISmartUpdate>();

        private bool isEvenFrame = true;

        private float elapsedTime;

        private float elapsedTimSinceLastEven;
        private float elapsedTimeSinceLastOdd;

        #endregion

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }

#if UNITY_ANDROID || UNITY_IOS
            Application.targetFrameRate = targetFrameRate;
#endif

        }

        public void Register(ISmartUpdate updateable)
        {
            switch (updateable.Group)
            {
                case UpdateGroup.Always:
                    if (alwaysUpdate.Contains(updateable))
                    {
                        return;
                    }
                    alwaysUpdate.Add(updateable);
                    break;

                case UpdateGroup.EvenFrames:
                    if (evenFramesUpdate.Contains(updateable))
                    {
                        return;
                    }
                    evenFramesUpdate.Add(updateable);
                    break;

                case UpdateGroup.OddFrames:
                    if (oddFramesUpdate.Contains(updateable))
                    {
                        return;
                    }
                    oddFramesUpdate.Add(updateable);
                    break;

                case UpdateGroup.Timed:
                    if (timedUpdate.Contains(updateable))
                    {
                        return;
                    }
                    timedUpdate.Add(updateable);
                    break;
            }
        }

        public void Unregister(ISmartUpdate updateable)
        {
            switch (updateable.Group)
            {
                case UpdateGroup.Always:
                    if (alwaysUpdate.Contains(updateable))
                    {
                        alwaysUpdate.Remove(updateable);
                    }
                    break;

                case UpdateGroup.EvenFrames:
                    if (evenFramesUpdate.Contains(updateable))
                    {
                        evenFramesUpdate.Remove(updateable);
                    }
                    break;

                case UpdateGroup.OddFrames:
                    if (oddFramesUpdate.Contains(updateable))
                    {
                        oddFramesUpdate.Remove(updateable);
                    }
                    break;

                case UpdateGroup.Timed:
                    if (timedUpdate.Contains(updateable))
                    {
                        timedUpdate.Remove(updateable);
                    }
                    break;
            }
        }

        private void FixedUpdate()
        {
            elapsedTime += Time.deltaTime;
            elapsedTimSinceLastEven += Time.deltaTime;
            elapsedTimeSinceLastOdd += Time.deltaTime;

            foreach (var updateable in alwaysUpdate)
            {
                updateable.SmartUpdate(Time.deltaTime);
            }

            if (isEvenFrame)
            {
                foreach (var updateable in evenFramesUpdate)
                {
                    updateable.SmartUpdate(elapsedTimSinceLastEven);
                }

                elapsedTimSinceLastEven = 0f;
            }
            else
            {
                foreach (var updateable in oddFramesUpdate)
                {
                    updateable.SmartUpdate(elapsedTimeSinceLastOdd);
                }

                elapsedTimeSinceLastOdd = 0f;
            }

            isEvenFrame = !isEvenFrame;

            if (elapsedTime >= timedUpdateRate)
            {
                
                foreach (var updateable in timedUpdate)
                {
                    updateable.SmartUpdate(elapsedTime);
                }
                elapsedTime = 0f;
            }
        }
    }
}