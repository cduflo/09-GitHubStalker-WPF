using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GitHubStalker_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class SelName
    {
        string name { get; set; }
    }


    public partial class MainWindow : Window
    {
        string username;
        public MainWindow()
        {
            InitializeComponent();
            
        }



        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                username = textBoxUsername.Text;
                WebClient wcu = new WebClient();
                wcu.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                string json = wcu.DownloadString("https://api.github.com/users/" + username);

                WebClient wcc = new WebClient();
                wcc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                string repos = wcc.DownloadString("https://api.github.com/users/" + username + "/repos");

                var o = JObject.Parse(json);
                var re = JArray.Parse(repos);

                textBlockUserResult.Text = "Name: " + o["name"].ToString() + "\n";
                textBlockUserResult.Text += "Url: " + o["url"].ToString() + "\n";
                textBlockUserResult.Text += "Followers: " + o["followers"].ToString() + "\n";
                textBlockUserResult.Text += "Repositories: " + o["public_repos"].ToString() + "\n";

                dataGridRepos.IsReadOnly = true;
                dataGridRepos.ItemsSource = re;
                dataGridRepos.Columns.Add(addColumn("Repository", "name"));
                dataGridRepos.Columns.Add(addColumn("Stars", "stargazers_count"));
                dataGridRepos.Columns.Add(addColumn("Watchers", "watchers_count"));

                textBlockRepoDeets.Text = "";
            }
            catch
            {
                MessageBox.Show("User not found, please check your query and try again.");
            }
        }

       

        public DataGridTextColumn addColumn(string header, string source)
        {
            DataGridTextColumn x = new DataGridTextColumn();
            x.Header = header;
            x.Binding = new Binding(source);
            return x;
        }

        private void dataGridRepos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dynamic x = dataGridRepos.SelectedItem;
            string repo = x.name;

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                string cmt = wc.DownloadString("https://api.github.com/repos/" + username + "/" + repo + "/commits");
                var c = JArray.Parse(cmt);
                textBlockRepoDeets.Text = "Commits: " + c.Count.ToString();
            }
            catch
            {
                textBlockRepoDeets.Text = "Commits: ";
            }

            try
            {
                WebClient wci = new WebClient();
                wci.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                string iss = wci.DownloadString("https://api.github.com/repos/" + username + "/" + repo + "/issues");
                var issue = JArray.Parse(iss);
                textBlockRepoDeets.Text += "\nIssues: " + issue.Count.ToString();
            }
            catch
            {
                textBlockRepoDeets.Text += "\nIssues: " ;
            }
            
            
        }
    }
}
