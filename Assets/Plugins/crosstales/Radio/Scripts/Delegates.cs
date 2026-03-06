using Crosstales.Radio.Model;

namespace Crosstales.Radio
{
   #region BasePlayer

   [System.Serializable]
   public class PlaybackStartEvent : UnityEngine.Events.UnityEvent<string, int>
   {
   }

   [System.Serializable]
   public class PlaybackEndEvent : UnityEngine.Events.UnityEvent<string, int>
   {
   }

   [System.Serializable]
   public class BufferingStartEvent : UnityEngine.Events.UnityEvent<string, int>
   {
   }

   [System.Serializable]
   public class BufferingEndEvent : UnityEngine.Events.UnityEvent<string, int>
   {
   }

   [System.Serializable]
   public class AudioStartEvent : UnityEngine.Events.UnityEvent<string, int>
   {
   }

   [System.Serializable]
   public class AudioEndEvent : UnityEngine.Events.UnityEvent<string, int>
   {
   }

   [System.Serializable]
   public class RecordChangeEvent : UnityEngine.Events.UnityEvent<string, int>
   {
   }

   [System.Serializable]
   public class ErrorEvent : UnityEngine.Events.UnityEvent<string, int, string>
   {
   }

   public delegate void PlaybackStart(RadioStation station);

   public delegate void PlaybackEnd(RadioStation station);

   public delegate void BufferingStart(RadioStation station);

   public delegate void BufferingEnd(RadioStation station);

   public delegate void BufferingProgressUpdate(RadioStation station, float progress);

   public delegate void AudioStart(RadioStation station);

   public delegate void AudioEnd(RadioStation station);

   public delegate void AudioPlayTimeUpdate(RadioStation station, float playtime);

   public delegate void RecordChange(RadioStation station, RecordInfo newRecord);

   public delegate void RecordPlayTimeUpdate(RadioStation station, RecordInfo record, float playtime);

   public delegate void NextRecordChange(RadioStation station, RecordInfo nextRecord, float delay);

   public delegate void NextRecordDelayUpdate(RadioStation station, RecordInfo nextRecord, float delay);

   public delegate void ErrorInfo(RadioStation station, string info);

   #endregion


   #region SimplePlayer

   [System.Serializable]
   public class StationChangeEvent : UnityEngine.Events.UnityEvent<string, int>
   {
   }

   public delegate void StationChange(RadioStation newStation);

   #endregion


   #region Set, RadioManager and SimplePlayer

   [System.Serializable]
   public class FilterChangeEvent : UnityEngine.Events.UnityEvent
   {
   }

   public delegate void FilterChange();

   #endregion


   #region Provider, Set, RadioManager and SimplePlayer

   [System.Serializable]
   public class StationsChangeEvent : UnityEngine.Events.UnityEvent
   {
   }

   [System.Serializable]
   public class ProviderReadyEvent : UnityEngine.Events.UnityEvent
   {
   }

   public delegate void StationsChange();

   public delegate void ProviderReady();

   #endregion
}
// © 2018-2024 crosstales LLC (https://www.crosstales.com)