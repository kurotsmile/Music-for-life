using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class App_shop : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;

    [Header("Data Obj")]
    public Sprite[] p_icon;
    public string[] p_name;
    public string[] p_name_en;
    public string[] p_tip;
    public string[] p_tip_en;
    public string[] p_key_check_buy;
    public int[] p_index_buy;

    public void Show()
    {
        this.app.clear_all_contain();

        Carrot_Box_Item item_title = app.Create_item("shop_title");
        item_title.set_icon(app.sp_icon_shop);
        item_title.set_title(app.carrot.L("shop", "Shop"));
        item_title.set_tip(app.carrot.L("shop_tip", "Purchase and use in-app functions"));

        for (int i = 0; i < p_name.Length; i++)
        {
            Carrot_Box_Item item_shop=app.Create_item("item_shop_" + i);
            item_shop.set_title(app.carrot.L(this.p_name[i], this.p_name_en[i]));
            item_shop.set_tip(app.carrot.L(this.p_tip[i], this.p_tip_en[i]));
            if (i % 2 == 0)
                item_shop.GetComponent<Image>().color = app.color_row_1;
            else
                item_shop.GetComponent<Image>().color = app.color_row_2;

            if (p_index_buy[i] != -1)
            {
                if (PlayerPrefs.GetInt(this.p_key_check_buy[i], 0) != 0)
                {
                    Carrot_Box_Btn_Item btn_buy = item_shop.create_item();
                    btn_buy.set_icon(app.carrot.icon_carrot_buy);
                    btn_buy.set_icon_color(Color.white);
                    btn_buy.set_color(app.carrot.color_highlight);
                    Destroy(btn_buy.GetComponent<Button>());
                }
            }
        }
    }
}
