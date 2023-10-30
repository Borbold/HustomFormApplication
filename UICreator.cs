using System.Diagnostics;

namespace HustonUI {
    internal class UICreator {
        public static void RemoveAll(Panel panel) {
            for(int i = 0; i < panel.Controls.Count;) {
                string? tag = panel.Controls[i].Tag != null ? panel.Controls[i].Tag.ToString() : "";
                if(string.Compare(tag, "NotInvisible") != 0)
                    panel.Controls.RemoveAt(i);
                else
                    i++;
            }
        }
        public static Label CreateLabel(string text, Point location, int width, Panel page) {
            Label newLab = new() {
                Text = text,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                Location = location,
                Width = width,
            };
            page.Controls.Add(newLab);
            return newLab;
        }
        public static Label CreateLabel(string text, Point location, int width, int height, Panel page) {
            Label newLab = new() {
                Text = text,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                Location = location,
                Width = width,
                Height = height,
            };
            page.Controls.Add(newLab);
            return newLab;
        }
        public static TextBox CreateTextBox(string name, string text, Point location, int width, Panel page) {
            TextBox newTextBox = new() {
                Name = name,
                Text = text,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                Location = location,
                Width = width,
            };
            page.Invoke(new Action(() => {
                page.Controls.Add(newTextBox);
            }));
            return newTextBox;
        }
        public static CheckBox CreateCheckBox(string name, Point location, int width, Panel page) {
            CheckBox newCheckBox = new() {
                Name = name,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                Location = location,
                Width = width,
            };
            page.Controls.Add(newCheckBox);
            return newCheckBox;
        }
        public static Button CreateButton(string text, Point location, int width, Panel page) {
            Button newButton = new() {
                Text = text,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                Location = location,
                Width = width,
            };
            page.Controls.Add(newButton);
            return newButton;
        }
    }
}
