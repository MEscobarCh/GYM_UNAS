import os
import re

path = r"C:\Users\Marco\Desktop\GYM UNAS\Gym.UI\PagoForm.cs"
with open(path, "r", encoding="utf-8") as f: content = f.read()

# Controles anchos
content = re.sub(r'(txtDni\.Size = new Size\(.*?\);)', r'\1\n        txtDni.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(cbTipoPase\.Size = new Size\(.*?\);)', r'\1\n        cbTipoPase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(cbMetodoPago\.Size = new Size\(.*?\);)', r'\1\n        cbMetodoPago.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(txtNroOperacion\.Size = new Size\(.*?\);)', r'\1\n        txtNroOperacion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(lblMonto\.Size = new Size\(.*?\);)', r'\1\n        lblMonto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)
content = re.sub(r'(lblError\.Size = new Size\(.*?\);)', r'\1\n        lblError.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;', content)

# btnBuscar
content = re.sub(r'(btnBuscar\.Size = new Size\(.*?\);)', r'\1\n        btnBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Right;', content)

# Botones inferiores
content = re.sub(r'(btnProcesar\.Size = new Size\(.*?\);)', r'\1\n        btnProcesar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;', content)
content = re.sub(r'(btnCancelar\.Size = new Size\(.*?\);)', r'\1\n        btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;', content)

with open(path, "w", encoding="utf-8") as f: f.write(content)
