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
    match = re.search(r'lblTitulo\.Text = "(.*?)";', content)
    titulo = match.group(1) if match else "Ventana"
    
    # Quitar bloque de panelTop:
    # Usaremos una aproximacion mas precisa para no romper codigo
    # Se reemplaza la asignacion this.Text por this.SetTitulo
    content = re.sub(r'this\.Text = "(.*?)";', f'this.SetTitulo("\\1");', content, count=1)
    
    # Borrar lineas relacionadas a panelTop, lblTitulo y btnCerrar
    content = re.sub(r'^\s*panelTop\..*\n', "", content, flags=re.MULTILINE)
    content = re.sub(r'^\s*lblTitulo\..*\n', "", content, flags=re.MULTILINE)
    content = re.sub(r'^\s*btnCerrar\..*\n', "", content, flags=re.MULTILINE)
    content = re.sub(r'^\s*this\.Controls\.Add\(panelTop\);\n', "", content, flags=re.MULTILINE)
    content = re.sub(r'^\s*this\.FormBorderStyle = FormBorderStyle\.None;\n', "", content, flags=re.MULTILINE)
    
    # Responsividad: Agregar Anchor (sin romper variables de Location)
    # TextBoxes / ComboBoxes
    def replace_size(m):
        varname = m.group(1)
        w = m.group(2)
        h = m.group(3)
        indent = m.group(4)
        next_line = m.group(5)
        return f"{varname}.Size = new Size({w}, {h});\n{indent}{varname}.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;\n{indent}{next_line}"
        
    content = re.sub(r'(txt\w+)\.Size = new Size\((\d+), (\d+)\);\n(\s*)(\w+\.\w+)', replace_size, content)
    content = re.sub(r'(cb\w+)\.Size = new Size\((\d+), (\d+)\);\n(\s*)(\w+\.\w+)', replace_size, content)
    content = re.sub(r'(btn\w+)\.Size = new Size\((\d+), (\d+)\);\n(\s*)(\w+\.\w+)', replace_size, content)

    with open(filepath, "w", encoding="utf-8") as file:
        file.write(content)

