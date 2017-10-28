using System;
using GTA.Extensions;
using UniRx;

namespace SanAndreasRiots
{
    /// <summary>
    /// 各スクリプトの基底部分です。
    /// </summary>
    public abstract class Script : IDisposable
    {
        private readonly Subject<Unit> preupdateSubject = new Subject<Unit>();

        private readonly Subject<Unit> updateSubject = new Subject<Unit>();

        private readonly BooleanDisposable cancellation = new BooleanDisposable();

        /// <summary>
        /// スクリプトの実行をキャンセルします。
        /// </summary>
        public IDisposable Cancellation => cancellation;

        /// <summary>
        /// スクリプトが有効かどうかを表す値を取得します。
        /// </summary>
        public bool IsActive => !cancellation.IsDisposed;

        /// <summary>
        /// 更新処理の前に発火するストリームを取得します。
        /// </summary>
        public UniRx.IObservable<Unit> OnPreUpdate { get; }

        /// <summary>
        /// 更新処理時に発火するストリームを取得します。
        /// </summary>
        public UniRx.IObservable<Unit> OnUpdate { get; }

        /// <summary>
        /// スクリプト実行用スレッドで動作するスケジューラを取得します。
        /// </summary>
        public IScheduler Scheduler => ScriptDispatcher.Instance.Scheduler;

        /// <summary>
        /// コルーチンシステムの参照を取得します。
        /// </summary>
        public CoroutineManager Coroutine => ScriptDispatcher.Instance.Coroutine;

        /// <summary>
        /// CompositeDisposableを取得します。
        /// </summary>
        public CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public Script()
        {
            OnPreUpdate = preupdateSubject.AsObservable();
            OnUpdate = updateSubject.AsObservable();
        }

        /// <summary>
        /// セットアップを行います。
        /// </summary>
        public virtual void Setup() { }

        /// <summary>
        /// 事前更新処理を行います。
        /// </summary>
        public virtual void PreUpdate()
        {
            preupdateSubject.OnNext(Unit.Default);
        }

        /// <summary>
        /// 更新処理を行います。
        /// </summary>
        public virtual void Update()
        {
            updateSubject.OnNext(Unit.Default);
        }

        /// <summary>
        /// リソースの破棄を行います。
        /// </summary>
        public void Dispose()
        {
            cancellation.Dispose();
            Disposable.Dispose();

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// リソースの破棄を行います。
        /// </summary>
        /// <param name="disposing">親のDisposeメソッドから呼ばれた場合はtrueが、デストラクタから呼ばれた場合はfalseが与えられます。</param>
        protected virtual void Dispose(bool disposing) { }

        ~ Script()
        {
            Dispose(false);
        }
    }
}