using System;
using System.IO;
using System.Net;

public class FtpUploader
{
	private enum Protocol
	{
		FTP,
		SFTP
	}

	public static void Upload(string filename, string server, string username, string password, string initialPath)
	{
		//FileInfo fileInfo = new FileInfo(filename);
		//Uri requestUri = new Uri("ftp://" + server + "/" + Path.Combine(initialPath, fileInfo.Name));
		//FtpWebRequest ftpWebRequest = WebRequest.Create(requestUri) as FtpWebRequest;
		//ftpWebRequest.Credentials = new NetworkCredential(username, password);
		//ftpWebRequest.KeepAlive = false;
		//ftpWebRequest.Method = "STOR";
		//ftpWebRequest.UseBinary = true;
		//ftpWebRequest.ContentLength = fileInfo.Length;
		//int num = 2048;
		//byte[] buffer = new byte[num];
		//int num2 = 0;
		//FileStream fileStream = fileInfo.OpenRead();
		//try
		//{
		//	Stream requestStream = ftpWebRequest.GetRequestStream();
		//	for (num2 = fileStream.Read(buffer, 0, num); num2 != 0; num2 = fileStream.Read(buffer, 0, num))
		//	{
		//		requestStream.Write(buffer, 0, num2);
		//	}
		//	requestStream.Close();
		//	fileStream.Close();
		//}
		//catch (Exception)
		//{
		//}
	}
}
