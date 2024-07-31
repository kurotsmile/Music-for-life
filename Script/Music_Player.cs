using Carrot;
using Crosstales.Radio;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Music_Player : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("Obj Player")]
    public Sprite icon_music;
    public Sprite icon_lyrics;
    public Sprite icon_music_defautl;
    public Sprite icon_play;
    public Sprite icon_pause;
    public Sprite[] icon_loop;

    [Header("Player Mini")]
    public GameObject panel_player_mini;
    public Text txt_name_song_mini;
    public Image avatar_mini;
    public GameObject panel_loading_download;
    public Image img_btn_play;
    public Slider slider_timer_music;
    public Image img_slider_timer_music;
    public GameObject obj_btn_save;
    public Image img_icon_loading;
    public Image img_icon_loop_mini;
    public Slider slider_download_mini;

    [Header("Player Full Detail")]
    public GameObject panel_player_full;
    public Text txt_name_song_full;
    public Image avatar_full;
    public Slider slider_timer_music_full;
    public Image img_slider_timer_music_full;
    public Image img_btn_timer_music_full;
    public Image img_btn_play_full;
    public GameObject panel_loading_download_full;
    public Animator animation_avatar_full;
    public GameObject btn_lyrics_full;
    public GameObject prefab_lyrics_full;
    public GameObject btn_ytb_full;
    public GameObject obj_btn_save_full;
    public GameObject button_download_file_mp3;
    public GameObject button_add_song_to_playlist;
    public Text txt_feel_tip;
    public Image img_icon_loop_full;
    public Text txt_time_full;
    public GameObject panel_feel_full;
    public GameObject btn_share_full;
    public GameObject btn_link_full;
    public Text[] txt_feel_count_full;
    public Color32 color_sel_feel_full_nomal;
    public Color32 color_sel_feel_full_active;
    public Slider slider_download_full;

    [Header("Bar info music")]
    public GameObject panel_bar_info;
    public GameObject Item_info_artist;
    public GameObject Item_info_album;
    public GameObject Item_info_genre;
    public GameObject Item_info_year;
    public Text txt_info_artist;
    public Text txt_info_album;
    public Text txt_info_genre;
    public Text txt_info_year;

    public GameObject[] panel_sel_feel;

    private bool is_status_play;

    private byte[] data_music_save;

    [Header("Audio mix")]
    public GameObject panel_aduio_mixer;
    public Dropdown dropdown_ReverbFilters;
    public Slider[] slider_mixer;
    private System.Collections.Generic.List<AudioReverbPreset> reverbPresets = new System.Collections.Generic.List<AudioReverbPreset>();

    [Header("Audio Wave")]
    public Sprite[] icon_wave_rotate;
    public Sprite[] icon_wave_style;
    public GameObject panel_canva_main;
    public GameObject panel_canva_audiowave;
    public RhythmVisualizator rhythmVisualizator;
    public GameObject camera_main;
    public GameObject camrea_visualizator;
    public bool is_show_audio_wave = false;
    private int audio_wave_style = 0;
    public Image img_audio_wave_style;
    public Image img_aduo_wave_rotate;

    Carrot_Box box = null;
    private IDictionary data_music_cur = null;
    private IList list_data_music = null;
    private int index_loop = 0;
    private int index_item_play = -1;
    private bool is_click_control = false;

    void Start()
    {
        this.GetComponent<RadioPlayer>().OnErrorInfo += this.Radio_act_error;
        this.GetComponent<RadioPlayer>().OnAudioPlayTimeUpdate += onAudioPlayTimeUpdate;
        this.panel_player_mini.SetActive(false);
        this.panel_player_full.SetActive(false);
        this.index_loop = PlayerPrefs.GetInt("index_loop", 0);
        this.img_icon_loop_full.sprite = this.icon_loop[this.index_loop];
        this.img_icon_loop_mini.sprite = this.icon_loop[this.index_loop];

        System.Collections.Generic.List<Dropdown.OptionData> options = new System.Collections.Generic.List<Dropdown.OptionData>();

        foreach (AudioReverbPreset arp in System.Enum.GetValues(typeof(AudioReverbPreset)))
        {
            options.Add(new Dropdown.OptionData(arp.ToString()));
            reverbPresets.Add(arp);
        }

        if (dropdown_ReverbFilters != null)
        {
            dropdown_ReverbFilters.ClearOptions();
            dropdown_ReverbFilters.AddOptions(options);
        }
    }

    public void show_audio_wave()
    {
        this.panel_canva_main.SetActive(false);
        this.panel_canva_audiowave.SetActive(true);
        this.camera_main.SetActive(false);
        this.camrea_visualizator.SetActive(true);
        this.rhythmVisualizator.gameObject.SetActive(true);
        this.is_show_audio_wave = true;
    }


    public void hide_audio_wave()
    {
        this.panel_canva_main.SetActive(true);
        this.panel_canva_audiowave.SetActive(false);
        this.camera_main.SetActive(true);
        this.camrea_visualizator.SetActive(false);
        this.rhythmVisualizator.gameObject.SetActive(false);
        this.is_show_audio_wave = false;
    }

    public void change_style_audio_wave()
    {
        this.audio_wave_style++;
        this.img_audio_wave_style.sprite = this.icon_wave_style[this.audio_wave_style];
        if (this.audio_wave_style == 0) { this.rhythmVisualizator.form = RhythmVisualizator.BarsForm.Line; }
        if (this.audio_wave_style == 1) { this.rhythmVisualizator.form = RhythmVisualizator.BarsForm.Circle; }
        if (this.audio_wave_style == 2) { this.rhythmVisualizator.form = RhythmVisualizator.BarsForm.ExpansibleCircle; this.audio_wave_style = -1; }
        this.rhythmVisualizator.UpdateScript();
    }

    public void change_rotate_audio_wave()
    {
        if (this.rhythmVisualizator.rotateCamera)
        {
            this.rhythmVisualizator.rotateCamera = false;
            this.img_aduo_wave_rotate.sprite = this.icon_wave_rotate[1];
        }
        else
        {
            this.rhythmVisualizator.rotateCamera = true;
            this.img_aduo_wave_rotate.sprite = this.icon_wave_rotate[0];
        }
    }

    public void ReverbFilterDropdownChanged()
    {
        this.GetComponent<AudioReverbFilter>().reverbPreset = reverbPresets[dropdown_ReverbFilters.value];
        this.check_mixer();
    }

    private void check_mixer()
    {
        this.slider_mixer[0].value = this.GetComponent<AudioReverbFilter>().dryLevel;
        this.slider_mixer[1].value = this.GetComponent<AudioReverbFilter>().room;
        this.slider_mixer[2].value = this.GetComponent<AudioReverbFilter>().roomHF;
        this.slider_mixer[3].value = this.GetComponent<AudioReverbFilter>().roomLF;
        this.slider_mixer[4].value = this.GetComponent<AudioReverbFilter>().decayTime;
        this.slider_mixer[5].value = this.GetComponent<AudioReverbFilter>().decayHFRatio;
        this.slider_mixer[6].value = this.GetComponent<AudioReverbFilter>().reflectionsLevel;
        this.slider_mixer[7].value = this.GetComponent<AudioReverbFilter>().reflectionsDelay;
        this.slider_mixer[8].value = this.GetComponent<AudioReverbFilter>().reverbLevel;
        this.slider_mixer[9].value = this.GetComponent<AudioReverbFilter>().reverbDelay;
        this.slider_mixer[10].value = this.GetComponent<AudioReverbFilter>().hfReference;
        this.slider_mixer[11].value = this.GetComponent<AudioReverbFilter>().lfReference;
        this.slider_mixer[12].value = this.GetComponent<AudioReverbFilter>().diffusion;
        this.slider_mixer[13].value = this.GetComponent<AudioReverbFilter>().density;
    }

    public void change_mixer()
    {
        this.GetComponent<AudioReverbFilter>().dryLevel = this.slider_mixer[0].value;
        this.GetComponent<AudioReverbFilter>().room = this.slider_mixer[1].value;
        this.GetComponent<AudioReverbFilter>().roomHF = this.slider_mixer[2].value;
        this.GetComponent<AudioReverbFilter>().roomLF = this.slider_mixer[3].value;
        this.GetComponent<AudioReverbFilter>().decayTime = this.slider_mixer[4].value;
        this.GetComponent<AudioReverbFilter>().decayHFRatio = this.slider_mixer[5].value;
        this.GetComponent<AudioReverbFilter>().reflectionsLevel = this.slider_mixer[6].value;
        this.GetComponent<AudioReverbFilter>().reflectionsDelay = this.slider_mixer[7].value;
        this.GetComponent<AudioReverbFilter>().reverbLevel = this.slider_mixer[8].value;
        this.GetComponent<AudioReverbFilter>().reverbDelay = this.slider_mixer[9].value;
        this.GetComponent<AudioReverbFilter>().hfReference = this.slider_mixer[10].value;
        this.GetComponent<AudioReverbFilter>().lfReference = this.slider_mixer[11].value;
        this.GetComponent<AudioReverbFilter>().diffusion = this.slider_mixer[12].value;
        this.GetComponent<AudioReverbFilter>().density = this.slider_mixer[13].value;
    }

    public void reset_mixer()
    {
        this.dropdown_ReverbFilters.value = 0;
        this.dropdown_ReverbFilters.RefreshShownValue();
    }

    public void Play_by_data(IDictionary data)
    {
        Debug.Log(data["type"].ToString());

        this.data_music_save = null;
        this.is_status_play = false;
        this.app.carrot.ads.show_ads_Interstitial();
        this.panel_player_mini.SetActive(true);
        this.GetComponent<AudioSource>().Stop();
        this.GetComponent<AudioSource>().clip = null;
        this.data_music_cur = data;
        this.index_item_play = int.Parse(data["index_play"].ToString());
        this.txt_name_song_mini.text = data["name"].ToString();
        this.txt_name_song_full.text = data["name"].ToString();
        this.avatar_mini.sprite = app.sp_avata_music_default;
        this.avatar_full.sprite = app.sp_avata_music_default;
        this.slider_timer_music.gameObject.SetActive(false);

        this.animation_avatar_full.enabled = false;
        this.button_download_file_mp3.SetActive(false);
        this.button_add_song_to_playlist.SetActive(false);

        this.obj_btn_save.SetActive(false);
        this.obj_btn_save_full.SetActive(false);
        if (data["type"].ToString() == "radio_online"|| data["type"].ToString() == "radio_offline")
        {
            this.panel_player_mini.SetActive(true);
            this.animation_avatar_full.enabled = true;

            this.slider_download_full.value = 0;
            this.slider_download_mini.value = 0;
            this.panel_loading_download.SetActive(true);
            this.panel_loading_download_full.SetActive(true);
            this.GetComponent<RadioPlayer>().Stop();
            this.GetComponent<RadioPlayer>().Restart(0.5f);
            this.GetComponent<RadioPlayer>().Station.Url = data["url"].ToString();
            this.GetComponent<RadioPlayer>().Station.Name = data["name"].ToString();
            this.GetComponent<RadioPlayer>().Play();

            this.is_status_play = true;
            this.img_btn_play.sprite = icon_pause;
            this.img_btn_play_full.sprite = icon_pause;
            this.img_icon_loop_full.gameObject.SetActive(false);
            this.img_icon_loop_mini.gameObject.SetActive(false);
            this.panel_feel_full.SetActive(false);
            this.txt_feel_tip.gameObject.SetActive(false);

            this.slider_timer_music.gameObject.SetActive(false);
            this.slider_timer_music_full.gameObject.SetActive(false);

            this.Check_show_btn_save();
        }

        if (data["type"].ToString() == "music_online" || 
            data["type"].ToString() == "music_offline" || 
            data["type"].ToString() == "sound_online"||
            data["type"].ToString() == "sound_offline"
        )
        {
            if (data["type"].ToString()== "music_online"|| data["type"].ToString() == "music_offline")
            {
                if (data["genre"].ToString() == "" && data["album"].ToString() == "" && data["artist"].ToString() == "" && data["year"].ToString() == "")
                {
                    this.panel_bar_info.SetActive(false);
                }
                else
                {
                    this.Item_info_artist.SetActive(false);
                    this.Item_info_genre.SetActive(false);
                    this.Item_info_album.SetActive(false);
                    this.Item_info_year.SetActive(false);

                    this.panel_bar_info.SetActive(true);
                    this.txt_info_artist.text = data["artist"].ToString();
                    this.txt_info_album.text = data["album"].ToString();
                    this.txt_info_genre.text = data["genre"].ToString();
                    this.txt_info_year.text = data["year"].ToString();
                    if (data["artist"].ToString().ToString() != "") this.Item_info_artist.SetActive(true);
                    if (data["genre"].ToString() != "") this.Item_info_genre.SetActive(true);
                    if (data["album"].ToString() != "") this.Item_info_album.SetActive(true);
                    if (data["year"].ToString() != "") this.Item_info_year.SetActive(true);
                }

                string s_id_avatar = "pic_avatar_" + data["id"].ToString();
                Sprite sp_pic_avatar = app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_avatar);
                if (sp_pic_avatar != null)
                {
                    this.avatar_full.sprite = sp_pic_avatar;
                    this.avatar_mini.sprite = sp_pic_avatar;
                }
                else
                {
                    app.carrot.get_img_and_save_playerPrefs(data["avatar"].ToString(), null, s_id_avatar, this.Get_avatar_music_done);
                }
            }

            this.panel_loading_download.SetActive(true);
            this.panel_loading_download_full.SetActive(true);
            this.img_icon_loop_full.gameObject.SetActive(true);
            this.img_icon_loop_mini.gameObject.SetActive(true);

            if (data["type"].ToString() == "music_online"|| data["type"].ToString() == "sound_online")
            {
                this.panel_feel_full.SetActive(true);
                this.txt_feel_tip.gameObject.SetActive(true);
                if (data["mp3"].ToString() != "") this.download_music(data["mp3"].ToString());
            }
            else
            {
                this.panel_feel_full.SetActive(false);
                this.txt_feel_tip.gameObject.SetActive(false);
                string path_file = data["index"].ToString() + ".data";
                if (app.carrot.get_tool().check_file_exist(path_file))
                {
                    string url_mp3=app.carrot.get_tool().get_file_path(path_file);
                    Debug.Log(url_mp3);
                    this.download_music(url_mp3);
                }
                else
                {
                    if (data["mp3"].ToString() != "") this.download_music(data["mp3"].ToString(), data["index"].ToString());
                }
            }
        }
    }

    private void Get_avatar_music_done(Texture2D tex)
    {
        this.avatar_full.sprite = app.carrot.get_tool().Texture2DtoSprite(tex);
        this.avatar_mini.sprite = app.carrot.get_tool().Texture2DtoSprite(tex);
    }

    public void stop()
    {
        this.StopAllCoroutines();
        if (this.data_music_cur["type"].ToString() == "radio_online" || this.data_music_cur["type"].ToString() == "radio_offline")
        {
            this.GetComponent<RadioPlayer>().Stop();
        }
        else
        {
            this.GetComponent<AudioSource>().Stop();
        }
        this.panel_player_mini.SetActive(false);
        this.panel_player_full.SetActive(false);
        this.is_status_play = false;
    }

    private void download_music(string url,string index_save_offline="")
    {
        StartCoroutine(DownloadAudio(url,index_save_offline));
    }

    void Update()
    {
        if (this.is_status_play)
        {
            if (this.data_music_cur["type"].ToString() == "music_online"|| this.data_music_cur["type"].ToString() == "music_offline"|| this.data_music_cur["type"].ToString() == "sound_online" || this.data_music_cur["type"].ToString() == "sound_offline")
            {
                if (this.is_click_control == false && this.GetComponent<AudioSource>().isPlaying == false)
                {
                    if (this.index_loop == 0)
                    {
                        this.stop();
                    }

                    if (this.index_loop == 1)
                    {
                        this.GetComponent<AudioSource>().Play();
                    }

                    if (this.index_loop == 2)
                    {
                        this.btn_next();
                    }
                }
                else
                {
                    this.txt_time_full.text = string.Format("{0}:{1:00}", (int)this.GetComponent<AudioSource>().time / 60, (int)this.GetComponent<AudioSource>().time % 60);
                    this.slider_timer_music.value = this.GetComponent<AudioSource>().time;
                    this.slider_timer_music_full.value = this.GetComponent<AudioSource>().time;
                }
            }
        }
    }

    public void act_play_and_pause()
    {
        if (this.data_music_cur["type"].ToString() == "radion_online"|| this.data_music_cur["type"].ToString() == "radion_offline")
        {

            if (this.GetComponent<RadioPlayer>().isAudioPlaying)
            {
                this.GetComponent<RadioPlayer>().Stop();
                this.img_btn_play.sprite = this.icon_play;
                this.img_btn_play_full.sprite = this.icon_play;
                this.is_status_play = false;
                this.animation_avatar_full.enabled = false;
            }
            else
            {
                this.GetComponent<RadioPlayer>().Play();
                this.img_btn_play.sprite = this.icon_pause;
                this.img_btn_play_full.sprite = this.icon_pause;
                this.is_status_play = true;
                this.animation_avatar_full.enabled = true;
            }
        }
        else
        {
            this.is_click_control = true;
            if (this.GetComponent<AudioSource>().isPlaying)
            {
                this.GetComponent<AudioSource>().Pause();
                this.img_btn_play.sprite = this.icon_play;
                this.img_btn_play_full.sprite = this.icon_play;
                this.is_status_play = false;
                this.animation_avatar_full.enabled = false;
            }
            else
            {
                this.GetComponent<AudioSource>().Play();
                this.img_btn_play.sprite = this.icon_pause;
                this.img_btn_play_full.sprite = this.icon_pause;
                this.is_status_play = true;
                this.animation_avatar_full.enabled = true;
                this.is_click_control = false;
            }
        }
    }

    IEnumerator DownloadAudio(string s_url,string index_save_offline="")
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(s_url, AudioType.MPEG))
        {
            www.SendWebRequest();
            while (!www.isDone)
            {
                this.slider_download_mini.value = www.downloadProgress;
                this.slider_download_full.value = www.downloadProgress;
                yield return null;
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                this.stop();
            }
            else
            {
                this.GetComponent<AudioSource>().clip = DownloadHandlerAudioClip.GetContent(www);
                this.GetComponent<AudioSource>().Play();
                this.data_music_save = www.downloadHandler.data;
                if (index_save_offline != "") app.carrot.get_tool().save_file(index_save_offline + ".data",this.data_music_save);
                this.slider_timer_music.maxValue = this.GetComponent<AudioSource>().clip.length;
                this.slider_timer_music_full.maxValue = this.GetComponent<AudioSource>().clip.length;
                this.panel_loading_download.SetActive(false);
                this.panel_loading_download_full.SetActive(false);
                this.slider_timer_music.gameObject.SetActive(true);
                this.slider_timer_music_full.gameObject.SetActive(true);
                this.is_status_play = true;
                this.img_btn_play.sprite = icon_pause;
                this.img_btn_play_full.sprite = icon_pause;
                this.animation_avatar_full.enabled = true;

                this.Check_show_btn_save();
            }
        }
    }

    public void btn_next()
    {
        this.index_item_play++;
        IDictionary data_music = (IDictionary) this.list_data_music[this.index_item_play];
        this.Play_by_data(data_music);
    }

    public void btn_prev()
    {
        this.index_item_play--;
        IDictionary data_music = (IDictionary)this.list_data_music[this.index_item_play];
        this.Play_by_data(data_music);
    }

    public void btn_loop()
    {
        this.index_loop++;
        this.img_icon_loop_full.sprite = this.icon_loop[this.index_loop];
        this.img_icon_loop_mini.sprite = this.icon_loop[this.index_loop];
        if (this.index_loop >= this.icon_loop.Length - 1)
        {
            this.index_loop = -1;
        }
    }

    public void btn_save()
    {
        if (this.data_music_cur["type"].ToString()=="music_online")
        {
            this.data_music_cur["type"] = "music_offline";
            app.playlist_offline.Add(this.data_music_cur, this.data_music_save);
        }

        if (this.data_music_cur["type"].ToString() == "radio_online")
        {
            this.data_music_cur["type"] = "radio_offline";
            app.playlist_offline.Add(this.data_music_cur, this.data_music_save);
        }

        if (this.data_music_cur["type"].ToString() == "sound_online")
        {
            this.data_music_cur["type"] = "sound_offline";
            app.playlist_offline.Add(this.data_music_cur, this.data_music_save);
        }

        app.carrot.Show_msg(app.carrot.L("playlist", "Playlist"), app.carrot.L("save_song_success", "Successfully stored, you can listen to the song again in the playlist"));
        this.Check_show_btn_save();
    }

    private void Check_show_btn_save()
    {
        if (this.data_music_cur["type"].ToString()=="music_online"||this.data_music_cur["type"].ToString() == "radio_online" || this.data_music_cur["type"].ToString() == "sound_online")
        {
            this.obj_btn_save.SetActive(true);
            this.obj_btn_save_full.SetActive(true);
        }
        else
        {
            this.obj_btn_save.SetActive(false);
            this.obj_btn_save_full.SetActive(false);
        }
    }

    public void show_full()
    {
        if (this.data_music_cur["type"].ToString() == "music_online")
        {
            this.get_feel_music();
        }
        this.panel_player_full.SetActive(true);
        this.panel_player_mini.SetActive(false);
    }

    public void back_mini_player()
    {
        this.panel_player_full.SetActive(false);
        this.panel_player_mini.SetActive(true);
    }

    public void show_lyrics()
    {
        app.carrot.show_loading();
        StructuredQuery q = new("song");
        q.Add_select("lyrics");
        q.Add_where("id", Query_OP.EQUAL, this.data_music_cur["id"].ToString());
        app.carrot.server.Get_doc(q.ToJson(), Get_lyrics_done);
    }

    private void Get_lyrics_done(string s_data)
    {
        app.carrot.hide_loading();
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            IDictionary data_lyrics = fc.fire_document[0].Get_IDictionary();
            if (data_lyrics["lyrics"] != null)
            {
                this.box = app.carrot.Create_Box(app.carrot.L("m_lyrics", "Lyrics"), this.icon_lyrics);
                GameObject lyrics = Instantiate(this.prefab_lyrics_full);
                Text txt_lyrics = lyrics.GetComponent<Text>(); 
                txt_lyrics.text = Regex.Replace(data_lyrics["lyrics"].ToString(), "<.*?>", string.Empty);
                this.box.add_item(lyrics);
                Destroy(lyrics);
                app.carrot.delay_function(1f, Refesh_lyrics);
            }
        }
    }

    private void Refesh_lyrics()
    {
        this.box.gameObject.SetActive(false);
        this.box.gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }

    public void btn_ytb()
    {
        Application.OpenURL(this.data_music_cur["link_ytb"].ToString());
    }

    public void btn_link()
    {
        Application.OpenURL(this.app.carrot.mainhost + "?p=song&id=" + this.data_music_cur["id"].ToString());
    }

    public void btn_share()
    {
        app.carrot.show_share(this.app.carrot.mainhost + "?p=song&id=" + this.data_music_cur["id"].ToString(), this.data_music_cur["name"].ToString());
    }

    public void sel_feel_music(int sel_index)
    {
        /*
        WWWForm frm = GameObject.Find("App").GetComponent<App>().carrot.frm_act("rate_music");
        frm.AddField("id_music", this.music_item_cur.id_m);
        frm.AddField("lang_music", this.music_item_cur.lang);
        frm.AddField("sel_rate", sel_index);
        GameObject.Find("App").GetComponent<App>().carrot.send(frm, this.act_handle_set_feel);
        */
    }

    public void get_feel_music()
    {
        /*
        WWWForm frm = GameObject.Find("App").GetComponent<App>().carrot.frm_act("rate_get");
        frm.AddField("id_music", this.music_item_cur.id_m);
        frm.AddField("lang_music", this.music_item_cur.lang);
        GameObject.Find("App").GetComponent<App>().carrot.send_hide(frm, act_handle_feel_music);
        */
    }

    private void act_handle_feel_music(string s_data)
    {
        Debug.Log("Feel:" + s_data);
        IList data = (IList)Json.Deserialize(s_data);
        this.txt_feel_count_full[0].text = data[0].ToString();
        this.txt_feel_count_full[1].text = data[1].ToString();
        this.txt_feel_count_full[2].text = data[2].ToString();
        this.txt_feel_count_full[3].text = data[3].ToString();
        this.reset_box_feel(data[4].ToString());
    }

    private void reset_box_feel(string s_index)
    {
        for (int i = 0; i < this.panel_sel_feel.Length; i++)
        {
            this.panel_sel_feel[i].GetComponent<Image>().color = this.color_sel_feel_full_nomal;
        }

        if (s_index != "-1")
        {
            int i_sel = int.Parse(s_index);
            this.panel_sel_feel[i_sel].GetComponent<Image>().color = this.color_sel_feel_full_active;
        }
    }

    private void act_handle_set_feel(string s_data)
    {
        //GameObject.Find("App").GetComponent<App>().carrot.Show_msg(PlayerPrefs.GetString("box_feel_title", "Music For Life"), PlayerPrefs.GetString("rate_music_thanks", "Thank you for evaluating your feelings about this song"));
        //GameObject.Find("App").GetComponent<App>().carrot.delay_function(2.3f, this.get_feel_music);
    }

    public void btn_show_info_artist()
    {
        this.back_mini_player();
        this.app.playlist_online.Show_list_item_in_info("artist", this.data_music_cur["artist"].ToString(), this.data_music_cur["lang"].ToString());
    }
    public void btn_show_info_album()
    {
        this.back_mini_player();
        this.app.playlist_online.Show_list_item_in_info("album", this.data_music_cur["album"].ToString(), this.data_music_cur["lang"].ToString());
    }
    public void btn_show_info_genre()
    {
        this.back_mini_player();
        this.app.playlist_online.Show_list_item_in_info("genre", this.data_music_cur["genre"].ToString(), this.data_music_cur["lang"].ToString());
    }
    public void btn_show_info_year()
    {
        this.back_mini_player();
        this.app.playlist_online.Show_list_item_in_info("year", this.data_music_cur["year"].ToString(), this.data_music_cur["lang"].ToString());
    }

    public void show_auido_mixer()
    {
        app.carrot.play_sound_click();
        this.panel_aduio_mixer.SetActive(true);
    }

    public void download_mp3_file()
    {
        if (PlayerPrefs.GetInt("is_all_mp3", 0) == 0)
            GameObject.Find("App").GetComponent<App>().carrot.buy_product(1);
        else
            this.act_download_mp3_file();
    }

    public void act_download_mp3_file()
    {
        Application.OpenURL(this.data_music_cur["mp3"].ToString());
        app.carrot.Show_msg(app.carrot.L("title", "Music for life"), app.carrot.L("get_mp3_success", "Export mp3 file download link successfully!"));
    }

    public void Radio_act_play_audio()
    {
        this.panel_loading_download.SetActive(false);
        this.panel_loading_download_full.SetActive(false);
        this.panel_player_mini.SetActive(true);
    }

    public void Radio_act_get_data()
    {
        this.panel_loading_download.SetActive(true);
        this.panel_loading_download_full.SetActive(true);
        this.panel_player_mini.SetActive(true);
    }

    public void Radio_act_end_get_data()
    {
        this.panel_loading_download.SetActive(false);
        this.panel_loading_download_full.SetActive(false);
        this.panel_player_mini.SetActive(true);
    }

    public void Radio_act_error(Crosstales.Radio.Model.RadioStation station, string info)
    {
        if(!info.Contains("Station is already playing!"))
        {
            app.carrot.Show_msg(app.carrot.L("m_radio","Radio"), app.carrot.L("radio_error", "This radio channel is currently inactive, please try again another time. Now choose another radio station to listen to!"), Msg_Icon.Alert);
        }
    }

    private void onAudioPlayTimeUpdate(Crosstales.Radio.Model.RadioStation station, float _playtime)
    {
         this.txt_time_full.text = Crosstales.Radio.Util.Helper.FormatSecondsToHRF(_playtime);
    }

    private void OnDestroy()
    {
        this.GetComponent<RadioPlayer>().OnErrorInfo -= this.Radio_act_error;
        this.GetComponent<RadioPlayer>().OnAudioPlayTimeUpdate -= onAudioPlayTimeUpdate;
    }

    public void btn_add_song_to_playlist()
    {
        app.carrot.play_sound_click();
        app.playlist_offline.Show_move_playlist(this.data_music_cur);
    }

    public void Set_list_music(IList iList_music)
    {
        this.list_data_music = iList_music;
    }

    public void Btn_show_list_cur()
    {
        this.box = app.carrot.Create_Box();
        this.box.set_icon(app.sp_icon_music_list_now);
        this.box.set_title("Playlist currently playing");

        for (int i = 0; i < this.list_data_music.Count; i++)
        {
            IDictionary data_m = (IDictionary) this.list_data_music[i];
            Carrot_Box_Item item_m=box.create_item();
            item_m.set_icon(app.carrot.game.icon_play_music_game);
            item_m.set_title(data_m["name"].ToString());
            item_m.set_tip(data_m["type"].ToString());

            if (this.data_music_cur["type"].ToString() == data_m["type"].ToString())
            {
                if (this.data_music_cur["id"].ToString() == data_m["id"].ToString()) item_m.set_icon(app.carrot.game.icon_pause_music_game);
            }

            if (i % 2 == 0)
                item_m.GetComponent<Image>().color = app.color_row_1;
            else
                item_m.GetComponent<Image>().color = app.color_row_2;

            item_m.set_act(() =>
            {
                if (box != null) box.close();
                this.Play_by_data(data_m);
            });
        }
    }

    public bool get_status_play()
    {
        return this.is_status_play;
    }
}
