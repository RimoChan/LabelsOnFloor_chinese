using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    public struct CharBoundsInTexture
    {
        public float Left, Right;
    }

    public class FontHandler
    {
        private float _charWidthAsTexturePortion = -1f;

        private Material _material;

        public bool IsFontLoaded()
        {
            if (Resources.Font == null)
                return false;

            if (_charWidthAsTexturePortion < 0f)
                _charWidthAsTexturePortion =  35f / Resources.Font.width;

            return true;
        }

        public Material GetMaterial()
        {
            if (_material == null)
            {
                var color = (Main.Instance.UseLightText()) ? Color.white : Color.black;
                color.a = Main.Instance.GetOpacity();
                _material = MaterialPool.MatFrom(Resources.Font, ShaderDatabase.Transparent, color);
            }

            return _material;
        }

        public void Reset()
        {
            _material = null;
        }

        public IEnumerable<CharBoundsInTexture> GetBoundsInTextureFor(string text)
        {
            foreach (char c in text)
            {
                yield return GetCharBoundsInTextureFor(c);
            }
        }

        private CharBoundsInTexture GetCharBoundsInTextureFor(char c)
        {
            var index = GetIndexInFontForChar(c);
            var left = index * _charWidthAsTexturePortion;
            return new CharBoundsInTexture()
            {
                Left = left,
                Right = left + _charWidthAsTexturePortion
            };
        }

        private int GetIndexInFontForChar(char c)
        {
            var asciiVal = (int) c;
            if (asciiVal < 128)
                return asciiVal;
            else
                return 128 + "七乐人仓体作储儿公共冷加务区医卧厅厕厨园土圣地场圾垃多妓娱存客室宿小尸山工干幼床库御房所教料有木机材林果枪树械棉棚植毒水池洞浴火热燃牢牧物玻璃生电畜病的皮研种稻究纤维舍船色花苹草药莓藏见谒豆野钢铀间防阵院飞餐麦".IndexOf(c);
        }
    }
}
