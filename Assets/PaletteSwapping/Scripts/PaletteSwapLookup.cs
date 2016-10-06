using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class PaletteSwapLookup : MonoBehaviour
{
	public Texture[] LookupTexture;
    //public Sprite[] paletteSprites;
    [SerializeField]
    int paletteIndex;
	Material _mat;

	void OnEnable()
	{
		Shader shader = Shader.Find("Hidden/PaletteSwapLookup");
		if (_mat == null)
			_mat = new Material(shader);
    }

    public void SetPaletteIndex(int upDown,Text textComp)
    {
        paletteIndex -= upDown;
        if (paletteIndex > LookupTexture.Length - 1)
            paletteIndex = 0;

        if (paletteIndex < 0)
            paletteIndex = LookupTexture.Length - 1;

        if (paletteIndex == 0)
            textComp.text = "0. Original";
        else
            textComp.text = '\n' +""+ paletteIndex + ". " + LookupTexture[paletteIndex].name;
    }

	void OnDisable()
	{
		if (_mat != null)
			DestroyImmediate(_mat);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        //Sprite spr = paletteSprites[paletteIndex];
        /*Texture2D newTexture = new Texture2D((int)spr.rect.width, (int)spr.rect.height);
        Color[] c = spr.texture.GetPixels(
            (int)spr.textureRect.x,
            (int)spr.textureRect.y,
            (int)spr.textureRect.width,
            (int)spr.textureRect.height);

        newTexture.SetPixels(c);
        newTexture.Apply();*/
        if (paletteIndex != 0)
        {
            _mat.SetTexture("_PaletteTex", LookupTexture[paletteIndex]);
            Graphics.Blit(src, dst, _mat);
        }
        else
            this.enabled = false;
	}

}
