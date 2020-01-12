using System;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{
    public static class CoreGameSingletonInstances
    {
        private static Coroutiner coroutiner;
        private static Canvas persistantCanvas;
        private static CanvasScaler canvasScaler;
        public static Coroutiner Coroutiner => FindAndSetInstanceIfNull(coroutiner, obj => coroutiner = obj);

        public static Canvas GameCanvas
        {
            get
            {
                var gameCanvas = GameObject.FindGameObjectWithTag(TagConstants.GAME_CANVAS);
                if (gameCanvas == null)
                {
                    gameCanvas = GameObject.FindGameObjectWithTag(TagConstants.START_MENU_CANVAS);
                }

                return gameCanvas.GetComponent<Canvas>();
            }
        }

        public static Canvas PersistantCanvas()
        {
            if (persistantCanvas == null)
            {
                persistantCanvas = GameObject.FindGameObjectWithTag(TagConstants.PERSISTANT_CANVAS).GetComponent<Canvas>();
            }

            return persistantCanvas;
        }

        public static CanvasScaler CanvasScaler()
        {
            if (canvasScaler == null)
            {
                canvasScaler = GameCanvas.GetComponent<CanvasScaler>();
            }

            return canvasScaler;
        }

        public static T FindAndSetInstanceIfNull<T>(T obj, Action<T> setter) where T : Behaviour
        {
            if (obj == null)
            {
                var foundObj = GameObject.FindObjectOfType<T>();
                setter.Invoke(foundObj);
                return foundObj;
            }
            else
            {
                return obj;
            }
        }

        public static T NewInstanceIfNull<T>(T obj, Action<T> setter)
        {
            if (obj == null)
            {
                var createdObject = Activator.CreateInstance<T>();
                setter.Invoke(createdObject);
                return createdObject;
            }
            else
            {
                return obj;
            }
        }
    }
}