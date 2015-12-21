using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NumberDisplayScript : MonoBehaviour {

    public int digitPos;
    public Sprite noneSprite;
    public Sprite[] hitSprites = new Sprite[ 10 ];
    public Sprite[] failSprites = new Sprite[ 10 ];
    public Sprite[] inputSprites = new Sprite[ 10 ];

    private int number = 0;
    public enum TYPE{ HIT, FAIL, INPUT, NONE };
    private TYPE type = TYPE.NONE;

    // Use this for initialization
    void Start () {
	SetNone();
    }
	
    // Update is called once per frame
    void Update () {
    }

    ////////////////////////////////////////////////////////////////////
    //public 
    ////////////////////////////////////////////////////////////////////
    public TYPE Type(){ return type; }
    public int Number(){ return number; }
    public void SetHit( int no ){
	if( !RangeCheck( no ) ) return;
	GetComponent< Image >().sprite = hitSprites[ no ];
	type = TYPE.HIT;
	number = no;
    }
    public void SetFail( int no ){
	if( !RangeCheck( no ) ) return;
	GetComponent< Image >().sprite = failSprites[ no ];
	type = TYPE.FAIL;
	number = no;
    }
    public void SetInput( int no ){
	if( !RangeCheck( no ) ) return;
	GetComponent< Image >().sprite = inputSprites[ no ];
	type = TYPE.INPUT;
	number = no;
    }
    public void SetNone(){
	GetComponent< Image >().sprite = noneSprite;
	type = TYPE.NONE;
	number = -1;
    }

    ////////////////////////////////////////////////////////////////////
    //private
    ////////////////////////////////////////////////////////////////////
    bool RangeCheck( int no ){
	return ( no >= 0 && no <10 );
    }

}
