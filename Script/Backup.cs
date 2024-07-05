using Carrot;
using UnityEngine;

public class Backup : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;
    private Carrot_Box box;

    public void Show()
    {
        app.carrot.play_sound_click();
        this.box = app.carrot.Create_Box();
        this.box.set_icon(app.sp_icon_sync);
        this.box.set_title(app.carrot.L("backup", "Backup"));
    }
}
