using UnityEngine;
using Crosstales.Radio.Model;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio
{
   /// <summary>Interface for all players.</summary>
   public interface IPlayer
   {
      #region Properties

      /// <summary>Current RadioStation of this player.</summary>
      RadioStation Station { get; set; }

      /// <summary>Starts and stops the RadioPlayer depending on the focus and running state.</summary>
      bool HandleFocus { get; set; }

      /// <summary>Size of the cache stream in bytes.</summary>
      int CacheStreamSize { get; set; }

      /// <summary>Enable or disable legacy mode. Legacy mode disables all record information, but is more stable.</summary>
      bool LegacyMode { get; set; }

      /// <summary>Capture the encoded PCM-stream from this player.</summary>
      bool CaptureDataStream { get; set; }

      /// <summary>Enable or disable skipping of the pre-buffering. Skip pre-buffering allows for faster playback, but is less resilient.</summary>
      bool SkipPreBuffering { get; set; }

      /// <summary>Returns the AudioSource of for this player.</summary>
      /// <returns>The AudioSource for this player.</returns>
      AudioSource Source { get; }

      /// <summary>Returns the codec of for this player.</summary>
      /// <returns>The codec for this player.</returns>
      AudioCodec Codec { get; }

      /// <summary>Returns the current playtime of this player.</summary>
      /// <returns>The current playtime of this player.</returns>
      float PlayTime { get; }

      /// <summary>Returns the current buffer progress in percent.</summary>
      /// <returns>The current buffer progress in percent.</returns>
      float BufferProgress { get; }

      /// <summary>Is this player buffering?</summary>
      /// <returns>True if this player is buffering.</returns>
      bool isBuffering { get; }

      /// <summary>Returns the size of the current buffer in bytes.</summary>
      /// <returns>Size of the current buffer in bytes.</returns>
      long CurrentBufferSize { get; }

      /// <summary>Is this player in playback-mode?</summary>
      /// <returns>True if this player is in playback-mode.</returns>
      bool isPlayback { get; }

      /// <summary>Is this player playing audio?</summary>
      /// <returns>True if this player is playing audio.</returns>
      bool isAudioPlaying { get; }

      /// <summary>Returns the playtime of the current audio record.</summary>
      /// <returns>Playtime of the current audio record.</returns>
      float RecordPlayTime { get; }

      /// <summary>Returns the information about the current audio record.</summary>
      /// <returns>Information about the current audio record.</returns>
      RecordInfo RecordInfo { get; }

      /// <summary>Returns the information about the next audio record. This information is updated a few seconds before a new record starts.</summary>
      /// <returns>Information about the next audio record.</returns>
      RecordInfo NextRecordInfo { get; }

      /// <summary>Returns the current delay in seconds until the next audio record starts.</summary>
      /// <returns>Current delay in seconds until the next audio record starts.</returns>
      float NextRecordDelay { get; }

      /// <summary>Returns the current download speed in Bytes per second.</summary>
      /// <returns>Current download speed in Bytes per second.</returns>
      long CurrentDownloadSpeed { get; }

      /// <summary>Returns the encoded PCM-stream from this player.</summary>
      /// <returns>Encoded PCM-stream from this player.</returns>
      Crosstales.Common.Util.MemoryCacheStream DataStream { get; }

      /// <summary>Current audio channels of the current station.</summary>
      int Channels { get; }

      /// <summary>Current audio sample rate of the current station.</summary>
      int SampleRate { get; }

      /// <summary>Current volume of this player.</summary>
      float Volume { get; set; }

      /// <summary>Current pitch of this player.</summary>
      float Pitch { get; set; }

      /// <summary>Current stereo pan of this player.</summary>
      float StereoPan { get; set; }

      /// <summary>Is this player muted?</summary>
      bool isMuted { get; set; }

      #endregion


      #region Methods

      /// <summary>Plays the radio-station.</summary>
      void Play();

      /// <summary>Plays or stops the radio-station.</summary>
      void PlayOrStop();

      /// <summary>Stops the playback of the radio-station.</summary>
      void Stop();

/*
      /// <summary>Silences the AudioSource on the Player-component.</summary>
      void Silence();
*/
      /// <summary>Restarts the playback of the radio-station.</summary>
      /// <param name="invokeDelay">Delay for the restart (default: 0.4, optional)</param>
      void Restart(float invokeDelay = Crosstales.Radio.Util.Constants.INVOKE_DELAY);

      /// <summary>Mute or unmute the playback of the record.</summary>
      void MuteOrUnMute();

      /// <summary>Mute the playback of the record.</summary>
      void Mute();

      /// <summary>Unmute the playback of the record.</summary>
      void UnMute();

      #endregion
   }
}
// © 2018-2024 crosstales LLC (https://www.crosstales.com)