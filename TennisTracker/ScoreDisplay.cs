using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisTracker
{
    internal class ScoreDisplay
    {
        Border _border1;
        Border _border2;
        Label _label1;
        Label _label2;

        public Border Border1 { get => _border1; set => _border1 = value; }
        public Border Border2 { get => _border2; set => _border2 = value; }
        public Label Label1 { get => _label1; set => _label1 = value; }
        public Label Label2 { get => _label2; set => _label2 = value; }

        public ScoreDisplay(Border border1, Border border2, Label label1, Label label2)
        {
            Border1 = border1;
            Border2 = border2;
            Label1 = label1;
            Label2 = label2;

            Border1.Content = label1;
            Border2.Content = label2;
        }

        public void HideDisplay()
        {
            Border1.IsVisible = false;
            Border2.IsVisible = false;
            Label1.IsVisible = false;
            Label2.IsVisible = false;
        }

        public void ShowDisplay()
        {
            Border1.IsVisible = true; 
            Border2.IsVisible = true;
            Label1.IsVisible = true;
            Label2.IsVisible = true;
        }
    }
}
