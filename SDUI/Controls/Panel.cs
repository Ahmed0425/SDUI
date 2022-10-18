﻿using SDUI.Extensions;
using SDUI.Helpers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SDUI.Controls;

public class Panel : System.Windows.Forms.Panel
{
    private int _radius = 10;
    public int Radius
    {
        get => _radius;
        set
        {
            _radius = value;

            Invalidate();
        }
    }

    private Padding _border;
    public Padding Border
    {
        get => _border;
        set
        {
            if (_border == value)
                return;

            _border = value;
            Invalidate();
        }
    }

    private Color _borderColor = Color.Transparent;
    public Color BorderColor
    {
        get => _borderColor;
        set
        {
            if (_borderColor == value)
                return;

            _borderColor = value;
            Invalidate();
        }
    }

    private float _shadowDepth = 4;
    public float ShadowDepth
    {
        get => _shadowDepth;
        set
        {
            if (_shadowDepth == value)
                return;

            _shadowDepth = value;
            Invalidate();
        }
    }

    public Panel()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint, true);

        BackColor = Color.Transparent;
        UpdateStyles();
    }

    protected override void OnParentBackColorChanged(EventArgs e)
    {
        base.OnParentBackColorChanged(e);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var graphics = e.Graphics;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        GroupBoxRenderer.DrawParentBackground(graphics, ClientRectangle, this);
        if (ColorScheme.DrawDebugBorders)
        {
            var redPen = new Pen(Color.Red, 1);
            redPen.Alignment = PenAlignment.Inset;
            e.Graphics.DrawRectangle(redPen, new Rectangle(0, 0, Width - 1, Height - 1));
        }

        var rect = ClientRectangle.ToRectangleF();

        var color = BackColor == Color.Transparent ? ColorScheme.BackColor2 : BackColor;
        var borderColor = _borderColor == Color.Transparent ? ColorScheme.BorderColor : _borderColor;

        var inflate = _shadowDepth / 4f;
        rect.Inflate(-inflate, -inflate);

        if (_radius > 0)
        {
            using (var path = rect.Radius(_radius))
            {
                /*var shadow = DropShadow.Create(path, Color.Black.Alpha(20), _shadowDepth);

                var shadowBounds = DropShadow.GetBounds(shadowRect, _shadowDepth);
                //shadowBounds.Offset(0, 0);

                e.Graphics.DrawImageUnscaled(shadow, shadowBounds.Location);

                */

                using (var brush = new SolidBrush(color))
                    e.Graphics.FillPath(brush, path);

                ControlPaintHelper.DrawShadow(e.Graphics, rect, _shadowDepth, _radius);

                using (var pen = new Pen(borderColor, 1))
                    e.Graphics.DrawPath(pen, path);
            }

            return;
        }

        using (var brush = new SolidBrush(color))
            e.Graphics.FillRectangle(brush, rect);

        ControlPaintHelper.DrawShadow(e.Graphics, rect, _shadowDepth, _radius == 0 ? 1 : _radius);

        ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                              borderColor, _border.Left, ButtonBorderStyle.Solid,
                              borderColor, _border.Top, ButtonBorderStyle.Solid,
                              borderColor, _border.Right, ButtonBorderStyle.Solid,
                              borderColor, _border.Bottom, ButtonBorderStyle.Solid);
    }
}