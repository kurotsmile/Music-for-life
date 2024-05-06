﻿using Carrot;
using Crosstales.Radio;
using Crosstales.Radio.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Music_Player : MonoBehaviour
{
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
    private Panel_item_music music_item_cur;
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
    Color myColor = new Color();

    private byte[] data_music_save;

    private int index_loop = 0;
    private bool is_click_control = false;

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

    Carrot_Box box_lyrics;
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

    public void play_music_online(Panel_item_music m)
    {
        this.reset_list_music_data();
        this.GetComponent<AudioSource>().Stop();
        this.GetComponent<AudioSource>().clip = null;
        m.btn_statu_play.SetActive(true);
        this.music_item_cur = m;
        this.txt_name_song_mini.text = m.txt_name.text;
        this.txt_name_song_full.text = m.txt_name.text;
        if (m.genre.ToString() == "" && m.album.ToString() == "" && m.artist.ToString() == "" && m.year.ToString() == "")
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
            this.txt_info_artist.text = m.artist;
            this.txt_info_album.text = m.album;
            this.txt_info_genre.text = m.genre;
            this.txt_info_year.text = m.year;
            if (m.artist.ToString() != "") this.Item_info_artist.SetActive(true);
            if (m.genre.ToString() != "") this.Item_info_genre.SetActive(true);
            if (m.album.ToString() != "") this.Item_info_album.SetActive(true);
            if (m.year.ToString() != "") this.Item_info_year.SetActive(true);
        }

        this.avatar_mini.sprite = m.icon.sprite;
        this.animation_avatar_full.enabled = false;
        this.button_download_file_mp3.SetActive(false);
        this.button_add_song_to_playlist.SetActive(false);
        if (this.music_item_cur.icon.sprite == this.icon_music)
            this.avatar_full.sprite = this.icon_music_defautl;
        else
            this.avatar_full.sprite = m.icon.sprite;

        this.obj_btn_save.SetActive(false);
        this.obj_btn_save_full.SetActive(false);
        if (m.type == 1)
        {
            this.animation_avatar_full.enabled = true;

            this.slider_download_full.value = 0;
            this.slider_download_mini.value = 0;
            this.panel_loading_download.SetActive(true);
            this.panel_loading_download_full.SetActive(true);
            this.GetComponent<RadioPlayer>().Stop();
            this.GetComponent<RadioPlayer>().Restart(0.5f);
            this.GetComponent<RadioPlayer>().Station.AllowOnlyHTTPS = true;
            this.GetComponent<RadioPlayer>().Station.Url = m.url;
            this.GetComponent<RadioPlayer>().Station.Name = m.txt_name.text;
            this.GetComponent<RadioPlayer>().Play();

            this.slider_timer_music.gameObject.SetActive(false);
            this.is_status_play = true;
            this.music_item_cur.btn_statu_play.GetComponent<Image>().color = myColor;
            this.img_btn_play.sprite = icon_pause;
            this.img_btn_play_full.sprite = icon_pause;
            this.img_icon_loop_full.gameObject.SetActive(false);
            this.img_icon_loop_mini.gameObject.SetActive(false);
            this.panel_feel_full.SetActive(false);
            this.txt_feel_tip.gameObject.SetActive(false);
        }
        else
        {
            this.panel_loading_download.SetActive(true);
            this.panel_loading_download_full.SetActive(true);
            this.img_icon_loop_full.gameObject.SetActive(true);
            this.img_icon_loop_mini.gameObject.SetActive(true);
            if (this.music_item_cur.type == 0)
            {
                this.panel_feel_full.SetActive(true);
                this.txt_feel_tip.gameObject.SetActive(true);
            }
            else
            {
                this.panel_feel_full.SetActive(false);
                this.txt_feel_tip.gameObject.SetActive(false);
            }
            if (m.url == "")
            {
                this.download_music(GameObject.Find("App").GetComponent<App>().carrot.mainhost + "/app_mygirl/app_my_girl_" + m.lang + "/" + m.id_m + ".mp3");
            }
            else
            {
                this.download_music(m.url);
            }
        }

        this.slider_timer_music.gameObject.SetActive(false);
        this.slider_timer_music_full.gameObject.SetActive(false);
        this.is_status_play = false;

        if (m.lyrics.ToString() == "")
        {
            this.btn_lyrics_full.SetActive(false);
        }
        else
        {
            this.btn_lyrics_full.SetActive(true);
        }

        if (m.link_ytb.ToString() == "")
        {
            this.btn_ytb_full.SetActive(false);
        }
        else
        {
            this.btn_ytb_full.SetActive(true);
        }

        if (this.music_item_cur.link_store.ToString() == "")
        {
            this.btn_link_full.SetActive(false);
            this.btn_share_full.SetActive(false);
        }
        else
        {
            this.btn_link_full.SetActive(true);
            this.btn_share_full.SetActive(true);
        }

        ColorUtility.TryParseHtmlString(m.s_color, out myColor);
        this.img_slider_timer_music.color = myColor;
        this.img_slider_timer_music_full.color = myColor;
        this.img_btn_timer_music_full.color = myColor;
        this.txt_feel_tip.color = myColor;
        this.img_icon_loading.color = myColor;
        if (this.panel_player_full.activeInHierarchy == false)
        {
            this.panel_player_mini.SetActive(true);
        }
        else
        {
            if (this.music_item_cur.type == 0) this.get_feel_music();

        }

        if(this.music_item_cur.type == 0 || this.music_item_cur.type == 2) if (this.music_item_cur.id_m != "") this.button_download_file_mp3.SetActive(true);
        if(this.music_item_cur.type==0) this.button_add_song_to_playlist.SetActive(true); 
    }

    public void stop()
    {
        this.StopAllCoroutines();
        if (this.music_item_cur.type == 1)
        {
            this.GetComponent<RadioPlayer>().Stop();
        }
        else
        {
            this.GetComponent<AudioSource>().Stop();
        }
        if (this.music_item_cur != null) this.music_item_cur.btn_statu_play.SetActive(false);
        this.panel_player_mini.SetActive(false);
        this.panel_player_full.SetActive(false);
    }

    private void download_music(string url)
    {
        StartCoroutine(downloadAudio(url));
    }

    void Update()
    {
        if (this.is_status_play)
        {
            if (this.music_item_cur.type == 1)
            {
  
            }
            else
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
        if (this.music_item_cur.type == 1)
        {

            if (this.GetComponent<RadioPlayer>().isAudioPlaying)
            {
                this.GetComponent<RadioPlayer>().Stop();
                this.img_btn_play.sprite = this.icon_play;
                this.img_btn_play_full.sprite = this.icon_play;
                if (this.music_item_cur != null) this.music_item_cur.btn_statu_play.GetComponent<Image>().color = Color.gray;
                this.is_status_play = false;
                this.animation_avatar_full.enabled = false;
            }
            else
            {
                this.GetComponent<RadioPlayer>().Play();
                this.img_btn_play.sprite = this.icon_pause;
                this.img_btn_play_full.sprite = this.icon_pause;
                if (this.music_item_cur != null) this.music_item_cur.btn_statu_play.GetComponent<Image>().color = myColor;
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
                if (this.music_item_cur != null) this.music_item_cur.btn_statu_play.GetComponent<Image>().color = Color.gray;
                this.is_status_play = false;
                this.animation_avatar_full.enabled = false;
            }
            else
            {
                this.GetComponent<AudioSource>().Play();
                this.img_btn_play.sprite = this.icon_pause;
                this.img_btn_play_full.sprite = this.icon_pause;
                if (this.music_item_cur != null) this.music_item_cur.btn_statu_play.GetComponent<Image>().color = myColor;
                this.is_status_play = true;
                this.animation_avatar_full.enabled = true;
                this.is_click_control = false;
            }
        }
    }


    IEnumerator downloadAudio(string s_url)
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
                this.slider_timer_music.maxValue = this.GetComponent<AudioSource>().clip.length;
                this.slider_timer_music_full.maxValue = this.GetComponent<AudioSource>().clip.length;
                this.panel_loading_download.SetActive(false);
                this.panel_loading_download_full.SetActive(false);
                this.slider_timer_music.gameObject.SetActive(true);
                this.slider_timer_music_full.gameObject.SetActive(true);
                this.is_status_play = true;
                this.music_item_cur.btn_statu_play.GetComponent<Image>().color = myColor;
                this.img_btn_play.sprite = icon_pause;
                this.img_btn_play_full.sprite = icon_pause;
                this.animation_avatar_full.enabled = true;

                if (this.music_item_cur.type == 3)
                {
                    this.obj_btn_save.SetActive(false);
                    this.obj_btn_save_full.SetActive(false);
                }
                else
                {
                    this.obj_btn_save.SetActive(true);
                    this.obj_btn_save_full.SetActive(true);
                }
            }
        }
    }

    public void btn_next()
    {
        this.reset_list_music_data();
        if (this.music_item_cur.index < GameObject.Find("App").GetComponent<App>().get_list_music_data().Count - 1)
        {
            this.play_music_online(GameObject.Find("App").GetComponent<App>().get_list_music_data()[this.music_item_cur.index + 1]);
        }
        else
        {
            this.play_music_online(GameObject.Find("App").GetComponent<App>().get_list_music_data()[0]);
        }
    }

    public void btn_prev()
    {
        this.reset_list_music_data();
        if (this.music_item_cur.index <= 0)
        {
            int last = GameObject.Find("App").GetComponent<App>().get_list_music_data().Count - 1;
            this.play_music_online(GameObject.Find("App").GetComponent<App>().get_list_music_data()[last]);
        }
        else
        {
            this.play_music_online(GameObject.Find("App").GetComponent<App>().get_list_music_data()[this.music_item_cur.index - 1]);
        }
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
        GameObject.Find("App").GetComponent<Music_offiline>().add_music(this.music_item_cur, this.data_music_save);
        this.obj_btn_save.SetActive(false);
        this.obj_btn_save_full.SetActive(false);
    }

    private void reset_list_music_data()
    {
        foreach (Panel_item_music m in GameObject.Find("App").GetComponent<App>().get_list_music_data())
        {
            if(m!=null) m.btn_statu_play.SetActive(false);
        }
    }

    public void show_full()
    {
        if (this.music_item_cur.type == 0)
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
        this.box_lyrics=GameObject.Find("App").GetComponent<App>().carrot.Create_Box(PlayerPrefs.GetString("m_lyrics", "Lyrics"), this.icon_lyrics);
        GameObject lyrics = Instantiate(this.prefab_lyrics_full);
        Text txt_lyrics = lyrics.GetComponent<Text>();
        txt_lyrics.text= this.music_item_cur.lyrics;
        this.box_lyrics.add_item(lyrics);
        Destroy(lyrics);
        GameObject.Find("App").GetComponent<App>().carrot.delay_function(1f, refesh_lyrics);
    }

    private void refesh_lyrics()
    {
        this.box_lyrics.gameObject.SetActive(false);
        this.box_lyrics.gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }

    public void btn_ytb()
    {
        Application.OpenURL(this.music_item_cur.link_ytb);
    }

    public void btn_link()
    {
        Application.OpenURL(this.music_item_cur.link_store);
    }

    public void btn_share()
    {
        GameObject.Find("App").GetComponent<App>().carrot.show_share(this.music_item_cur.link_store, this.music_item_cur.txt_name.text);
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
        GameObject.Find("App").GetComponent<App>().carrot.Show_msg(PlayerPrefs.GetString("box_feel_title", "Music For Life"), PlayerPrefs.GetString("rate_music_thanks", "Thank you for evaluating your feelings about this song"));
        GameObject.Find("App").GetComponent<App>().carrot.delay_function(2.3f, this.get_feel_music);
    }

    public void btn_show_info_artist()
    {
        this.back_mini_player();
        GameObject.Find("App").GetComponent<Music_online>().show_list_item_in_info(this.music_item_cur.artist, "artist", this.music_item_cur.lang);
    }
    public void btn_show_info_album()
    {
        this.back_mini_player();
        GameObject.Find("App").GetComponent<Music_online>().show_list_item_in_info(this.music_item_cur.album, "album", this.music_item_cur.lang);
    }
    public void btn_show_info_genre()
    {
        this.back_mini_player();
        GameObject.Find("App").GetComponent<Music_online>().show_list_item_in_info(this.music_item_cur.genre, "genre", this.music_item_cur.lang);
    }
    public void btn_show_info_year()
    {
        this.back_mini_player();
        GameObject.Find("App").GetComponent<Music_online>().show_list_item_in_info(this.music_item_cur.year, "year", this.music_item_cur.lang);
    }

    public void show_auido_mixer()
    {
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
        Application.OpenURL(this.music_item_cur.url);
        GameObject.Find("App").GetComponent<App>().carrot.Show_msg(PlayerPrefs.GetString("title", "Music for life"), PlayerPrefs.GetString("get_mp3_success", "Export mp3 file download link successfully!"));
    }

    public void Radio_act_play_audio()
    {
        this.panel_loading_download.SetActive(false);
        this.panel_loading_download_full.SetActive(false);
    }

    public void Radio_act_get_data()
    {
        this.panel_loading_download.SetActive(true);
        this.panel_loading_download_full.SetActive(true);
    }

    public void Radio_act_end_get_data()
    {
        this.panel_loading_download.SetActive(false);
        this.panel_loading_download_full.SetActive(false);
    }

    public void Radio_act_error(Crosstales.Radio.Model.RadioStation station, string info)
    {
        if(!info.Contains("Station is already playing!"))
        {
            GameObject.Find("App").GetComponent<App>().carrot.Show_msg(PlayerPrefs.GetString("m_radio","Radio"), PlayerPrefs.GetString("radio_error", "This radio channel is currently inactive, please try again another time. Now choose another radio station to listen to!"), Msg_Icon.Alert);
            this.stop();
        }
    }

    private void onAudioPlayTimeUpdate(Crosstales.Radio.Model.RadioStation station, float _playtime)
    {
         this.txt_time_full.text = Crosstales.Radio.Util.Helper.FormatSecondsToHourMinSec(_playtime);
    }

    private void OnDestroy()
    {
        this.GetComponent<RadioPlayer>().OnErrorInfo -= this.Radio_act_error;
        this.GetComponent<RadioPlayer>().OnAudioPlayTimeUpdate -= onAudioPlayTimeUpdate;
    }

    public void btn_add_song_to_playlist()
    {
        if(GameObject.Find("App").GetComponent<App>().carrot.user.get_id_user_login()!="")
            GameObject.Find("App").GetComponent<Playlist>().show_add_song_to_playlist(this.music_item_cur.id_m, this.music_item_cur.lang);
        else
            GameObject.Find("App").GetComponent<App>().carrot.show_login();
    }

}