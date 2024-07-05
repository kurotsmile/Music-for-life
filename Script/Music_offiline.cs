using Carrot;
using Crosstales.Common.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Music_offiline : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("Obj Offline")]
    public Sprite ps_icon_offline;
    int leng=0;

    private Carrot_Window_Input box_inp = null;
    private Carrot_Box box = null;
    private IDictionary data_cur = null;
    private List<IDictionary> list_data_play;

    public void On_Load()
    {
        this.leng = PlayerPrefs.GetInt("mo_length",0);
        this.list_data_play = new();
    }

    public void Add(IDictionary data, byte[] data_mp3)
    {
        data["index"] = leng;
        app.carrot.get_tool().save_file(this.leng+".data", data_mp3);
        this.Add(data);
    } 

    public void Add(IDictionary data)
    {
        data["index"] = leng;
        PlayerPrefs.SetString("mo_" + this.leng, Json.Serialize(data));
        this.leng++;
        PlayerPrefs.SetInt("mo_length", this.leng);
    }

    private void Update_data(int index,IDictionary data)
    {
        PlayerPrefs.SetString("mo_" + index, Json.Serialize(data));
    }

    public void Show()
    {
        if (box != null) box.close();
        if (box_inp != null) box_inp.close();
        this.GetComponent<App>().StopAllCoroutines();
        this.GetComponent<App>().clear_all_contain();

        Carrot_Box_Item item_title = app.Create_item("item_title_offline");
        item_title.set_icon(app.sp_icon_storage);
        item_title.set_title(app.carrot.L("playlist", "Playlist"));
        item_title.set_tip(app.carrot.L("playlist_tip", "Playlists you have stored for listening when not connected to the network"));

        Carrot_Box_Item item_item_song = app.Create_item("item_item_song");
        item_item_song.set_icon(app.sp_icon_import);
        item_item_song.set_title(app.carrot.L("add_song_sd", "Add songs from storage"));
        item_item_song.set_tip(app.carrot.L("add_song_sd_tip", "Add songs from storage or from files on your device"));
        item_item_song.set_act(() => Import_song_from_sd());

        Carrot_Box_Item item_item_folder = app.Create_item("item_item_folder");
        item_item_folder.set_icon(app.sp_icon_import_folder);
        item_item_folder.set_title(app.carrot.L("add_song_sd_folder", "Add folder songs from storage"));
        item_item_folder.set_tip(app.carrot.L("add_song_sd_folder_tip", "Add folder songs from storage or from files on your device"));
        item_item_folder.set_act(() => Import_song_from_sd_folder());

        Carrot_Box_Item item_add = app.Create_item("item_add");
        item_add.set_icon(app.carrot.icon_carrot_add);
        item_add.set_title(app.carrot.L("create_playlist", "Create a new list"));
        item_add.set_tip(app.carrot.L("create_playlist_tip", "Manage your songs in lists and folders"));
        item_add.set_act(() => Create_folder());

        List<IDictionary> list_item = this.get_list_all_type();
        for(int i = 0; i < list_item.Count; i++) this.Create_item(list_item[i]);

        Carrot_Box_Item item_backup = app.Create_item("item_backup");
        item_backup.set_icon(app.sp_icon_sync);
        item_backup.set_title(app.carrot.L("backup", "Backup"));
        item_backup.set_tip(app.carrot.L("backup_tip", "Backup and sync playlists to the cloud"));
        item_backup.set_act(() => Create_folder());
    }

    private List<IDictionary> get_list_all_type(string index_father = "")
    {
        List<IDictionary> list = new();
        if (this.leng > 0)
        {
            int count_item_valible = 0;
            this.list_data_play = new List<IDictionary>();
            for (int i = 0; i < this.leng; i++)
            {
                string s_data = PlayerPrefs.GetString("mo_" + i);
                if (s_data != "")
                {
                    IDictionary data_m = (IDictionary)Json.Deserialize(s_data);
                    
                    if (index_father == "")
                    {
                        if (data_m["father"] == null)
                        {
                            list.Add(data_m);
                            if (data_m["type"].ToString() != "folder")
                            {
                                data_m["index_play"] = count_item_valible.ToString();
                                this.list_data_play.Add(data_m);
                                count_item_valible++;
                            }
                        }
                    }
                    else
                    {
                        if (data_m["father"] != null)
                        {
                            if (data_m["father"].ToString() == index_father)
                            {
                                list.Add(data_m);
                                if (data_m["type"].ToString() != "folder")
                                {
                                    data_m["index_play"] = count_item_valible.ToString();
                                    this.list_data_play.Add(data_m);
                                    count_item_valible++;
                                }
                            }
                        }
                    }
                }
            }
        }
        return list;
    }

    private List<IDictionary> Get_list_folder()
    {
        List<IDictionary> list = new();
        if (this.leng > 0)
        {
            for (int i = 0; i < this.leng; i++)
            {
                string s_data = PlayerPrefs.GetString("mo_" + i);
                if (s_data != "")
                {
                    IDictionary data_m = (IDictionary)Json.Deserialize(s_data);
                    if (data_m["type"].ToString() == "folder") list.Add(data_m);
                }
            }
        }
        return list;
    }

    private Carrot_Box_Item Create_item(IDictionary data_m,bool is_list_main=true)
    {
        var index = int.Parse(data_m["index"].ToString());
        Carrot_Box_Item box_item = null;
        if(is_list_main)
            box_item=app.Create_item("mo_item_" + index);
        else
            box_item =this.box.create_item("mo_item_" + index);

        box_item.set_title(data_m["name"].ToString());
        if (data_m["artist"] != null) box_item.set_tip(data_m["artist"].ToString());

        Carrot_Box_Btn_Item btn_del = box_item.create_item();
        btn_del.set_icon(app.carrot.sp_icon_del_data);
        btn_del.set_icon_color(Color.white);
        btn_del.set_color(Color.red);
        btn_del.set_act(() => this.Delete(index));

        if (index % 2 == 0)
            box_item.GetComponent<Image>().color = app.color_row_1;
        else
            box_item.GetComponent<Image>().color = app.color_row_2;

        if (data_m["type"].ToString() == "folder")
        {
            box_item.set_tip(app.carrot.L("playlist", "Playlist"));
            box_item.set_icon(this.app.sp_icon_playlist);
            box_item.set_act(() => Show_all_item_in_folder(data_m));
        }

        if (data_m["type"].ToString() == "music_offline")
        {
            box_item.set_tip(app.carrot.L("m_music", "Music"));
            box_item.set_icon(this.app.sp_icon_music);

            if (data_m["id"] != null)
            {
                string s_id_avatar = "pic_avatar_" + data_m["id"].ToString();
                Sprite sp_pic_avatar = app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_avatar);
                if (sp_pic_avatar != null)
                    box_item.set_icon_white(sp_pic_avatar);
                else
                    if (data_m["avatar"] != null) app.carrot.get_img_and_save_playerPrefs(data_m["avatar"].ToString(), box_item.img_icon, s_id_avatar);
            }
            box_item.set_act(() => play_item_from_playlist(data_m));
            app.Create_btn_add_play(box_item);
        }

        if (data_m["type"].ToString() == "radio_offline")
        {
            box_item.set_tip(app.carrot.L("m_radio", "Radio"));
            box_item.set_icon(this.app.sp_icon_radio);

            if (data_m["id"] != null)
            {
                string s_id_avatar = "pic_avatar_" + data_m["id"].ToString();
                Sprite sp_pic_avatar = app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_avatar);
                if (sp_pic_avatar != null)
                    box_item.set_icon_white(sp_pic_avatar);
                else
                    if (data_m["avatar"] != null) app.carrot.get_img_and_save_playerPrefs(data_m["avatar"].ToString(), box_item.img_icon, s_id_avatar);
            }
            box_item.set_act(() => play_item_from_playlist(data_m));
            app.Create_btn_add_play(box_item);
        }

        if (data_m["type"].ToString() == "sound_offline")
        {
            box_item.set_tip(app.carrot.L("m_sound", "Sound"));
            box_item.set_icon(this.app.sp_icon_audio);
            box_item.set_act(() => play_item_from_playlist(data_m));
            app.Create_btn_add_play(box_item);
        }

        this.Create_btn_menu(box_item).set_act(() => this.Show_menu_folder(data_m));
        return box_item;
    }

    private Carrot_Box_Btn_Item Create_btn_menu(Carrot_Box_Item item)
    {
        Carrot_Box_Btn_Item btn_menu=item.create_item();
        btn_menu.set_icon(app.carrot.icon_carrot_all_category);
        btn_menu.set_icon_color(Color.white);
        btn_menu.set_color(app.carrot.color_highlight);
        return btn_menu;
    }

    private void Delete(int index)
    {
        app.carrot.Show_msg(app.carrot.L("delete","Delete song"),"Successfully deleted song from archive!",Msg_Icon.Success);
        PlayerPrefs.DeleteKey("mo_" + index);
        this.Show();
    }

    private void Create_folder()
    {
        app.carrot.play_sound_click();
        if (box_inp != null) box_inp.close();
        box_inp=app.carrot.Show_input(app.carrot.L("create_playlist", "Create a new list"), app.carrot.L("create_playlist_inp", "Enter the name of the list you want to create"));
        box_inp.set_act_done(Create_folder_done);
    }

    private void Create_folder_done(string s_name)
    {
        app.carrot.play_sound_click();
        IDictionary data_folder = (IDictionary)Json.Deserialize("{}");
        data_folder["name"]=s_name;
        data_folder["type"] = "folder";
        this.Add(data_folder);
        if (box_inp != null) box_inp.close();
        this.Show();
    }

    private void Show_menu_folder(IDictionary data)
    {
        this.data_cur = data;
        app.carrot.play_sound_click();
        if (box != null) this.box.close();
        this.box = app.carrot.Create_Box();
        this.box.set_icon(app.carrot.icon_carrot_advanced);
        this.box.set_title(app.carrot.L("menu","Menu")+" - " + data["name"].ToString());

        var index = int.Parse(data["index"].ToString());
        Carrot_Box_Item item_rename = box.create_item("item_rename");
        item_rename.set_icon(app.carrot.icon_carrot_write);
        item_rename.set_title("Rename");
        item_rename.set_tip("Change the name of this item");
        item_rename.set_act(() => Rename(data));

        if (data["artist"] != null)
        {
            string s_artist = data["artist"].ToString();
            Carrot_Box_Item item_edit_atrict = box.create_item("item_rename");
            item_edit_atrict.set_icon(app.sp_icon_artist);
            item_edit_atrict.set_title("Artist");
            if(s_artist!="")
                item_edit_atrict.set_tip(s_artist);
            else
                item_edit_atrict.set_tip("Change the Artist of this item");
            item_edit_atrict.set_act(() =>
            {
                this.box_inp = app.carrot.Show_input("Artist", "Change the Artist of this item", s_artist);
                box_inp.set_act_done((s_val) =>
                {
                    this.data_cur["artist"] = s_val;
                    int index = int.Parse(this.data_cur["index"].ToString());
                    this.Update_data(index, this.data_cur);
                    app.carrot.Show_msg("Update name item success!");
                    this.Show();
                });
            });
        }
        
        if (data["type"].ToString()=="music_offline"|| data["type"].ToString() == "radio_offline"|| data["type"].ToString() == "sound_offline")
        {
            Carrot_Box_Item item_move = box.create_item("item_move");
            item_move.set_icon(app.sp_icon_move);
            item_move.set_title("Move");
            item_move.set_tip("Move this item to another list");
            item_move.set_act(() => Show_move_playlist(data));
        }
 
        Carrot_Box_Item item_del = box.create_item("item_del");
        item_del.set_icon(app.carrot.sp_icon_del_data);
        item_del.set_title("Delete");
        item_del.set_tip("Remove this item from the list");
        item_del.set_act(() => this.Delete(index));
    }

    private void Rename(IDictionary data)
    {
        app.carrot.play_sound_click();
        this.data_cur = data;
        this.box_inp= app.carrot.Show_input("Rename", "Change the name of this item", data["name"].ToString());
        box_inp.set_act_done((s_name) =>
        {
            this.data_cur["name"] = s_name;
            int index = int.Parse(this.data_cur["index"].ToString());
            this.Update_data(index, this.data_cur);
            app.carrot.Show_msg("Update name item success!");
            this.Show();
        });
    }

    private void Show_move_playlist(IDictionary data)
    {
        if (box != null) box.close();
        this.box = this.app.carrot.Create_Box();
        this.box.set_icon(app.sp_icon_move);
        this.box.set_title("Move"+" - " + data["name"].ToString());

        List<IDictionary> list_item = this.Get_list_folder();
        for (int i = 0; i < list_item.Count; i++)
        {
            var data_item = list_item[i];
            Carrot_Box_Item item_folder=this.Create_item(list_item[i], false);
            item_folder.set_act(() =>
            {
                this.Act_move_item_to_playlist(data, data_item);
            });
        }
    }

    private void Act_move_item_to_playlist(IDictionary data_to,IDictionary data_from)
    {
        data_to["father"] = data_from["index"].ToString();
        this.Update_data(int.Parse(data_to["index"].ToString()),data_to);
        app.carrot.play_sound_click();
        app.carrot.Show_msg("Successfully moved to playlist","success");
        this.Show();
    }

    private void Show_all_item_in_folder(IDictionary data_folder)
    {
        app.clear_all_contain();

        Carrot_Box_Item item_title = app.Create_item("item_title_offline");
        item_title.set_icon(app.sp_icon_playlist);
        item_title.set_title(data_folder["name"].ToString());
        item_title.set_tip(app.carrot.L("playlist", "Playlist"));

        Carrot_Box_Item item_play = app.Create_item("item_play");
        item_play.set_icon(app.carrot.game.icon_play_music_game);
        item_play.set_title("Play");
        item_play.set_tip("Play all items in this list");
        item_play.set_act(() => play_all_item_in_playlist());

        Carrot_Box_Item item_back = app.Create_item("item_back");
        item_back.set_icon(app.sp_icon_back);
        item_back.set_title("Back");
        item_back.set_tip("Return to the original playlist");
        item_back.set_act(() => Show());


        List<IDictionary> list_item = this.get_list_all_type(data_folder["index"].ToString());
        for (int i = 0; i < list_item.Count; i++)
        {
            Carrot_Box_Item item_folder = this.Create_item(list_item[i]);
        }
    }

    private void play_item_from_playlist(IDictionary data)
    {
        app.player_music.Play_by_data(data);
        if (this.list_data_play.Count > 0) app.player_music.Set_list_music(this.list_data_play);
    }

    private void play_all_item_in_playlist()
    {
        app.carrot.play_sound_click();
        if (this.list_data_play.Count > 0)
        {
            app.player_music.Play_by_data(this.list_data_play[0]);
            app.player_music.Set_list_music(this.list_data_play);
        }
        else
        {
            app.carrot.Show_msg(app.carrot.L("title","Music For Life"),"There are no songs in this list yet, add songs here to start playing",Msg_Icon.Error);
            app.carrot.play_vibrate();
        }
    }

    private void Import_song_from_sd() 
    {
        this.app.carrot.play_sound_click();
        this.app.file.Set_filter(Carrot_File_Data.AudioData);
        this.app.file.Open_file(Import_song_done);
    }

    private void Import_song_from_sd_folder()
    {
        this.app.carrot.play_sound_click();
        this.app.file.Set_filter(Carrot_File_Data.AudioData);
        this.app.file.Open_folders(Import_song_folder_done);
    }

    private void Import_song_folder_done(string[] s_path)
    {
        foreach (var s_url_folder in s_path)
        {
            int index_folder = leng;
            IDictionary data_folder = (IDictionary)Json.Deserialize("{}");
            data_folder["name"] = FileHelper.GetDirectoryName(s_url_folder);
            data_folder["type"] = "folder";
            this.Add(data_folder);

            foreach (var s in FileHelper.GetFiles(s_url_folder))
            {
                string s_extension=FileHelper.GetExtension(s);
                if (s_extension == "mp3" || s_extension == "wav" || s_extension == "ogg")
                {
                    IDictionary data = this.Create_data_song(s);
                    data["father"] = index_folder;
                    data["type"] = "music_offline";
                    this.Add(data);
                }
            }
        }

        this.Show();
        this.app.carrot.Show_msg("Add Song", "Import " + s_path.Length + " folder success!");
    }

    private void Import_song_done(string[] s_path)
    {
        foreach (var s_url_file in s_path)
        {
            IDictionary data = this.Create_data_song(s_url_file);
            data["type"] = "music_offline";
            this.Add(data);
        }
        this.Show();
        this.app.carrot.Show_msg("Add Song","Import "+s_path.Length+" file success!");
    }

    private IDictionary Create_data_song(string s_url_mp3)
    {
        IDictionary data_song =(IDictionary) Json.Deserialize("{}");
        data_song["id"] = "song" + app.carrot.generateID();
        data_song["name"] = "Song "+app.carrot.generateID();
        data_song["mp3"] = s_url_mp3;
        data_song["artist"] = "None";
        data_song["year"] = DateTime.Now.ToString("yyyy");
        data_song["album"] = "None";
        data_song["lang"] = app.carrot.lang.Get_key_lang();
        data_song["avatar"] = "";
        data_song["link_ytb"] = "";
        data_song["genre"] = "";
        return data_song;
    }
}
