using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Media.SpeechSynthesis;
using System.Collections;
using Windows.Data.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private List<Instrument> instrumentList = null;
        private MediaElement player = null;
        private SpeechSynthesizer speech = null;
      
        public MainPage()
        {
            this.InitializeComponent();
            DispatcherTimerSetup();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        DispatcherTimer dispatcherTimer;
        int timesTicked = 1;
        int timesToTick = 50;

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
           
            dispatcherTimer.Start();
        }

        async void dispatcherTimer_Tick(object sender, object e)
        {
            if (instrumentList == null)
            {
                InstrumentReader reader = new InstrumentReader();
                instrumentList = await reader.read();

            }

            if (player == null)
                player = new MediaElement();
            if(speech == null)
                speech = new SpeechSynthesizer();
            //this block is gonna change
            //System.Diagnostics.Debug.WriteLine("Hello");
            String url = "http://finance.google.com/finance/info?client=ig&q=";
            HttpClient client = new HttpClient();
            JsonValue jsonList = null;
            JsonArray jsonArray = null;
            JsonObject jsonObject = null;
            IJsonValue currentValue = null;
             foreach (Instrument instrument in instrumentList)
            {
                String shareName = instrument.instrument_Code;
                var data = await client.GetStringAsync(new Uri(url + shareName));
                data = data.Replace("//", "").Replace("\r\n", "");
                //System.Diagnostics.Debug.WriteLine(data);
                try
                {
                    jsonList = JsonValue.Parse(data);
                    jsonArray = jsonList.GetArray();
                    if (jsonArray.Count > 0)
                    {
                        jsonObject = jsonArray.GetObjectAt(0);
                        bool success = jsonObject.TryGetValue("l_cur", out currentValue); 
                        if (success)
                            instrument.instrument_Curr_Value = currentValue.GetString();
 
                     }
                }
                catch (Exception f)
                {
                    //System.Diagnostics.Debug.WriteLine(f);
                }
            }
            //System.Diagnostics.Debug.WriteLine(instrumentList);


           // using (speech)
            {
                string playString = "";
                foreach (Instrument instrument in instrumentList)
                {
                    playString += " " + instrument.instrument_Voice_code + " " + instrument.instrument_Curr_Value + ".";
                }
                var voiceStream = await speech.SynthesizeTextToStreamAsync(playString);
               //player.Stop();
                player.SetSource(voiceStream, voiceStream.ContentType);
                player.Play();
                voiceStream = null;
            }
        }

        private void TimerStart_Click_1(object sender, RoutedEventArgs e)
        {
            DispatcherTimerSetup();
        }

        private void TimerStop_Click_1(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        /*
        private async void RunProcess(List<Instrument> instrumentList)
        {
            HttpClient client = new HttpClient();
            String url = "http://finance.google.com/finance/info?client=ig&q=";
            JsonObject curr_Value = new JsonObject();
  
            foreach(Instrument instrument in instrumentList) {
                String nameOfShare = instrument.instrument_Code;
                String data = await client.GetStringAsync(new Uri(url + nameOfShare));
                data = data.Replace(@"//", string.Empty);
                //TODO convert to json object and write to list element
                var obj = JsonValue.TryParse(data);
                System.Diagnostics.Debug.WriteLine(data);
            } 
        
        }*/


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
    }
}
