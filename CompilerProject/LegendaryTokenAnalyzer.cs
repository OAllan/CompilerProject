using System;
using System.IO;
using System.Text;


namespace DeepLingo {

	public class LegendaryTokenAnalyzer {

		public static void Main(string[] args){
			

			try {                 
				int count = 1;
				var str = File.ReadAllText ("binary.deep");
				foreach (var tok in new Reader(str).Start()) {
					Console.WriteLine (String.Format ("[{0}] {1}", 
						count++, tok.ToString()));
				}


			
			} catch (FileNotFoundException e) {
				Console.Error.WriteLine (e.Message);
				Environment.Exit (1);
			}
		}
	}
}
	
