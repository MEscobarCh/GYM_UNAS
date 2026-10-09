using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gym.UI;

public static class Prompt
{
    public static string ShowDialog(string text, string caption)
    {
        Form prompt = new Form()
        {
            Width = 350,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = caption,
            StartPosition = FormStartPosition.CenterScreen,
            MaximizeBox = false,
            MinimizeBox = false
        };
        Label textLabel = new Label() { Left = 20, Top = 20, Text = text, AutoSize = true };
        TextBox inputBox = new TextBox() { Left = 20, Top = 45, Width = 290 };
        Button confirmation = new Button() { Text = "Aceptar", Left = 210, Top = 80, Width = 100, DialogResult = DialogResult.OK };
        
        confirmation.Click += (sender, e) => { prompt.Close(); };
        prompt.Controls.Add(textLabel);
        prompt.Controls.Add(inputBox);
        prompt.Controls.Add(confirmation);
        prompt.AcceptButton = confirmation;

        return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : string.Empty;
    }
}
