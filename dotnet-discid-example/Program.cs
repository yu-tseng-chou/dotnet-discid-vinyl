//  Author:
//       Philipp Wolfer <ph.wolfer@gmail.com>
//
//  Copyright (c) 2013 Philipp Wolfer
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace DiscIdExample
{
    using System;
    using DiscId;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Out.WriteLine("Please enter the filename of vinyl image:");

            try
            {
                String filename = Console.ReadLine();
                // Create the MATLAB instance 
                MLApp.MLApp matlab = new MLApp.MLApp();

                // Change to the directory where the function is located 
                matlab.Execute(@"cd C:\Users\Jenny\Documents\'SDSU CS696-2'\dotnet-discid-master\dotnet-discid-example\bin\Debug");

                // Define the output 
                object result = null;
                

                // Call the MATLAB function myfunc
                matlab.Feval("GetGrooveCounts", 1, out result, filename);

                // Display result 
                double[,] groove_counts = ((double[,])((object[])result)[0]) as double[,];
                
                int sum         = 150;
                int[] offsets   = new int[groove_counts.Length];
                offsets[0]      = 150;

                for (int i = 0; i < groove_counts.Length; i++)
                {
                    double seconds = Math.Floor(groove_counts[0, i] * 1.8);
                    int sectors = Convert.ToInt32(seconds) * 75;
                    sum += sectors;
                    if (i < offsets.Length-1)
                        offsets[i + 1] = offsets[i] + sectors;
                }

                
                //Console.WriteLine(res[1]);
                //Console.ReadLine();

                Console.Out.WriteLine("libdiscid version: {0}", Disc.LibdiscidVersion);
                Console.Out.WriteLine("Read feature     : {0}", Disc.HasFeatures(Features.Read));
                Console.Out.WriteLine("MCN feature      : {0}", Disc.HasFeatures(Features.Mcn));
                Console.Out.WriteLine("ISRC feature     : {0}", Disc.HasFeatures(Features.Isrc));
                Console.Out.WriteLine("All features     : {0}", Disc.HasFeatures(Features.Read | Features.Mcn | Features.Isrc));

                ////using (var disc = Disc.Read(device, Features.Mcn | Features.Isrc))
                ////using (var disc = Disc.Put(1, 64050, new int[6] { 0, 12375, 22950, 33750, 44175, 54450 }))
                ////using (var disc = Disc.Put(1, 145275, new int[12] { 0, 12375, 22950, 33750, 44175, 54450, 64050, 77625, 86925, 97875, 109500, 123675 }))
                //using (var disc = Disc.Put(1, 80175, new int[4] { 0, 19500, 39150, 59550}))
                using(var disc = Disc.Put(1, sum, offsets))
                {
                    string[] lines = { "DiscId         : " + disc.Id, "FreeDB ID      : " + disc.FreedbId, "Submission URL : " + disc.SubmissionUrl };
                    System.IO.File.WriteAllLines("output.txt", lines);

                    Console.Out.WriteLine();
                    Console.Out.WriteLine("DiscId         : {0}", disc.Id);
                    Console.Out.WriteLine("FreeDB ID      : {0}", disc.FreedbId);
                    Console.Out.WriteLine("MCN            : {0}", disc.Mcn);
                    Console.Out.WriteLine("First track no.: {0}", disc.FirstTrackNumber);
                    Console.Out.WriteLine("Last track no. : {0}", disc.LastTrackNumber);
                    Console.Out.WriteLine("Sectors        : {0}", disc.Sectors);
                    Console.Out.WriteLine("Submission URL : {0}", disc.SubmissionUrl);

                    Console.Out.WriteLine();
                    foreach (var track in disc.Tracks)
                    {
                        Console.Out.WriteLine("Track #{0}:", track.Number);
                        Console.Out.WriteLine("  Offset: {0} sectors", track.Offset);
                        Console.Out.WriteLine("  Length: {0} sectors", track.Sectors);
                        Console.Out.WriteLine("  ISRC  : {0}", track.Isrc);
                    }

                    Console.In.ReadLine();
                }
            }
            catch (DiscIdException ex)
            {
                Console.Out.WriteLine("Could not read disc: {0}.", ex.Message);
                Console.In.ReadLine();
            }
        }
    }
}
