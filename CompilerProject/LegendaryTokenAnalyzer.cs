// Program Analyzer

// Legendary Program Analyzer
// Date: 29-Jan-2018
// Authors:
//          A01374648 Mario Lagunes Nava 
//          A01375640 Brandon Alain Cruz Ruiz
//			A01376200 Oscar Allan Ruiz Toledo
// File name: TokenCatergory.cs

using System;
using System.IO;
using System.Text;


namespace DeepLingo {

	public class LegendaryTokenAnalyzer {

		public static void Main(string[] args){
			
			//*** Read the name of the file from line console ***\\
			
			var archivo = "";
			string[] argus = Environment.GetCommandLineArgs();
			if(argus.Length == 2){
				archivo = argus[1];
				fileReader(archivo);
			}
			else{
				Console.WriteLine("Incorrect number of arguments.");
				Environment.Exit(1);
			}
			//*** End of read the... ***\\
		}

		public static void fileReader(string archivo){
			
			//*** Read the file using the Reader namespace ***\\
			try {                 
				int count = 1;
				var str = File.ReadAllText (archivo);
				foreach (var tok in new Reader(str).Start()) {
					Console.WriteLine (String.Format ("[{0}] {1}", 
						count++, tok.ToString()));
				}
			
			} catch (FileNotFoundException e) {
				Console.Error.WriteLine (e.Message);
				Environment.Exit (1);
			}
			//*** End of read the... ***\\
		}
	}
}
	
