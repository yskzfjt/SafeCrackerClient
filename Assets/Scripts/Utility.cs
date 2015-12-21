using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Utility {
    public static void SetUIAlpha( GameObject go, float a ){
	Color col = go.GetComponent< Image >().color;
	col.a = a;
	go.GetComponent< Image >().color = col;
    }
    public static void SetUIGroupAlpha( GameObject go, float a ){
	go.GetComponent< CanvasGroup >().alpha = a;
    }

    public static float MinMax( float f, float min, float max ){
	if( f < min ) return min;
	if( f > max ) return max;
	return f;
    }
    public static float Rate( int num, int den ){
	return MinMax( num / (float)den, 0.0f, 1.0f );
    }
    public static float IRate( int num, int den ){
	return MinMax( 1.0f - num / (float)den, 0.0f, 1.0f );
    }



}
