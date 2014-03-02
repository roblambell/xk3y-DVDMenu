namespace xk3yDVDMenu
{
    interface IMediaDownloader
    {

        byte[] DownloadBanner(string gameID, string filename);
        byte[] DownloadCover(string gameID, string filename);

        string DownloadTitle(string gameID, string filename);
        string DownloadDesc(string gameID, string filename);
        string DownloadProp(string property ,string gameID, string filename);
        string AddonName { get; }
    
    }
}
