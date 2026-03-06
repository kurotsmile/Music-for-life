using UnityEngine;

namespace Crosstales.Radio.Tool
{
   /// <summary>
   /// Saves the streams of a player as audio files in the WAV-format.
   /// NOTE: Copyright laws for music are VERY STRICT and MUST BE respected! If you save music, make sure YOU have the RIGHT to do so! crosstales LLC denies any responsibility for YOUR actions with this tool - use it at your OWN RISK!
   /// For more, see  https://en.wikipedia.org/wiki/Radio_music_ripping and the rights applying to your country.
   /// </summary>
   [RequireComponent(typeof(AudioSource))]
   [HelpURL("https://crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_tool_1_1_stream_saver.html")]
   public class StreamSaver : MonoBehaviour
   {
      #region Variables

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("Player")] [Tooltip("Origin Player."), SerializeField]
      private BasePlayer player;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("SilenceSource")] [Tooltip("Silence the origin (default: true)."), SerializeField]
      private bool silenceSource = true;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("OutputPath")] [Tooltip("Output path for the audio files."), SerializeField]
      private string outputPath;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("RecordStartDelay")] [Tooltip("Record delay in seconds before start saving the audio (default: 0)."), SerializeField, Range(0f, 20f)]
      private float recordStartDelay;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("RecordStopDelay")] [Tooltip("Record delay in seconds before stop saving the audio (default: 0)."), SerializeField, Range(0f, 20f)]
      private float recordStopDelay;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("AddStationName")] [Tooltip("Add the station name to the audio files (default: true)."), SerializeField]
      private bool addStationName = true;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("AddTimestamp")] [Tooltip("Add the current timestamp to the audio files (default: false)."), SerializeField]
      private bool addTimestamp;

      private System.IO.FileStream fileStream;

      private const int HEADER_SIZE = 44; //default for uncompressed wav
      private const float RESCALE_FACTOR = 32767f;

      private AudioSource audioSource;

      private bool recOutput;
      private bool stopped = true;

      private long dataPosition;

      private string fileName;

      #endregion


      #region Properties

      /// <summary>Origin Player.</summary>
      public BasePlayer Player
      {
         get => player;
         set
         {
            stopped = true;

            unregister();

            player = value;

            register();

            if (player != null)
               player.CaptureDataStream = true;
         }
      }

      /// <summary>Silence the origin.</summary>
      public bool SilenceSource
      {
         get => silenceSource;
         set => Player.isMuted = silenceSource = value;
      }

      /// <summary>Output path for the audio files.</summary>
      public string OutputPath
      {
         get => outputPath;
         set => outputPath = Crosstales.Common.Util.FileHelper.ValidatePath(value);
      }

      /// <summary>Record delay in seconds before start saving the audio (range 0-20).</summary>
      public float RecordStartDelay
      {
         get => recordStartDelay;
         set => recordStartDelay = Mathf.Clamp(value, 0, 20);
      }

      /// <summary>Record delay in seconds before stop saving the audio (range 0-20).</summary>
      public float RecordStopDelay
      {
         get => recordStopDelay;
         set => recordStopDelay = Mathf.Clamp(value, 0, 20);
      }

      /// <summary>Add the station name to the audio files.</summary>
      public bool AddStationName
      {
         get => addStationName;
         set => addStationName = value;
      }

      /// <summary>Add the current timestamp to the audio files.</summary>
      public bool AddTimestamp
      {
         get => addTimestamp;
         set => addTimestamp = value;
      }

      #endregion


      #region MonoBehaviour methods

#if !UNITY_WEBGL || UNITY_EDITOR
      private void Awake()
      {
         audioSource = GetComponent<AudioSource>();
         audioSource.playOnAwake = false;
         audioSource.Stop(); //always stop the AudioSource at startup
      }

      private void Start()
      {
         if (Player == null)
         {
            Debug.LogWarning("No 'Player' added to the StreamSaver!", this);
         }
         else
         {
            Player.CaptureDataStream = true;
            Player.LegacyMode = false;
         }

         if (string.IsNullOrEmpty(OutputPath))
            Debug.LogWarning("No 'OutputPath' added to the StreamSaver, saving in the project root!", this);
      }

      private void Update()
      {
         if (Player.isAudioPlaying)
         {
            if (stopped)
            {
               stopped = false;

               if (Player.DataStream != null)
                  dataPosition = Player.DataStream.Position;

               AudioClip myClip = AudioClip.Create(Player.Station.Name, int.MaxValue, Player.Station.Channels, Player.Station.SampleRate, true, readPCMData);

               audioSource.clip = myClip;

               audioSource.Play();

               Player.isMuted = SilenceSource;
            }
         }
         else
         {
            if (!stopped)
            {
               audioSource.Stop();
               audioSource.clip = null;
               stopped = true;
            }
         }
      }

      private void OnEnable()
      {
         register();
      }

      private void OnDisable()
      {
         unregister();

         closeFile();

         audioSource.Stop();
         audioSource.clip = null;
         stopped = true;
      }
#else
      private void Start()
      {
         Debug.LogWarning("'StreamSaver' is not supported for the current platform.", this);
      }
#endif

      private void OnValidate()
      {
         if (!string.IsNullOrEmpty(outputPath))
            outputPath = Crosstales.Common.Util.FileHelper.ValidatePath(outputPath);
      }

      #endregion


      #region Private methods

      private void register()

      {
         if (Player != null)
         {
            Player.OnAudioEnd += onAudioEnd;
            Player.OnNextRecordChange += onNextRecordChange;
         }
      }

      private void unregister()

      {
         if (Player != null)
         {
            Player.OnAudioEnd -= onAudioEnd;
            Player.OnNextRecordChange -= onNextRecordChange;
         }
      }

      private void openFile()
      {
         if (Util.Config.DEBUG)
            Debug.Log("openFile: " + fileName, this);

         if (fileStream?.CanWrite == true)
            closeFile();

         try
         {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));

            fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
            const byte emptyByte = 0;

            for (int ii = 0; ii < HEADER_SIZE; ii++) //preparing the header
            {
               fileStream.WriteByte(emptyByte);
            }

            recOutput = true;
         }
         catch (System.Exception ex)
         {
            Debug.LogError("Could not open file '" + fileName + "': " + ex, this);
         }
      }

      private void convertAndWrite(float[] dataSource)
      {
         if (fileStream?.CanWrite == true)
         {
            short[] intData = new short[dataSource.Length];
            byte[] bytesData = new byte[dataSource.Length * 2];

            for (int ii = 0; ii < dataSource.Length; ii++)
            {
               intData[ii] = (short)(dataSource[ii] * RESCALE_FACTOR);
               byte[] byteArr = System.BitConverter.GetBytes(intData[ii]);
               byteArr.CopyTo(bytesData, ii * 2);
            }

            try
            {
               fileStream.Write(bytesData, 0, bytesData.Length);
            }
            catch (System.Exception ex)
            {
               Debug.LogError("Could write to file '" + fileName + "': " + ex, this);
            }
         }
      }

      private void closeFile()
      {
         if (Util.Config.DEBUG)
            Debug.Log("closeFile", this);

         recOutput = false;

         if (fileStream?.CanWrite == true)
         {
            try
            {
               fileStream.Seek(0, System.IO.SeekOrigin.Begin);

               byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
               fileStream.Write(riff, 0, 4);

               byte[] chunkSize = System.BitConverter.GetBytes(fileStream.Length - 8);
               fileStream.Write(chunkSize, 0, 4);

               byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
               fileStream.Write(wave, 0, 4);

               byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
               fileStream.Write(fmt, 0, 4);

               byte[] subChunk1 = System.BitConverter.GetBytes(16);
               fileStream.Write(subChunk1, 0, 4);

               const ushort one = 1;

               byte[] audioFormat = System.BitConverter.GetBytes(one);
               fileStream.Write(audioFormat, 0, 2);

               byte[] numChannels = System.BitConverter.GetBytes(Player.Station.Channels);
               fileStream.Write(numChannels, 0, 2);

               byte[] sampleRate = System.BitConverter.GetBytes(Player.Station.SampleRate); //Hz?
               fileStream.Write(sampleRate, 0, 4);

               byte[] byteRate = System.BitConverter.GetBytes(Player.Station.SampleRate * Player.Station.Channels * 2);

               fileStream.Write(byteRate, 0, 4);

               ushort blockAlign = (ushort)(Player.Station.Channels * 2);
               fileStream.Write(System.BitConverter.GetBytes(blockAlign), 0, 2);

               const ushort bps = 16;
               byte[] bitsPerSample = System.BitConverter.GetBytes(bps);
               fileStream.Write(bitsPerSample, 0, 2);

               byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
               fileStream.Write(datastring, 0, 4);

               byte[] subChunk2 = System.BitConverter.GetBytes(fileStream.Length - HEADER_SIZE);
               fileStream.Write(subChunk2, 0, 4);
            }
            catch (System.Exception ex)
            {
               Debug.LogError("Could not write header for file '" + fileName + "': " + ex, this);
            }
#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
            finally
            {
               try
               {
                  fileStream.Close();
               }
               catch (System.Exception ex)
               {
                  Debug.LogError("Could not close file '" + fileName + "': " + ex, this);
               }
            }
#endif
         }
      }

      private void readPCMData(float[] data)
      {
         if (data != null)
         {
            if (Player.isAudioPlaying && Player.DataStream != null)
            {
               byte[] buffer = new byte[data.Length * 2];

               int count;
               long tempPosition = Player.DataStream.Position;
               Player.DataStream.Position = dataPosition;

               if ((count = Player.DataStream.Read(buffer, 0, buffer.Length)) > 0)
               {
                  float[] converted = buffer.CTToFloatArray(count);

                  //System.Buffer.BlockCopy(floats, 0, data, 0, data.Length * 4);
                  System.Array.Copy(converted, 0, data, 0, converted.Length);

                  if (recOutput)
                     convertAndWrite(converted);

                  dataPosition += count;
               }

               Player.DataStream.Position = tempPosition;
            }
            else
            {
               //System.Buffer.BlockCopy(new float[data.Length], 0, data, 0, data.Length * 4);
               System.Array.Copy(new float[data.Length], 0, data, 0, data.Length);
            }
         }
      }

      #endregion


      #region Callback methods

      private void onAudioEnd(Model.RadioStation station)
      {
         if (Util.Config.DEBUG)
            Debug.Log("onAudioEnd", this);

         closeFile();
      }

      private void onNextRecordChange(Model.RadioStation station, Model.RecordInfo nextRecord, float delay)
      {
         if (Util.Config.DEBUG)
            Debug.Log("onNextRecordChange: " + delay, this);

         if (delay > 0f)
            Invoke(nameof(closeFile), delay - RecordStopDelay - 0.2f);

         string _name = string.IsNullOrEmpty(nextRecord?.Artist) || string.IsNullOrEmpty(nextRecord.Title) ? station?.Name + " - " + System.DateTime.Now.ToString("yyyyMMdd HH_mm_ss") + ".wav" : (AddStationName ? station.Name + " - " : string.Empty) + (AddTimestamp ? System.DateTime.Now.ToString("yyyyMMdd HH_mm_ss") + " - " : string.Empty) + nextRecord.Artist + " - " + nextRecord.Title + ".wav";
         fileName = Crosstales.Common.Util.FileHelper.ValidateFile(OutputPath + _name);
         Invoke(nameof(openFile), delay + RecordStartDelay + 0.2f);
      }

      #endregion
   }
}
// © 2017-2021 crosstales LLC (https://www.crosstales.com)