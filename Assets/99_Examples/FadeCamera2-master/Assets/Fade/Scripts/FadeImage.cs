/*
 The MIT License (MIT)

Copyright (c) 2013 yamamura tatsuhiko

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeImage : UnityEngine.UI.Graphic, IFade
{
    public enum ImageType
    {
        FADEOUT, FADEIN
    }

    [SerializeField] private Texture fadeOutTexture = null;
    [SerializeField] private Texture fadeInTexture = null;
    [SerializeField, Range(0, 1)] private float cutoutRange;

    public float Range
    {
        get
        {
            return cutoutRange;
        }
        set
        {
            cutoutRange = value;
            UpdateMaskCutout(cutoutRange);
        }
    }

    protected override void Start()
    {
        base.Start();
        UpdateMask(ImageType.FADEOUT);
    }

    private void UpdateMaskCutout(float range)
    {
        enabled = true;
        material.SetFloat("_Range", 1 - range);

        if (range <= 0)
        {
            this.enabled = false;
        }
    }

    public void UpdateMask(ImageType type)
    {
        switch (type)
        {
            case ImageType.FADEOUT:
                material.SetTexture("_MaskTex", fadeOutTexture);
                material.SetColor("_Color", color);
                break;
            case ImageType.FADEIN:
                material.SetTexture("_MaskTex", fadeInTexture);
                material.SetColor("_Color", color);
                break;
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        UpdateMaskCutout(Range);
        UpdateMask(ImageType.FADEOUT);
    }
#endif
}
