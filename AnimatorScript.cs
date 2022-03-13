using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorScript : MonoBehaviour
{
    // Start is called before the first frame update
    Animator anim;
    public Button btn;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        btn.onClick.AddListener(Play);
    }

    public void Play()
    {
        anim.SetInteger("AnimIdx", Random.Range(0,4));
        anim.SetTrigger("AnimTrig");
    }
}
