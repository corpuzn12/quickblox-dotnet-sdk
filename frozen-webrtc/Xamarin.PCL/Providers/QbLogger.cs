﻿using System;
using Quickblox.Sdk.Logger;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Xamarin.PCL
{
	public class QbLogger : ILogger
	{
		public QbLogger ()
		{
		}

		#region ILogger implementation

		public async Task Log (LogLevel logLevel, string message)
		{
			// TODO: Add another way to loggin

			if (logLevel == LogLevel.Debug) {
				Debug.WriteLine (message);
			}
		}

		#endregion
	}
}

