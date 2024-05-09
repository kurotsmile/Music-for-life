using UnityEngine;
using UnityEngine.UI;

public class App_shop : MonoBehaviour
{
    public Sprite[] p_icon;
    public string[] p_name;
    public string[] p_name_en;
    public string[] p_tip;
    public string[] p_tip_en;
    public string[] p_key_check_buy;
    public int[] p_index_buy;

    public void show_list_shop()
    {
        this.GetComponent<App>().clear_all_contain();

        for (int i = 0; i < p_name.Length; i++)
        {
            /*
            GameObject p_create = Instantiate(this.prefab_item_shop);
            p_create.transform.SetParent(this.GetComponent<App>().canvas_render.transform);
            p_create.GetComponent<Item_shop_app>().txt_name.text = PlayerPrefs.GetString(this.p_name[i], this.p_name_en[i]);
            p_create.GetComponent<Item_shop_app>().txt_tip.text = PlayerPrefs.GetString(this.p_tip[i], this.p_tip_en[i]);
            p_create.GetComponent<Item_shop_app>().img_icon.sprite = this.p_icon[i];
            p_create.GetComponent<Item_shop_app>().index_p = this.p_index_buy[i];
            p_create.name = "item_shop";
            p_create.transform.localPosition = new Vector3(p_create.transform.localPosition.x, p_create.transform.localPosition.y, 0f);
            p_create.transform.localRotation = Quaternion.Euler(Vector3.zero);
            p_create.transform.localScale = new Vector3(1f, 1f, 1f);
            if (i % 2 == 0)
                p_create.GetComponent<Image>().color=this.GetComponent<App>().color_row_1;
            else
                p_create.GetComponent<Image>().color=this.GetComponent<App>().color_row_2;
            p_create.GetComponent<Item_shop_app>().icon_check_buy.SetActive(false);
            if (p_index_buy[i] != -1)
            {
                if (PlayerPrefs.GetInt(this.p_key_check_buy[i], 0) != 0)
                {
                    p_create.GetComponent<Item_shop_app>().icon_check_buy.SetActive(true);
                    p_create.GetComponent<Item_shop_app>().index_p = -2;
                }
            }
            */
        }
    }
}
