﻿using Carrot;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum Playlist_Type {artist,genre,year,album}

public class Playlist : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    private string s_data_artist = "";
    private string s_data_genre = "";
    private string s_data_year = "";
    private string s_data_album = "";

    private Playlist_Type type= Playlist_Type.artist;

    public void Show_List_Artist()
    {
        this.type = Playlist_Type.artist;
        app.Create_loading();

        if (s_data_artist != "")
        {
            this.Load_list_by_meta(s_data_artist);
        }
        else
        {
            this.app.carrot.show_loading();
            this.app.carrot.Get_Data(this.app.carrot.random(app.list_url_data_artist),(s_data) =>
            {
                this.app.carrot.hide_loading();
                this.s_data_artist = s_data;
                this.Load_list_by_meta(s_data);
            }, this.Show_List_Artist);
        }
    }

    public void Show_List_Genre()
    {
        this.type = Playlist_Type.genre;
        app.Create_loading();

        if (this.s_data_genre != "")
        {
            this.Load_list_by_meta(s_data_genre);
        }
        else
        {
            this.app.carrot.show_loading();
            this.app.carrot.Get_Data(this.app.carrot.random(this.app.list_url_data_genre),(s_data) =>
            {
                this.app.carrot.hide_loading();
                this.s_data_genre = s_data;
                this.Load_list_by_meta(s_data);
            },this.Show_List_Genre);
        }
    }

    public void Show_List_Year()
    {
        this.type = Playlist_Type.year;
        app.Create_loading();
        if (this.s_data_year != "")
        {
            this.Load_list_by_meta(s_data_year);
        }
        else
        {
            this.app.carrot.show_loading();
            this.app.carrot.Get_Data(this.app.carrot.random(this.app.list_url_data_year), (s_data) =>
            {
                this.app.carrot.hide_loading();
                this.s_data_year = s_data;
                this.Load_list_by_meta(s_data);
            },this.Show_List_Year);
        }
    }

    public void Show_List_Album()
    {
        this.type = Playlist_Type.album;
        app.Create_loading();
        if (this.s_data_album != "")
        {
            this.Load_list_by_meta(this.s_data_album);
        }
        else
        {
            this.app.carrot.Get_Data(this.app.carrot.random(this.app.list_url_data_album), (s_data) =>
            {
                this.s_data_album = s_data;
                this.Load_list_by_meta(s_data);
            },this.Show_List_Album);
        }
    }


    public void Show_list_song_by(string field_name, string val_equal)
    {
        app.carrot.show_loading();
        StructuredQuery q = app.playlist_online.Q_basic_song();
        q.Add_where(field_name, Query_OP.EQUAL, val_equal);
        this.app.carrot.server.Get_doc(q.ToJson(), (s_data) =>
        {
            app.carrot.hide_loading();
            this.app.playlist_online.Load_list_by_data(s_data);
        }, (s_err)=>
        {
            app.carrot.hide_loading();
            app.Act_server_fail(s_err);
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
            item_title.set_tip(this.app.carrot.L("artist_tip","List of singers with songs in the system"));
        }

        if (this.type == Playlist_Type.genre)
        {
            item_title.set_icon(app.sp_icon_genre);
            item_title.set_title(this.app.carrot.L("genre", "Genre"));
            item_title.set_tip(this.app.carrot.L("genre_tip","List of genre with songs in the system"));
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
            item_title.set_title(this.app.carrot.L("year","Year"));
            item_title.set_tip(this.app.carrot.L("year_tip","List of year with songs in the system"));
        }

        for (int i = 0; i < list_artist.Count; i++)
        {
            IDictionary data_a = (IDictionary)list_artist[i];
            if (data_a["lang"].ToString() != app.carrot.lang.Get_key_lang()) continue;
            Carrot_Box_Item item_m = this.box_item(data_a, this.type.ToString());
            if (i % 2 == 0)
                item_m.GetComponent<Image>().color = app.color_row_1;
            else
                item_m.GetComponent<Image>().color = app.color_row_2;
        }
    }

    private Carrot_Box_Item box_item(IDictionary data_item,string s_type)
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
        if (this.s_data_artist != "")
        {
            this.Load_item_seach_by_metaData(this.s_data_artist, s_key_seach, "artist");
        }
        else
        {
            this.app.carrot.Get_Data(this.app.carrot.random(this.app.list_url_data_artist), (s_data) =>
            {
                this.s_data_artist = s_data;
                this.Load_item_seach_by_metaData(s_data, s_key_seach, "artist");
            });
        }

        if (this.s_data_album != "")
        {
            this.Load_item_seach_by_metaData(this.s_data_album, s_key_seach, "album");
        }
        else
        {
            this.app.carrot.Get_Data(this.app.carrot.random(this.app.list_url_data_album), (s_data) =>
            {
                this.s_data_album = s_data;
                this.Load_item_seach_by_metaData(s_data, s_key_seach, "album");
            });
        }

        if (this.s_data_genre != "")
        {
            this.Load_item_seach_by_metaData(this.s_data_genre, s_key_seach, "genre");
        }
        else
        {
            this.app.carrot.Get_Data(this.app.carrot.random(this.app.list_url_data_genre), (s_data) =>
            {
                this.s_data_genre = s_data;
                this.Load_item_seach_by_metaData(s_data, s_key_seach, "genre");
            });
        }

        if (this.s_data_year != "")
        {
            this.Load_item_seach_by_metaData(this.s_data_year, s_key_seach, "year");
        }
        else
        {
            this.app.carrot.Get_Data(this.app.carrot.random(this.app.list_url_data_year), (s_data) =>
            {
                this.s_data_year = s_data;
                this.Load_item_seach_by_metaData(s_data, s_key_seach, "year");
            });
        }
        app.carrot.delay_function(2f,app.Update_row_color);
    }

    private void Load_item_seach_by_metaData(string s_data,string s_key,string s_type)
    {
        IDictionary data_artists = Json.Deserialize(s_data) as IDictionary;
        IList list_data = data_artists["all_item"] as IList;
        for (int i = 0; i < list_data.Count; i++)
        {
            IDictionary data_item = list_data[i] as IDictionary;
            if (data_item["name"].ToString().IndexOf(s_key, StringComparison.OrdinalIgnoreCase) >=0) box_item(data_item, s_type);
        }
    }
}
