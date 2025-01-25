import os
from pathlib import Path

here = Path(__file__).parent

from PIL import Image, ImageDraw, ImageFont


def draw(s):
    img = Image.new('RGBA', (35 * len(s), 70), color=(0, 0, 0, 0))
    fnt1 = ImageFont.truetype(f'consola', 64)
    fnt2 = ImageFont.truetype(f'NotoSerifSC-SemiBold', 35)
    d = ImageDraw.Draw(img)
    for i, c in enumerate(s):
        if ord(c) < 128:
            d.text((35*i, 0), c, font=fnt1, fill=(255, 252, 216))
        else:
            d.text((35*i, 0), c, font=fnt2, fill=(255, 252, 216))
    return img


汉字 = '七乐人仓体作储儿公共冷加务区医卧厅厕厨园土圣地场圾垃多妓娱存客室宿小尸山工干幼床库御房所教料有木机材林果枪树械棉棚植毒水池洞浴火热燃牢牧物玻璃生电畜病的皮研种稻究纤维舍船色花苹草药莓藏见谒豆野钢铀间防阵院飞餐麦'
s = ''.join([chr(i) for i in range(128)]) + 汉字

draw(s).save(here / 'mod-structure/Textures/Consolas.png')
