// using System;
// using BehaviorDesigner.Runtime;
// using UnityEngine;
//
// namespace GoPlay
// {
//     [Serializable]
//     public class ExternalBehaviorTreeRefer : AssetRefer
//     {
// #if UNITY_EDITOR
//         private ExternalBehaviorTree _object;
// #endif
//         
//         public static implicit operator ExternalBehaviorTree(ExternalBehaviorTreeRefer data)
//         {
// #if UNITY_EDITOR
//             if (data._object) return data._object;
// #endif
//             
//             if (!data) return null;
//             
//             var gameObject = data.Load<ExternalBehaviorTree>();
// #if UNITY_EDITOR
//             data._object = gameObject;
// #endif
//             return gameObject;
//         }
//     }
// }