using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Playlist : MonoBehaviour
{
    public Sprite icon;
    public GameObject prefab_item_playlist;
    private bool is_show_in_account = false;
    private string s_data_curent = "";
    private string s_id_curent = "";
    private string s_name_current = "";
    public GameObject prefab_new_playlist;

    [Header("New Or Changer Playlist")]
    public Text txt_title_new_playlist;
    public Text txt_tip_new_playlist;
    public InputField inp_new_playlist;
    public GameObject panel_new_playlist;

    private string id_music_add = "";
    private string lang_music_add = "";
    private bool is_change_name_playlist = false;

    private bool is_show_main = false;
    private Carrot.Carrot_Box box_Playlist;

    public void show_list()
    {
        this.lang_music_add = "";
        this.id_music_add = "";
        this.is_show_in_account = true;
        this.is_show_main = false;

        /*
        WWWForm frm = GameObject.Find("App").GetComponent<App>().carrot.frm_act("show_list_playlist");
        if (this.GetComponent<App>().carrot.user.get_id_user_login() != "")
        {
            frm.AddField("user_id", this.GetComponent<App>().carrot.user.get_id_user_login());
            frm.AddField("user_lang", this.GetComponent<App>().carrot.user.get_lang_user_login());
        }
        GameObject.Find("App").GetComponent<App>().carrot.send(frm, act_get_playlist_handle);
        */
    }

    public void show_list_on_main()
    {
        this.lang_music_add = "";
        this.id_music_add = "";
        this.is_show_in_account = false;
        this.is_show_main = true;
        /*
        WWWForm frm = this.GetComponent<App>().carrot.frm_act("show_list_playlist");
        if (this.GetComponent<App>().carrot.user.get_id_user_login() != "")
        {
            frm.AddField("user_id", this.GetComponent<App>().carrot.user.get_id_user_login());
            frm.AddField("user_lang", this.GetComponent<App>().carrot.user.get_lang_user_login());
        }
       this.GetComponent<App>().carrot.send(frm, act_get_playlist_handle);
        */
    }

    private void act_get_playlist_handle(string s_data)
    {
        IList list_data = (IList)Carrot.Json.Deserialize(s_data);
        if (this.is_show_main)
        {
            this.GetComponent<App>().title_name.text = PlayerPrefs.GetString("account_playlist", "Account Playlist");
            this.GetComponent<App>().title_tip.text = PlayerPrefs.GetString("account_playlist_tip", "Your playlist, you can manage and use this playlist on carrotstore.com");
            this.GetComponent<App>().clear_all_contain();
        }
        else
        {
            this.box_Playlist=this.GetComponent<App>().carrot.Create_Box(PlayerPrefs.GetString("account_playlist", "Account Playlist"), this.icon);
        }

        foreach (IDictionary playlist in list_data)
        {
            GameObject p = Instantiate(this.prefab_item_playlist);
            p.name = "item_music";
            p.GetComponent<Item_playlist>().txt_name.text = playlist["name"].ToString();
            p.GetComponent<Item_playlist>().s_data = playlist["desc"].ToString();
            p.GetComponent<Item_playlist>().id_playlist = playlist["id"].ToString();
            p.GetComponent<Item_playlist>().txt_length_song.text = playlist["length"].ToString();
            if (this.lang_music_add == "")
                p.GetComponent<Item_playlist>().type = 0;
            else
                p.GetComponent<Item_playlist>().type = 1;

            if (this.is_show_main)
                p.transform.SetParent(this.GetComponent<App>().canvas_render.transform);
            else
                this.box_Playlist.add_item(this.prefab_item_playlist);

            if (this.is_show_main)
            {
                p.GetComponent<Item_playlist>().button_delete.SetActive(true);
                p.GetComponent<Item_playlist>().button_edit.SetActive(true);
                p.GetComponent<Item_playlist>().button_share.SetActive(true);
            }
            else
            {
                p.GetComponent<Item_playlist>().button_delete.SetActive(false);
                p.GetComponent<Item_playlist>().button_edit.SetActive(false);
                p.GetComponent<Item_playlist>().button_share.SetActive(false);
            }
            p.transform.localPosition = new Vector3(p.transform.localPosition.x, p.transform.localPosition.y, 0f);
            p.transform.localRotation = Quaternion.Euler(Vector3.zero);
            p.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        GameObject p_create = Instantiate(this.prefab_new_playlist);
        if (this.is_show_main)
            p_create.transform.SetParent(this.GetComponent<App>().canvas_render.transform);
        else
            this.box_Playlist.add_item(p_create);
        p_create.GetComponent<Item_playlist>().txt_name.text = PlayerPrefs.GetString("create_playlist", "Create Playlist");
        p_create.name = "item_music";
        p_create.transform.localPosition = new Vector3(p_create.transform.localPosition.x, p_create.transform.localPosition.y, 0f);
        p_create.transform.localRotation = Quaternion.Euler(Vector3.zero);
        p_create.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void add_prefab_item_playlist(int index, IDictionary item_music, GameObject prefab, Transform area_body)
    {
        if (item_music != null)
        {
            GameObject Obj_item_music = Instantiate(prefab);
            Obj_item_music.name = "item_music";
            Obj_item_music.transform.SetParent(area_body);

            Obj_item_music.GetComponent<Panel_item_music>().btn_statu_play.SetActive(false);
            Obj_item_music.GetComponent<Panel_item_music>().btn_delete.SetActive(true);
            Obj_item_music.GetComponent<Panel_item_music>().txt_name.text = item_music["chat"].ToString();
            Obj_item_music.GetComponent<Panel_item_music>().url = item_music["file_url"].ToString();
            Obj_item_music.GetComponent<Panel_item_music>().s_color = item_music["color"].ToString();
            Obj_item_music.GetComponent<Panel_item_music>().lang = item_music["author"].ToString();
            Obj_item_music.GetComponent<Panel_item_music>().type = 3;
            Obj_item_music.GetComponent<Panel_item_music>().id_m = item_music["id"].ToString();
            Obj_item_music.GetComponent<Panel_item_music>().index = index;
            Obj_item_music.transform.localPosition = new Vector3(Obj_item_music.transform.localPosition.x, Obj_item_music.transform.localPosition.y, 0f);
            Obj_item_music.transform.localRotation = Quaternion.Euler(Vector3.zero);
            Obj_item_music.transform.localScale = new Vector3(1f, 1f, 1f);
            this.GetComponent<App>().list_music.Add(Obj_item_music.GetComponent<Panel_item_music>());
        }
    }

    public void show_playlist_by_id(string id_playlist,string s_data,string title_playlist)
    {
        this.s_name_current = title_playlist;
        this.s_id_curent = id_playlist;
        this.GetComponent<App>().clear_all_contain();
        this.GetComponent<App>().list_music = new List<Panel_item_music>();
        this.GetComponent<App>().panel_main_select_country.SetActive(false);
        this.GetComponent<App>().title_name.text = title_playlist;
        this.GetComponent<App>().title_icon.sprite = this.icon;
        this.GetComponent<App>().title_tip.text = PlayerPrefs.GetString("account_playlist_tip", "Your playlist, you can manage and use this playlist on carrotstore.com");
        IList list_data = (IList)Carrot.Json.Deserialize(s_data);
        for(int i=0;i<list_data.Count;i++)
        {
            IDictionary item_music = (IDictionary)list_data[i];
            this.add_prefab_item_playlist(i, item_music, this.GetComponent<App>().prefab_music_item_list, this.GetComponent<App>().canvas_render.transform);
        }
        this.s_data_curent = s_data;
    }


    public void delete_playlist(string s_id)
    {
        /*
        WWWForm frm = GameObject.Find("App").GetComponent<App>().carrot.frm_act("delete_playlist");
        frm.AddField("id", s_id);
        GameObject.Find("App").GetComponent<App>().carrot.send(frm, this.act_delete_handle);
        */
    }

    private void act_delete_handle(string s_data)
    {
        if (this.is_show_in_account)
            this.show_list();
        else
            this.show_list_on_main();
    }

    public void edit_playlist(string s_id,string s_name)
    {
        this.s_id_curent = s_id;
        this.is_change_name_playlist = true;
        this.inp_new_playlist.text = s_name;
        this.txt_title_new_playlist.text = PlayerPrefs.GetString("rename_playlist", "Rename Playlist");
        this.panel_new_playlist.SetActive(true);
    }

    public void show_new_playlist()
    {
        this.is_change_name_playlist = false;
        this.txt_title_new_playlist.text = PlayerPrefs.GetString("create_playlist", "Create Playlist");
        this.panel_new_playlist.SetActive(true);
        this.GetComponent<App>().carrot.close();
    }

    public void delete_song_in_playlist(string s_id,string s_lang)
    {
        /*
        WWWForm frm = GameObject.Find("App").GetComponent<App>().carrot.frm_act("delete_song_in_playlist");
        frm.AddField("id_delete", s_id);
        frm.AddField("lang_delete", s_lang);
        frm.AddField("id_playlist", this.s_id_curent);
        frm.AddField("data", this.s_data_curent);
        GameObject.Find("App").GetComponent<App>().carrot.send(frm,this.act_delete_song);
        */
    }

    private void act_delete_song(string s_data)
    {
        this.s_data_curent = s_data;
        show_playlist_by_id(this.s_id_curent, this.s_data_curent, this.s_name_current);
        Debug.Log("Data After delete:" + this.s_data_curent);
    }

    private void act_create_playlist_handle(string s_data)
    {
        if (this.is_show_in_account)
            this.show_list();
        else
            this.show_list_on_main();
        this.panel_new_playlist.SetActive(false);
    }

    public void show_add_song_to_playlist(string id,string lang)
    {
        this.id_music_add = id;
        this.lang_music_add = lang;
        this.is_show_in_account = false;
        this.is_show_main = false;
        /*
        WWWForm frm = GameObject.Find("App").GetComponent<App>().carrot.frm_act("show_list_playlist");
        if (this.GetComponent<App>().carrot.user.get_id_user_login() != "")
        {
            frm.AddField("user_id", this.GetComponent<App>().carrot.user.get_id_user_login());
            frm.AddField("user_lang", this.GetComponent<App>().carrot.user.get_lang_user_login());
        }
        GameObject.Find("App").GetComponent<App>().carrot.send(frm, this.act_get_playlist_handle);
        */
    }

    public void add_song_playlist(string id_playlist,string s_data)
    {
        /*
        WWWForm frm = GameObject.Find("App").GetComponent<App>().carrot.frm_act("add_song_playlist");
        frm.AddField("id_music_add", this.id_music_add);
        frm.AddField("lang_music_add", this.lang_music_add);
        frm.AddField("id_playlist", id_playlist);
        frm.AddField("data_playlist", s_data);
        GameObject.Find("App").GetComponent<App>().carrot.send(frm,this.act_add_song);
        */
    }

    private void act_add_song(string s_data){
        this.GetComponent<App>().carrot.Show_msg(PlayerPrefs.GetString("account_playlist", "Playlist"), PlayerPrefs.GetString("playlist_add_song_success", "Added song to playlist!"));
    }

    public void done_box_create_and_update_playlist()
    {
        if (this.inp_new_playlist.text.Trim() != "")
        {
            if (this.is_change_name_playlist)
            {
                /*
                WWWForm frm = GameObject.Find("App").GetComponent<App>().carrot.frm_act("update_name_playlist");
                frm.AddField("name_playlist", this.inp_new_playlist.text);
                frm.AddField("id_playlist", this.s_id_curent);
                GameObject.Find("App").GetComponent<App>().carrot.send(frm, this.act_update_playlist_name_handle);
                */
            }
            else
            {
                /*
                WWWForm frm = GameObject.Find("App").GetComponent<App>().carrot.frm_act("create_playlist");
                frm.AddField("name_playlist", this.inp_new_playlist.text);
                if (this.GetComponent<App>().carrot.user.get_id_user_login() != "")
                {
                    frm.AddField("user_id", this.GetComponent<App>().carrot.user.get_id_user_login());
                    frm.AddField("user_lang", this.GetComponent<App>().carrot.user.get_lang_user_login());
                }
                GameObject.Find("App").GetComponent<App>().carrot.send(frm, this.act_create_playlist_handle);
                */
            }
        }
    }

    private void act_update_playlist_name_handle(string s_data)
    {
        if (this.is_show_in_account)
            this.show_list();
        else
            this.show_list_on_main();
        this.panel_new_playlist.SetActive(false);
    }

}
