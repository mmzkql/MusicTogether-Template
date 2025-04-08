namespace MusicTogether.TimelineKit
{
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    public class ClipTiming
    {
        public float Start { get; set; }
        public float End { get; set; }
        public float Duration { get; set; }

        public ClipTiming(float start, float end, float duration)
        {
            Start = start;
            End = end;
            Duration = duration;
        }
    }

    public abstract class BaseBehaviour : PlayableBehaviour
    {
        private bool _started;
        private bool _played;
        private object _playerData;
        private Playable _playable = Playable.Null;
        private PlayableAsset _asset;
        private PlayableDirector _director;
        private TrackAsset _track;

        public static Playable CreatePlayable<T>(PlayableGraph graph, PlayableAsset data) where T : BaseBehaviour, new()
        {
            var playable = ScriptPlayable<T>.Create(graph);
            T behaviour = playable.GetBehaviour();
            behaviour._asset = data;
            behaviour._playable = playable;
            behaviour._director = graph.GetResolver() as PlayableDirector;
            return playable;
        }

        #region protected  

        protected T GetData<T>() where T : PlayableAsset => _asset as T;
        protected float LocalTime => (float)_playable.GetTime();
        protected float GlobalTime => (float)_director.time;
        protected float Duration => (float)_playable.GetDuration();
        protected float Percent => (float)(_playable.GetDuration().Equals(0) ? 0 : _playable.GetTime() / _playable.GetDuration());

        #endregion protected

        #region base
        protected virtual void OnCreate() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnStart(object binding) { }
        protected virtual void OnUpdate(object binding, float deltaTime) { }
        protected virtual void OnStop(object binding) { }
        #endregion base

        #region sealed
        public sealed override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (_played)
                return;
            _started = false;
            _played = true;
        }

        public sealed override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!_played)
                return;
            OnStop(_playerData);
            _played = false;
        }

        public sealed override void OnPlayableCreate(Playable playable)
        {
            OnCreate();
        }

        public sealed override void OnPlayableDestroy(Playable playable)
        {
            OnDestroy();
        }

        public sealed override void OnGraphStart(Playable playable) { }

        public sealed override void OnGraphStop(Playable playable) { }

        public sealed override void PrepareData(Playable playable, FrameData info) { }

        public sealed override void PrepareFrame(Playable playable, FrameData info) { }

        public sealed override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!_played)
                return;
            if (!_started)
            {
                _started = true;
                _playerData = playerData;
                OnStart(_playerData);
            }
            OnUpdate(_playerData, info.deltaTime);
        }
        #endregion sealed

        #region New methods

        /// <summary>
        /// 获取当前clip在所在的track中的序号
        /// </summary>
        /// <returns>当前clip在所在的track中的序号</returns>
        protected int GetClipIndexInTrack()
        {
            if (_director == null || _asset == null)
                return -1;

            TimelineAsset timeline = _director.playableAsset as TimelineAsset;
            if (timeline == null)
                return -1;

            foreach (TrackAsset track in timeline.GetOutputTracks())
            {
                TimelineClip[] clips = (TimelineClip[])track.GetClips();
                for (int i = 0; i < clips.Length; i++)
                {
                    if (clips[i].asset == _asset)
                    {
                        _track = track;
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取当前clip的前一个clip
        /// </summary>
        /// <returns>当前clip的前一个clip，如果没有则返回null</returns>
        protected TimelineClip GetPreviousClip()
        {
            int index = GetClipIndexInTrack();
            if (index <= 0 || _track == null)
                return null;

            TimelineClip[] clips = (TimelineClip[])_track.GetClips();
            return clips[index - 1];
        }

        /// <summary>
        /// 获取当前clip的后一个clip
        /// </summary>
        /// <returns>当前clip的后一个clip，如果没有则返回null</returns>
        protected TimelineClip GetNextClip()
        {
            int index = GetClipIndexInTrack();
            if (index < 0 || _track == null)
                return null;

            TimelineClip[] clips = (TimelineClip[])_track.GetClips();
            if (index >= clips.Length - 1)
                return null;

            return clips[index + 1];
        }

        /// <summary>
        /// 获取clip的Timing参数（start, End, Duration）
        /// </summary>
        /// <returns>包含clip的Timing参数的ClipTiming对象</returns>
        protected ClipTiming GetClipTiming()
        {
            float start = 0f;
            float end = 0f;
            float duration = 0f;

            if (_director == null || _asset == null)
                return new ClipTiming(start, end, duration);

            TimelineAsset timeline = _director.playableAsset as TimelineAsset;
            if (timeline == null)
                return new ClipTiming(start, end, duration);

            foreach (TrackAsset track in timeline.GetOutputTracks())
            {
                TimelineClip[] clips = (TimelineClip[])track.GetClips();
                foreach (TimelineClip clip in clips)
                {
                    if (clip.asset == _asset)
                    {
                        start = (float)clip.start;
                        end = (float)(clip.start + clip.duration);
                        duration = (float)clip.duration;
                        return new ClipTiming(start, end, duration);
                    }
                }
            }
            return new ClipTiming(start, end, duration);
        }
        #endregion New methods
    }
}