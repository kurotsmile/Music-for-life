using Carrot;
using UnityEngine;

public class Music_offiline : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("Obj Offline")]
    public Sprite ps_icon_offline;
    int leng=0;

    void Start()
    {
        this.leng = PlayerPrefs.GetInt("mfl_leng");
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
                if (PlayerPrefs.GetString("mfl_name_"+i) != "")
                {
                    Carrot_Box_Item box_offline_item=app.Create_item("item_offline_" + i);
                }
            }
        }
    }

    public void delete(int index)
    {
        PlayerPrefs.DeleteKey("mfl_name_" + index);
        PlayerPrefs.DeleteKey("mfl_color_" + index);
        PlayerPrefs.DeleteKey("mfl_url_" + index);
        PlayerPrefs.DeleteKey("mfl_lyrics_" + index);
        PlayerPrefs.DeleteKey("mfl_link_" + index);
        PlayerPrefs.DeleteKey("mfl_ytb_" + index);

        this.GetComponent<App>().carrot.get_tool().delete_file("mfl_" + index + ".data");
        this.GetComponent<App>().carrot.get_tool().delete_file("mfl_" + index + ".png");
        this.show_list_music_data();
    }

}
