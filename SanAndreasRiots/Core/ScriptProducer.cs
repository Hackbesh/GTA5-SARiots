using System;
using System.Collections.Generic;
using GTA.Extensions;
using UniRx;

namespace SanAndreasRiots
{
    /// <summary>
    /// スクリプトの生成と管理を行います。
    /// </summary>
    public abstract class ScriptProducer : ScriptEx
    {
        private readonly LinkedList<Script> scripts = new LinkedList<Script>();

        /// <summary>
        /// 実行中のスクリプトの数を取得します。
        /// </summary>
        protected int Count => scripts.Count;

        public ScriptProducer()
        {
            PreUpdateAsObservable.Subscribe(_ => InvokeUpdate(s => s.PreUpdate())).AddTo(this);
            UpdateAsObservable.Subscribe(_ => InvokeUpdate(s => s.Update())).AddTo(this);
        }

        private void InvokeUpdate(Action<Script> action)
        {
            var removeList = new LinkedList<LinkedListNode<Script>>();

            for (var node = scripts.First; node != null; node = node.Next)
            {
                var script = node.Value;

                //  有効状態なら更新処理を呼ぶ
                if (script.IsActive)
                {
                    action(script);
                }

                //  終了したら
                if (!script.IsActive)
                {
                    removeList.AddLast(node);
                }
            }

            //  削除リストのものを削除する
            foreach (var node in removeList)
            {
                Remove(node);
            }
        }

        /// <summary>
        /// スクリプトを追加します。
        /// </summary>
        /// <param name="script">追加するスクリプト。</param>
        /// <returns>リストのノード。</returns>
        protected LinkedListNode<Script> Add(Script script)
        {
            return scripts.AddLast(script);
        }

        /// <summary>
        /// スクリプトを削除します。
        /// </summary>
        /// <param name="scriptNode">削除するスクリプトのノード。</param>
        protected void Remove(LinkedListNode<Script> scriptNode)
        {
            scripts.Remove(scriptNode);
            scriptNode.Value.Dispose();
        }

        /// <summary>
        /// スクリプトを全て削除します。
        /// </summary>
        protected void Clear()
        {
            foreach (var script in scripts)
            {
                script.Dispose();
            }
            scripts.Clear();
        }

        /// <summary>
        /// スクリプトの終了処理を呼び出します。
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Clear();

            base.Dispose(disposing);
        }
    }
}