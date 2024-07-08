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

    [Header("Asset Data")]
    public String s_url_data_artist;

    public void Show_List_Artist()
    {
        app.Create_loading();
        StartCoroutine(GetDataFromUrl(s_url_data_artist, (s_data) =>
        {
            IDictionary data_json=(IDictionary) Json.Deserialize(s_data);
            IList list_artist = (IList)data_json["all_item"];
            app.clear_all_contain();

            Carrot.Carrot_Box_Item item_title = app.Create_item("title");
            item_title.set_icon(app.sp_icon_artist);
            item_title.set_title("Artist");
            item_title.set_tip("List of singers with songs in the system");

            for(int i = 0;i < list_artist.Count; i++)
            {
                IDictionary data_a= (IDictionary) list_artist[i];
                Carrot_Box_Item item_m=app.Create_item("item_artist");
                item_m.set_icon(app.sp_icon_singer);
                item_m.set_title(data_a["name"].ToString());
                item_m.set_tip(data_a["amount"].ToString()+" Song");

                if (i % 2 == 0)
                    item_m.GetComponent<Image>().color = app.color_row_1;
                else
                    item_m.GetComponent<Image>().color = app.color_row_2;
            }
        }));
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
}
