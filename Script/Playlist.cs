using Carrot;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Playlist : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    private string s_data_artist = "";
    private string s_data_genre = "";
    private string s_data_year = "";

    public void Show_List_Artist()
    {
        app.Create_loading();

        if (s_data_artist != "")
        {
            this.Load_list_artist(s_data_artist);
        }
        else
        {
            StartCoroutine(GetDataFromUrl(this.app.s_url_data_artist, (s_data) =>
            {
                this.s_data_artist = s_data;
                this.Load_list_artist(s_data);
            }));
        }
    }

    private void Load_list_artist(string s_data)
    {
        IDictionary data_json = (IDictionary)Json.Deserialize(s_data);
        IList list_artist = (IList)data_json["all_item"];
        app.clear_all_contain();

        Carrot.Carrot_Box_Item item_title = app.Create_item("title");
        item_title.set_icon(app.sp_icon_artist);
        item_title.set_title("Artist");
        item_title.set_tip("List of singers with songs in the system");

        for (int i = 0; i < list_artist.Count; i++)
        {
            IDictionary data_a = (IDictionary)list_artist[i];
            var s_name = data_a["name"].ToString();
            Carrot_Box_Item item_m = app.Create_item("item_artist");
            item_m.set_icon(app.sp_icon_singer);
            item_m.set_title(data_a["name"].ToString());
            item_m.set_tip(data_a["amount"].ToString() + " Song");
            item_m.set_act(() =>
            {
                this.Show_list_song_by("artist",s_name);
            });

            if (i % 2 == 0)
                item_m.GetComponent<Image>().color = app.color_row_1;
            else
                item_m.GetComponent<Image>().color = app.color_row_2;
        }
    }

    IEnumerator GetDataFromUrl(string url,UnityAction<string> act_done)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            act_done?.Invoke(json);
        }
    }

    public void Show_list_song_by(string field_name,string val_equal)
    {
        StructuredQuery q = app.playlist_online.Q_basic_song();
        q.Add_where("artist", Query_OP.EQUAL, val_equal);
        this.app.carrot.server.Get_doc(q.ToJson(), (s_data) =>
        {
            this.app.playlist_online.Load_list_by_data(s_data);
        }, app.Act_server_fail);
    }

    public void Show_List_Genre()
    {
        app.Create_loading();
    }

    public void Show_List_Year()
    {
        app.Create_loading();

        if (s_data_year != "")
        {
            this.Load_list_artist(s_data_artist);
        }
        else
        {
            StartCoroutine(GetDataFromUrl(this.app.s_url_data_artist, (s_data) =>
            {
                this.s_data_artist = s_data;
                this.Load_list_artist(s_data);
            }));
        }
    }

}
