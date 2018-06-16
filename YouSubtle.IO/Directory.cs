using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace YouSubtle.IO
{
    public class Directory
    {
		/// <summary>
		/// You doubt that it's not working when the container hierarcy of the required directory is missing
		/// the last one or more parts. Make a check.
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="maxNoOfTrials"></param>
		public static void EnsureDirectory(string directory)
		{
			bool success = false;

			Exception raisedException = null;

			if(System.IO.Directory.Exists(directory))
			{
				success = true;
			}
			else
			{
				for(int x=0; x<30; x++)
				{
					try
					{
						System.IO.Directory.CreateDirectory(directory);
						success = true;
						break;
					}
					catch (Exception expt)
					{
						raisedException = expt;
						Thread.Sleep(300);
						success = false;
					}
				}
			}

			if(! success)
			{
				throw new IOException($"Couldn't ensure directory [{raisedException}]", raisedException);
			}
		}

    }
}
