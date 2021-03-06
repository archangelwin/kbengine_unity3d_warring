using UnityEngine;
using System.Collections;

public class loadingbar : MonoBehaviour {
	public static loadingbar inst = null;
	
	public static UISlider obj;
	UIAtlas atlasobj = null;
	public static UILabel label = null;
	
	public static int loadingbar_maxValue = 0;
	public static int loadingbar_currValue = 0;
	public static bool loadingbar_show = false;
	
	void Awake ()     
	{
		obj = GetComponent<UISlider>();

		inst = this;
		Common.DEBUG_MSG("loadingbar::Start(): sliderValue=" + obj.sliderValue);
		
		getLoadingbarAtlas();
		NGUITools.SetActive(obj.gameObject, false);
		
		label = UnityEngine.GameObject.Find("loadingdescr").GetComponent<UILabel>();
		NGUITools.SetActive(label.gameObject, false);
	}
	
	public static void reset(bool isstart)
	{
		loadingbar_maxValue = 0;
		loadingbar_currValue = 0;
		loadingbar_show = isstart;
		
		if(obj != null)
			obj.sliderValue = 1.0f;
	}
	
	// Use this for initialization
	void Start () {
	}
	
	void getLoadingbarAtlas(){
		Common.DEBUG_MSG("loadingbar::getLoadingbarAtlas: starting download loadingbar_atlas(" + Common.safe_url("/StreamingAssets/loadingbar_atlas.unity3d") + ")!");
		
		UIAtlas[] atlases = Resources.FindObjectsOfTypeAll(typeof(UIAtlas)) as UIAtlas[];

		for (int i = 0; i < atlases.Length;  ++i)
		{
			UIAtlas atlas = atlases[i];
			if(atlas.name == "loadingbar_atlas")
			{
				atlasobj = atlas;
				break;
			}
		}
		
		foreach(Transform child in obj.transform)
		{			
			UISprite sp = child.gameObject.GetComponent<UISprite>();
			if(sp == null)
				continue;
			
			if(sp.name == "Background")
				sp.spriteName = "progress_fill.jpg";
			else
				sp.spriteName = "progress_bk.jpg";
			
			sp.atlas = atlasobj;
			sp.MakePixelPerfect(); 
		}
			
		Vector3 slicedScale = obj.transform.localScale;
		slicedScale.y *= 0.5f;
		obj.transform.localScale = slicedScale;
		
		Vector3 pos = obj.transform.localPosition;
		float half = (float)(Screen.height / 2);
		pos.y = -(half - (half * 0.1f));
		
		UISpriteData sprite = atlasobj.GetSprite("progress_fill.jpg");

		half = (float)(sprite.width / 2);
		pos.x = -half;
		obj.transform.localPosition = pos;

		updateLabel();
			
		Common.DEBUG_MSG("loadingbar::getLoadingbarAtlas: download loadingbar_atlas is finished(pos=" + obj.transform.position + ", scale=" +
			obj.transform.localScale + ")!");
	}

	// Update is called once per frame
	void Update () 
	{
		if(loadingbar.loadingbar_show == true)
		{
			enable();
			
			obj.sliderValue = 1.0f - ((float)loadingbar_currValue / (float)loadingbar_maxValue);
			updateLabel();
			
			if(obj.sliderValue < 0.01)
			{
				obj.sliderValue = 0.0f;
				updateLabel();
				disable();
			}
		}
	}
	
	void updateLabel()
	{
		Vector3 pos1 = obj.transform.position;
		pos1.z = -1.0f;
		obj.transform.position = pos1;
		
		pos1 = obj.transform.localPosition;
		pos1.z = -1.0f;
		obj.transform.localPosition = pos1;

		if(label != null)
		{
			label.text = loadingbar_currValue + "/" + loadingbar_maxValue + " " + (int)((1.0f - obj.sliderValue) * 100.0f) + "%";
			
			Vector3 pos = obj.transform.position;
			pos.z = -2.0f;
			label.transform.position = pos;
			
			pos = label.transform.localPosition;
			pos.x = 0.0f;
			pos.z = -2.0f;
			label.transform.localPosition = pos;
		}
	}
	
	public void enable()
	{
		if(loadingbar_show == false || obj.sliderValue != 1.0f)
		{
			Common.DEBUG_MSG("loadingbar::enable: is failed, sliderValue=" + obj.sliderValue + ", loadingbar_show=" + loadingbar_show);	
			return;
		}
	
		Common.DEBUG_MSG("loadingbar::enable(" + loader.inst.currentSceneName + "): sliderValue=" + obj.sliderValue + ",loadingbar_Value" + loadingbar_currValue + 
			"/" + loadingbar_maxValue);
		
		NGUITools.SetActive(loadingbar_backgroundpic.obj.gameObject, true);
		NGUITools.SetActive(obj.gameObject, true);
		NGUITools.SetActive(label.gameObject, true);
		obj.sliderValue = 1.0f; 
		
		loadingbar_backgroundpic.inst.startFadeOut = false;
		Color c = loadingbar_backgroundpic.obj.color;
		c.a = 1.0f;
		loadingbar_backgroundpic.obj.color = c;
	}

	public void disable()
	{
		if(loadingbar_show == false)
			return;
		
		loadingbar_backgroundpic.inst.fadeout_close();
		
		Common.DEBUG_MSG("loadingbar::disable: sliderValue=" + obj.sliderValue);		
		loadingbar_show = false;
		obj.sliderValue = 0.0f;
	}
}
