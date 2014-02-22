using System.Collections.Generic;
using System.Net;
using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;

namespace xk3yDVDMenu
{
internal class WbClient : WebClient
{
	public WbClient()
	{
	}

    public bool waiting;
    byte[] data = null;
    public Dictionary<string, object> Cache = new Dictionary<string, object>();
     

    public string RedirectURL(ISO game,string baseURL)
    {
        
        if (game.RedirectURL == "")
        {
            Uri uri = new Uri(baseURL);
            HttpWebRequest httpreqRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpreqRequest.Method = "GET";
            httpreqRequest.AllowAutoRedirect = true;
            
            
            game.RedirectURL = httpreqRequest.GetResponse().ResponseUri.AbsoluteUri;
            ;

        }

        return game.RedirectURL;
    }

	public byte[] DownloadData(string address, ProgressBar pb)
	{
	    
		
		pb.Maximum = 100;
		base.DownloadProgressChanged += (object s, DownloadProgressChangedEventArgs e) => pb.Value = e.ProgressPercentage;
	    base.DownloadDataCompleted += (object s, DownloadDataCompletedEventArgs e) =>
	                                      {
	                                          if (e.Error == null)
	                                          {
	                                              this.waiting = false;
	                                              this.data = e.Result;
	                                          }
	                                      };
		base.DownloadDataAsync(new Uri(address));
        while (waiting)
		{
			Application.DoEvents();
		}
		return data;
	}

	 public void DownloadFile(string address, string filename, ProgressBar pb)
	{
		
		pb.Maximum = 100;
		base.DownloadProgressChanged += (object s, DownloadProgressChangedEventArgs e) => pb.Value = e.ProgressPercentage;
		base.DownloadFileCompleted += (object s, AsyncCompletedEventArgs e) => this.waiting = false;
		base.DownloadFileAsync(new Uri(address), filename);
        while (waiting)
		{
			Application.DoEvents();
		}
	}

	new public string DownloadString(string address)
	{
        if (Cache.ContainsKey(address))
            return Cache[address].ToString();

        base.Encoding = Encoding.UTF8;
		Cache.Add(address, base.DownloadString(address));
        return Cache[address].ToString();
	}
}
}