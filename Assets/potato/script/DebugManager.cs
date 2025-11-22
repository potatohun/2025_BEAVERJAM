using UnityEngine;
using UnityEngine.SceneManagement;
public class DebugManager : MonoBehaviour
{
    public PortalController portalController_1;
    public PortalController portalController_2;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene("Intro");
        }
    }

    public void OnClickPortal1()
    {
        portalController_1.Teleport();
    }

    public void OnClickPortal2()
    {
        portalController_2.Teleport();
    }

    public void OnClickUnlockSkillCoco()
    {
        FriendManager.FM.UnlockSkill(FriendManager.CharacterSkill.Coco);
    }

    public void OnClickUnlockSkillToto()
    {
        FriendManager.FM.UnlockSkill(FriendManager.CharacterSkill.Toto);
    }

    public void OnClickUnlockSkillGalilei()
    {
        FriendManager.FM.UnlockSkill(FriendManager.CharacterSkill.Galilei);
    }

    public void OnClickUnlockSkillMiu()
    {
        FriendManager.FM.UnlockSkill(FriendManager.CharacterSkill.Miu);
    }
}
