using UnityEngine;

public class MusicWaveSkybox : MonoBehaviour
{
    public App app;
    public AudioSource musicSource;
    public Camera targetCamera;

    [Header("Palette")]
    public Color tintLow = new Color(0.03f, 0.36f, 0.78f);
    public Color tintHigh = new Color(0.14f, 0.86f, 1.00f);
    public Color groundLow = new Color(0.00f, 0.10f, 0.32f);
    public Color groundHigh = new Color(0.03f, 0.30f, 0.62f);

    [Header("Dynamics")]
    public float tintLerpSpeed = 4.5f;
    public float exposureMin = 1.00f;
    public float exposureMax = 1.90f;
    public float atmosphereMin = 0.70f;
    public float atmosphereMax = 1.70f;
    public float waveVisualStrength = 0.35f;
    public float waveSpeed = 1.8f;

    private readonly float[] spectrum = new float[64];
    private Material runtimeSkybox;
    private Material cachedCameraSkybox;
    private Material cachedRenderSettingsSkybox;
    private bool cachedCameraSkyboxEnabled = false;
    private CameraClearFlags cachedCameraClearFlags;
    private Skybox waveCameraSkybox;
    private bool isWaveMode = false;
    private float currentEnergy = 0f;
    private float waveClock = 0f;

    void Awake()
    {
        if (this.app == null) this.app = FindObjectOfType<App>();
        if (this.musicSource == null && this.app != null) this.musicSource = this.app.player_music.GetComponent<AudioSource>();
        if (this.targetCamera == null)
        {
            GameObject camObj = GameObject.Find("ExampleCamera");
            if (camObj != null) this.targetCamera = camObj.GetComponent<Camera>();
        }
    }

    public void SetWaveMode(bool isEnable)
    {
        if (isEnable)
            this.EnableWaveSkybox();
        else
            this.DisableWaveSkybox();
    }

    void Update()
    {
        if (!this.isWaveMode) return;
        if (this.runtimeSkybox == null) return;

        float energy = this.GetSpectrumEnergy();
        this.currentEnergy = Mathf.Lerp(this.currentEnergy, energy, Time.deltaTime * this.tintLerpSpeed);
        this.waveClock += Time.deltaTime * this.waveSpeed * Mathf.Lerp(0.8f, 2.6f, this.currentEnergy + 0.1f);

        float waveA = Mathf.Sin(this.waveClock);
        float waveB = Mathf.Sin(this.waveClock * 1.73f + 1.25f);
        float waveMix = (waveA * 0.65f + waveB * 0.35f) * this.waveVisualStrength;

        Color tint = Color.Lerp(this.tintLow, this.tintHigh, Mathf.Clamp01(this.currentEnergy + waveMix * 0.5f));
        Color ground = Color.Lerp(this.groundLow, this.groundHigh, Mathf.Clamp01(this.currentEnergy * 0.9f + waveMix * 0.35f));
        float ripple = waveMix * Mathf.Lerp(0.05f, 0.22f, this.currentEnergy + 0.2f);
        float exposure = Mathf.Lerp(this.exposureMin, this.exposureMax, this.currentEnergy) + ripple;
        float atmosphere = Mathf.Lerp(this.atmosphereMin, this.atmosphereMax, this.currentEnergy);

        this.runtimeSkybox.SetColor("_SkyTint", tint);
        this.runtimeSkybox.SetColor("_GroundColor", ground);
        this.runtimeSkybox.SetFloat("_Exposure", exposure);
        this.runtimeSkybox.SetFloat("_AtmosphereThickness", atmosphere);
        if (this.runtimeSkybox.HasProperty("_Rotation"))
            this.runtimeSkybox.SetFloat("_Rotation", this.waveClock * 12f);
        if (RenderSettings.skybox != this.runtimeSkybox)
            RenderSettings.skybox = this.runtimeSkybox;
    }

    private void EnableWaveSkybox()
    {
        if (this.isWaveMode) return;

        if (this.runtimeSkybox == null)
        {
            Shader skyShader = Shader.Find("Skybox/Procedural");
            if (skyShader == null) return;
            this.runtimeSkybox = new Material(skyShader);
            this.runtimeSkybox.name = "Skybox_Wave_Runtime";
            if (this.runtimeSkybox.HasProperty("_SunDisk")) this.runtimeSkybox.SetFloat("_SunDisk", 0f);
            this.runtimeSkybox.SetFloat("_SunSize", 0.02f);
            this.runtimeSkybox.SetFloat("_SunSizeConvergence", 6f);
            this.runtimeSkybox.SetColor("_SkyTint", this.tintLow);
            this.runtimeSkybox.SetColor("_GroundColor", this.groundLow);
            this.runtimeSkybox.SetFloat("_Exposure", this.exposureMin);
            this.runtimeSkybox.SetFloat("_AtmosphereThickness", this.atmosphereMin);
        }

        if (this.targetCamera == null)
        {
            GameObject camObj = GameObject.Find("ExampleCamera");
            if (camObj != null) this.targetCamera = camObj.GetComponent<Camera>();
        }

        if (this.targetCamera == null) return;

        this.waveCameraSkybox = this.targetCamera.GetComponent<Skybox>();
        if (this.waveCameraSkybox == null) this.waveCameraSkybox = this.targetCamera.gameObject.AddComponent<Skybox>();
        this.cachedCameraSkybox = this.waveCameraSkybox.material;
        this.cachedCameraSkyboxEnabled = this.waveCameraSkybox.enabled;
        this.cachedCameraClearFlags = this.targetCamera.clearFlags;
        this.cachedRenderSettingsSkybox = RenderSettings.skybox;
        this.waveCameraSkybox.material = this.runtimeSkybox;
        this.waveCameraSkybox.enabled = true;
        this.targetCamera.clearFlags = CameraClearFlags.Skybox;
        RenderSettings.skybox = this.runtimeSkybox;

        this.currentEnergy = 0f;
        this.waveClock = 0f;
        this.isWaveMode = true;
    }

    private void DisableWaveSkybox()
    {
        if (!this.isWaveMode) return;

        if (this.waveCameraSkybox != null)
        {
            this.waveCameraSkybox.material = this.cachedCameraSkybox;
            this.waveCameraSkybox.enabled = this.cachedCameraSkyboxEnabled;
        }
        if (this.targetCamera != null)
            this.targetCamera.clearFlags = this.cachedCameraClearFlags;
        RenderSettings.skybox = this.cachedRenderSettingsSkybox;

        this.isWaveMode = false;
    }

    private float GetSpectrumEnergy()
    {
        bool hasSpectrum = false;
        AudioListener.GetSpectrumData(this.spectrum, 0, FFTWindow.BlackmanHarris);
        for (int i = 0; i < this.spectrum.Length; i++)
        {
            if (this.spectrum[i] > 0.000001f)
            {
                hasSpectrum = true;
                break;
            }
        }

        if (!hasSpectrum)
        {
            if (this.musicSource == null && this.app != null && this.app.player_music != null)
                this.musicSource = this.app.player_music.GetComponent<AudioSource>();
            if (this.musicSource != null)
                this.musicSource.GetSpectrumData(this.spectrum, 0, FFTWindow.BlackmanHarris);
        }

        float bass = 0f;
        float mid = 0f;
        float high = 0f;

        for (int i = 0; i < this.spectrum.Length; i++)
        {
            float v = this.spectrum[i];
            if (i < 8) bass += v;
            else if (i < 24) mid += v;
            else high += v;
        }

        float energy = bass * 1.8f + mid * 0.9f + high * 0.45f;
        return Mathf.Clamp01(energy * 18f);
    }
}
