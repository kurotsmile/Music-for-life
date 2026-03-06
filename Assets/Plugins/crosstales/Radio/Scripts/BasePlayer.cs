using UnityEngine;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio
{
   /// <summary>Base class for all players.</summary>
   public abstract class BasePlayer : MonoBehaviour, IPlayer
   {
      #region Variables

      private static int playCount;
      private static int audioCount;

      #endregion


      #region Properties

      /// <summary>Checks if ANY RadioPlayer is in playback-mode on this system.</summary>
      /// <returns>True if RadioPlayer is in playback-mode on this system.</returns>
      public static bool isAnyPlayback => playCounter > 0;

      /// <summary>Checks if ANY RadioPlayer playing audio on this system.</summary>
      /// <returns>True if RadioPlayer playing audio on this system.</returns>
      public static bool isAnyAudioPlaying => audioCounter > 0;

      protected static int playCounter
      {
         get => playCount;

         set => playCount = value < 0 ? 0 : value;
      }

      protected static int audioCounter
      {
         get => audioCount;

         set => audioCount = value < 0 ? 0 : value;
      }

      protected abstract PlaybackStartEvent onPlaybackStarted { get; }

      protected abstract PlaybackEndEvent onPlaybackEnded { get; }

      protected abstract BufferingStartEvent onBufferingStarted { get; }

      protected abstract BufferingEndEvent onBufferingEnded { get; }

      protected abstract AudioStartEvent onAudioStarted { get; }

      protected abstract AudioEndEvent onAudioEnded { get; }

      protected abstract RecordChangeEvent onRecordChanged { get; }

      protected abstract ErrorEvent onError { get; }

      #endregion


      #region Events

      /// <summary>An event triggered whenever the playback starts.</summary>
      public event PlaybackStart OnPlaybackStart;

      /// <summary>An event triggered whenever the playback ends.</summary>
      public event PlaybackEnd OnPlaybackEnd;

      /// <summary>An event triggered whenever the buffering starts.</summary>
      public event BufferingStart OnBufferingStart;

      /// <summary>An event triggered whenever the buffering ends.</summary>
      public event BufferingEnd OnBufferingEnd;

      /// <summary>An event triggered whenever the buffering progress changes.</summary>
      public event BufferingProgressUpdate OnBufferingProgressUpdate;

      /// <summary>An event triggered whenever the audio starts.</summary>
      public event AudioStart OnAudioStart;

      /// <summary>An event triggered whenever the audio ends.</summary>ry>
      public event AudioEnd OnAudioEnd;

      /// <summary>An event triggered whenever the audio playtime changes.</summary>
      public event AudioPlayTimeUpdate OnAudioPlayTimeUpdate;

      /// <summary>An event triggered whenever an audio record changes.</summary>
      public event RecordChange OnRecordChange;

      /// <summary>An event triggered whenever the audio record playtime changes.</summary>
      public event RecordPlayTimeUpdate OnRecordPlayTimeUpdate;

      /// <summary>An event triggered whenever the next record information is available.</summary>
      public event NextRecordChange OnNextRecordChange;

      /// <summary>An event triggered whenever the next record delay time changes.</summary>
      public event NextRecordDelayUpdate OnNextRecordDelayUpdate;

      /// <summary>An event triggered whenever an error occurs.</summary>
      public event ErrorInfo OnErrorInfo;

      #endregion


      #region Implemented methods

      public abstract RadioStation Station { get; set; }

      public abstract bool HandleFocus { get; set; }

      public abstract int CacheStreamSize { get; set; }

      public abstract bool LegacyMode { get; set; }

      public abstract bool CaptureDataStream { get; set; }

      public abstract bool SkipPreBuffering { get; set; }

      public abstract AudioSource Source { get; protected set; }

      public abstract AudioCodec Codec { get; protected set; }

      public abstract float PlayTime { get; protected set; }

      public abstract float BufferProgress { get; protected set; }

      public abstract bool isBuffering { get; }

      public abstract long CurrentBufferSize { get; }

      public abstract bool isPlayback { get; }

      public abstract bool isAudioPlaying { get; }

      public abstract float RecordPlayTime { get; protected set; }

      public abstract RecordInfo RecordInfo { get; }

      public abstract RecordInfo NextRecordInfo { get; }

      public abstract float NextRecordDelay { get; }

      public abstract long CurrentDownloadSpeed { get; }

      public abstract Crosstales.Common.Util.MemoryCacheStream DataStream { get; protected set; }

      public abstract int Channels { get; }

      public abstract int SampleRate { get; }

      public abstract float Volume { get; set; }

      public abstract float Pitch { get; set; }

      public abstract float StereoPan { get; set; }

      public abstract bool isMuted { get; set; }

      public abstract void Play();

      public abstract void Stop();

      //public abstract void Silence();

      public abstract void Restart(float invokeDelay = Constants.INVOKE_DELAY);

      public abstract void Mute();

      public abstract void UnMute();

      public virtual void PlayOrStop()
      {
         if (isPlayback)
         {
            Stop();
         }
         else
         {
            Play();
         }
      }

      public virtual void MuteOrUnMute()
      {
         if (isMuted)
         {
            UnMute();
         }
         else
         {
            Mute();
         }
      }

      #endregion

      protected virtual void onPlaybackStart(RadioStation station)
      {
         if (!Helper.isEditorMode)
            onPlaybackStarted?.Invoke(station.Name, station.GetHashCode());

         OnPlaybackStart?.Invoke(station);
      }

      protected virtual void onPlaybackEnd(RadioStation station)
      {
         if (!Helper.isEditorMode)
            onPlaybackEnded?.Invoke(station.Name, station.GetHashCode());

         OnPlaybackEnd?.Invoke(station);
      }

      protected virtual void onBufferingStart(RadioStation station)
      {
         if (!Helper.isEditorMode)
            onBufferingStarted?.Invoke(station.Name, station.GetHashCode());

         OnBufferingStart?.Invoke(station);
      }

      protected virtual void onBufferingEnd(RadioStation station)
      {
         if (!Helper.isEditorMode)
            onBufferingEnded?.Invoke(station.Name, station.GetHashCode());

         OnBufferingEnd?.Invoke(station);
      }

      protected virtual void onBufferingProgressUpdate(RadioStation station, float progress)
      {
         OnBufferingProgressUpdate?.Invoke(station, progress);
      }

      protected virtual void onAudioStart(RadioStation station)
      {
         if (!Helper.isEditorMode)
            onAudioStarted?.Invoke(station.Name, station.GetHashCode());

         OnAudioStart?.Invoke(station);
      }

      protected virtual void onAudioEnd(RadioStation station)
      {
         if (!Helper.isEditorMode)
            onAudioEnded?.Invoke(station.Name, station.GetHashCode());

         OnAudioEnd?.Invoke(station);
      }

      protected virtual void onAudioPlayTimeUpdate(RadioStation station, float _playtime)
      {
         OnAudioPlayTimeUpdate?.Invoke(station, _playtime);
      }

      protected virtual void onRecordChange(RadioStation station, RecordInfo newRecord)
      {
         if (!Helper.isEditorMode)
            onRecordChanged?.Invoke(station.Name, station.GetHashCode());

         OnRecordChange?.Invoke(station, newRecord);
      }

      protected virtual void onRecordPlayTimeUpdate(RadioStation station, RecordInfo record, float playtime)
      {
         OnRecordPlayTimeUpdate?.Invoke(station, record, playtime);
      }

      protected virtual void onNextRecordChange(RadioStation station, RecordInfo nextRecord, float delay)
      {
         OnNextRecordChange?.Invoke(station, nextRecord, delay);
      }

      protected virtual void onNextRecordDelayUpdate(RadioStation station, RecordInfo nextRecord, float delay)
      {
         OnNextRecordDelayUpdate?.Invoke(station, nextRecord, delay);
      }

      protected virtual void onErrorInfo(RadioStation station, string info)
      {
         if (!Helper.isEditorMode)
            onError?.Invoke(station.Name, station.GetHashCode(), info);

         OnErrorInfo?.Invoke(station, info);
      }
   }
}
// © 2017-2024 crosstales LLC (https://www.crosstales.com)