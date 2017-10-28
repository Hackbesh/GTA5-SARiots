using System;
using GTA.Extensions;

namespace SanAndreasRiots
{
    public sealed class ScriptDispatcher : ScriptEx
    {
        public static ScriptDispatcher Instance { get; private set; }

        public ScriptDispatcher()
        {
            Instance = (Instance == null) ? this : throw new InvalidOperationException("コンストラクタを呼ぶな!");
        }
    }
}