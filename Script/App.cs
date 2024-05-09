using Carrot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
public class App : MonoBehaviour
{
    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Carrot.Carrot_DeviceOrientationChange deviceRotate;
    private string link_deep_app;
    public Playlist playlist;
    public Music_online playlist_online;
    public Music_Player player_music;

    [Header("UI")]
    public Color color_a;
    public Color color_b;

    [Header("Main Item Panel")]
    public GameObject panel_main_title;
    public GameObject panel_main_select_country;
    public GameObject panel_main_search;
    public GameObject panel_solution;
    public Panel_footer panel_footer;

    [Header("Panel Title")]
    public Image title_icon;
    public Text title_name;
    public Text title_tip;

    [Header ("Search Obj")]
    public InputField inp_search;
    public GameObject prefab_search_key;
    public GameObject panel_main_search_key;

    [Header("Obj Other")]
    public GameObject panel_music_player;
    public GameObject prefab_country_item;
    public GameObject prefab_more_item;
    public GameObject prefab_music_item_list;
    public Transform area_body_country;
    public GameObject canvas_render;
    public GameObject btn_account_playlist;
    public Skybox bk;

    [Header("Obj Scene Roation")]
    public Transform tr_panel_header;
    public Transform tr_panel_body;
    public Transform tr_panel_mini_player;
    public Transform tr_panel_player_full_info;
    public Transform tr_panel_player_full_control;

    public Transform tr_area_portrai;
    public Transform tr_area_landscape_left;
    public Transform tr_area_landscape_right;
    public Transform tr_area_landscape_right_menu;
    public Transform tr_area_portrait_player_full;
    public Transform tr_area_ladscape_player_full;

    [Header("Menu Footer")]
    public ScrollRect scrollrec_menu_footer;
    public Image[] arr_icon_menu_func;

    [Header("Obj Default")]
    public Sprite icon_background;
    public Sprite icon_avatar_default;
    public Sprite icon_avatar_login;
    public Image image_login_avatar;
    public Color32 color_row_1;
    public Color32 color_row_2;

    private int menu_sel = 0;
    private bool is_show_playlist_after_login = false;
    private string s_data_last;
    private Carrot.Carrot_Box box_bk;

    void Start()
    {
        this.link_deep_app = Application.absoluteURL;
        Application.deepLinkActivated += onDeepLinkActivated;
        this.carrot.Load_Carrot(this.check_exit_app);
        this.carrot.shop.onCarrotPaySuccess += this.onBuySuccessPayCarrot;
        this.carrot.shop.onCarrotRestoreSuccess += this.onRestoreSuccessPayCarrot;

        Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;

        this.panel_footer.panel_menu_full.SetActive(false);
        this.panel_music_player.GetComponent<Music_Player>().panel_aduio_mixer.SetActive(false);
        this.GetComponent<Playlist>().panel_new_playlist.SetActive(false);
        this.load_background();

        this.s_data_last = PlayerPrefs.GetString("s_data_last");
        this.check_scene();
    }

    public void start_app_online()
    {
        if (PlayerPrefs.GetString("lang") == "") this.carrot.delay_function(2f, this.check_set_lang);
        this.menu_sel = PlayerPrefs.GetInt("menu_sel",0);
        this.check_show_menu_main();
        if (this.carrot.store_public == Store.Microsoft_Store) this.arr_icon_menu_func[1].transform.parent.gameObject.SetActive(false);
    }

    public void start_app_offline()
    {
        this.menu_sel =4;
        this.check_show_menu_main();
    }

    private void check_set_lang()
    {
        if (PlayerPrefs.GetString("lang") == "") this.carrot.Show_list_lang(reload_list);
    }

    private void reload_list(string s_data)
    {
        this.clear_all_contain();
        string lang_sel = PlayerPrefs.GetString("lang", "en");
        PlayerPrefs.SetString("lang_music",lang_sel);
        this.show_more_list_music();
    }

    public void load_background()
    {
        Texture2D data_pic_bk=this.carrot.get_tool().get_texture2D_to_playerPrefs("bk_app");
        if (data_pic_bk != null) this.set_skybox_Texture(data_pic_bk);
    }

    public void open_menu_footer(int index)
    {
        this.panel_footer.hide_menu_full();
        PlayerPrefs.SetInt("menu_sel",index);
        this.menu_sel = index;
        this.check_show_menu_main();
    }

    public void check_show_menu_main()
    {
        this.panel_main_search.SetActive(false);
        this.panel_main_search_key.SetActive(false);
        this.panel_solution.SetActive(false);

        this.panel_main_select_country.SetActive(false);
        this.title_icon.sprite = this.arr_icon_menu_func[this.menu_sel].sprite;

        for (int i = 0; i < this.arr_icon_menu_func.Length; i++)
        {
            if (i == this.menu_sel)
                this.arr_icon_menu_func[i].color = Color.red;
            else
                this.arr_icon_menu_func[i].color = Color.black;
        }

        if (this.menu_sel == 0) this.show_list_music();

        if (this.menu_sel == 1)
        {
            this.panel_main_title.SetActive(true);
            this.title_name.text = PlayerPrefs.GetString("m_radio", "Radio");
            this.title_tip.text= PlayerPrefs.GetString("m_radio_tip", "List of online radio stations listed by their respective countries");
            this.panel_main_select_country.SetActive(true);
            this.show_list_radio(PlayerPrefs.GetString("lang_music", "en"));
        }

        if (this.menu_sel == 2)
        {
            this.panel_main_title.SetActive(true);
            this.title_name.text = PlayerPrefs.GetString("m_sound", "Sound");
            this.title_tip.text = PlayerPrefs.GetString("m_sound_tip", "Playlists without words, you can save offline to listen when there is no network connection");
            this.show_list_sound();
        }

        if (this.menu_sel == 3)
        {
            this.panel_main_title.SetActive(false);
            this.panel_main_search.SetActive(true);
            this.panel_main_search_key.SetActive(true);
            this.panel_solution.SetActive(true);
            this.show_list_key_search();
        }

        if (this.menu_sel == 4)
        {
            this.title_name.text = PlayerPrefs.GetString("playlist", "Playlist");
            this.title_tip.text = PlayerPrefs.GetString("playlist_tip", "Playlists you have stored for listening when not connected to the network");
            this.GetComponent<App>().clear_all_contain();
            this.carrot.delay_function(1f, this.GetComponent<Music_offiline>().show_list_music_data);
        }

        if (this.menu_sel == 5)
        {
            if (this.carrot.user.get_id_user_login() != "")
                this.GetComponent<Playlist>().show_list_on_main();
            else
            {
                this.is_show_playlist_after_login = true;
                this.carrot.show_login();
            }
        }
        if (this.menu_sel == 6)this.GetComponent<Music_online>().show_list_artist();
        if (this.menu_sel == 7)this.GetComponent<Music_online>().show_list_genre();
        if (this.menu_sel == 8)this.GetComponent<Music_online>().show_list_year();
        if (this.menu_sel == 9)this.GetComponent<App_shop>().show_list_shop();
    }

    private void show_list_music()
    {
        this.panel_main_title.SetActive(true);
        this.title_name.text = PlayerPrefs.GetString("m_music", "Music");
        this.title_tip.text = PlayerPrefs.GetString("m_music_tip", "Online playlists are listed by respective countries");
        this.panel_main_select_country.SetActive(true);
        this.show_list_music(PlayerPrefs.GetString("lang_music", "en"));
    }

    void check_exit_app()
    {
        if (this.panel_music_player.GetComponent<Music_Player>().panel_player_full.activeInHierarchy)
        {
            this.panel_music_player.GetComponent<Music_Player>().back_mini_player();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.panel_music_player.GetComponent<Music_Player>().is_show_audio_wave == true)
        {
            this.panel_music_player.GetComponent<Music_Player>().hide_audio_wave();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.GetComponent<Playlist>().panel_new_playlist.activeInHierarchy)
        {
            this.GetComponent<Playlist>().panel_new_playlist.SetActive(false);
            this.carrot.set_no_check_exit_app();
        }
        else if (this.panel_music_player.GetComponent<Music_Player>().panel_aduio_mixer.activeInHierarchy)
        {
            this.panel_music_player.GetComponent<Music_Player>().panel_aduio_mixer.SetActive(false);
            this.carrot.set_no_check_exit_app();
        }
    }

    public void show_list_music(string lang_music)
    {
        this.clear_all_contain();
        /*
        WWWForm frm = this.carrot.frm_act("list_music");
        frm.AddField("lang_music", lang_music);
        this.list_music = new List<Panel_item_music>();
        this.carrot.send(frm, this.act_get_list_music, act_fail_show_list_music);
        */
        playlist_online.Show(lang_music);
    }

    private void act_fail_show_list_music(string s_data)
    {
        //if(this.s_data_last!="") this.act_get_list_music(this.s_data_last);
    }

    public void show_more_list_music()
    {
        /*
        WWWForm frm = this.carrot.frm_act("list_music");
        frm.AddField("lang_music", PlayerPrefs.GetString("lang_music","en"));
        this.carrot.send(frm, this.act_get_list_music);
        */
    }

    public void show_list_radio(string lang_radio)
    {
        /*
        WWWForm frm = this.carrot.frm_act("list_radio");
        frm.AddField("lang_radio", lang_radio);
        this.carrot.send(frm, act_get_list_radio);
        */
    }

    public void show_list_sound()
    {
        /*
        WWWForm frm = this.carrot.frm_act("list_sound");
        this.carrot.send(frm,this.act_get_list_sound);
        */
    }

    public void show_list_key_search()
    {
        this.clear_all_contain();
        /*
        this.carrot.clear_contain(this.panel_main_search_key.transform);
        this.inp_search.text = "";
        this.carrot.send(this.carrot.frm_act("list_search_key"),this.act_get_list_key_search);
        */
    }

    private void act_get_list_music(string s_data)
    {
        this.s_data_last = s_data;
        IDictionary data = (IDictionary)Json.Deserialize(s_data);
        IList all_music = (IList)data["musics"];
        Debug.Log("list music:" + s_data);
        for (int i = 0; i < all_music.Count; i++)
        {
            IDictionary item_music = (IDictionary)all_music[i];
            this.add_prefab_music(i, item_music, this.prefab_music_item_list, this.canvas_render.transform);
        }

        GameObject Obj_item_more = Instantiate(this.prefab_more_item);
        Obj_item_more.name = "item_music";
        Obj_item_more.GetComponent<Panel_more_item>().txt_title.text = PlayerPrefs.GetString("more", "Click this button to hear 20 more songs!");
        Obj_item_more.transform.SetParent(this.canvas_render.transform);
        Obj_item_more.transform.localPosition = new Vector3(Obj_item_more.transform.localPosition.x, Obj_item_more.transform.localPosition.y, 0f);
        Obj_item_more.transform.localRotation = Quaternion.Euler(Vector3.zero);
        Obj_item_more.transform.localScale = new Vector3(1f, 1f, 1f);

        IList all_country;
        if (this.s_data_last != "")
        {
            IDictionary data_last = (IDictionary)Json.Deserialize(this.s_data_last);
            all_country = (IList)data_last["countrys"];
        }
        else
        {
            all_country = (IList)data["countrys"];
        }

        this.update_data_offline(s_data);
        this.show_list_panel_country(all_country);
    }

    public void add_prefab_music(int index,IDictionary item_music,GameObject prefab,Transform area_body)
    {
        string s_id_music= item_music["id"].ToString();
        GameObject Obj_item_music = Instantiate(prefab);
        Obj_item_music.name = "item_music";
        Obj_item_music.transform.SetParent(area_body);

        if (index % 2 == 0)
        {
            Obj_item_music.GetComponent<Image>().color = this.color_row_1;
        }
        else
        {
            Obj_item_music.GetComponent<Image>().color = this.color_row_2;
        }

        if(item_music["img_video"]!=null)
        if (item_music["img_video"].ToString() != "")
        {
            string id_img_video = "img_video_" + s_id_music;
            Obj_item_music.GetComponent<Panel_item_music>().icon.color = Color.white;
            Sprite sp_img_video = this.carrot.get_tool().get_sprite_to_playerPrefs(id_img_video);

                if (sp_img_video != null)
                    Obj_item_music.GetComponent<Panel_item_music>().icon.sprite = sp_img_video;
                else
                    this.carrot.get_img_and_save_playerPrefs(item_music["img_video"].ToString(), Obj_item_music.GetComponent<Panel_item_music>().icon, "img_video_" + s_id_music);
        }
        Obj_item_music.GetComponent<Panel_item_music>().btn_statu_play.SetActive(false);
        Obj_item_music.GetComponent<Panel_item_music>().btn_delete.SetActive(false);
        Obj_item_music.GetComponent<Panel_item_music>().txt_name.text = item_music["name"].ToString();
        Obj_item_music.GetComponent<Panel_item_music>().url = item_music["url"].ToString();
        Obj_item_music.GetComponent<Panel_item_music>().s_color = item_music["color"].ToString();
        if(item_music["lyrics"]!=null)Obj_item_music.GetComponent<Panel_item_music>().lyrics= item_music["lyrics"].ToString();
        if(item_music["link_ytb"]!=null)Obj_item_music.GetComponent<Panel_item_music>().link_ytb = item_music["link_ytb"].ToString();
        if(item_music["link_store"]!=null) Obj_item_music.GetComponent<Panel_item_music>().link_store= item_music["link_store"].ToString();
        if(item_music["lang"]!=null) Obj_item_music.GetComponent<Panel_item_music>().lang = item_music["lang"].ToString();
        Obj_item_music.GetComponent<Panel_item_music>().type = int.Parse(item_music["type"].ToString());
        Obj_item_music.GetComponent<Panel_item_music>().id_m = s_id_music;
        if(item_music["artist"]!=null) Obj_item_music.GetComponent<Panel_item_music>().artist = item_music["artist"].ToString();
        if (item_music["year"]!=null) Obj_item_music.GetComponent<Panel_item_music>().year= item_music["year"].ToString();
        if (item_music["album"]!=null) Obj_item_music.GetComponent<Panel_item_music>().album = item_music["album"].ToString();
        if (item_music["genre"]!=null) Obj_item_music.GetComponent<Panel_item_music>().genre = item_music["genre"].ToString();
        Obj_item_music.GetComponent<Panel_item_music>().index = index;
        if (this.menu_sel == 0)
        {
            if (this.carrot.user.get_id_user_login()!="")
                Obj_item_music.GetComponent<Panel_item_music>().btn_add_playlist.SetActive(true);
            else
                Obj_item_music.GetComponent<Panel_item_music>().btn_add_playlist.SetActive(false);
        }
        else
        {
            Obj_item_music.GetComponent<Panel_item_music>().btn_add_playlist.SetActive(false);
        }

        Obj_item_music.transform.localPosition = new Vector3(Obj_item_music.transform.localPosition.x, Obj_item_music.transform.localPosition.y, 0f);
        Obj_item_music.transform.localRotation = Quaternion.Euler(Vector3.zero);
        Obj_item_music.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void play_music(Panel_item_music item_music)
    {
        this.carrot.ads.show_ads_Interstitial();
        //this.panel_music_player.GetComponent<Music_Player>().play_music_online(item_music);
    }

    private void act_get_list_key_search(string s_data)
    {
        IList all_key = (IList)Json.Deserialize(s_data);
        for (int i = 0; i < all_key.Count; i++)
        {
            GameObject Obj_item_key = Instantiate(this.prefab_search_key);
            Obj_item_key.GetComponent<Item_key_search>().txt_name.text = all_key[i].ToString();
            Obj_item_key.transform.SetParent(this.panel_main_search_key.transform);
            Obj_item_key.transform.localPosition = new Vector3(Obj_item_key.transform.localPosition.x, Obj_item_key.transform.localPosition.y, 0f);
            Obj_item_key.transform.localRotation = Quaternion.Euler(Vector3.zero);
            Obj_item_key.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        Canvas.ForceUpdateCanvases();
        this.panel_main_search_key.SetActive(false);
        this.panel_main_search_key.SetActive(true);
    }

    public void show_select_lang()
    {
        this.carrot.Show_list_lang(this.reload_list);
    }

    public void set_text_inp_search(string key)
    {
        this.inp_search.text = key;
        this.search_sever_music(0);
    }

    public void act_search()
    {
        this.search_sever_music(1);
    }

    private void search_sever_music(int save_key)
    {
        if (this.inp_search.text.Trim() != "")
        {   
            /*
            WWWForm frm_search = this.carrot.frm_act("search");
            frm_search.AddField("key", this.inp_search.text);
            frm_search.AddField("save_key", save_key);
            this.carrot.send(frm_search,act_handle_search);
            */
        }
    }

    private void act_handle_search(string s_data)
    {
        IList all_data = (IList)Json.Deserialize(s_data);
        Debug.Log("Search data:" + s_data);
        this.clear_all_contain();
        for (int i = 0; i < all_data.Count; i++)
        {
            IDictionary item_data = (IDictionary)all_data[i];
            this.add_prefab_music(i, item_data, this.prefab_music_item_list, this.canvas_render.transform);
        }

        this.panel_solution.SetActive(false);
        this.panel_main_search_key.SetActive(false);
    }

    public void clear_all_contain()
    {
        foreach(Transform child in this.canvas_render.transform)
        {
            if (child.gameObject.name == "item_music"|| child.gameObject.name == "item_shop")
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void act_get_list_radio(string s_data)
    {
        IDictionary data = (IDictionary)Json.Deserialize(s_data);
        IList all_radio = (IList)data["arr_radio"];
        Debug.Log("list Radio:" + s_data);
        this.clear_all_contain();
        for (int i = 0; i < all_radio.Count; i++)
        {
            IDictionary item_radio = (IDictionary)all_radio[i];
            this.add_prefab_music(i, item_radio, this.prefab_music_item_list, this.canvas_render.transform);
        }

        IList all_country = (IList)data["arr_country"];
        this.show_list_panel_country(all_country);
    }

    private void act_get_list_sound(string s_data)
    {
        IList all_music=(IList)Json.Deserialize(s_data);
        Debug.Log("list sound:" + s_data);
        this.clear_all_contain();
        for (int i = 0; i < all_music.Count; i++)
        {
            IDictionary item_music = (IDictionary)all_music[i];
            this.add_prefab_music(i, item_music, this.prefab_music_item_list, this.canvas_render.transform);
        }
    }

    private void show_list_panel_country(IList list_data)
    {
        if (list_data.Count > 0)
        {
            this.carrot.clear_contain(this.area_body_country);
            for (int i = 0; i < list_data.Count; i++)
            {
                IDictionary item_country = (IDictionary)list_data[i];
                GameObject Obj_item_country = Instantiate(this.prefab_country_item);
                Obj_item_country.GetComponent<Panel_country_item>().txt_name.text = item_country["name"].ToString();
                Obj_item_country.GetComponent<Panel_country_item>().key_lang = item_country["key"].ToString();
                Obj_item_country.GetComponent<Panel_country_item>().type = int.Parse(item_country["type"].ToString());
                Obj_item_country.transform.SetParent(this.area_body_country);
                Obj_item_country.transform.localPosition = new Vector3(Obj_item_country.transform.localPosition.x, Obj_item_country.transform.localPosition.y, 0f);
                Obj_item_country.transform.localRotation = Quaternion.Euler(Vector3.zero);
                Obj_item_country.transform.localScale = new Vector3(1f, 1f, 1f);
                if (item_country["key"].ToString() == PlayerPrefs.GetString("lang_music", "vi"))
                {
                    Obj_item_country.GetComponent<Image>().color = Color.white;
                }

                string id_sp_icon_pic = "icon_country_musi_" + item_country["key"].ToString();
                Sprite sp_icon_pic = this.carrot.get_tool().get_sprite_to_playerPrefs(id_sp_icon_pic);
                if (sp_icon_pic != null)
                    Obj_item_country.GetComponent<Panel_country_item>().icon.sprite = sp_icon_pic;
                else
                    this.carrot.get_img_and_save_playerPrefs(item_country["icon"].ToString(), Obj_item_country.GetComponent<Panel_country_item>().icon, id_sp_icon_pic);
            }
        }
        else
        {
            foreach(Transform child in this.area_body_country)
            {
                if (child.GetComponent<Panel_country_item>().key_lang == PlayerPrefs.GetString("lang_music", "vi"))
                {
                    child.GetComponent<Image>().color = Color.yellow;
                }
                else {
                    child.GetComponent<Image>().color = Color.white;
                }
            }
        }
    }


    public void go_left_menu_footer()
    {
        this.scrollrec_menu_footer.horizontalNormalizedPosition = -1f;
    }

    public void go_right_menu_footer()
    {
        this.scrollrec_menu_footer.horizontalNormalizedPosition = 1f;
    }

    public void reset_app()
    {
        this.carrot.get_tool().delete_file("bk.png");
        this.carrot.Delete_all_data();
        this.clear_all_contain();
        this.check_set_lang();
    }

    public void share_app()
    {
        this.carrot.show_share();
    }

    public void rate_app()
    {
        this.carrot.show_rate();
    }

    public void show_login_or_account()
    {
        this.is_show_playlist_after_login = false;
        this.carrot.show_login();
    }

    public void show_select_background()
    {
        /*
        WWWForm frm = carrot.frm_act("list_background");
        this.carrot.send(frm,this.act_list_background);
        */
    }

    private void act_list_background(string s_data)
    {
        IList all_background = (IList)Json.Deserialize(s_data);
        if (this.box_bk != null) this.box_bk.close();
        this.box_bk=this.carrot.show_grid();
        this.box_bk.set_title("Background");
        foreach (IDictionary bk in all_background)
        {
            GameObject obj_img = new GameObject();
            GameObject item_bk = box_bk.add_item(obj_img);
            item_bk.AddComponent<Image>();
            item_bk.transform.localPosition = new Vector3(item_bk.transform.localPosition.x, item_bk.transform.localPosition.y, 0f);
            item_bk.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item_bk.transform.localScale = new Vector3(1f, 1f, 1f);
            item_bk.AddComponent<Button>().onClick.AddListener(() => download_bk_act(bk["url_bk"].ToString()));
            this.carrot.get_img(bk["url"].ToString(), item_bk.GetComponent<Image>());
            Destroy(obj_img);
        }
    }

    private void download_bk_act(string bk_link)
    {
        this.carrot.get_img_and_save_playerPrefs(bk_link, null, "bk_app", act_done_bk_act);
    }

    private void act_done_bk_act(Texture2D pic_data)
    {
        if (this.box_bk != null) this.box_bk.close();
        this.set_skybox_Texture(pic_data);
        this.panel_footer.hide_menu_full();
    }

    public void set_skybox_Texture(Texture textT)
    {
        Material result = new Material(Shader.Find("RenderFX/Skybox"));
        result.SetTexture("_FrontTex", textT);
        result.SetTexture("_BackTex", textT);
        result.SetTexture("_LeftTex", textT);
        result.SetTexture("_RightTex", textT);
        result.SetTexture("_UpTex", textT);
        result.SetTexture("_DownTex", textT);
        this.bk.material = result;
    }

    public void buy_success(Product product)
    {
        this.onBuySuccessPayCarrot(product.definition.id);
    }

    private void onBuySuccessPayCarrot(string id_product)
    {
        if (id_product == this.carrot.shop.get_id_by_index(0))
        {
            this.carrot.Show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("buy_success", "Buy Success! Thank you for purchasing the service pack of Carrot App, you should reboot the app so that the new functionality is set up!"), Carrot.Msg_Icon.Success);
            this.act_inapp_removeads();
        }

        if (id_product == this.carrot.shop.get_id_by_index(1)) this.panel_music_player.GetComponent<Music_Player>().act_download_mp3_file();

        if (id_product == this.carrot.shop.get_id_by_index(2))
        {
            this.carrot.Show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("buy_success", "Buy Success! Thank you for purchasing the service pack of Carrot App, you should reboot the app so that the new functionality is set up!"), Carrot.Msg_Icon.Success);
            this.act_inapp_allmp3();
        }

        if (id_product == this.carrot.shop.get_id_by_index(3))
        {
            this.carrot.Show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("buy_success", "Buy Success! Thank you for purchasing the service pack of Carrot App, you should reboot the app so that the new functionality is set up!"), Carrot.Msg_Icon.Success);
            this.act_inapp_allfunc();
        }
    }

    private void onRestoreSuccessPayCarrot(string[] arr_id)
    {
        for(int i = 0; i < arr_id.Length; i++)
        {
            string id_p = arr_id[i];
            if (id_p == this.carrot.shop.get_id_by_index(2)) this.act_inapp_allmp3();
            if (id_p == this.carrot.shop.get_id_by_index(3)) this.act_inapp_allfunc();
        }
    }

    private void act_inapp_removeads()
    {
        PlayerPrefs.SetInt("is_buy_ads", 1);
    }

    private void act_inapp_allmp3()
    {
        PlayerPrefs.SetInt("is_all_mp3", 1);
    }

    private void act_inapp_allfunc()
    {
        PlayerPrefs.SetInt("is_all_mp3", 1);
        PlayerPrefs.SetInt("is_buy_ads", 1);
        PlayerPrefs.SetInt("is_all_func", 1);
    }

    public void buy_product(int index)
    {
        this.carrot.buy_product(index);
    }

    public void restore_product()
    {
        this.carrot.restore_product();
    }

    public void show_list_carrot_app()
    {
        this.carrot.show_list_carrot_app();
    }

    public void onActionWhereAfterLogin()
    {
        if (this.is_show_playlist_after_login)
        {
            this.GetComponent<Playlist>().show_list_on_main();
        }
    }

    public void check_link_deep_app()
    {
        if (this.link_deep_app.Trim() != "")
        {
            if (this.carrot.is_online())
            {
                if (this.link_deep_app.Contains("show"))
                {
                    string data_link = this.link_deep_app.Replace("music://show/", "");
                    string[] paramet_music = data_link.Split('/');
                    this.GetComponent<Music_online>().get_song_buy_id_and_lang(paramet_music[0],paramet_music[1]);
                    this.link_deep_app = "";
                }
            }
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) carrot.delay_function(1f, check_link_deep_app);
    }

    private void onDeepLinkActivated(string url)
    {
        this.link_deep_app = url;
        if (this.carrot != null) this.carrot.delay_function(1f, this.check_link_deep_app);
    }

    public void btn_setting()
    {
        Carrot.Carrot_Box box_setting=this.carrot.Create_Setting();
        box_setting.update_color_table_row();
    }

    private void update_data_offline(string s_data)
    {
        if (s_data != "")
        {
            this.s_data_last = s_data;
            PlayerPrefs.SetString("s_data_last", this.s_data_last);
        }
    }

    public void check_scene()
    {
        this.carrot.delay_function(1f, check_scene_roation);
    }

    private void check_scene_roation()
    {
        bool is_portrait=this.deviceRotate.Get_status_portrait();
        Debug.Log("is_portrait"+is_portrait);
        if (is_portrait)
        {
            this.tr_panel_header.SetParent(this.tr_area_portrai);
            RectTransform r = this.tr_panel_header.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(0f, 1f);
            r.pivot = new Vector2(0.5f, 1f);
            r.offsetMin = new Vector2(0, r.offsetMin.y);
            r.offsetMax = new Vector2(0, r.offsetMax.y);
            r.offsetMax = new Vector2(r.offsetMax.x, 0f);
            r.offsetMin = new Vector2(r.offsetMin.x, -101.3f);

            this.tr_panel_body.SetParent(this.tr_area_portrai);
            RectTransform r2 = this.tr_panel_body.GetComponent<RectTransform>();
            r2.anchorMin = new Vector2(0f, 0f);
            r2.anchorMax = new Vector2(1f, 1f);
            r2.pivot = new Vector2(0.5f, 0.5f);
            r2.offsetMin = new Vector2(0, r2.offsetMin.y);
            r2.offsetMax = new Vector2(0, r2.offsetMax.y);
            r2.offsetMax = new Vector2(r2.offsetMax.x, 0f);
            r2.offsetMin = new Vector2(r2.offsetMin.x, 0f);

            this.tr_panel_mini_player.SetParent(this.tr_area_portrai);
            RectTransform m = this.tr_panel_mini_player.GetComponent<RectTransform>();
            m.anchorMin = new Vector2(0f, 0f);
            m.anchorMax = new Vector2(1f, 0f);
            m.pivot = new Vector2(0.5f, 0f);
            m.offsetMin = new Vector2(0, m.offsetMin.y);
            m.offsetMax = new Vector2(0, m.offsetMax.y);
            m.offsetMax = new Vector2(m.offsetMax.x, 193f);
            m.offsetMin = new Vector2(m.offsetMin.x, 93f);

            this.tr_panel_player_full_info.SetParent(this.tr_area_portrait_player_full);
            RectTransform f = this.tr_panel_player_full_info.GetComponent<RectTransform>();
            f.anchorMin = new Vector2(0f, 0f);
            f.anchorMax = new Vector2(1f, 1f);
            f.pivot = new Vector2(0.5f, 0.5f);
            f.offsetMin = new Vector2(0, f.offsetMin.y);
            f.offsetMax = new Vector2(0, f.offsetMax.y);
            f.offsetMax = new Vector2(f.offsetMax.x, 0f);
            f.offsetMin = new Vector2(f.offsetMin.x, 0f);

            this.tr_panel_player_full_control.SetParent(this.tr_area_portrait_player_full);
            RectTransform l = this.tr_panel_player_full_control.GetComponent<RectTransform>();
            l.anchorMin = new Vector2(0f, 0f);
            l.anchorMax = new Vector2(1f, 1f);
            l.pivot = new Vector2(0.5f, 0.5f);
            l.offsetMin = new Vector2(0, l.offsetMin.y);
            l.offsetMax = new Vector2(0, l.offsetMax.y);
            l.offsetMax = new Vector2(l.offsetMax.x, 0f);
            l.offsetMin = new Vector2(l.offsetMin.x, 0f);

            this.panel_footer.show_menu_for_portrait();
        }
        else
        {
            this.tr_panel_header.SetParent(this.tr_area_landscape_left);
            RectTransform r = this.tr_panel_header.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(0f, 1f);
            r.pivot = new Vector2(0.5f, 1f);
            r.offsetMin = new Vector2(0, r.offsetMin.y);
            r.offsetMax = new Vector2(0, r.offsetMax.y);
            r.offsetMax = new Vector2(r.offsetMax.x, 0f);
            r.offsetMin = new Vector2(r.offsetMin.x, -101.3f);

            this.tr_panel_body.SetParent(this.tr_area_landscape_left);
            RectTransform r2 = this.tr_panel_body.GetComponent<RectTransform>();
            r2.anchorMin = new Vector2(0f,0f);
            r2.anchorMax = new Vector2(1f,1f);
            r2.pivot = new Vector2(0.5f, 0.5f);
            r2.offsetMin = new Vector2(0, r2.offsetMin.y);
            r2.offsetMax = new Vector2(0, r2.offsetMax.y); 
            r2.offsetMax = new Vector2(r2.offsetMax.x, 0f);
            r2.offsetMin = new Vector2(r2.offsetMin.x, 0f);

            this.tr_panel_mini_player.SetParent(this.tr_area_landscape_right);
            RectTransform m = this.tr_panel_mini_player.GetComponent<RectTransform>();
            m.anchorMin = new Vector2(0f, 0f);
            m.anchorMax = new Vector2(1f, 0f);
            m.pivot = new Vector2(0.5f, 0f);
            m.offsetMin = new Vector2(0, m.offsetMin.y);
            m.offsetMax = new Vector2(0, m.offsetMax.y);
            m.offsetMax = new Vector2(m.offsetMax.x, 99f);
            m.offsetMin = new Vector2(m.offsetMin.x, 0f);

            this.tr_panel_player_full_info.SetParent(this.tr_area_ladscape_player_full);
            RectTransform f = this.tr_panel_player_full_info.GetComponent<RectTransform>();
            f.anchorMin = new Vector2(0f, 0f);
            f.anchorMax = new Vector2(1f, 1f);
            f.pivot = new Vector2(0.5f, 0.5f);
            f.offsetMin = new Vector2(0, f.offsetMin.y);
            f.offsetMax = new Vector2(0, f.offsetMax.y);
            f.offsetMax = new Vector2(f.offsetMax.x, 0f);
            f.offsetMin = new Vector2(f.offsetMin.x, 0f);

            this.tr_panel_player_full_control.SetParent(this.tr_area_ladscape_player_full);
            RectTransform l = this.tr_panel_player_full_control.GetComponent<RectTransform>();
            l.anchorMin = new Vector2(0f, 0f);
            l.anchorMax = new Vector2(1f, 1f);
            l.pivot = new Vector2(0.5f, 0.5f);
            l.offsetMin = new Vector2(0, l.offsetMin.y);
            l.offsetMax = new Vector2(0, l.offsetMax.y);
            l.offsetMax = new Vector2(l.offsetMax.x, 0f);
            l.offsetMin = new Vector2(l.offsetMin.x, 0f);

            this.panel_footer.show_menu_for_landscape();
        }

        this.tr_panel_body.SetSiblingIndex(0);
    }
}
