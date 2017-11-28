using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardInGame : MonoBehaviour {



    public GameObject wizardPlayer;
    public GameObject wizardAnimation;
    public float trackTimer;





    void Start() {
        updatetimer();

        //get wizard
        //even snel gepakt
        //wizard = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/WizardUI", new Vector2(0f, 0f), new Vector2(0f, 0f).gamebject;
        //wizardAnimController = wizard.GetComponent<Animator>();
    }

    void Update() {
        wizardPlayer.GetComponent<Animation>().Play();
    }

    void updatetimer()
    {
        trackTimer += Time.deltaTime;
    }

    void CastSpell()
    {
        //when spell is casted, track all the stuff that is necessary to place the wizard (need to make a list of all interfering stuff)
        //place the wizard on a good spot (wizard start pos)
        //move the wizard to the new spot
        //do wizardy stuff
        //popout *for now
        //track the time the spell is taking so it can be made easier
    }



    //Vector2[] wizardEndPos = new Vector2[4];
    //wizardEndPos[0] = new Vector3(0.0f,0.0f,0.0f);
    //positionArray[1] = new Vector3(0.1f,0.1f,0.1f);
    //positionArray[2] = new Vector3(0.2f,0.3f,0.4f);
    //positionArray[3] = new Vector3(0.5f,0.5f,0.5f);

    //Vector2[] wizardStartPos = new[] {new Vector2(0f, 0f),
    //                                new Vector2(2f, 4f),
    //                                new Vector2(5f, 6f),
    //                                new Vector2(9f, 9f),
    //                                new Vector2(1f, 1f)};

    //Vector2[] wizardEndPos = new[] {new Vector2(0f, 0f),
    //                                new Vector2(1f, 1f),
    //                                new Vector2(1f, 1f),
    //                                new Vector2(1f, 1f),
    //                                new Vector2(1f, 1f)};

    //private IEnumerator spellCasting(float waitInBetween)
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(waitInBetween);

    //    }
    //}
}
