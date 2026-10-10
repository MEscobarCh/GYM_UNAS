import os
import re

dir_path = r"C:\Users\Marco\Desktop\GYM UNAS\Gym.UI"
files = [f for f in os.listdir(dir_path) if f.endswith("Form.cs")]

for f in files:
    if f == "DashboardForm.cs":
        continue
    filepath = os.path.join(dir_path, f)
    with open(filepath, "r", encoding="utf-8") as file:
        content = file.read()
    
    # Reemplazar Sizable por FixedDialog
    content = content.replace("this.FormBorderStyle = FormBorderStyle.Sizable;", "this.FormBorderStyle = FormBorderStyle.FixedDialog;")
    
    with open(filepath, "w", encoding="utf-8") as file:
        file.write(content)

print("Modales convertidos a FixedDialog.")
