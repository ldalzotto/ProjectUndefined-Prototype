using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using CoreGame;
using UnityEngine;

namespace Persistence
{
    public class PersistanceManager : GameSingleton<PersistanceManager>
    {
        private BinaryFormatter binaryFormatter = new BinaryFormatter();

        private PersistanceManagerThreadObject PersistanceManagerThreadObject;

        private AutoSaveIcon AutoSaveIcon;

        // End of processing async event
        private bool NoMorePersistanceEvent;

        public virtual void Init()
        {
            this.AutoSaveIcon = AutoSaveIcon.Get();

            if (this.PersistanceManagerThreadObject == null)
            {
                this.PersistanceManagerThreadObject = new PersistanceManagerThreadObject(OnNoMorePersistanceProcessingCallback: this.OnNoMorePersistanceProcessing);
            }
        }

        public virtual void Tick(float d)
        {
            if (this.NoMorePersistanceEvent)
            {
                this.AutoSaveIcon.OnSaveEnd();
                this.NoMorePersistanceEvent = false;
            }

            if (this.PersistanceManagerThreadObject.IsInError)
            {
                Debug.LogError(this.PersistanceManagerThreadObject.ErrorOccured);
            }
        }

        #region External Event

        public virtual void OnPersistRequested(Action persistAction)
        {
            this.AutoSaveIcon.OnSaveStart();
            this.PersistanceManagerThreadObject.OnPersistRequested(persistAction);
        }

        public void OnNoMorePersistanceProcessing()
        {
            this.NoMorePersistanceEvent = true;
        }

        #endregion

        #region Loading

        public virtual T Load<T>(string folderPath, string dataPath, string filename, string fileExtension)
        {
            Debug.Log(MyLog.Format("Load PersistanceManager"));
            return LoadStatic<T>(folderPath, dataPath, filename, fileExtension, this.binaryFormatter);
        }

        public static T LoadStatic<T>(string folderPath, string dataPath, string filename, string fileExtension, BinaryFormatter binaryFormatter)
        {
            if (Directory.Exists(folderPath))
            {
                var directoryFiles = Directory.GetFiles(folderPath, filename + fileExtension);
                if (directoryFiles.Length > 0)
                {
                    using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
                    {
                        Debug.Log(MyLog.Format("Loaded : " + dataPath));
                        return (T) binaryFormatter.Deserialize(fileStream);
                    }
                }
            }

            return default(T);
        }

        #endregion
    }

    class PersistanceManagerThreadObject
    {
        private bool executingActions;
        private Queue<Action> persistQueueActions;

        private Action OnNoMorePersistanceProcessingCallback;

        private bool isInError;
        private Exception errorOccured;

        public bool IsInError
        {
            get => isInError;
        }

        public Exception ErrorOccured
        {
            get => errorOccured;
        }

        public PersistanceManagerThreadObject(Action OnNoMorePersistanceProcessingCallback)
        {
            this.persistQueueActions = new Queue<Action>();
            this.OnNoMorePersistanceProcessingCallback = OnNoMorePersistanceProcessingCallback;
        }

        public void OnPersistRequested(Action persistAction)
        {
            lock (this.persistQueueActions)
            {
                this.persistQueueActions.Enqueue(persistAction);
            }

            if (!this.executingActions)
            {
                this.executingActions = true;
                this.ProcessNextAction();
            }
        }

        private void ProcessNextAction()
        {
            Action nextAction;
            lock (this.persistQueueActions)
            {
                nextAction = this.persistQueueActions.Dequeue();
            }

            Task.Factory.StartNew(() => this.DoTask(nextAction));
        }

        private void DoTask(Action action)
        {
            action.Invoke();

            bool processNextAction = false;
            lock (this.persistQueueActions)
            {
                if (this.persistQueueActions.Count != 0)
                {
                    processNextAction = true;
                }
            }

            if (processNextAction)
            {
                this.ProcessNextAction();
            }
            else
            {
                try
                {
                    this.OnNoMorePersistanceProcessingCallback.Invoke();
                }
                catch (Exception e)
                {
                    this.isInError = true;
                    this.errorOccured = e;
                }

                this.executingActions = false;
            }
        }
    }
}