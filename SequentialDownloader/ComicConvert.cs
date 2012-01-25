using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace ScraperLib
{
	public class ComicConvert
	{
		public static bool CbzToImgs (string cbz)
		{			
			var dirName = Path.GetFileNameWithoutExtension (cbz);
			var unpackDirectory = Path.GetFullPath (dirName);
			
			try {
				if (!Directory.Exists (unpackDirectory)) {
					Directory.CreateDirectory (unpackDirectory);
				}
			
				using (ZipInputStream s = new ZipInputStream(File.OpenRead(cbz))) {
		
					ZipEntry theEntry;
					while ((theEntry = s.GetNextEntry()) != null) {
				
//						Console.WriteLine (theEntry.Name);
				
						string directoryName = Path.GetDirectoryName (theEntry.Name);
						string fileName = Path.GetFileName (theEntry.Name);
				
						// create directory
						if (directoryName.Length > 0) {
							Directory.CreateDirectory (directoryName);
						}
				
						if (fileName != String.Empty) {
							using (FileStream streamWriter = File.Create(Path.Combine(unpackDirectory,theEntry.Name))) {
					
								int size = 2048;
								byte[] data = new byte[2048];
								while (true) {
									size = s.Read (data, 0, data.Length);
									if (size > 0) {
										streamWriter.Write (data, 0, size);
									} else {
										break;
									}
								}
							}
						}
					}
				}
				return true;
			} catch {
				return false;
			}
		}
		
		public static bool AddToCbz (string fileName, string zipFileName)
		{
			if (Path.GetExtension (zipFileName).Length == 0) {
				zipFileName = Path.ChangeExtension (zipFileName, ".zip");
			}
			
			ZipFile zipFile;

			try {
				if (File.Exists (zipFileName)) {
					zipFile = new ZipFile (zipFileName);
				} else {
					zipFile = ZipFile.Create (zipFileName);
				}

				using (zipFile) {
					zipFile.UseZip64 = UseZip64.On;
					zipFile.BeginUpdate ();
					zipFile.Add (Path.GetFullPath (fileName));
					zipFile.CommitUpdate ();
					
					return true;
				}
			} catch {
				return false;
			}
		}
		
		public static bool ImgsToCbz (string dir, string cbz)
		{
			string fileName;
			
			if (Path.GetExtension (cbz).ToLower () == ".zip" || Path.GetExtension (cbz).ToLower () == ".cbz") {
				fileName = cbz;
			} else {
				fileName = cbz + ".zip";
			}
			
			var imgFiles = Directory.GetFiles (Path.GetFullPath (dir));
			
			try {
				using (ZipOutputStream s = new ZipOutputStream(File.Create(fileName))) {
			
					s.SetLevel (9); // 0 - store only to 9 - means best compression
		
					byte[] buffer = new byte[4096];
				
					foreach (string file in imgFiles) {
						
						var info = new FileInfo(file);
						if (info.Length == 0) {
							continue;
						}
						
						// Using GetFileName makes the result compatible with XP
						// as the resulting path is not absolute.
						ZipEntry entry = new ZipEntry (Path.GetFileName (file));
					
						// Setup the entry data as required.
					
						// Crc and size are handled by the library for seakable streams
						// so no need to do them here.

						// Could also use the last write time or similar for the file.
						entry.DateTime = DateTime.Now;
						s.PutNextEntry (entry);
					
						using (FileStream fs = File.OpenRead(file)) {
		
							// Using a fixed size buffer here makes no noticeable difference for output
							// but keeps a lid on memory usage.
							int sourceBytes;
							do {
								sourceBytes = fs.Read (buffer, 0, buffer.Length);
								s.Write (buffer, 0, sourceBytes);
							} while ( sourceBytes > 0 );
						}
					}
				}
				return true;
			} catch {
				return false;
			}			
		}
	}
}

