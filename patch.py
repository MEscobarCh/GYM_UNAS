import os
import re

dir_path = r"C:\Users\Marco\Desktop\GYM UNAS\Gym.UI"
files = [f for f in os.listdir(dir_path) if f.endswith("Form.cs")]

for f in files:
    if f in ("DashboardForm.cs", "ModalFormBase.cs"):
        continue
    
    filepath = os.path.join(dir_path, f)
    with open(filepath, "r", encoding="utf-8") as file:
        content = file.read()
        
    # Herencia
    if "public class EnrolamientoForm : Form, DPFP.Capture.EventHandler" in content:
        content = content.replace("public class EnrolamientoForm : Form, DPFP.Capture.EventHandler", "public class EnrolamientoForm : ModalFormBase, DPFP.Capture.EventHandler")
    else:
        content = re.sub(r"public class (\w+Form)\s*:\s*Form", r"public class \1 : ModalFormBase", content)
    
    # Remover declaraciones de UI top
    content = re.sub(r"^\s*private Panel panelTop = new\(\);\n", "", content, flags=re.MULTILINE)
    content = re.sub(r"^\s*private Label lblTitulo = new\(\);\n", "", content, flags=re.MULTILINE)
    content = re.sub(r"^\s*private Button btnCerrar = new\(\);\n", "", content, flags=re.MULTILINE)
    
    # Reemplazar inicializacion.
    # Buscamos el texto de lblTitulo
    match = re.search(r'lblTitulo\.Text = "(.*?)";', content)
    titulo = match.group(1) if match else "Ventana"
    
    # Quitar bloque: desde panelTop.BackColor = ... hasta panelTop.Controls.Add(btnCerrar);
    content = re.sub(r'\s*panelTop\.BackColor = .*?panelTop\.Controls\.Add\(btnCerrar\);', f'\n        this.SetTitulo("{titulo}");', content, flags=re.DOTALL)
    
    # Quitar this.Controls.Add(panelTop);
    content = re.sub(r'^\s*this\.Controls\.Add\(panelTop\);\n', "", content, flags=re.MULTILINE)
    
    # Remover this.Text = ... y this.FormBorderStyle = FormBorderStyle.None;
    content = re.sub(r'^\s*this\.Text = .*?;\n', "", content, flags=re.MULTILINE)
    content = re.sub(r'^\s*this\.FormBorderStyle = FormBorderStyle\.None;\n', "", content, flags=re.MULTILINE)

    # Responsividad: Agregar Anchor
    # Buscar txt[Nombre].Size = new Size(..., ...);
    content = re.sub(r'(txt\w+)\.Size = new Size\((\d+), (\d+)\);\n(\s*)(\w+\.\w+)', r'\1.Size = new Size(\2, \3);\n\4\1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;\n\4\5', content)
    # Buscar cb[Nombre] o combinaciones
    content = re.sub(r'(cb\w+)\.Size = new Size\((\d+), (\d+)\);\n(\s*)(\w+\.\w+)', r'\1.Size = new Size(\2, \3);\n\4\1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;\n\4\5', content)

    # Para los botones btnProcesar, btnCancelar u otros, anclarlos a Bottom|Right
    content = re.sub(r'(btn\w+)\.Location = new Point\((\w+\s*\+\s*\d+|\d+),\s*\w+\);\n(\s*)(\w+\.\w+)', r'\1.Location = new Point(\2, startY);\n\3\1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;\n\3\4', content)

    with open(filepath, "w", encoding="utf-8") as file:
        file.write(content)
    
    print(f"Modificado {f}")

