import os
import re

path = r"C:\Users\Marco\Desktop\GYM UNAS\Gym.UI\RegistroExternoForm.cs"
with open(path, "r", encoding="utf-8") as f: content = f.read()

content = re.sub(r'(txtDni\.Size = new Size\(.*?\);)', r'\1\n        txtDni.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(txtNombres\.Size = new Size\(.*?\);)', r'\1\n        txtNombres.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(txtTelefono\.Size = new Size\(.*?\);)', r'\1\n        txtTelefono.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(txtCorreo\.Size = new Size\(.*?\);)', r'\1\n        txtCorreo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)

content = re.sub(r'(btnCapturarHuella\.Location = new Point\(.*?\);)', r'\1\n        btnCapturarHuella.Anchor = AnchorStyles.Top | AnchorStyles.Left;', content)
content = re.sub(r'(lblHuellaEstado\.Location = new Point\(.*?\);)', r'\1\n        lblHuellaEstado.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)

content = re.sub(r'(btnGuardar\.Location = new Point\(.*?\);)', r'\1\n        btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;', content)
content = re.sub(r'(btnCancelar\.Location = new Point\(.*?\);)', r'\1\n        btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;', content)

with open(path, "w", encoding="utf-8") as f: f.write(content)
