using System;
using Puerts;
using Puerts.TSLoader;
using UnityEngine;

namespace Tests.TestPuerTs
{
    public class HelloPuerTs : MonoBehaviour
    {
        private JsEnv _jsEnv;

        private void Awake()
        {
            _jsEnv = new JsEnv(new TSLoader(), 8888);
            _jsEnv.ExecuteModule("helloworld");
        }
    }
}