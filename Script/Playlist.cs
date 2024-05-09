using Carrot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Playlist : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("UI")]
    public Sprite icon;
    public GameObject prefab_item_playlist;
    private bool is_show_in_account = false;
    public GameObject prefab_new_playlist;

    [Header("New Or Changer Playlist")]
    public Text txt_title_new_playlist;
    public Text txt_tip_new_playlist;
    public InputField inp_new_playlist;
    public GameObject panel_new_playlist;

    private bool is_change_name_playlist = false;

    private bool is_show_main = false;
    private Carrot.Carrot_Box box_Playlist;

    public void show_list()
    {
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
            this.GetComponent<App>().clear_all_contain();
        }
        else
        {
            this.box_Playlist = app.carrot.Create_Box(PlayerPrefs.GetString("account_playlist", "Account Playlist"), this.icon);
        }

        GameObject p_create = Instantiate(this.prefab_new_playlist);
        if (this.is_show_main)
            p_create.transform.SetParent(this.GetComponent<App>().canvas_render.transform);
        else
            this.box_Playlist.add_item(p_create);

        p_create.name = "item_music";
        p_create.transform.localPosition = new Vector3(p_create.transform.localPosition.x, p_create.transform.localPosition.y, 0f);
        p_create.transform.localRotation = Quaternion.Euler(Vector3.zero);
        p_create.transform.localScale = new Vector3(1f, 1f, 1f);
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

    public void show_add_song_to_playlist(string id,string lang)
    {
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
