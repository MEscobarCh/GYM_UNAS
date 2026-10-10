using System;
using System.IO;
using System.Text.RegularExpressions;

class Program {
    static void Main() {
        string dir = @"C:\Users\Marco\Desktop\GYM UNAS\Gym.UI";
        string[] files = Directory.GetFiles(dir, "*Form.cs");
        foreach (string file in files) {
            string name = Path.GetFileName(file);
            if (name == "DashboardForm.cs" || name == "ModalFormBase.cs") continue;
            
            string content = File.ReadAllText(file);
            
            // Reemplazar herencia
            content = Regex.Replace(content, @"public class (\w+Form) : Form", "public class  : ModalFormBase");
            if (name == "EnrolamientoForm.cs") {
                content = Regex.Replace(content, @"public class EnrolamientoForm : Form, (.*)", "public class EnrolamientoForm : ModalFormBase, ");
            }

            // Eliminar declaraciones
            content = Regex.Replace(content, @"^\s*private Panel panelTop = new\(\);\r?\n", "", RegexOptions.Multiline);
            content = Regex.Replace(content, @"^\s*private Label lblTitulo = new\(\);\r?\n", "", RegexOptions.Multiline);
            content = Regex.Replace(content, @"^\s*private Button btnCerrar = new\(\);\r?\n", "", RegexOptions.Multiline);

            // Reemplazar la configuracion de la barra de titulo y Size
            // Buscaremos algo como lblTitulo.Text = "TITULO";
            Match titleMatch = Regex.Match(content, @"lblTitulo\.Text = "(.*?)";");
            string titulo = titleMatch.Success ? titleMatch.Groups[1].Value : "Ventana";

            // Eliminar bloque de configuracion repetitivo
            content = Regex.Replace(content, @"this\.Text = .*?this\.Controls\.Add\(btnCerrar\);", "this.SetTitulo(\"" + titulo + "\");", RegexOptions.Singleline);
            
            // Eliminar this.Controls.Add(panelTop);
            content = Regex.Replace(content, @"^\s*this\.Controls\.Add\(panelTop\);\r?\n", "", RegexOptions.Multiline);

            // Agregar Anchor para TextBoxes y ComboBoxes para Responsividad (si Size > 200)
            content = Regex.Replace(content, @"(\w+)\.Size = new Size\((4\d\d|3\d\d), \d+\);\r?\n\s*(\w+)\.Font =", $1.Size = new Size(, 30);\n        .Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;\n        .Font =");
            content = Regex.Replace(content, @"(\w+)\.Size = new Size\((4\d\d|3\d\d), \d+\);\r?\n\s*(\w+)\.Enabled =", $1.Size = new Size(, 30);\n        .Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;\n        .Enabled =");
            
            File.WriteAllText(file, content);
            Console.WriteLine("Processed " + name);
        }
    }
}
