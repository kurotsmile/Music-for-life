using Carrot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Music_online : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;
    public GameObject prefab_item_music;

    private string s_data_temp = "";
    private string cached_lang = "";
    private string order_at = "publishedAt";
    private Query_Order_Direction order_type = Query_Order_Direction.ASCENDING;

    [Header("Obj Online")]
    public Sprite icon_artist;
    public Sprite icon_genre;
    public Sprite icon_year;
    public Sprite icon_album;

    private Carrot_Box box;
    private List<IDictionary> list_data_play;
    private bool is_auto_retrying = false;

    public void On_load()
    {
        if (app.carrot.is_offline())
        {
            string lang = PlayerPrefs.GetString("lang_music", "en");
            this.s_data_temp = PlayerPrefs.GetString(Get_cache_key(lang), "");
            this.cached_lang = lang;
        }
    }

    public void Get_song_by_id(string s_id)
    {
        app.carrot.show_loading();
        StructuredQuery q = new("song");
        q.Add_where("id", Query_OP.EQUAL, s_id);
        app.carrot.hub.Get_doc(q.ToJson(), Get_song_done);
    }

    private void Get_song_done(string s_data)
    {
        app.carrot.hide_loading();
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            IDictionary data_song = fc.fire_document[0].Get_IDictionary();
            app.player_music.Play_by_data(data_song);
        }
    }

    public void Show(string s_lang)
    {
        if (this.cached_lang != s_lang)
        {
            this.s_data_temp = PlayerPrefs.GetString(Get_cache_key(s_lang), "");
            this.cached_lang = s_lang;
        }

        if (this.Is_empty_song_cache(this.s_data_temp))
            this.Get_data_from_server(s_lang);
        else
            this.Load_list_by_data(this.s_data_temp);
    }

    private bool Is_empty_song_cache(string s_data)
    {
        if (string.IsNullOrEmpty(s_data)) return true;
        IList list_song = Carrot.Json.Deserialize(s_data) as IList;
        if (list_song == null) return true;
        return list_song.Count == 0;
    }

    public StructuredQuery Q_basic_song()
    {
        StructuredQuery q = new("song");
        q.Add_select("name");
        q.Add_select("genre");
        q.Add_select("artist");
        q.Add_select("year");
        q.Add_select("album");
        q.Add_select("lang");
        q.Add_select("mp3");
        q.Add_select("avatar");
        q.Add_select("link_ytb");
        q.Set_limit(30);
        return q;
    }

    private void Get_data_from_server(string s_lang)
    {
        app.Create_loading();
        List<string> lang_candidates = this.Build_lang_candidates(s_lang);
        this.Read_song_with_lang_candidates(s_lang, lang_candidates, 0);
        /*
        StructuredQuery q = this.Q_basic_song();
        q.Add_where("lang", Query_OP.EQUAL, s_lang);
        q.Add_order(this.order_at,this.order_type);
        q.Set_limit(30);
        this.app.carrot.hub.Get_doc(q.ToJson(), (s_data) =>
        {
            this.s_data_temp = s_data;
            PlayerPrefs.SetString("s_data_music_", s_data);
            this.Load_list_by_data(s_data);
        },app.Act_server_fail);
        */
    }

    private void Read_song_with_lang_candidates(string selected_lang, List<string> lang_candidates, int index)
    {
        if (index >= lang_candidates.Count)
        {
            Debug.LogWarning("Music_online: no data by lang filter. Fallback to unfiltered read_table.");
            Dictionary<string, object> fallback = new()
            {
                { "page", 1 },
                { "limit", 300 },
                { "order_key", "updated_at" },
                { "order_type", "DESC" }
            };

            app.carrot.hub.ReadTable("song", fallback, act =>
            {
                IList raw_list = Carrot.Json.Deserialize(act) as IList;
                int raw_count = raw_list == null ? 0 : raw_list.Count;
                string filtered_data = this.Filter_song_by_lang(act, selected_lang);
                IList filtered_list = Carrot.Json.Deserialize(filtered_data) as IList;
                int filtered_count = filtered_list == null ? 0 : filtered_list.Count;

                Debug.LogWarning("Music_online fallback: total=" + raw_count + ", matched_lang=" + filtered_count + ", lang=" + selected_lang);

                if (filtered_count > 0)
                    this.Apply_loaded_data(filtered_data, selected_lang);
                else
                    this.Apply_loaded_data(act, selected_lang);
            }, err =>
            {
                Debug.LogError(err);
                this.Handle_first_load_fail(selected_lang, err);
            });
            return;
        }

        string lang_try = lang_candidates[index];
        Dictionary<string, object> filters = new()
        {
            { "lang", lang_try }
        };

        app.carrot.hub.ReadTable("song", filters, act =>
        {
            IList list_try = Carrot.Json.Deserialize(act) as IList;
            int count_try = list_try == null ? 0 : list_try.Count;
            Debug.Log("Music_online worker lang try '" + lang_try + "' => " + count_try + " rows");

            if (count_try > 0)
                this.Apply_loaded_data(act, selected_lang);
            else
                this.Read_song_with_lang_candidates(selected_lang, lang_candidates, index + 1);
        }, err =>
        {
            Debug.LogError(err);
            this.Read_song_with_lang_candidates(selected_lang, lang_candidates, index + 1);
        });
    }

    private void Apply_loaded_data(string s_data, string s_lang)
    {
        this.is_auto_retrying = false;
        string filtered_data = this.Filter_song_by_lang(s_data, s_lang);
        this.s_data_temp = filtered_data;
        this.cached_lang = s_lang;
        PlayerPrefs.SetString(Get_cache_key(s_lang), filtered_data);
        this.Load_list_by_data(filtered_data);
    }

    private void Handle_first_load_fail(string selected_lang, string error)
    {
        this.Load_list_by_data("[]");
        if (this.is_auto_retrying) return;
        this.is_auto_retrying = true;
        Debug.LogWarning("Music_online: auto retry in 1.5s after worker error. lang=" + selected_lang + " err=" + error);
        app.carrot.delay_function(1.5f, () =>
        {
            this.s_data_temp = "";
            this.Show(selected_lang);
        });
    }

    private List<string> Build_lang_candidates(string s_lang)
    {
        string lang = this.Normalize_lang_key(s_lang);
        List<string> keys = new();
        this.Add_candidate_lang(keys, lang);

        if (lang == "en")
        {
            this.Add_candidate_lang(keys, "us");
            this.Add_candidate_lang(keys, "uk");
            this.Add_candidate_lang(keys, "en-us");
            this.Add_candidate_lang(keys, "en_us");
            this.Add_candidate_lang(keys, "en-gb");
            this.Add_candidate_lang(keys, "en_gb");
        }

        if (lang == "vi") this.Add_candidate_lang(keys, "vn");
        if (lang == "vn") this.Add_candidate_lang(keys, "vi");

        int base_count = keys.Count;
        for (int i = 0; i < base_count; i++)
            this.Add_candidate_lang(keys, keys[i].ToUpperInvariant());

        return keys;
    }

    private void Add_candidate_lang(List<string> keys, string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        if (keys.Contains(value)) return;
        keys.Add(value);
    }

    private string Get_cache_key(string lang)
    {
        return "s_data_music_" + lang;
    }

    private string Filter_song_by_lang(string s_data, string s_lang)
    {
        IList list_song = Carrot.Json.Deserialize(s_data) as IList;
        if (list_song == null) return "[]";

        List<object> list_song_by_lang = new();
        for (int i = 0; i < list_song.Count; i++)
        {
            IDictionary data_m = list_song[i] as IDictionary;
            if (data_m == null) continue;
            if (!this.Is_match_lang(data_m, s_lang)) continue;
            list_song_by_lang.Add(data_m);
        }
        return Carrot.Json.Serialize(list_song_by_lang);
    }

    private bool Is_match_lang(IDictionary data, string selected_lang)
    {
        if (data == null) return false;
        if (data["lang"] == null) return false;

        string data_lang = this.Normalize_lang_key(data["lang"].ToString());
        string selected = this.Normalize_lang_key(selected_lang);

        if (data_lang == selected) return true;

        // Common alias pairs used inconsistently by some sources.
        if ((selected == "vi" && data_lang == "vn") || (selected == "vn" && data_lang == "vi")) return true;
        if (this.Is_english_alias(selected) && this.Is_english_alias(data_lang)) return true;

        return false;
    }

    private bool Is_english_alias(string key)
    {
        return key == "en"
            || key == "us"
            || key == "uk"
            || key == "en-us"
            || key == "en_us"
            || key == "en-gb"
            || key == "en_gb";
    }

    private string Normalize_lang_key(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Trim().ToLowerInvariant();
    }

    public void Load_list_by_data(string s_data)
    {
            IList list_song=Carrot.Json.Deserialize(s_data) as IList;
            if (list_song == null) list_song = new List<object>();
            app.clear_all_contain();
            this.s_data_temp = s_data;
            string selected_lang = PlayerPrefs.GetString("lang_music", "en");

            Carrot_Box_Item item_title = app.Create_item("item_title");
            item_title.set_icon(app.sp_icon_music);
            item_title.set_title(app.carrot.L("m_music", "Music"));
            item_title.set_tip(app.carrot.L("m_music_tip", "Online playlists are listed by respective countries"));

            Carrot_Box_Btn_Item btn_sort = item_title.create_item();
            btn_sort.set_icon(app.sp_icon_sort);
            btn_sort.set_color(app.carrot.color_highlight);
            btn_sort.set_icon_color(Color.white);
            btn_sort.set_act(() => Show_change_sort());

            Carrot_Box_Item item_sel_lang = app.Create_item("item_sel_lang");
            item_sel_lang.set_icon(app.carrot.lang.icon);
            item_sel_lang.set_title(app.carrot.L("m_music_country", "Listen to music by country"));
            item_sel_lang.set_tip(app.carrot.L("m_music_country_tip", "Show music list by country")+" ("+PlayerPrefs.GetString("lang_music", "en")+")");

            Carrot_Box_Btn_Item btn_list_lang = item_sel_lang.create_item();
            btn_list_lang.set_icon(this.app.carrot.icon_carrot_all_category);
            btn_list_lang.set_icon_color(Color.white);
            btn_list_lang.set_color(app.carrot.color_highlight);
            btn_list_lang.set_act(() => app.carrot.lang.Show_list_lang(this.Act_show_list_by_lang,false));
            item_sel_lang.set_act(()=> app.carrot.lang.Show_list_lang(this.Act_show_list_by_lang, false));

            Carrot_Box_Item item_reload_list = app.Create_item("item_reload_list");
            item_reload_list.set_icon(app.sp_icon_sync);
            item_reload_list.set_title(app.carrot.L("reload_list", "Reload list"));
            item_reload_list.set_tip(app.carrot.L("reload_list_tip", "Load latest online music list on a new page"));
            item_reload_list.set_act(() => this.Reload_list_page());

            this.list_data_play = new List<IDictionary>();
            int row_index = 0;
            int shown_count = 0;
            for (int i = 0; i < list_song.Count; i++)
            {
                IDictionary data_m = list_song[i] as IDictionary;
                if (!this.Is_match_lang(data_m, selected_lang)) continue;
                data_m["type"] = "music_online";
                data_m["index_play"] = this.list_data_play.Count;
                Carrot_Box_Item box_item = app.Create_item("item_m_" + i);
                box_item.set_icon(app.carrot.game.icon_play_music_game);
                box_item.set_title(data_m["name"].ToString());
                if (data_m["artist"] != null) box_item.set_tip(data_m["artist"].ToString());
                box_item.set_act(() => this.Play(data_m));

                if (row_index % 2 == 0)
                    box_item.GetComponent<Image>().color = app.color_row_1;
                else
                    box_item.GetComponent<Image>().color = app.color_row_2;
                row_index++;

                string s_id_avatar = "pic_avatar_" + data_m["id"].ToString();
                Sprite sp_pic_avatar = app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_avatar);
                if (sp_pic_avatar != null)
                    box_item.set_icon_white(sp_pic_avatar);
                else
                    app.carrot.get_img_and_save_playerPrefs(data_m["avatar"].ToString(), box_item.img_icon, s_id_avatar);

                Carrot_Box_Btn_Item btn_save = box_item.create_item();
                btn_save.set_icon(app.sp_icon_storage_save);
                btn_save.set_icon_color(Color.white);
                btn_save.set_color(app.carrot.color_highlight);
                btn_save.set_act(() =>
                {
                    this.Storage_item(data_m, btn_save.gameObject);
                });

                this.list_data_play.Add(data_m);
                app.Create_btn_add_play(box_item, data_m);
                shown_count++;
            }

            if (shown_count == 0)
            {
                Debug.LogWarning("Music_online: no songs for lang=" + selected_lang + ", total_source=" + list_song.Count);
                Carrot_Box_Item item_empty = app.Create_item("item_empty_lang");
                item_empty.set_icon(app.sp_icon_sad);
                item_empty.set_title(app.carrot.L("none_data", "No data"));
                item_empty.set_tip(app.carrot.L("m_music_country_tip", "Show music list by country") + " (" + selected_lang + ")");
            }
            else
            {
                Debug.Log("Music_online: loaded " + shown_count + " songs for lang=" + selected_lang + ", total_source=" + list_song.Count);
            }

            Carrot_Box_Item item_sort = app.Create_item("item_sort");
            item_sort.set_icon(app.sp_icon_sort);
            item_sort.set_title(app.carrot.L("sort_list", "Sort List"));
            item_sort.set_tip(app.carrot.L("sort_list_tip","Change the way the list is sorted according to different data types"));
            item_sort.set_act(() => Show_change_sort());
    }

    private void Act_show_list_by_lang(string key)
    {
        PlayerPrefs.SetString("lang_music", key);
        this.s_data_temp = PlayerPrefs.GetString(Get_cache_key(key), "");
        this.cached_lang = key;
        this.Show(key);
    }

    private void Reload_list_page()
    {
        app.carrot.play_sound_click();
        string lang = PlayerPrefs.GetString("lang_music", "en");
        this.s_data_temp = "";
        this.cached_lang = lang;
        PlayerPrefs.DeleteKey(Get_cache_key(lang));
        this.Get_data_from_server(lang);
    }

    private void Show_change_sort()
    {
        app.carrot.play_sound_click();

        this.box = app.carrot.Create_Box();
        this.box.set_icon(app.sp_icon_sort);
        this.box.set_title(app.carrot.L("sort_list","Sort List"));

        Carrot_Box_Item item_sort_name_asc = box.create_item("sort_name_a");
        item_sort_name_asc.set_icon(app.sp_icon_sort_name);
        item_sort_name_asc.set_title(app.carrot.L("sort_name","Sort by name"));
        item_sort_name_asc.set_tip(app.carrot.L("sort_name_a","Sort by name in descending order"));
        item_sort_name_asc.set_act(() =>
        {
            this.Act_change_sort("name", Query_Order_Direction.DESCENDING);
        });

        Carrot_Box_Item item_sort_name_desc = box.create_item("sort_name_z");
        item_sort_name_desc.set_icon(app.sp_icon_sort_name);
        item_sort_name_desc.set_title(app.carrot.L("sort_name", "Sort by name"));
        item_sort_name_desc.set_tip(app.carrot.L("sort_name_z", "Sort by name in ascending order"));
        item_sort_name_desc.set_act(() =>
        {
            this.Act_change_sort("name", Query_Order_Direction.ASCENDING);
        });

        Carrot_Box_Item sort_date_asc = box.create_item("sort_date_a");
        sort_date_asc.set_icon(app.sp_icon_sort_date);
        sort_date_asc.set_title(app.carrot.L("sort_date","Sort by date"));
        sort_date_asc.set_tip(app.carrot.L("sort_date_a", "Sort by date in descending order"));
        sort_date_asc.set_act(() =>
        {
            this.Act_change_sort("publishedAt", Query_Order_Direction.DESCENDING);
        });

        Carrot_Box_Item sort_date_desc = box.create_item("sort_date_z");
        sort_date_desc.set_icon(app.sp_icon_sort_date);
        sort_date_desc.set_title(app.carrot.L("sort_date", "Sort by date"));
        sort_date_desc.set_tip(app.carrot.L("sort_date_z", "Sort by date in ascending order"));
        sort_date_desc.set_act(() =>
        {
            this.Act_change_sort("publishedAt", Query_Order_Direction.ASCENDING);
        });
    }

    private void Act_change_sort(string key_sort, Query_Order_Direction direction)
    {
        app.carrot.play_sound_click();
        if (box != null) box.close();
        this.order_at = key_sort;
        this.order_type = direction;
        string lang = PlayerPrefs.GetString("lang_music", "en");
        this.s_data_temp = "";
        this.cached_lang = lang;
        PlayerPrefs.DeleteKey(Get_cache_key(lang));
        this.Show(lang);
    }

    private void Storage_item(IDictionary data, GameObject obj_btn_storage)
    {
        app.carrot.play_sound_click();
        Destroy(obj_btn_storage);
        data["type"] = "music_offline";
        app.playlist_offline.Add(data);
        app.carrot.Show_msg(app.carrot.L("playlist", "Playlist"), app.carrot.L("save_song_success", "Successfully stored, you can listen to the song again in the playlist"));
    }

    private void Play(IDictionary data)
    {
        app.player_music.Play_by_data(data);
        if (this.list_data_play.Count > 0) app.player_music.Set_list_music(this.list_data_play);
    }

    internal void Show_list_item_in_info(string s_type, string s_val, string s_lang)
    {
        app.carrot.show_loading();
        StructuredQuery q = new("song");
        q.Add_where(s_type, Query_OP.EQUAL, s_val);
        q.Add_where("lang", Query_OP.EQUAL, s_lang);
        app.carrot.hub.Get_doc(q.ToJson(), (data) =>
        {
            Fire_Collection fc = new Fire_Collection(data);
            if (!fc.is_null)
            {
                Debug.Log(data);
            }
        },app.Act_server_fail);
    }
}
