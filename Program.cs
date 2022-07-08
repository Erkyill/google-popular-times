using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace WebScraping
{
    class Program
    {
        private static IWebDriver driver = new ChromeDriver();

        static void Main(string[] args)
        {
            //TODO: Scrape time aswell instead of using DateTime
            //TODO: Indicate LIVE view pole in Populat time

            //All objects accessible through CssSelector [class="VkpGBb"]
            //If it has popular time CssSelector [class='u9sbk W2lMue']
            //If at that day closed CssSelector [class='AJumKe']
            //Popular time individual pole CssSelector [class^='cwiwob']
            //--Note: this selector with ^ looks for all cases with class'cwiwob'

            //Example url
            SetUpBrowser("https://shorturl.at/ntuOR");

            MoveToOtherBlock();


            driver.Quit();

        }
        private static void SetUpBrowser(string url)
        {
            new DriverManager().SetUpDriver(new ChromeConfig());

            driver.Navigate().GoToUrl(url);

            //Annoying Google agreement button
            IWebElement button = driver.FindElement(By.CssSelector("[class='VfPpkd-LgbsSe VfPpkd-LgbsSe-OWXEXe-k8QpJ VfPpkd-LgbsSe-OWXEXe-dgl2Hf nCP5yc AjY5Oe DuMIQc qfvgSe']"));
            button.Click();
        }

        //Sleeps for 1 sec so that web has time to load
        //Tries using CssSelector and if it doesn't find that will throw an error
        private static bool CheckIfSearchable()
        {
            Console.WriteLine("Checking if Popular time exists...");

            Thread.Sleep(1000);

            try
            {
                IWebElement tryingHere = driver.FindElement(By.CssSelector("[class='u9sbk W2lMue']"));
            }
            catch
            {
                Console.WriteLine("--No popular time exists for this place");
                return false;
            }

            Console.WriteLine("--Popular time exists");
            return true;
        }

        //Same as CheckIfSearchable() but inverse
        private static bool CheckIfOpen()
        {
            Console.WriteLine("Checking if place is open...");
            try
            {
                IWebElement tryingHere = driver.FindElement(By.CssSelector("[class='AJumKe']"));
                Console.WriteLine("--Today place is closed");
            }
            catch
            {
                Console.WriteLine("--Place is open!");
                return true;
            }
            return false;
        }

        // CssSelector for individual pole in Popular times
        // Scraping the css height value Example: 36px
        // 'px' string value gets removed so we can use the data for our purposes
        private static void SearchResults()
        {
            //Time keeping until I scrape time of the google
            DateTime timeOfPopular = new DateTime(2022, 1, 1, 3, 0, 0);
            //Sleep to load the page
            Thread.Sleep(1000);

            IList<IWebElement> all = driver.FindElements(By.CssSelector("[class^='cwiwob']"));
            
            foreach (IWebElement element in all)
            {
                string stringPharsed = element.GetCssValue("height");

                string stringPharsedRemoved = stringPharsed.Remove(stringPharsed.Length - 2);
                decimal popularTimesInt = decimal.Parse(stringPharsedRemoved);

                timeOfPopular = timeOfPopular.AddHours(1);

                string toTextFile = $"Ammount: {stringPharsedRemoved} Time: {timeOfPopular}";
                TextEditing(toTextFile);

                switch (popularTimesInt)
                {
                    case decimal n when n >= 75:
                        Console.WriteLine($"Ammount: {stringPharsedRemoved} Time: {timeOfPopular}");
                        Console.WriteLine("--Very strong");
                        break;
                    case decimal n when n >= 50:
                        Console.WriteLine($"Ammount: {stringPharsedRemoved} Time: {timeOfPopular}");
                        Console.WriteLine("--Strong");
                        break;
                    case decimal n when n >= 45:
                        Console.WriteLine($"Ammount: {stringPharsedRemoved} Time: {timeOfPopular}");
                        Console.WriteLine("--Good");
                        break;
                    case decimal n when n >= 35:
                        Console.WriteLine($"Ammount: {stringPharsedRemoved} Time: {timeOfPopular}");
                        Console.WriteLine("--Normal");
                        break;
                    case decimal n when n >= 25:
                        Console.WriteLine($"Ammount: {stringPharsedRemoved} Time: {timeOfPopular}");
                        Console.WriteLine("--Low");
                        break;
                    case decimal n when n >= 5:
                        Console.WriteLine($"Ammount: {stringPharsedRemoved} Time: {timeOfPopular}");
                        Console.WriteLine("--Depression zone");
                        break;
                    default:
                        Console.WriteLine("Closed or no activity");
                        break;
                }
            }
        }
        private static void MoveToOtherBlock()
        {
            //All objects accessible through CssSelector [class='VkpGBb']
            IList<IWebElement> objectBlocks = driver.FindElements(By.CssSelector("[class='VkpGBb']"));
            foreach(IWebElement objectBlock in objectBlocks)
            {
                IWebElement objectButton = objectBlock;
                objectButton.Click();
                if (CheckIfSearchable() && CheckIfOpen())
                {
                    Console.WriteLine("Place can be searched\nSearching for activity...");

                    //Add place name in file
                    //[class='qrShPb kno-ecr-pt PZPZlf q8U8x PPT5v'] access block name
                    string blockName = driver.FindElement(By.CssSelector("[data-attrid='title']")).Text;
                    TextEditing(blockName);

                    SearchResults();
                }
                else
                {
                    Console.WriteLine("Place cannot be searched");
                }
            }
        }

        //Appends given string to a file end
        private static void TextEditing(string magicText)
        {
            //Path to file
            string path = @"enter path to file";

            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine(magicText);
            }
        }

    }
}

