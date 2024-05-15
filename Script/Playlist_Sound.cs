using Carrot;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Playlist_Sound : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    private string s_data_temp = "";

    public void Show()
    {
        app.Create_loading();
        if(s_data_temp=="")
            this.Get_data_list_sound();
        else
            this.Load_list_by_data(s_data_temp);
    }

    private void Get_data_list_sound()
    {
        StructuredQuery q = new("audio");
        q.Set_limit(50);
        app.carrot.server.Get_doc(q.ToJson(), (data) =>
        {
            this.s_data_temp = data;
            this.Load_list_by_data(data);
        });
    }

    private void Load_list_by_data(string s_data)
    {
        app.clear_all_contain();
        Carrot_Box_Item item_title = app.Create_item("title");
        item_title.set_icon(app.sp_icon_sound);
        item_title.set_title(app.carrot.L("m_sound", "Sound"));
        item_title.set_tip(app.carrot.L("m_sound_tip", "Playlists without words, you can save offline to listen when there is no network connection"));

        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            for(int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data_sound = fc.fire_document[i].Get_IDictionary();
                data_sound["type"] = "sound_online";
                data_sound["index"] = i;
                Carrot_Box_Item item_sound = app.Create_item("item_sound_" + i);
                item_sound.set_icon(app.sp_icon_audio);
                item_sound.set_title(data_sound["name"].ToString());
                item_sound.set_tip(data_sound["author"].ToString());

                if (i % 2 == 0)
                    item_sound.GetComponent<Image>().color = app.color_row_1;
                else
                    item_sound.GetComponent<Image>().color = app.color_row_2;

                item_sound.set_act(() => app.player_music.Play_by_data(data_sound));

                Carrot_Box_Btn_Item btn_add_playlist = item_sound.create_item();
                btn_add_playlist.set_icon(app.sp_icon_storage_save);
                btn_add_playlist.set_icon_color(Color.white);
                btn_add_playlist.set_color(app.carrot.color_highlight);
                btn_add_playlist.set_act(() =>
                {
                    this.Storage_item(data_sound, btn_add_playlist.gameObject);
                });
            }
        }
        else
        {
            app.Create_list_none();
        }
    }

    private void Storage_item(IDictionary data, GameObject obj_btn_storage)
    {
        app.carrot.play_sound_click();
        Destroy(obj_btn_storage);
        data["type"] = "sound_offline";
        app.playlist_offline.Add(data);
        app.carrot.Show_msg(app.carrot.L("playlist", "Playlist"), app.carrot.L("save_song_success", "Successfully stored, you can listen to the song again in the playlist"));
    }
}
