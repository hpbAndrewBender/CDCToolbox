using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CDCHelper.Logic
{
	public class WaitCursor : IDisposable
	{
		private Cursor PrevCursor;


		public WaitCursor()
		{
			PrevCursor=Mouse.OverrideCursor;
			Mouse.OverrideCursor=Cursors.Wait;

		}

		public void Dispose()
		{
			Mouse.OverrideCursor=PrevCursor;
		}
	}
}
