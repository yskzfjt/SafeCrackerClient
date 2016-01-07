using UnityEngine;
using System.Collections;

public class DummyServer{
//     public string httpResponse;

//     public class RNG {
// 	static readonly float denom = 1000;
// 	static readonly float [] numerators = {
// 	    0.100f,		//初回の確率
// 	    0.111f,
// 	    0.125f,
// 	    0.142f,
// 	    0.166f,
// 	    0.200f,
// 	    0.250f,
// 	    0.333f,
// 	    0.500f,
// 	    1.0f
// 	};
// 	static public bool IsHit( int tryID ){
// 	    float thr = numerators[ tryID ] * denom;
// 	    float val = UnityEngine.Random.value * denom;
// // 	    Debug.Log("TEST");
// // 	    Debug.Log(thr);
// // 	    Debug.Log(val);
// // 	    Debug.Log(tryNo);
// 	    return (bool)( thr >= val );
// 	}
//     }


//     public DummyServer( string url, string name, string xml ){
// 	SystemScript sys = GameObject.Find( "SystemObject" ).GetComponent<SystemScript>();
// 	GameInfo info = sys.Info;

// 	if( url.Contains(GameInfo.StaticInfoURL) ){
// 	    //そのまんま返す。
// 	    httpResponse = xml;
// 	}else{
// 	    switch( (int)info.CurPhase ){
// 	    case (int)GameInfo.PHASE.NEW_GAME:{
// 		if( info.GameID == 0 ){
// 		    info.CreditBalance = (int)(UnityEngine.Random.Range(0,5000)) + 10000;
// 		}
// 		info.NewDynamicInfo();
// 		info.RequestPhase( GameInfo.PHASE.SELECT );
// 	    }break;
// 	    case (int)GameInfo.PHASE.TRY:{
// 		info.CurCrack = 0;
// 		for( int i=0; i<info.MaxDigits; ++i ){
// 		    if( info.IsCrackedDigitAt( i ) ){
// 			//あたってる。
// 		    }else if( info.IsUniqueDigitAt( i ) && RNG.IsHit( info.TryID ) ){
// 			info.CrackDigitAt( i );
// 			++info.CurCrack;
// 		    }
// 		}
// 		info.CreditBalance -= info.Bet;
// 		if( info.IsWin() ){
// 		    info.CreditBalance += info.Reward;
// 		    info.RequestPhase( GameInfo.PHASE.WIN );
// 		}else if( info.IsLost() ){
// 		    info.RequestPhase( GameInfo.PHASE.LOST );
// 		}else{
// 		    info.RequestPhase( GameInfo.PHASE.RESULT );
// 		}
// 	    }break;
// 	    case (int)GameInfo.PHASE.GIVE_UP:{
// 		info.NewDynamicInfo();
// 		info.RequestPhase( GameInfo.PHASE.SELECT );
// 	    }break;
// 	    }

// 	    httpResponse = info.SerializeDynamicInfo();
// 	}
//     }

}
