using Carrot;
using System.Collections;
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
    public Music_offiline playlist_offline;
    public Music_Player player_music;

    [Header("Obj Prefab")]
    public GameObject prefab_item_music;
    public GameObject prefab_item_loading;

    [Header("UI")]
    public Color color_a;
    public Color color_b;

    [Header("Asset icon")]
    public Sprite sp_icon_music;
    public Sprite sp_avata_music_default;

    [Header("Main Item Panel")]
    public Panel_footer panel_footer;

    [Header("Obj Other")]
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
        this.GetComponent<Playlist>().panel_new_playlist.SetActive(false);
        this.load_background();

        this.s_data_last = PlayerPrefs.GetString("s_data_last");
        this.check_scene();

        this.playlist_offline.On_Load();
        this.playlist_online.On_load();
    }

    public void start_app_online()
    {
        if (PlayerPrefs.GetString("lang") == "") this.carrot.delay_function(2f, this.Check_set_lang);
        this.menu_sel = PlayerPrefs.GetInt("menu_sel",0);
        this.check_show_menu_main();
        if (this.carrot.store_public == Store.Microsoft_Store) this.arr_icon_menu_func[1].transform.parent.gameObject.SetActive(false);
    }

    public void start_app_offline()
    {
        this.menu_sel =4;
        this.check_show_menu_main();
    }

    private void Check_set_lang()
    {
        if (PlayerPrefs.GetString("lang") == "") this.carrot.Show_list_lang(After_select_lang);
    }

    private void After_select_lang(string s_lang)
    {
        PlayerPrefs.SetString("lang_music", s_lang);
        this.check_show_menu_main();
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
        for (int i = 0; i < this.arr_icon_menu_func.Length; i++)
        {
            if (i == this.menu_sel)
                this.arr_icon_menu_func[i].color = Color.red;
            else
                this.arr_icon_menu_func[i].color = Color.black;
        }

        if (this.menu_sel == 0) this.show_list_music();
        if (this.menu_sel == 1) this.show_list_radio(PlayerPrefs.GetString("lang_music", "en"));
        if (this.menu_sel == 2) this.show_list_sound();

        if (this.menu_sel == 4)
        {
            this.clear_all_contain();
            this.carrot.delay_function(1f, this.GetComponent<Music_offiline>().show_list_music_data);
        }

        if (this.menu_sel == 5)
        {
            if (this.carrot.user.get_id_user_login() != "")
                this.playlist.show_list_on_main();
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
        this.show_list_music(PlayerPrefs.GetString("lang_music", "en"));
    }

    void check_exit_app()
    {
        if (this.player_music.panel_player_full.activeInHierarchy)
        {
            this.player_music.back_mini_player();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.player_music.is_show_audio_wave == true)
        {
            this.player_music.hide_audio_wave();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.playlist.panel_new_playlist.activeInHierarchy)
        {
            this.playlist.panel_new_playlist.SetActive(false);
            this.carrot.set_no_check_exit_app();
        }
        else if (this.player_music.panel_aduio_mixer.activeInHierarchy)
        {
            this.player_music.panel_aduio_mixer.SetActive(false);
            this.carrot.set_no_check_exit_app();
        }
    }

    public void show_list_music(string lang_music)
    {
        playlist_online.Show(lang_music);
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


    public void show_select_lang()
    {
        this.carrot.Show_list_lang(this.After_select_lang);
    }

    public void act_search()
    {
    }

 
    public void clear_all_contain()
    {
        foreach(Transform child in this.canvas_render.transform) Destroy(child.gameObject);
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
        this.Check_set_lang();
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

        if (id_product == this.carrot.shop.get_id_by_index(1)) this.player_music.act_download_mp3_file();

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
                    this.playlist_online.Get_song_by_id(paramet_music[0]);
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

    public Carrot_Box_Item Create_item(string s_name)
    {
        GameObject obj_item_m = Instantiate(this.prefab_item_music);
        obj_item_m.name = s_name;
        obj_item_m.transform.SetParent(this.canvas_render.transform);
        obj_item_m.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_item_m.transform.localPosition = Vector3.zero;
        obj_item_m.GetComponent<Carrot_Box_Item>().check_type();

        Carrot_Box_Item box_item = obj_item_m.GetComponent<Carrot_Box_Item>();
        box_item.txt_tip.color = carrot.color_highlight;
        return box_item;
    }

    public void Create_loading()
    {
        this.clear_all_contain();
        GameObject obj_item_loading = Instantiate(this.prefab_item_loading);
        obj_item_loading.name = "loading";
        obj_item_loading.transform.SetParent(this.canvas_render.transform);
        obj_item_loading.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_item_loading.transform.localPosition = Vector3.zero;
        obj_item_loading.GetComponentInChildren<Image>().color = carrot.color_highlight;
    }

}
