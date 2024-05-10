// Created by Carlos Arturo Rodriguez Silva (Legend) https://twitter.com/xLegendx97 or https://www.facebook.com/legendxh
// Thread: http://forum.unity3d.com/threads/rhythm-visualizator.423168/ 
// Video: https://www.youtube.com/watch?v=i5uRU45fi8U

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class RhythmVisualizator : MonoBehaviour {
	
	public GameObject soundBarPrefab;
	AudioSource audioSource;

	public AudioSource metronomeTick;

	[Space(5)]

	[HideInInspector]
	public GameObject[] soundBars;
	int numberOfBars;
	ParticleSystem rhythmParticleSystem;

	[Header("Camera Control")] [Tooltip("Deactivate to use your own camera")]
	public bool cameraControl = true;
	[Tooltip("Rotating around camera")]
	public bool rotateCamera;
	[Range(-35, 35)] [Tooltip("Camera rotating velocity, positive = right, negative = left")]
	public float velocity = 15f;
	[Range(0, 100)]
	public float distance = 40f;
	[Range(1, 179)]
	public int fieldOfView = 60;

	[Header("Form Control")]
	[Range(10, 50f)] [Tooltip("Form Length")]
	public float length = 25f;
	const int barsQuantity = 150; // 0, 2, 4, 6, 8, 10...
	public enum BarsForm {Line, Circle, ExpansibleCircle};
	[Tooltip("Bars Form")]
	public BarsForm form = BarsForm.Line;

	[Header("Central Particles Control")]
	public bool rhythmParticles = true;
	[Range(1, 300)] [Tooltip("Amount of particles to emit")]
	public int amountToEmit = 100;
	public enum RhythmScale {x25, x50, Normal, x150, x200}
	[Tooltip("Rhythm velocity scale")]
	public RhythmScale rhythmScale = RhythmScale.Normal;
	float escRhythm = 1f;
	[Range(0.0f, 10f)] [Tooltip("Rhythm minimum sensibility")]
	public float rhythmSensibility = 4f;
	[Tooltip("Beats Per Minute. Press the Update button after change")]
	public float BPM = 88f;
	[Tooltip("Rhythm Off-set in miliseconds. Not recommended to change while running. Press the Update button after change")]
	public float offsetMS = 200f;

	float waitRhythm;
	float waitRhythm2;

	[Header("Bass Control")] // Channel 0
	[Range(1f, 300f)]
	public float bassSensibility = 40f;
	[Range(0.5f, 2f)]
	public float bassHeight = 1.5f;
	[Range(1, 5)]
	public int bassHorizontalScale = 1;
	[Range(0, 300)] [Tooltip("Bass Horizontal Off-set")]
	public int bassOffset = 0;

	[Header("Treble Control")] // Channel 1
	[Range(1f, 300f)]
	public float trebleSensibility = 80f;
	[Range(0.5f, 2f)]
	public float trebleHeight = 1.35f;
	[Range(1, 5)]
	public int trebleHorizontalScale = 3;
	[Range(0, 300)] [Tooltip("Treble Horizontal Off-set, don't decrease or you will get bass values")]
	public int trebleOffset = 67;

	[Header("Levels Control")]
	[Range(0.75f, 5f)] [Tooltip("Sound Bars global scale")]
	public float globalScale = 3f;
	[Range(3f, 15f)] [Tooltip("Sound Bars smooth velocity to return to 0")]
	public float smoothVelocity = 10f;
	public enum Channels {n512, n1024, n2048, n4096, n8192};
	[Tooltip("Large value of channels represents more spectrum values, you will need increase the SoundBars amount to represent all these values. Recommended: 4096, 2048")]
	public Channels channels = Channels.n4096;
	[Tooltip("FFTWindow to use, it is a type of filter. Rectangular = Very Low filter, BlackmanHarris = Very High filter. Recommended = Blackman")]
	public FFTWindow method = FFTWindow.Blackman;
	int channelValue = 4096;

	[Header("Appearance Control")]
	public bool particles = true;
	const float time = 3f;
	const float colorVelocity = 2f;
	[Range(0.1f, 2f)]
	public float minParticleSensibility = 1.5f;
	public bool changeColor = true;
	public Color color1 = Color.yellow;
	public Color color2 = Color.blue;
	public Color color3 = Color.red;
	public Color color4 = new Color (1f, 0.56f, 0f);

	int posColor;

	Color newColor;
	float timeChange;
	Vector3 previousScale;
	Vector3 rightScale;
	Vector3 leftScale;

	bool ready;
	float sca;

	bool rhythm = true;
	bool initiated;
	int halfBarsValue;

	bool songEnded = true;
	float actualBPM;
	float nextBPMTime;
	float lastBPM = 0f;
	float newBPM = 0f;
	float[] taps = new float[3];
	float average;

	int tap = 1;

	bool MsUpdated;
	float lastOffsetMS;

	void Awake () {
		Application.targetFrameRate = 60;

		rhythmParticleSystem = GetComponentInChildren<ParticleSystem> ();

		if (soundBarPrefab != null) {
			halfBarsValue = barsQuantity / 2;

			audioSource = GetComponent<AudioSource> ();
			CreateSoundBars ();
		} else {
			Debug.LogWarning ("Please assign Sound Bar Prefab to the script");
			enabled = false;
		}
	}

	// Apply the time beetween BPM and other
	public void MSDelay () {
		MsUpdated = false;

		if (initiated) {
			rhythm = false;
		} else {
			initiated = true;
		}
		waitRhythm2 = (BPM / 60f);
		waitRhythm = (1f / waitRhythm2);
		offsetMS = Mathf.Abs (offsetMS);
	}

	// Tap BPM
	public void TapBPM () {
		// If the TimeLimit is Excedeed, return 0
		if ((Time.realtimeSinceStartup - lastBPM) > 2f) {
			tap = 0;
			BPM = 0;
		}

		// If user tapped Get the time for that tap
		if (tap >= 0) {
			newBPM = (Time.realtimeSinceStartup - lastBPM);
			newBPM = (60f / newBPM);

			// If greater than 1 Get BPM from first 2 taps
			if (tap > 1) {
				taps [tap - 1] = newBPM;
			}

			tap++;
		}

		// Get Average from last 4 taps and apply
		if (tap >= 4) {
			tap = 1;
			foreach (float i in taps) {
				average += i;
			}
			average = average / taps.Length;
			BPM = Mathf.RoundToInt (average);
		}

		// Save the average
		lastBPM = Time.realtimeSinceStartup;
	}

	// Update Rhythm Scale for Central Particles
	void ReturnScale () {
		if (rhythmScale == RhythmScale.x25) {
			sca = 0.25f;
		} else if (rhythmScale == RhythmScale.x50) {
			sca = 0.5f;
		} else if (rhythmScale == RhythmScale.Normal) {
			sca = 1f;
		} else if (rhythmScale == RhythmScale.x150) {
			sca = 1.5f;
		} else if (rhythmScale == RhythmScale.x200) {
			sca = 2f;
		}

		if (sca != escRhythm) {
			escRhythm = sca;
		}
	} 

	// Create the SoundBars
	void CreateSoundBars () {

		// Instantiate the required Bars Quantity
		for (int i = 0; i < barsQuantity; i++) {
			var clone = Instantiate (soundBarPrefab, transform.position, Quaternion.identity) as GameObject;
			clone.transform.SetParent (gameObject.transform);
		}

		// Assign all these bars to the script
		try {
		soundBars = GameObject.FindGameObjectsWithTag ("SoundBar");
		numberOfBars = soundBars.Length;
		} catch {
			Debug.LogWarning ("Please add the tag SoundBar in the editor. Edit > Project Settings > Tags and Layers > Tags > and create it, then assign that tag to the SoundBarPrefab");
			enabled = false;
			return;
		}
		if (numberOfBars <= 0) {
			Debug.LogWarning ("Please add the tag SoundBar in the editor. Edit > Project Settings > Tags and Layers > Tags > and create it, then assign that tag to the SoundBarPrefab");
			enabled = false;
			return;
		}
			
		UpdateScript ();

		MSDelay ();

		if (!audioSource.isPlaying) {
			rhythmParticleSystem.Play ();
			audioSource.Play ();
			rhythm = true;
			songEnded = false;
		}
	}

	// Update Bars Form when is changed
	public void UpdateScript () {
		ReturnScale ();

		if (form == BarsForm.Circle) {
			for (int i = 0; i < barsQuantity; i++) {
				float angle = i * Mathf.PI * 2f / barsQuantity;
				Vector3 pos = transform.position;
				pos -= new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle)) * length;
				soundBars [i].transform.position = pos;
				soundBars [i].transform.LookAt (transform.position);
			}
		} else if (form == BarsForm.Line) {
			for (int i = 0; i < barsQuantity; i++) {
				Vector3 pos = transform.position;
				pos.x -= length * 5;
				pos.x += (length / barsQuantity) * (i * 10);
				soundBars [i].transform.position = pos;
				soundBars [i].transform.eulerAngles = Vector3.zero;
			}
		} else if (form == BarsForm.ExpansibleCircle) {
			for (int i = 0; i < barsQuantity; i++) {
				float angle = i * Mathf.PI * 2f / barsQuantity;
				Vector3 pos = transform.position;
				pos -= new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle)) * length;
				soundBars [i].transform.position = pos;
				soundBars [i].transform.LookAt (transform.position);
				var rot = soundBars [i].transform.eulerAngles;
				rot.x -= 90;
				soundBars [i].transform.eulerAngles = rot;
			}
		}
		UpdateChannels ();
		if (cameraControl) {
			CameraPosition ();
		}
	}

	// Update Channels of Audio
	void UpdateChannels () {
		if (channels == Channels.n512) {
			channelValue = 512;
		}
		  else if (channels == Channels.n1024) {
			channelValue = 1024;
		} else if (channels == Channels.n2048) {
			channelValue = 2048;
		} else if (channels == Channels.n4096) {
			channelValue = 4096;
		}  else if (channels == Channels.n8192) {
			channelValue = 8192;
		}
	}

	// Camera position
	void CameraPosition () {
		if (form == BarsForm.Line) {
			Camera.main.fieldOfView = fieldOfView;
			var cameraPos = transform.position;
			cameraPos.z -= 70f + distance;
			Camera.main.transform.position = cameraPos;
			Camera.main.transform.LookAt (transform.position);
			cameraPos.y += 5f;
			Camera.main.transform.position = cameraPos;
		} else if (form == BarsForm.Circle) {
			Camera.main.fieldOfView = fieldOfView;
			var cameraPos = transform.position;
			cameraPos.y += 11f;
			cameraPos.z -= 15f + distance;
			Camera.main.transform.position = cameraPos;
			Camera.main.transform.LookAt (transform.position);
		} else if (form == BarsForm.ExpansibleCircle) {
			Camera.main.fieldOfView = fieldOfView;
			var cameraPos = transform.position;
			cameraPos.y += 25f + distance;
			Camera.main.transform.position = cameraPos;
			Camera.main.transform.LookAt (transform.position);
		}
	}

	// Camera Rotating Around Movement
	void CameraMovement () {
		Camera.main.transform.RotateAround (transform.position, Vector3.up, -velocity * Time.deltaTime);
	}

	// Central Rhythm Particles
	void RhythmParticles () {
		if (!rhythm) {
			rhythm = true;
		}

		if (rhythmParticles) {
			if (ready) {
				rhythmParticleSystem.Emit (amountToEmit);
				ready = false;
			}
		}
	}

	void FixedUpdate () {
		if (!songEnded) {
			if (audioSource.time >= nextBPMTime) {
				actualBPM++;
				if (MsUpdated) {
					nextBPMTime += ((waitRhythm / audioSource.pitch) / sca);
				} else {
					nextBPMTime += ((waitRhythm / audioSource.pitch) / sca) + (offsetMS / 1000f) - lastOffsetMS;
					lastOffsetMS = (offsetMS / 1000f);
					MsUpdated = true;
				}
				metronomeTick.Play ();
				RhythmParticles ();
			}
		}
	}

	void Update () {
		
		// Get Spectrum Data from Both Channels of audio
		#pragma warning disable 618
		float[] spectrumleft = audioSource.GetSpectrumData (channelValue, 0, method);
		float[] spectrumright = audioSource.GetSpectrumData (channelValue, 1, method);
		#pragma warning restore 618

		// Stop Rhythm Particles after the Audio Clip ends
		if (!audioSource.isPlaying) {
			rhythmParticleSystem.Stop ();
			// StopAllCoroutines ();
		}

		// Play or stop particles
		if (!rhythmParticles) {
			rhythmParticleSystem.Stop ();
		} else if (rhythmParticles && rhythmParticleSystem.isStopped) {
			rhythmParticleSystem.Play ();
		}

		// SoundBars for Left Channel 
		for (int i = 0; i < barsQuantity / 2; i++) {
			
			// Apply Off-Sets to get the AudioSpectrum
			int spectrum = i * bassHorizontalScale + bassOffset;

			// Get Actual Scale from SoundBar in "i" position
			previousScale = soundBars [i].transform.localScale;

			float newScale;

			var spectrumLeftValue = spectrumleft [spectrum] * bassSensibility;

			// Optional	// Uncomment the next line and below if you want the Central Particles Velocity Follow the Rhythm
			// rhythmParticleSystem.startSpeed = 10; // Minimum Start Speed

			// If Rhythm Sensibility is exceeded
			if (spectrumLeftValue >= rhythmSensibility) {
				
				ready = true; // then central rhythm particles are ready to emit

				//
				// Do another action
				//

				// Uncomment the next line and above if you want the Central Particles Velocity Follow the Rhythm
				/*
			// Change Velocity of Central Particles with the Rhythm
			// Optional	// rhythmParticleSystem.startSpeed = spectrumleft [spectrum] * bassSensibility * 7.5f;
				int numParticles = rhythmParticleSystem.particleCount;
					var parS = new ParticleSystem.Particle[numParticles];
					rhythmParticleSystem.GetParticles (parS);

				for (int par = 0; par < parS.Length; par++) {
					parS [par].velocity = parS [par].velocity.normalized * spectrumleft [spectrum] * bassSensibility * 7.5f;
				}

				rhythmParticleSystem.SetParticles (parS, numParticles); 
				*/
			} 

			// If Minimum Particle Sensibility is exceeded (volume is clamped beetween 0.01 and 1 to avoid 0)
			if (spectrumLeftValue >= minParticleSensibility * Mathf.Clamp (audioSource.volume, 0.01f, 1f)) {

				// Apply extra scale to that SoundBar using Lerp
				newScale = Mathf.Lerp (previousScale.y, spectrumLeftValue * bassHeight * globalScale, Time.deltaTime * 21.8f);

				// If the Particles are activated, emit a particle too
				if (particles) {
					soundBars [i].GetComponentInChildren<ParticleSystem> ().Play ();
				}

			} else {  // Else, Lerp to the previous scale
				
				newScale = Mathf.Lerp (previousScale.y, spectrumLeftValue * globalScale * 0.5f, Time.deltaTime * 21.8f);
			}

			// If the New Scale is greater than Previous Scale, set the New Value to Previous Scale
			if (newScale > previousScale.y) {
				previousScale.y = newScale;
				leftScale = previousScale;
			} else { // Else, Lerp to 0.1 value
				leftScale = previousScale;
				leftScale.y = Mathf.Lerp (previousScale.y, 0.1f, Time.deltaTime * smoothVelocity);
			} 

			// Set new scale
			soundBars [i].transform.localScale = leftScale;

			// Fix minimum Y Scale
			if (soundBars [i].transform.localScale.y < 0.11f) {
				soundBars [i].transform.localScale = new Vector3 (1f, 0.11f, 1f);
			}
		}

		// SoundBars for Right Channel 
		for (int i = 0; i < halfBarsValue; i++) {

			// Apply Off-Sets to get the AudioSpectrum
			int spectrum = i * trebleHorizontalScale + trebleOffset;

			// Get Actual Scale from SoundBar in "i" position
			previousScale = soundBars [i + halfBarsValue].transform.localScale;

			float newScale;

			// Apply sensibility
			var spectrumRightValue = spectrumright [spectrum] * trebleSensibility;

			// If Minimum Particle Sensibility is exceeded (volume is clamped beetween 0.01 and 1 to avoid 0)
			if (spectrumRightValue >= minParticleSensibility * Mathf.Clamp (audioSource.volume, 0.01f, 1f)) {
				
				// Apply extra scale to that SoundBar using Lerp
				newScale = Mathf.Lerp (previousScale.y, spectrumRightValue * trebleHeight * globalScale, Time.deltaTime * 21.8f);

				// If the Particles are activated, emit a particle too
				if (particles) {
					soundBars [i + halfBarsValue].GetComponentInChildren<ParticleSystem> ().Play ();
				}

			} else { 	// Else, Lerp to the previous scale
				newScale = Mathf.Lerp (previousScale.y, spectrumRightValue * globalScale * 0.5f, Time.deltaTime * 21.8f);
			}

			// If the New Scale is greater than Previous Scale, set the New Value to Previous Scale
			if (newScale > previousScale.y) {
				previousScale.y = newScale;
				rightScale = previousScale;
			} else { // Else, Lerp to 0.1
				rightScale = previousScale;
				rightScale.y = Mathf.Lerp (previousScale.y, 0.1f, Time.deltaTime * smoothVelocity);
			}

			// Set new scale
			soundBars [i + halfBarsValue].transform.localScale = rightScale;

			// Fix minimum Y Scale
			if (soundBars [i + halfBarsValue].transform.localScale.y < 0.11f) {
				soundBars [i + halfBarsValue].transform.localScale = new Vector3 (1f, 0.11f, 1f);
			}
		}

		// Change Colors
		if (changeColor) {
			timeChange -= Time.deltaTime;

			// When the counter are less than 0, change to the next Color
			if (timeChange < 0f) {
				NextColor ();
			}

			// Execute color lerping
			ChangeColor ();
		}

		// Execute Camera Control
		if (cameraControl) {
			if (rotateCamera) {
				CameraMovement ();
			}
		}
	}
		
	void ChangeColor () {
		var currentColor = soundBars[0].GetComponentInChildren<Renderer> ().material.color;

		if (posColor == 0) {
			newColor = Color.Lerp (currentColor, color1, Time.deltaTime / colorVelocity);
		}
		else if (posColor == 1) {
			newColor = Color.Lerp (currentColor, color2, Time.deltaTime / colorVelocity);
		} 
		else if (posColor == 2) {
			newColor = Color.Lerp (currentColor, color3, Time.deltaTime / colorVelocity);
		}

		else if (posColor == 3) {
			newColor = Color.Lerp (currentColor, color4, Time.deltaTime / colorVelocity);
		}

		foreach (GameObject cube in soundBars) {
			cube.GetComponentInChildren<Renderer> ().material.color = newColor;
			cube.GetComponentInChildren<ParticleSystem> ().startColor = newColor;
			rhythmParticleSystem.startColor = newColor;
		}
	}

	void NextColor () {
		
		timeChange = time;
		changeColor = false;

		if (posColor < 3) {
			posColor++;
		} else {
			posColor = 0;
		}
		changeColor = true;
	}
}

// PREVIOUS VERSION TIME FOLLOW
/*
	public void InvokeRealTime(string functionName, float delay) {
		StartCoroutine(InvokeRealTimeHelper(functionName, delay));
	}

	IEnumerator InvokeRealTimeHelper(string functionName, float delay) {
		float wait = ((delay / audioSource.pitch) / sca);
	
		yield return new WaitForRealSeconds (wait);

		SendMessage(functionName);
	}

public sealed class WaitForRealSeconds : CustomYieldInstruction
{
	private readonly float _endTime;

	public override bool keepWaiting
	{
		get { return _endTime > Time.realtimeSinceStartup; }
	}

	public WaitForRealSeconds(float seconds)
	{
		_endTime = Time.realtimeSinceStartup + seconds;
	}
}
    */