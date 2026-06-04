using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinBooster;

public partial class Form1 : Form
{
    Booster boost = new Booster();
    bool isSystemBoosted = false;

    private Button btnBoost;
    private Button btnRestore;
    private Label lblResults;

    public Form1()
    {
        InitializeComponent();
        SetupProfessionalUI();
    }

    private void SetupProfessionalUI()
    {
        int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
        int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

        this.Size = new Size(screenWidth / 2, (int)(screenHeight * 0.6));
        this.Text = "WinBooster";
        this.StartPosition = FormStartPosition.CenterScreen;

        // מונע ריצודים בזמן ציור הרקע
        this.DoubleBuffered = true;

        int centerX = this.ClientSize.Width / 2;
        int centerY = this.ClientSize.Height / 2;

        Label lblTitle = new Label
        {
            Text = "WinBooster",
            Font = new Font("Segoe UI", 28, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Size = new Size(400, 60),
            Location = new Point(centerX - 200, centerY - 150),
            TextAlign = ContentAlignment.MiddleCenter,
            Anchor = AnchorStyles.None
        };

        btnBoost = CreateButton("⚡ RUN BOOST", Color.LimeGreen, new Point(centerX - 150, centerY - 50), new Size(300, 70), btnBoost_Click);
        btnBoost.Font = new Font("Segoe UI", 16, FontStyle.Bold);

        btnRestore = CreateButton("↺ RESTORE SYSTEM", Color.FromArgb(200, 50, 50), new Point(centerX - 100, centerY + 50), new Size(200, 40), btnRestore_Click);
        btnRestore.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnRestore.Enabled = false;

        lblResults = new Label
        {
            Text = "Ready to optimize system.",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.LightGray,
            BackColor = Color.Transparent,
            Size = new Size(800, 60),
            Location = new Point(centerX - 400, centerY + 120),
            TextAlign = ContentAlignment.MiddleCenter,
            Anchor = AnchorStyles.None
        };

        this.Controls.AddRange(new Control[] { lblTitle, btnBoost, btnRestore, lblResults });

        this.FormClosing += Form1_FormClosing;
    }

    // ציור מעבר הצבעים ברקע החלון (אפור כהה למעלה -> שחור למטה)
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(45, 45, 45), Color.FromArgb(10, 10, 10), 90F))
        {
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }
    }

    private Button CreateButton(string text, Color bg, Point loc, Size size, EventHandler click)
    {
        Button btn = new Button
        {
            Text = text,
            BackColor = bg,
            Location = loc,
            Size = size,
            Anchor = AnchorStyles.None,
            ForeColor = (bg == Color.LimeGreen) ? Color.Black : Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.Click += click;
        return btn;
    }

    private void btnBoost_Click(object? sender, EventArgs e)
    {
        if (isSystemBoosted) return;

        btnBoost.Text = "BOOSTING...";
        this.Refresh();

        lblResults.Text = boost.StartBooster();

        isSystemBoosted = true;
        btnBoost.Text = "⚡ BOOSTED";
        btnRestore.Enabled = true;
    }

    private void btnRestore_Click(object? sender, EventArgs e)
    {
        lblResults.Text = boost.CloseAndRestore();

        isSystemBoosted = false;
        btnRestore.Enabled = false;
        btnBoost.Text = "⚡ RUN BOOST";
    }

    private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (isSystemBoosted)
        {
            boost.CloseAndRestore();
        }
    }
}