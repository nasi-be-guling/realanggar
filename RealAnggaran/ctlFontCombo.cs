using System.Drawing;
using System.Windows.Forms;

namespace RealAnggaran
{
    public class ctlFontCombo : ComboBox
    {
        #region Declarations
        private Image ttImg = Properties.Resources.ttfIcon;
        #endregion

        #region Constructor
        public ctlFontCombo()
        {
            this.MaxDropDownItems = 20;
            this.IntegralHeight = false;
            this.Sorted = false;
            this.DropDownStyle = ComboBoxStyle.DropDownList;
            this.DrawMode = DrawMode.OwnerDrawFixed;
            //Populate combo box with current TTF fonts
            foreach (FontFamily ff in FontFamily.Families)
            {
                if (ff.IsStyleAvailable(FontStyle.Regular)) { this.Items.Add(ff); }
            }
        }
        #endregion

        #region Draw item text
        protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                string fontName = ((FontFamily)this.Items[e.Index]).Name;

                if ((e.State & DrawItemState.Focus) == 0)
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.Window), e.Bounds.X + ttImg.Width, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                    e.Graphics.DrawString(fontName, base.Font, new SolidBrush(SystemColors.WindowText), e.Bounds.X + ttImg.Width, e.Bounds.Y);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds.X + ttImg.Width, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                    e.Graphics.DrawString(fontName, base.Font, new SolidBrush(SystemColors.HighlightText), e.Bounds.X + ttImg.Width, e.Bounds.Y);
                }
                //Draw icon
                e.Graphics.DrawImage(ttImg, new Point(e.Bounds.X, e.Bounds.Y));
            }
        }
        #endregion
    }
}

