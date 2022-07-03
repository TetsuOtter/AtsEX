﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx
{
    internal partial class VersionForm : Form
    {
        protected Label Title;

        protected Label Description;
        protected Label Copyright;

        protected LinkLabel LicenseLink;
        protected LinkLabel HomepageLink;
        protected LinkLabel RepositoryLink;

        protected Label PluginListHeader;
        protected ListView PluginList;

        protected Button OK;

        protected void InitializeComponent(App app)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(800, 480);
            Font = new Font("Yu Gothic UI", 9);
            Text = string.Format(Resources.GetString("Caption").Value, app.ProductShortName);


            Title = new Label()
            {
                Left = 16,
                Top = 16,
                Width = 160,
                Height = 48,
                Font = new Font("Yu Gothic UI", 28, FontStyle.Bold),
                Text = app.ProductShortName,
            };
            Controls.Add(Title);


            Description = new Label()
            {
                Left = 16,
                Top = 88,
                Width = 480,
                UseMnemonic = false,
                Text = string.Format(Resources.GetString("Description").Value, app.ProductName, app.AtsExAssembly.GetName().Version),
            };
            Controls.Add(Description);

            int year = DateTime.Now.Year;
            Copyright = new Label()
            {
                Left = 16,
                Top = 108,
                Width = 400,
                Text = $"Copyright ©  {(year == 2022 ? "2022" : $"2022-{year}")}  おーとま (automatic9045)",
            };
            Controls.Add(Copyright);


            LicenseLink = new LinkLabel()
            {
                Name = nameof(LicenseLink),
                Left = 16,
                Top = 148,
                Width = 56,
                Text = Resources.GetString("License").Value,
            };
            LicenseLink.LinkClicked += LinkClicked;
            Controls.Add(LicenseLink);

            HomepageLink = new LinkLabel()
            {
                Name = nameof(HomepageLink),
                Left = 96,
                Top = 148,
                Width = 176,
                Text = string.Format(Resources.GetString("Website").Value, app.ProductShortName),
            };
            HomepageLink.LinkClicked += LinkClicked;
            Controls.Add(HomepageLink);

            RepositoryLink = new LinkLabel()
            {
                Name = nameof(RepositoryLink),
                Left = 296,
                Top = 148,
                Width = 128,
                Text = Resources.GetString("Repository").Value,
            };
            RepositoryLink.LinkClicked += LinkClicked;
            Controls.Add(RepositoryLink);


            PluginListHeader = new Label()
            {
                Left = 16,
                Top = 192,
                Width = 400,
                Text = Resources.GetString("PluginListHeader").Value,
            };
            Controls.Add(PluginListHeader);

            PluginList = new ListView()
            {
                View = View.Details,
                GridLines = true,
                Left = 16,
                Top = 216,
                Width = 768,
                Height = 224,
            };
            PluginList.Columns.Add(Resources.GetString("PluginListColumnFileName").Value, 128);
            PluginList.Columns.Add(Resources.GetString("PluginListColumnName").Value, 192);
            PluginList.Columns.Add(Resources.GetString("PluginListColumnType").Value, 96);
            PluginList.Columns.Add(Resources.GetString("PluginListColumnVersion").Value, 96);
            PluginList.Columns.Add(Resources.GetString("PluginListColumnDescription").Value, 224);
            Controls.Add(PluginList);


            OK = new Button()
            {
                Left = 704,
                Top = 448,
                Text = Resources.GetString("OK").Value,
            };
            OK.Click += (sender, e) => Hide();
            Controls.Add(OK);
        }

        private void LinkClicked(object sender, EventArgs e)
        {
            if (sender is LinkLabel linkLabel)
            {
                linkLabel.LinkVisited = true;

                string link = "";
                switch (linkLabel.Name)
                {
                    case nameof(LicenseLink):
                        link = "https://github.com/automatic9045/AtsEX/blob/main/README.md";
                        break;

                    case nameof(HomepageLink):
                        link = "https://automatic9045.github.io/contents/bve/AtsEX/";
                        break;

                    case nameof(RepositoryLink):
                        link = "https://github.com/automatic9045/AtsEX/";
                        break;

                    default:
                        return;
                }

                Process.Start(link);
            }
        }
    }
}
