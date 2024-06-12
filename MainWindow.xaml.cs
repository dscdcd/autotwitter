using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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

namespace Autotwitter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IWebDriver driver = null;
        public MainWindow()//https://googlechromelabs.github.io/chrome-for-testing/
        {
            InitializeComponent();
           
            // 从文件中读取cookie并添加到浏览器
            string cookieFile = "mycookies.txt";
            if (File.Exists(cookieFile))
            {

            }
            else
            {
                
            }


        }

    

      
        private void Button_Click(object sender, RoutedEventArgs e)
        {


            string twwiteerid = jiankongid;


            driver.Navigate().Refresh();
            Thread.Sleep(3000);



            List<string> tims = new List<string>();
            // 保存cookie到文件
            string cookieFile = "cookies.txt";
            SaveCookiesToFile(driver, cookieFile);

            IReadOnlyList<IWebElement> times = driver.FindElements(By.TagName("time"));
            foreach (IWebElement time in times)
            {
                System.Console.WriteLine(time.Text);
                string dateTimeValue = time.GetAttribute("datetime");
                tims.Add(dateTimeValue);
                Console.WriteLine($"Datetime value: {dateTimeValue}");
            }
            获取推文(tims, twwiteerid);
        }

        private void 获取推文(List<string> tims, string twwiteerid)
        {
            tb1.Text = "";
            //获取图片
            Dictionary<string, List<string>> ph_Groups = new Dictionary<string, List<string>>();
            List<string> ph_urls = new List<string>();
            try
            {


                IReadOnlyCollection<IWebElement> elements_photo =
                    driver.FindElements(By.CssSelector(".css-175oi2r.r-1pi2tsx.r-1ny4l3l.r-1loqt21")); //css - 175oi2r r-1pi2tsx r-1ny4l3l r-1loqt21

                int phnum = 0;
                List<string> photos = new List<string>();
                foreach (IWebElement element in elements_photo)
                {

                    string p = element.GetAttribute("href");
                    if (p.Contains($"{twwiteerid}/status/"))
                        photos.Add(p);

                    phnum++;
                }

                // Group the texts based on the unique {id} values


                foreach (string p in photos)
                {
                    int startIndex = p.IndexOf("/status/") + 8;
                    int endIndex = p.IndexOf("/photo/");
                    string id = p.Substring(startIndex, endIndex - startIndex);

                    if (!ph_Groups.ContainsKey(id))
                    {
                        ph_Groups[id] = new List<string>();
                        ph_urls.Add(id);
                    }
                    if (!ph_Groups[id].Contains(p))
                        ph_Groups[id].Add(p);
                }

                //tb1.Text = tb1.Text + $"图片链接如下:\n";
                // Print the results
                foreach (var group in ph_Groups)
                {
                    Console.WriteLine($"Group with ID 图片: {group.Key}");
                    //tb1.Text = tb1.Text+ $"Group with ID: {group.Key}\n";
                    foreach (string item in group.Value)
                    {
                        Console.WriteLine(item);
                        //tb1.Text = tb1.Text + $"{item}\n";
                    }
                }

            }
            catch (Exception eee)
            {
                string message = eee.Message;
            }


            //获取推文
            try
            {// 使用 CSS 选择器定位所有匹配的元素
                IReadOnlyCollection<IWebElement> elements = driver.FindElements(By.CssSelector(".css-1jxf684.r-bcqeeo.r-1ttztb7.r-qvutc0.r-poiln3"));

                // 遍历所有匹配的元素,获取它们的文本内容
                int textnum = 0;
                List<string> texts = new List<string>();
                foreach (IWebElement element in elements)
                {
                    if (textnum != 0)
                    {
                        string text = element.Text.Replace(jiankongid + jiankongid, "");
                        texts.Add(text);
                    }
                    textnum++;
                }



                int 获取个数 = 0;
                try
                {
                    int num = 0;

                    List<string> textGroups = new List<string>();
                    int startIndex = 0;
                    foreach (string text in texts)
                    {
                        if (text.Contains(twwiteerid))
                        {
                            if (startIndex != 0)
                            {
                                if (获取个数 == 0)
                                { }
                                else
                                {
                                    int 相差 = num - startIndex;
                                    //结合pho

                                    textGroups.Add(tims[获取个数 - 1] + string.Concat(texts.GetRange(startIndex, 相差)));

                                }
                                获取个数++;
                            }
                            startIndex = num + 1;
                        }
                        num++;
                    }


                    tb1.Text = tb1.Text + "推文如下：\n\n";
                    foreach (string text in textGroups)
                    {
                        Console.WriteLine(text + "\n\n");
                        tb1.Text = tb1.Text + $"\n\n{text}\n\n";
                    }
                }
                catch (Exception eee)
                {
                    string message = eee.Message;
                }
            }
            catch (Exception eee)
            {
                string message = eee.Message;
            }






        }


    

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            driver.Close();
        }
        // 保存cookie到文件的方法
        private static void SaveCookiesToFile(IWebDriver driver, string filePath)
        {
            // 获取所有cookie
            var cookies = driver.Manage().Cookies.AllCookies;

            // 将cookie保存到文件
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var cookie in cookies)
                {
                    writer.WriteLine($"{cookie.Name}={cookie.Value}");
                }
            }
        }

        // 从文件中读取cookie并添加到浏览器的方法
        private static void LoadCookiesFromFile(IWebDriver driver, string filePath)
        {
            // 从文件中读取cookie
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            // 添加cookie到浏览器
                            driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(parts[0], parts[1]));
                        }
                    }
                }
            }
        }

        private string jiankongid = "123";
        private void tb_start_Click(object sender, RoutedEventArgs e)
        {
           

            try
            {
                string cookieFile = "mycookies.txt";
                jiankongid = tb2.Text;

                string sss = Directory.GetCurrentDirectory() + "//1//chromedriver.exe";
                driver = new ChromeDriver(sss);
                driver.Navigate().GoToUrl($"https://x.com/{jiankongid}");//https://x.com/jay_zhou https://x.com/   yua_mikami

                // 从文件中读取cookie并添加到浏览器
                cookieFile = "cookies.txt";
                if (File.Exists(cookieFile))
                {
                    LoadCookiesFromFile(driver, cookieFile);
                }
            }
            catch
            { }



        }




     
    }
}
