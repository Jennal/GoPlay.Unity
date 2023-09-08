// Copyright (c) 2018 Clark Yang

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoPlay
{
    /// <summary>
    /// 可劫持的协程迭代器
    /// 注入catch代码块：当协程中发生异常时回调
    /// 注入finally代码块：当协程结束时回调
    /// </summary>
    public class InterceptableEnumerator : IEnumerator
    {

        private object current;
        private Stack<IEnumerator> stack = new Stack<IEnumerator>();

        private Action<Exception> onException;
        private Action onFinally;
        private Func<bool> hasNext;

        public InterceptableEnumerator(IEnumerator routine, Action<Exception> eAction = null, Action fAction = null)
        {
            this.stack.Push(routine);

            if (eAction != null)
            {
                this.RegisterCatchBlock(eAction);
            }

            if (fAction != null)
            {
                this.RegisterFinallyBlock(fAction);
            }
        }

        public object Current { get { return this.current; } }

        public bool MoveNext()
        {
            try
            {
                if (!this.HasNext())
                {
                    this.OnFinally();
                    return false;
                }

                if (stack.Count <= 0)
                {
                    this.OnFinally();
                    return false;
                }

                IEnumerator ie = stack.Peek();
                bool hasNext = ie.MoveNext();
                if (!hasNext)
                {
                    this.stack.Pop();
                    return MoveNext();
                }

                this.current = ie.Current;
                if (this.current is IEnumerator)
                {
                    stack.Push(this.current as IEnumerator);
                    return MoveNext();
                }

#if UNITY_EDITOR
                if (this.current is Coroutine)
                    Debug.LogWarning("The Enumerator's results contains the 'UnityEngine.Coroutine' type,If occurs an exception,it can't be catched." +
                                     "It is recommended to use 'yield return routine',rather than 'yield return StartCoroutine(routine)'.");
#endif
                
                return true;
            }
            catch (Exception e)
            {
                this.OnException(e);
                this.OnFinally();
                return false;
            }
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        private void OnException(Exception e)
        {
            try
            {
#if UNITY_EDITOR
                Debug.LogError($"[Coroutine Exeption] {e}");
#endif

                if (this.onException == null)
                    return;

                foreach (Action<Exception> action in this.onException.GetInvocationList())
                {
                    try
                    {
                        action(e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError(ex);
#else
                        Debug.LogWarning(ex);
#endif
                    }
                }
            }
            catch (Exception) { }
        }

        private void OnFinally()
        {
            try
            {
                if (this.onFinally == null)
                    return;

                foreach (Action action in this.onFinally.GetInvocationList())
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError(ex);
#else
                        Debug.LogWarning(ex);
#endif
                    }
                }
            }
            catch (Exception) { }
        }

        private bool HasNext()
        {
            if (hasNext == null)
                return true;
            return hasNext();
        }

        /// <summary>
        /// Register a condition code block.
        /// </summary>
        /// <param name="hasNext"></param>
        public virtual void RegisterConditionBlock(Func<bool> hasNext)
        {
            this.hasNext = hasNext;
        }

        /// <summary>
        /// Register a code block, when an exception occurs it will be executed.
        /// </summary>
        /// <param name="onException"></param>
        public virtual void RegisterCatchBlock(Action<Exception> onException)
        {
            this.onException += onException;
        }

        /// <summary>
        /// Register a code block, when the end of the operation is executed.
        /// </summary>
        /// <param name="onFinally"></param>
        public virtual void RegisterFinallyBlock(Action onFinally)
        {
            this.onFinally += onFinally;
        }
    }
}