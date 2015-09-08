using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
using Windows.ApplicationModel.Background;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace TestApp
{
    class InstrumentReader
    {
        String filePath;
        Instrument ins = null;

        public InstrumentReader() {
            //this.filePath = filePath;
        }

        public async Task<List<Instrument>> read() 
        {

            List<Instrument> instrumentList = new List<Instrument>();
            
            StorageFolder folder = KnownFolders.PicturesLibrary;
            if (null != folder)
            {
                try
                {
                    StorageFile MyFile = await folder.GetFileAsync("BROTHER.CSV");
                    String sr = await Windows.Storage.FileIO.ReadTextAsync(MyFile);
                    System.Diagnostics.Debug.WriteLine(sr);

                    foreach (string id2 in sr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Skip(1))
                    {
                        string id = null;
                        id = id2;
                        System.Diagnostics.Debug.WriteLine(id);

                        var value = id.Split(',');
                        System.Diagnostics.Debug.WriteLine(value);

                        instrumentList.Add(new Instrument(Convert.ToString(value[0]), Convert.ToString(value[1]),
                                Convert.ToSingle(value[2]), Convert.ToSingle(value[3]), getBool(value[4])));
                    }
                    System.Diagnostics.Debug.WriteLine(instrumentList);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
            return instrumentList;
        }

        public Boolean getBool(String input) 
        {
            bool theanswer = false;
            switch(input)
            {
                case "y": 
                    theanswer = true; 
                    break;

                case "n": 
                    theanswer = false; 
                    break;

                case "Y": 
                    theanswer = true; 
                    break;

                case "N": 
                    theanswer = false; 
                    break;
            }
            return theanswer;
        }
    }
}
