import os
import re

path = r"C:\Users\Marco\Desktop\GYM UNAS\Gym.UI\CajaForm.cs"
with open(path, "r", encoding="utf-8") as f: content = f.read()

content = re.sub(r'(gbResumen\.Size = new Size\(.*?\);)', r'\1\n        gbResumen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(pnlFiltros\.Size = new Size\(.*?\);)', r'\1\n        pnlFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(btnNuevoPago\.Size = new Size\(.*?\);)', r'\1\n        btnNuevoPago.Anchor = AnchorStyles.Top | AnchorStyles.Right;', content)
content = re.sub(r'(dgvHistorial\.Size = new Size\(.*?\);)', r'\1\n        dgvHistorial.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;', content)

with open(path, "w", encoding="utf-8") as f: f.write(content)
