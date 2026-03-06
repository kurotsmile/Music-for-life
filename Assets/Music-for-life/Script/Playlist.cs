using Carrot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum Playlist_Type { artist, genre, year, album }

public class Playlist : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    private string s_data_artist = "";
    private string s_data_genre = "";
    private string s_data_year = "";
    private string s_data_album = "";

    private Playlist_Type type = Playlist_Type.artist;

    public void Show_List_Artist(string error = null)
    {
        this.type = Playlist_Type.artist;
        app.Create_loading();
        this.Load_or_fetch_meta(Playlist_Type.artist, this.Show_List_Artist);
    }

    public void Show_List_Genre(string error_null = "")
    {
        this.type = Playlist_Type.genre;
        app.Create_loading();
        this.Load_or_fetch_meta(Playlist_Type.genre, this.Show_List_Genre);
    }

    public void Show_List_Year(string error = null)
    {
        this.type = Playlist_Type.year;
        app.Create_loading();
        this.Load_or_fetch_meta(Playlist_Type.year, this.Show_List_Year);
    }

    public void Show_List_Album(string error = null)
    {
        this.type = Playlist_Type.album;
        app.Create_loading();
        this.Load_or_fetch_meta(Playlist_Type.album, this.Show_List_Album);
    }

    public void Show_list_song_by(string field_name, string val_equal)
    {
        string lang = PlayerPrefs.GetString("lang_music", this.app.carrot.lang.Get_key_lang());
        string url = this.app.carrot.hub.SERVER_WORKER_PUBLIC + "/list_song"
            + "?page=1"
            + "&limit=-1"
            + "&lang=" + UnityWebRequest.EscapeURL(lang)
            + "&key=" + UnityWebRequest.EscapeURL(field_name)
            + "&value=" + UnityWebRequest.EscapeURL(val_equal);

        this.app.carrot.Get(url, (s_data) =>
        {
            this.app.playlist_online.Load_list_by_data(s_data);
        }, (s_err) =>
        {
            this.app.Act_server_fail(s_err);
        });
    }

    private void Load_list_by_meta(string s_data)
    {
        IDictionary data_json = (IDictionary)Json.Deserialize(s_data);
        IList list_artist = (IList)data_json["all_item"];

        app.clear_all_contain();

        Carrot.Carrot_Box_Item item_title = app.Create_item("title");
        if (this.type == Playlist_Type.artist)
        {
            item_title.set_icon(app.sp_icon_artist);
            item_title.set_title(this.app.carrot.L("artist", "Artist"));
            item_title.set_tip(this.app.carrot.L("artist_tip", "List of singers with songs in the system"));
        }

        if (this.type == Playlist_Type.genre)
        {
            item_title.set_icon(app.sp_icon_genre);
            item_title.set_title(this.app.carrot.L("genre", "Genre"));
            item_title.set_tip(this.app.carrot.L("genre_tip", "List of genre with songs in the system"));
        }

        if (this.type == Playlist_Type.album)
        {
            item_title.set_icon(app.sp_icon_album);
            item_title.set_title(this.app.carrot.L("album", "Album"));
            item_title.set_tip(this.app.carrot.L("album_tip", "List of songs in the album"));
        }

        if (this.type == Playlist_Type.year)
        {
            item_title.set_icon(app.sp_icon_year);
            item_title.set_title(this.app.carrot.L("year", "Year"));
            item_title.set_tip(this.app.carrot.L("year_tip", "List of year with songs in the system"));
        }

        string lang_music = PlayerPrefs.GetString("lang_music", app.carrot.lang.Get_key_lang());
        for (int i = 0; i < list_artist.Count; i++)
        {
            IDictionary data_a = (IDictionary)list_artist[i];
            if (data_a["lang"].ToString() != lang_music) continue;
            Carrot_Box_Item item_m = this.box_item(data_a, this.type.ToString());
            if (i % 2 == 0)
                item_m.GetComponent<Image>().color = app.color_row_1;
            else
                item_m.GetComponent<Image>().color = app.color_row_2;
        }
    }

    private Carrot_Box_Item box_item(IDictionary data_item, string s_type)
    {
        string s_name = data_item["name"].ToString();
        Carrot_Box_Item item_m = app.Create_item("item_p");
        if (s_type == "artist") item_m.set_icon(app.sp_icon_singer);
        if (s_type == "genre") item_m.set_icon(app.sp_icon_genre_item);
        if (s_type == "year") item_m.set_icon(app.sp_icon_date);
        if (s_type == "album") item_m.set_icon(app.sp_icon_playlist);
        item_m.set_title(data_item["name"].ToString());
        if (data_item["amount"] != null) item_m.set_tip(data_item["amount"].ToString() + " " + this.app.carrot.L("song", "Song"));
        item_m.set_act(() =>
        {
            if (s_type == "artist") this.Show_list_song_by("artist", s_name);
            if (s_type == "genre") this.Show_list_song_by("genre", s_name);
            if (s_type == "year") this.Show_list_song_by("year", s_name);
            if (s_type == "album") this.Show_list_song_by("album", s_name);
        });
        return item_m;
    }

    public void Search(string s_key_seach)
    {
        this.app.clear_all_contain();
        this.Ensure_all_meta_loaded(() =>
        {
            this.Load_item_seach_by_metaData(this.s_data_artist, s_key_seach, "artist");
            this.Load_item_seach_by_metaData(this.s_data_album, s_key_seach, "album");
            this.Load_item_seach_by_metaData(this.s_data_genre, s_key_seach, "genre");
            this.Load_item_seach_by_metaData(this.s_data_year, s_key_seach, "year");
            this.app.carrot.delay_function(0.1f, this.app.Update_row_color);
        });
    }

    private void Load_or_fetch_meta(Playlist_Type meta_type, Action<string> on_fail)
    {
        string s_data = this.Get_cache_by_type(meta_type);
        if (!string.IsNullOrEmpty(s_data))
        {
            this.Load_list_by_meta(s_data);
            return;
        }

        this.Fetch_meta_from_worker(meta_type, (meta_json) =>
        {
            this.Set_cache_by_type(meta_type, meta_json);
            this.Load_list_by_meta(meta_json);
        }, (s_err) =>
        {
            if (on_fail != null) on_fail(s_err);
            else this.app.Act_server_fail(s_err);
        });
    }

    private void Ensure_all_meta_loaded(Action act_done)
    {
        this.Ensure_meta_loaded(Playlist_Type.artist, () =>
        {
            this.Ensure_meta_loaded(Playlist_Type.album, () =>
            {
                this.Ensure_meta_loaded(Playlist_Type.genre, () =>
                {
                    this.Ensure_meta_loaded(Playlist_Type.year, act_done);
                });
            });
        });
    }

    private void Ensure_meta_loaded(Playlist_Type meta_type, Action act_done)
    {
        string s_data = this.Get_cache_by_type(meta_type);
        if (!string.IsNullOrEmpty(s_data))
        {
            act_done?.Invoke();
            return;
        }

        this.Fetch_meta_from_worker(meta_type, (meta_json) =>
        {
            this.Set_cache_by_type(meta_type, meta_json);
            act_done?.Invoke();
        }, _ =>
        {
            act_done?.Invoke();
        });
    }

    private string Get_cache_by_type(Playlist_Type meta_type)
    {
        if (meta_type == Playlist_Type.artist) return this.s_data_artist;
        if (meta_type == Playlist_Type.genre) return this.s_data_genre;
        if (meta_type == Playlist_Type.album) return this.s_data_album;
        if (meta_type == Playlist_Type.year) return this.s_data_year;
        return "";
    }

    private void Set_cache_by_type(Playlist_Type meta_type, string s_data)
    {
        if (meta_type == Playlist_Type.artist) this.s_data_artist = s_data;
        if (meta_type == Playlist_Type.genre) this.s_data_genre = s_data;
        if (meta_type == Playlist_Type.album) this.s_data_album = s_data;
        if (meta_type == Playlist_Type.year) this.s_data_year = s_data;
    }

    private void Fetch_meta_from_worker(Playlist_Type meta_type, Action<string> act_done, Action<string> act_fail)
    {
        string lang = PlayerPrefs.GetString("lang_music", this.app.carrot.lang.Get_key_lang());
        string url = this.app.carrot.hub.SERVER_WORKER_PUBLIC + "/list_song"
            + "?page=1"
            + "&limit=-1"
            + "&lang=" + UnityWebRequest.EscapeURL(lang);

        this.app.carrot.Get(url, (s_data) =>
        {
            string s_meta_json = this.Build_meta_json_from_song_data(s_data, meta_type, lang);
            act_done?.Invoke(s_meta_json);
        }, (s_err) =>
        {
            act_fail?.Invoke(s_err);
        });
    }

    private string Build_meta_json_from_song_data(string s_song_data, Playlist_Type meta_type, string lang)
    {
        IList list_song = Json.Deserialize(s_song_data) as IList;
        if (list_song == null) list_song = new List<object>();

        Dictionary<string, int> map_amount = new(StringComparer.OrdinalIgnoreCase);
        string field_key = meta_type.ToString();
        for (int i = 0; i < list_song.Count; i++)
        {
            IDictionary song_data = list_song[i] as IDictionary;
            if (song_data == null || song_data[field_key] == null) continue;
            string val = song_data[field_key].ToString().Trim();
            if (string.IsNullOrEmpty(val)) continue;

            if (map_amount.ContainsKey(val))
                map_amount[val]++;
            else
                map_amount[val] = 1;
        }

        List<string> list_key = new(map_amount.Keys);
        list_key.Sort(StringComparer.OrdinalIgnoreCase);

        IList list_meta = Json.Deserialize("[]") as IList;
        for (int i = 0; i < list_key.Count; i++)
        {
            string key = list_key[i];
            IDictionary item = Json.Deserialize("{}") as IDictionary;
            item["name"] = key;
            item["amount"] = map_amount[key].ToString();
            item["lang"] = lang;
            list_meta.Add(item);
        }

        IDictionary data_wrap = Json.Deserialize("{}") as IDictionary;
        data_wrap["all_item"] = list_meta;
        return Json.Serialize(data_wrap);
    }

    private void Load_item_seach_by_metaData(string s_data, string s_key, string s_type)
    {
        if (string.IsNullOrEmpty(s_data)) return;
        IDictionary data_artists = Json.Deserialize(s_data) as IDictionary;
        if (data_artists == null || data_artists["all_item"] == null) return;

        IList list_data = data_artists["all_item"] as IList;
        if (list_data == null) return;

        for (int i = 0; i < list_data.Count; i++)
        {
            IDictionary data_item = list_data[i] as IDictionary;
            if (data_item == null || data_item["name"] == null) continue;
            if (data_item["name"].ToString().IndexOf(s_key, StringComparison.OrdinalIgnoreCase) >= 0) box_item(data_item, s_type);
        }
    }
}
