
using Microsoft.Maui.Controls;
using NHM.Model;
using NHM.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHM.Pages
{
    public class DevicePage : ContentPage
    {

        private NetDevice device;
        public DevicePage(NetDevice device)
        {
            this.device = device;
            this.Title = device.getName();
            ScrollView sw = new ScrollView();
            this.Content = sw;

            VerticalStackLayout content = new VerticalStackLayout();
            sw.Content = content;

            foreach (DeviceCondition dc in device.list)
            {
                ConditionPresentation cp = dc.cp;
                VerticalStackLayout vsl = new VerticalStackLayout();
                vsl.BackgroundColor = cp.backgroundColor;
                vsl.Margin = 5;
                vsl.Padding = 5;
                Label l1 = new Label();
                Label l2 = new Label();
                l1.Text = cp.name;
                l1.FontSize = 25;
                l2.Text = cp.lines;
                vsl.Add(l1);
                vsl.Add(l2);

                content.Add(vsl);
            }
        }

        ~DevicePage()
        {
        }
    }

    public class DeviceShell : ShellContent
    {

        public DeviceShell(NetDevice dev)
        {
            Title = dev.getName();
            ContentTemplate = new DataTemplate(() => new DevicePage(dev));
        }

        ~DeviceShell()
        {
        }
    }
}