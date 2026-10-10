import os
import re

dir_path = r"C:\Users\Marco\Desktop\GYM UNAS\Gym.UI"
files = [f for f in os.listdir(dir_path) if f.endswith("Form.cs")]

# Eliminar ModalFormBase
modal_base_path = os.path.join(dir_path, "ModalFormBase.cs")
if os.path.exists(modal_base_path):
    os.remove(modal_base_path)

for f in files:
    if f in ("DashboardForm.cs", "ModalFormBase.cs"):
        continue
    
    filepath = os.path.join(dir_path, f)
    with open(filepath, "r", encoding="utf-8") as file:
        content = file.read()
        
    # Restaurar Herencia a Form
    if "public class EnrolamientoForm : ModalFormBase, DPFP.Capture.EventHandler" in content:
        content = content.replace("public class EnrolamientoForm : ModalFormBase, DPFP.Capture.EventHandler", "public class EnrolamientoForm : Form, DPFP.Capture.EventHandler")
    else:
        content = re.sub(r"public class (\w+Form)\s*:\s*ModalFormBase", r"public class \1 : Form", content)
    
    # Reemplazar SetTitulo por this.Text y configurar FormBorderStyle
    # Si encontramos this.SetTitulo("..."), lo reemplazamos por this.Text = "..."; y agregamos FormBorderStyle.FixedDialog
    
    def replacer_titulo(match):
        titulo = match.group(1)
        return f'this.Text = "{titulo}";\n        this.FormBorderStyle = FormBorderStyle.Sizable;\n        this.MaximizeBox = false;\n        this.MinimizeBox = false;'

    content = re.sub(r'this\.SetTitulo\("(.*?)"\);', replacer_titulo, content)

    with open(filepath, "w", encoding="utf-8") as file:
        file.write(content)
        
print("Modales actualizados a UI Nativa.")

# Ahora, cambiar texto en DashboardForm.cs
dash_path = os.path.join(dir_path, "DashboardForm.cs")
with open(dash_path, "r", encoding="utf-8") as file:
    dash_content = file.read()

# Reemplazaremos cualquier btnModuloEnrolar o btnEnrolarHuella.Text
dash_content = re.sub(r'btnModuloEnrolar\.Text\s*=\s*".*?";', 'btnModuloEnrolar.Text = "Actualizar Huella";', dash_content)
dash_content = re.sub(r'btnEnrolarHuella\.Text\s*=\s*".*?";', 'btnEnrolarHuella.Text = "Actualizar Huella";', dash_content)

with open(dash_path, "w", encoding="utf-8") as file:
    file.write(dash_content)
    
print("DashboardForm.cs actualizado.")

