using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_shop_app : MonoBehaviour
{
    public Image img_icon;
    public Text txt_name;
    public Text txt_tip;
    public int index_p;
    public GameObject icon_check_buy;
    public void click_item()
    {
        if (index_p == -2) return;
        else if (index_p == -1) GameObject.Find("App").GetComponent<App>().restore_product();
        else GameObject.Find("App").GetComponent<App>().buy_product(this.index_p);
    }
}
