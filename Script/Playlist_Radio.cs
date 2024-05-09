using Carrot;
using System.Collections;
using UnityEngine;

public class Playlist_Radio : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    public void show()
    {
        StructuredQuery q = new("radio");
        app.carrot.server.Get_doc(q.ToString(),this.Act_get_done);
    }

    void Act_get_done(string s_data)
    {
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            for(int i=0;i<fc.fire_document.Length;i++)
            {
                IDictionary data_radio = fc.fire_document[i].Get_IDictionary();
                data_radio["type"] = "1";
                Carrot_Box_Item box_item = app.Create_item("item_radio_" + i);
                box_item.set_title(data_radio["name"].ToString());
                box_item.set_tip(data_radio["url"].ToString());
                box_item.set_act(() => app.player_music.Play_by_data(data_radio));
            }
        }
    }

}
