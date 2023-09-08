using System;
using System.Collections;
using UnityEngine.Events;

namespace GoPlay.Async
{
    [Serializable] public class ExceptionEvent : UnityEvent<Exception> { }
    [Serializable] public class CustomAsyncOperationEvent : UnityEvent<CustomAsyncOperation> { }
    
    public abstract class CustomAsyncOperation
    {
        //
        // Summary:
        //     Has the operation finished? (Read Only)
        public bool IsDone
        {
            get { return Progress >= 1; }
        }

        //
        // Summary:
        //     What's the operation's progress. (Read Only)
        public abstract float Progress { get; }

        public ExceptionEvent OnError = new ExceptionEvent();
        public CustomAsyncOperationEvent OnCompleted = new CustomAsyncOperationEvent();

        public abstract IEnumerator Start();

        protected void OnErrorEvent(Exception e)
        {
            OnError.Invoke(e);
        }

        protected void OnCompletedEvent()
        {
            OnCompleted.Invoke(this);
        }
    }
}
