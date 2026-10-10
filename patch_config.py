import os
import re

path = r"C:\Users\Marco\Desktop\GYM UNAS\Gym.UI\ConfigForm.cs"
with open(path, "r", encoding="utf-8") as f: content = f.read()

# Anchos completos
content = re.sub(r'(txtUrl\.Size = new Size\(.*?\);)', r'\1\n        txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(txtDbServidor\.Size = new Size\(.*?\);)', r'\1\n        txtDbServidor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(lblMensaje\.Size = new Size\(.*?\);)', r'\1\n        lblMensaje.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;', content)

# Derecha
content = re.sub(r'(lblDbPassword\.Location = new Point\(.*?\);)', r'\1\n        lblDbPassword.Anchor = AnchorStyles.Top | AnchorStyles.Right;', content)
content = re.sub(r'(txtDbPassword\.Size = new Size\(.*?\);)', r'\1\n        txtDbPassword.Anchor = AnchorStyles.Top | AnchorStyles.Right;', content)
content = re.sub(r'(btnTestear\.Size = new Size\(.*?\);)', r'\1\n        btnTestear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;', content)

# Izquierda inferior
content = re.sub(r'(btnGuardar\.Size = new Size\(.*?\);)', r'\1\n        btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;', content)

with open(path, "w", encoding="utf-8") as f: f.write(content)
