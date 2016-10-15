using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class PaletteSwapLookup : MonoBehaviour
{
	public Texture[] LookupTexture;
    //public Sprite[] paletteSprites;
    [SerializeField]
    int paletteIndex=6;
	Material _mat;
    [SerializeField]
    Shader swappingShader;
	void OnEnable()
    {
        if (PlayerPrefs.HasKey("Palette"))
            paletteIndex = PlayerPrefs.GetInt("Palette");
        if (_mat == null)
			_mat = new Material(swappingShader);
    }

    public void SetPaletteIndex(int upDown,Text textComp)
    {
        paletteIndex -= upDown;
        if (paletteIndex > LookupTexture.Length - 1)
            paletteIndex = 0;

        if (paletteIndex < 0)
            paletteIndex = LookupTexture.Length - 1;

        /*if (paletteIndex == 0)
            textComp.text = "0. Original";
        else*/
            textComp.text = '\n' +""+ paletteIndex + ". " + LookupTexture[paletteIndex].name;

        PlayerPrefs.SetInt("Palette",paletteIndex);
    }

	void OnDisable()
	{
		if (_mat != null)
			DestroyImmediate(_mat);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        _mat.SetTexture("_PaletteTex", LookupTexture[paletteIndex]);
        Graphics.Blit(src, dst, _mat);
    }

}
