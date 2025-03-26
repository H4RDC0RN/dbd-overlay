using System.Diagnostics;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace DBDOverlay.MVVM.View
{
    public partial class AboutTabView : UserControl
    {
        private readonly string discordProfile = "https://discord.com/users/756964796959555664";
        private readonly string discordServer = "https://discord.gg/v6y86MMXxM";
        private readonly string boosty = "https://boosty.to/h4rdc0rn/donate";
        private readonly string donationAlerts = "https://www.donationalerts.com/r/h4rdc0rn";

        public AboutTabView()
        {
            InitializeComponent();
        }

        private void Click_BoostyButton(object sender, RoutedEventArgs e)
        {
            OpenLinkInBrowser(boosty);
        }

        private void Click_DonationAlertsButton(object sender, RoutedEventArgs e)
        {
            OpenLinkInBrowser(donationAlerts);
        }

        private void Click_DiscordProfileButton(object sender, RoutedEventArgs e)
        {
            OpenLinkInBrowser(discordProfile);
        }

        private void Click_DiscordServerButton(object sender, RoutedEventArgs e)
        {
            OpenLinkInBrowser(discordServer);
        }

        private void OpenLinkInBrowser(string link)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            });
        }
    }
}
