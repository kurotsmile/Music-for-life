using Carrot;
using System.Collections;
using UnityEngine;

public class Music_offiline : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("Obj Offline")]
    public Sprite ps_icon_offline;
    int leng=0;

    public void On_Load()
    {
        this.leng = PlayerPrefs.GetInt("mo_length",0);
    }

    public void Add(IDictionary data)
    {
        PlayerPrefs.SetString("mo_"+this.leng,Json.Serialize(data));
        this.leng++;
        PlayerPrefs.SetInt("mo_length", this.leng);

    }

    public void show_list_music_data()
    {
        this.GetComponent<App>().StopAllCoroutines();
        this.GetComponent<App>().clear_all_contain();
        if (this.leng > 0)
        {
            int index_m = 0;
            for(int i = 0; i < this.leng; i++)
            {
                string s_data = PlayerPrefs.GetString("mo_" + i);
                if (s_data!= "")
                {
                    IDictionary data_m = (IDictionary) Json.Deserialize(s_data);
                    Carrot_Box_Item box_item=app.Create_item("mo_item_" + i);
                    data_m["index"] = index_m;
                    box_item.set_title(data_m["name"].ToString());
                    box_item.set_tip(data_m["artist"].ToString());
                }
            }
        }
    }

    public void delete(int index)
    {
        PlayerPrefs.DeleteKey("mo_" + index);
    }

}
