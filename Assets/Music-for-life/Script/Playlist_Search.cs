using Carrot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Playlist_Search : MonoBehaviour
{
    public App app;
    private Carrot_Window_Input box_input;
    private List<IDictionary> list_data_play = new();
    private string keyword = "";
    private int current_page = 1;
    private const int page_limit = 20;

    public void Show()
    {
        this.app.carrot.play_sound_click();
        this.box_input = this.app.carrot.show_search(Act_done, app.carrot.L("search_tip", "You can search for song title, singer, genre or radio channel by search keyword."));
    }

    private void Act_done(string s_key)
    {
        if (this.box_input != null) this.box_input.close();
        this.app.carrot.play_sound_click();
        this.keyword = s_key == null ? "" : s_key.Trim();
        this.current_page = 1;

        if (this.keyword == "")
        {
            this.app.carrot.Show_msg(this.app.carrot.L("search", "Search"), this.app.carrot.L("search_empty", "Please enter a keyword"));
            return;
        }

        this.Search_song_worker(this.current_page, true);
    }

    private void Search_song_worker(int page, bool clearOld)
    {
        if (clearOld) this.app.Create_loading();
        StartCoroutine(Search_song_worker_session(page, clearOld));
    }

    private IEnumerator Search_song_worker_session(int page, bool clearOld)
    {
        string lang = PlayerPrefs.GetString("lang_music", PlayerPrefs.GetString("lang", "en"));
        string userId = this.app.carrot.user.get_id_user_login();
        if (string.IsNullOrEmpty(userId)) userId = SystemInfo.deviceUniqueIdentifier;
        string log = page <= 1 ? "1" : "0";

        string url = this.app.carrot.hub.SERVER_WORKER_PUBLIC + "/search_song"
            + "?q=" + UnityWebRequest.EscapeURL(this.keyword)
            + "&lang=" + UnityWebRequest.EscapeURL(lang)
            + "&page=" + page
            + "&limit=" + page_limit
            + "&userId=" + UnityWebRequest.EscapeURL(userId)
            + "&log=" + log;

        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            this.current_page = page;
            this.Load_search_result(req.downloadHandler.text, clearOld, page);
        }
        else
        {
            if (clearOld) this.app.clear_all_contain();
            this.app.Act_server_fail(req.downloadHandler != null ? req.downloadHandler.text : req.error);
        }
    }

    private void Load_search_result(string s_data, bool clearOld, int page)
    {
        IList list_song = Json.Deserialize(s_data) as IList;
        if (list_song == null) list_song = new List<object>();

        if (clearOld)
        {
            this.app.clear_all_contain();
            this.list_data_play = new List<IDictionary>();
        }

        Carrot_Box_Item item_title = this.app.Create_item("title_search");
        item_title.set_icon(this.app.carrot.icon_carrot_search);
        item_title.set_title(this.app.carrot.L("search_results", "Search Results"));
        item_title.set_tip(list_song.Count + " " + this.app.carrot.L("song", "Song") + " - page " + page);

        int row_index = 0;
        for (int i = 0; i < list_song.Count; i++)
        {
            IDictionary data_m = list_song[i] as IDictionary;
            if (data_m == null) continue;

            data_m["type"] = "music_online";
            data_m["index_play"] = this.list_data_play.Count;

            Carrot_Box_Item box_item = this.app.Create_item("item_search_" + page + "_" + i);
            box_item.set_icon(this.app.carrot.game.icon_play_music_game);
            box_item.set_title(data_m["name"] != null ? data_m["name"].ToString() : "Unknown");
            if (data_m["artist"] != null) box_item.set_tip(data_m["artist"].ToString());
            box_item.set_act(() => this.Play(data_m));

            if (row_index % 2 == 0)
                box_item.GetComponent<Image>().color = this.app.color_row_1;
            else
                box_item.GetComponent<Image>().color = this.app.color_row_2;
            row_index++;

            if (data_m["id"] != null && data_m["avatar"] != null)
            {
                string s_id_avatar = "pic_avatar_" + data_m["id"].ToString();
                Sprite sp_pic_avatar = this.app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_avatar);
                if (sp_pic_avatar != null)
                    box_item.set_icon_white(sp_pic_avatar);
                else
                    this.app.carrot.get_img_and_save_playerPrefs(data_m["avatar"].ToString(), box_item.img_icon, s_id_avatar);
            }

            Carrot_Box_Btn_Item btn_save = box_item.create_item();
            btn_save.set_icon(this.app.sp_icon_storage_save);
            btn_save.set_icon_color(Color.white);
            btn_save.set_color(this.app.carrot.color_highlight);
            btn_save.set_act(() => this.Storage_item(data_m, btn_save.gameObject));

            this.list_data_play.Add(data_m);
            this.app.Create_btn_add_play(box_item, data_m);
        }

        if (list_song.Count == 0)
        {
            Carrot_Box_Item item_none = this.app.Create_item("search_none");
            item_none.set_icon(this.app.sp_icon_sad);
            item_none.set_title(this.app.carrot.L("none_data", "No data"));
            item_none.set_tip(this.app.carrot.L("search_empty", "No matching songs found"));
            return;
        }

        if (list_song.Count >= page_limit)
        {
            Carrot_Box_Item item_next = this.app.Create_item("search_next_page");
            item_next.set_icon(this.app.sp_icon_sync);
            item_next.set_title(this.app.carrot.L("next_page", "Next page"));
            item_next.set_tip(this.app.carrot.L("next_page_tip", "Load more search results"));
            item_next.set_act(() => this.Search_song_worker(this.current_page + 1, true));
        }
    }

    private void Storage_item(IDictionary data, GameObject obj_btn_storage)
    {
        this.app.carrot.play_sound_click();
        Destroy(obj_btn_storage);
        data["type"] = "music_offline";
        this.app.playlist_offline.Add(data);
        this.app.carrot.Show_msg(this.app.carrot.L("playlist", "Playlist"), this.app.carrot.L("save_song_success", "Successfully stored, you can listen to the song again in the playlist"));
    }

    private void Play(IDictionary data)
    {
        this.app.player_music.Play_by_data(data);
        if (this.list_data_play.Count > 0) this.app.player_music.Set_list_music(this.list_data_play);
    }
}
